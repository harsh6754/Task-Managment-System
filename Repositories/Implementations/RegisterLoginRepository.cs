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
            "INSERT INTO t_user (c_username, c_email, c_password, c_address, c_mobile, c_gender, c_dob, c_countryid, c_stateid, c_districtid, c_image) " +
            "VALUES(@c_username, @c_email, @c_password, @c_address, @c_mobile, @c_gender, @c_dob, @c_countryid, @c_stateid, @c_districtid, @c_image)", _conn))
        {
            cm.Parameters.AddWithValue("@c_username", register.c_username);
            cm.Parameters.AddWithValue("@c_email", register.c_email);
            cm.Parameters.AddWithValue("@c_password", register.c_password);
            cm.Parameters.AddWithValue("@c_address", register.c_address);
            cm.Parameters.AddWithValue("@c_mobile", register.c_mobile);
            cm.Parameters.AddWithValue("@c_gender", register.c_gender ?? (object)DBNull.Value);
            cm.Parameters.AddWithValue("@c_dob", register.c_dob);
            cm.Parameters.AddWithValue("@c_countryid", register.c_countryid ?? (object)DBNull.Value);
            cm.Parameters.AddWithValue("@c_stateid", register.c_stateid ?? (object)DBNull.Value);
            cm.Parameters.AddWithValue("@c_districtid", register.c_districtid ?? (object)DBNull.Value);
            cm.Parameters.AddWithValue("@c_image", imageBytes ?? (object)DBNull.Value);

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



        public async Task<List<t_Country>> GetAllCountries()
        {
            var countries = new List<t_Country>();
            string qry = "SELECT c_countryid, c_countryname FROM t_country ORDER BY c_countryid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        countries.Add(
                            new t_Country
                            {
                                c_countryid = Convert.ToInt32(reader["c_countryid"]),
                                c_countryname = reader["c_countryname"].ToString()
                            }
                        );
                    }
                }
                return countries;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAllCountries Error: " + e.Message);
                return countries; // Return the list even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<t_Country> GetCountryById(int id)
        {
            t_Country country = new t_Country();
            string qry = "SELECT c_countryid, c_countryname FROM t_country WHERE c_countryid = @c_countryid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_countryid", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            country.c_countryid = Convert.ToInt32(reader["c_countryid"]);
                            country.c_countryname = reader["c_countryname"].ToString();
                        }
                    }
                }
                return country;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetCountryById Error: " + e.Message);
                return country; // Return the object even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<List<t_State>> GetAllStates()
        {
            var states = new List<t_State>();
            string qry = "SELECT c_stateid, c_statename FROM t_state ORDER BY c_stateid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        states.Add(
                            new t_State
                            {
                                c_stateid = Convert.ToInt32(reader["c_stateid"]),
                                c_statename = reader["c_statename"].ToString()
                            }
                        );
                    }
                }
                return states;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetAllStates Error: " + e.Message);
                return states; // Return the list even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<List<t_State>> GetStatesByCountryId(int countryId)
        {
            var states = new List<t_State>();
            string qry = "SELECT c_stateid, c_statename FROM t_state WHERE c_countryid = @c_countryid ORDER BY c_stateid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_countryid", countryId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            states.Add(
                                new t_State
                                {
                                    c_stateid = Convert.ToInt32(reader["c_stateid"]),
                                    c_statename = reader["c_statename"].ToString()
                                }
                            );
                        }
                    }
                }
                return states;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetStatesByCountryId Error: " + e.Message);
                return states; // Return the list even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<t_State> GetStateById(int id)
        {
            t_State state = new t_State();
            string qry = "SELECT c_stateid, c_statename FROM t_state WHERE c_stateid = @c_stateid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_stateid", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            state.c_stateid = Convert.ToInt32(reader["c_stateid"]);
                            state.c_statename = reader["c_statename"].ToString();
                        }
                    }
                }
                return state;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetStateById Error: " + e.Message);
                return state; // Return the object even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<List<t_District>> GetDistrictsByStateId(int stateId)
        {
            var districts = new List<t_District>();
            string qry = "SELECT c_districtid, c_districtname FROM t_district WHERE c_stateid = @c_stateid ORDER BY c_districtid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_stateid", stateId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            districts.Add(
                                new t_District
                                {
                                    c_districtid = Convert.ToInt32(reader["c_districtid"]),
                                    c_districtname = reader["c_districtname"].ToString()
                                }
                            );
                        }
                    }
                }
                return districts;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetDistrictsByStateId Error: " + e.Message);
                return districts; // Return the list even if an error occurs
            }
            finally
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
        }

        public async Task<t_District> GetDistrictById(int id)
        {
            t_District district = new t_District();
            string qry = "SELECT c_districtid, c_districtname FROM t_district WHERE c_districtid = @c_districtid";
            try
            {
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_districtid", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            district.c_districtid = Convert.ToInt32(reader["c_districtid"]);
                            district.c_districtname = reader["c_districtname"].ToString();
                        }
                    }
                }
                return district;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetDistrictById Error: " + e.Message);
                return district; // Return the object even if an error occurs
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