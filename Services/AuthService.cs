using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POET.Models;

namespace POET.Services
{
    public class AuthService
    {
        private readonly PoetContext _context;

        public AuthService(PoetContext context)
        {
            _context = context;
        }

        public string Register(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return "All fields must be filled.";

            if (password != confirmPassword)
                return "Passwords do not match.";

            if (_context.Users.Any(u => u.Username == username))
                return "Username already exists.";

            _context.Users.Add(new User
            {
                Username = username,
                Password = password,
                Role = "Student"
            });

            _context.SaveChanges();
            return "Registration successful!";
        }
    }
}
