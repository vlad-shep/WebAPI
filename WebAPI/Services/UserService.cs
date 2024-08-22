using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private UsersRepository _usersRepository;

        private UsersRepository UsersRepository
        {
            get
            {
                if (_usersRepository == null)
                {
                    _usersRepository = new UsersRepository(_configuration);
                }
                return _usersRepository;
            }
        }

        public int GetLogin(Users users)
        {
            string encryptPassword = GetHash(users.userPassword);
            users.userPassword = encryptPassword;
            if(UsersRepository.GetLogin(users) == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private string GetHash(string password)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hash);
        }
    }
}
