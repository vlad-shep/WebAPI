using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Repository;
using WebAPI.Models;
using System.Data.SqlClient;

namespace WebAPI.Repository
{
    public class UsersRepository
    {
        private readonly IConfiguration _configuration;

        public UsersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int GetLogin(Users users)
        {
            int count = 0;
            string connection = _configuration.GetConnectionString("DefaultConnection");
            string query = $@"SELECT COUNT(*) FROM tbUsers WHERE UserLogin = @login and UserPassword = @password";
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("login", users.userLogin);
                    sqlCommand.Parameters.AddWithValue("password", users.userPassword);
                    count = (Int32)sqlCommand.ExecuteScalar();
                }
            }
            return count;
        }
    }
}
