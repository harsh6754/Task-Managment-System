using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;
using Microsoft.AspNetCore.Http;

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
        // Process Profile Picture if provided
        if (register.ProfilePic?.Length > 0)
        {
            var fileName = $"{register.c_email}{Path.GetExtension(register.ProfilePic.FileName)}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
            var filePath = Path.Combine(uploadsFolder, fileName);
            
            // Ensure directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            register.c_image = fileName;
            
            // Save the file
            await using var stream = new FileStream(filePath, FileMode.Create);
            await register.ProfilePic.CopyToAsync(stream);
            
            Console.WriteLine($"Image saved at: {filePath}");
        }

        Console.WriteLine($"Registering user: {register.c_username}");
        
        int status = await _registerLoginInterface.Register(register);

        return status switch
        {
            1 => Ok(new { success = true, message = "User Registered Successfully" }),
            0 => Conflict(new { success = false, message = "User Already Exists" }),
            _ => StatusCode(500, new { success = false, message = "Internal Server Error during registration" })
        };
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error in Register: {ex.Message}");
        return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
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
