using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Models;
using Repositories.Interfaces;
using System.Data;
using System.Runtime.InteropServices;
using System.Linq.Expressions;


namespace Repositories.Implementations
{
    public class RegisterLoginRepository : IRegisterLoginInterface
    {
        private readonly NpgsqlConnection _conn;

        public RegisterLoginRepository(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<t_Register> Login(t_Login login)
        {
            t_Register UserData = new t_Register();
            var qry = "SELECT * FROM t_user WHERE c_email = @c_email AND c_password = @c_password";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_email", login.c_email);
                    cmd.Parameters.AddWithValue("@c_password", login.c_password);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            UserData.c_userid = Convert.ToInt32(reader["c_userid"]);
                            UserData.c_username = reader["c_username"].ToString();
                            UserData.c_email = reader["c_email"].ToString();
                            UserData.c_password = reader["c_password"].ToString();
                            UserData.c_address = reader["c_address"].ToString();
                            UserData.c_mobile = reader["c_mobile"].ToString();
                            UserData.c_country = reader["c_country"].ToString();
                            UserData.c_state = reader["c_state"].ToString();
                            UserData.c_district = reader["c_district"].ToString();  
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Login Error : " + e.Message);
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
            return UserData;
        }

        public async Task<int> Register(t_Register register)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }

                // Check if the email already exists
                using (NpgsqlCommand checkUsercmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM t_user WHERE c_email = @c_email",
                    _conn
                ))
                {
                    checkUsercmd.Parameters.AddWithValue("@c_email", register.c_email);
                    int count = Convert.ToInt32(await checkUsercmd.ExecuteScalarAsync());
                    if (count > 0)
                    {
                        return 0; // User already exists
                    }
                }

                // Convert IFormFile to byte array for storage in `bytea`
                byte[] imageBytes = null;
                if (register.c_image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await register.c_image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                // Insert user data
                using (NpgsqlCommand cm = new NpgsqlCommand(
                    "INSERT INTO t_user (c_username, c_email, c_password,c_confirmPassword ,c_address, c_mobile, c_gender, c_dob, c_country, c_state, c_district, c_imagePath) " +
                    "VALUES(@c_username, @c_email, @c_password,@c_confirmPassword, @c_address, @c_mobile, @c_gender, @c_dob, @c_country, @c_state, @c_district, @c_imagePath)", _conn))
                {
                    cm.Parameters.AddWithValue("@c_username", register.c_username);
                    cm.Parameters.AddWithValue("@c_email", register.c_email);
                    cm.Parameters.AddWithValue("@c_password", register.c_password);
                    cm.Parameters.AddWithValue("@c_confirmPassword", register.c_confirmpassword);
                    cm.Parameters.AddWithValue("@c_address", register.c_address);
                    cm.Parameters.AddWithValue("@c_mobile", register.c_mobile);
                    cm.Parameters.AddWithValue("@c_gender", register.c_gender);
                    cm.Parameters.AddWithValue("@c_dob", register.c_dob);
                    cm.Parameters.AddWithValue("@c_country", register.c_country);
                    cm.Parameters.AddWithValue("@c_state", register.c_state);
                    cm.Parameters.AddWithValue("@c_district", register.c_district);
                    cm.Parameters.AddWithValue("@c_imagePath", imageBytes ?? (object)DBNull.Value);

                    await cm.ExecuteNonQueryAsync();
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Register Error: " + e.Message);
                return -1;
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }
    }
}