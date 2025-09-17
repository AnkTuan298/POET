using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POET.Models;

namespace POET.Services
{
    public class ClassService
    {
        private readonly PoetContext _context;

        public ClassService(PoetContext context)
        {
            _context = context;
        }

        public string JoinClass(int userId, string classCode)
        {
            if (string.IsNullOrWhiteSpace(classCode))
                return "Please enter a class code";

            var targetClass = _context.Classes.FirstOrDefault(c => c.ClassCode == classCode);
            if (targetClass == null)
                return "Invalid class code";

            var user = _context.Users.Include(u => u.ClassesNavigation).FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return "User not found";

            if (user.ClassesNavigation.Any(c => c.ClassId == targetClass.ClassId))
                return "You're already in this class";

            user.ClassesNavigation.Add(targetClass);
            _context.SaveChanges();
            return "Successfully joined class!";
        }
    }
}
