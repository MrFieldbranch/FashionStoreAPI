using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FashionStoreAPI.Services
{
    public class RegistrationService
    {
        private readonly ApplicationDbContext _context;

        public RegistrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterNewUserAsync(CreateNewUserRequest request)
        {
            // Check if the email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)            
                return false; // Email already exists

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            try
            {
                var newUser = new User
                {
                    Email = request.Email,
                    Password = hashedPassword,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                _context.Users.Add(newUser);

                await _context.SaveChangesAsync();

                return true; // Registration successful
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")  // Testa detta sen
            {
                throw new ArgumentException("Email kan max vara 40 tecken långt, förnamnet och efternamnet kan max vara 30 tecken långa var.");
            }            
        }
    }
}
