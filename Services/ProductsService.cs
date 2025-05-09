using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Services
{
    public class ProductsService
    {
        private readonly ApplicationDbContext _context;

        public ProductsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponse> CreateNewProductAsync(CreateNewProductRequest request)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == request.Name);

            if (existingProduct != null)
                throw new ConflictException("En produkt med detta namn finns redan.");

            var newProduct = new Product
            {
                Name = request.Name,
                Description = request.Description ?? "",
                ImageUrl = request.ImageUrl,
                Color = request.Color
            };

            try
            {
                _context.Products.Add(newProduct);

                await _context.SaveChangesAsync();

                return new ProductResponse
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    ImageUrl = newProduct.ImageUrl,
                    Description = newProduct.Description ?? "",
                    Color = newProduct.Color
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 40 tecken.", ex); // Testa detta sedan.
            }            
            catch (Exception ex)
            {
                throw new Exception("Ett fel inträffade när produkten skulle sparas i databasen. Vänligen försök igen.", ex);
            }
        }
    }
}
