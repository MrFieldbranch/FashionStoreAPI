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
            
            try
            {
                var newUser = new User
                {
                    Email = request.Email,
                    Password = request.Password, // In a real application, hash the password
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                _context.Users.Add(newUser);

                await _context.SaveChangesAsync();

                return true; // Registration successful
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)  // Testa detta sen
            {
                throw new ArgumentException("Email kan max vara 40 tecken långt, lösenordet, förnamnet och efternamnet kan max vara 30 tecken långa var.");
            }
            catch (Exception ex)
            {
                throw new Exception("Ett fel inträffade när användaren skulle sparas i databasen. Vänligen försök igen.", ex);
            }
        }
    }
}
