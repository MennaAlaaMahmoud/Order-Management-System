using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {

        private readonly OrderManagementDbContext _context;

        public DbInitializer(OrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            // Apply pending migrations
            if (_context.Database.GetPendingMigrations().Any())
                await _context.Database.MigrateAsync();

            // Create default admin user if not exists
            if (!await _context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin"
                };

                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}


