using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterLoginController : ControllerBase
    {
        private readonly IRegisterLoginInterface _registerLoginInterface;

        public RegisterLoginController(IRegisterLoginInterface registerLoginInterface)
        {
            _registerLoginInterface = registerLoginInterface;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] t_Register register)
        {
            if (register == null || string.IsNullOrWhiteSpace(register.c_email))
            {
                return BadRequest(new { success = false, message = "Invalid input data" });
            }

            try
            {
                if (register.c_image != null && register.c_image.Length > 0)
                {
                    // Generate a unique filename
                    var fileName = $"{Guid.NewGuid()}_{register.c_email}{Path.GetExtension(register.c_image.FileName)}";

                    // Define the file path
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                    var filePath = Path.Combine(uploadsFolder, fileName);
                    register.c_imagePath = $"/images/{fileName}"; // Store relative path for database

                    // Save file asynchronously
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await register.c_image.CopyToAsync(stream);
                }

                var status = await _registerLoginInterface.Register(register);

                return status switch
                {
                    1 => Ok(new { success = true, message = "User Registered Successfully" }),
                    0 => Conflict(new { success = false, message = "User Already Exists" }),
                    _ => StatusCode(500, new { success = false, message = "Internal Server Error during registration" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] t_Login login)
        {
            var userData = await _registerLoginInterface.Login(login);

            if (userData == null || userData.c_userid == 0)
            {
                return Ok(new { success = false, message = "Invalid email or password" });
            }

            return Ok(new { success = true, message = "Login Success", UserData = userData });
        }
    }
}
