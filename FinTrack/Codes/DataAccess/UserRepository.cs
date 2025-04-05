using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FinTrack.Codes.DataAccess
{
    public class UserRepository
    {
        private static UserRepository instance;
        public static UserRepository Instance => instance ??= new UserRepository();

        private readonly string connectionString;

        public UserRepository()
        {
            connectionString = App.Configuration.GetConnectionString("PostgresConnection");
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public async Task<users> LoginUser(string email, string password)
        {
            using var connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM users WHERE email = @Email AND password_hash = @Password";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new users
                    {
                        user_id = reader.GetGuid(reader.GetOrdinal("user_id")),
                        full_name = reader.GetString(reader.GetOrdinal("full_name")),
                        email = reader.GetString(reader.GetOrdinal("email")),
                        password_hash = "Empty for privacy.",
                        profile_picture = reader["profile_picture"] == DBNull.Value ? null : (byte[])reader["profile_picture"]
                    };
                }

                return null; // User not found or login failed.

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public async Task RegisterUser(users user)
        {
            using var connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                string query = "INSERT INTO users (full_name, email, password_hash, profile_picture) VALUES (@FullName, @Email, @PasswordHash, @ProfilePicture)";

                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@FullName", user.full_name);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.password_hash);
                cmd.Parameters.AddWithValue("@ProfilePicture", user.profile_picture);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public async Task UpdateUser(users user)
        {
            using var connection = GetConnection();
            try
            {
                await connection.OpenAsync();
                string query = "UPDATE users SET full_name = @FullName, email = @Email, password_hash = @PasswordHash, profile_picture = @ProfilePicture WHERE user_id = @UserId";
                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@FullName", user.full_name);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.password_hash);
                cmd.Parameters.AddWithValue("@ProfilePicture", user.profile_picture);
                cmd.Parameters.AddWithValue("@UserId", user.user_id);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public async Task DeleteUser(int userId)
        {
            using var connection = GetConnection();
            try
            {
                await connection.OpenAsync();
                string query = "DELETE FROM users WHERE user_id = @UserId";
                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public async Task<users> GetUserById(int userId)
        {
            using var connection = GetConnection();
            try
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM users WHERE user_id = @UserId";
                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new users
                    {
                        user_id = reader.GetGuid(reader.GetOrdinal("user_id")),
                        full_name = reader.GetString(reader.GetOrdinal("full_name")),
                        email = reader.GetString(reader.GetOrdinal("email")),
                        password_hash = "Empty for privacy.",
                        profile_picture = reader["profile_picture"] == DBNull.Value ? null : (byte[])reader["profile_picture"]
                    };
                }
                return null; // User not found.
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }
    }
}
