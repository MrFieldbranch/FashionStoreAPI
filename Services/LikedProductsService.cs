using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class LikedProductsService
    {
        private readonly ApplicationDbContext _context;

        public LikedProductsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProductToLikedAsync(int userId, int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten hittades inte.");

            var existingLike = await _context.LikedProducts
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.ProductId == productId);

            if (existingLike != null)            
                throw new ArgumentException("Produkten är redan i dina gillade produkter.");
            
            var likedProduct = new LikedProduct
            {
                UserId = userId,
                ProductId = productId
            };

            _context.LikedProducts.Add(likedProduct);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromLikedAsync(int userId, int productId)
        {
            var existingLike = await _context.LikedProducts
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.ProductId == productId) 
                ?? throw new ResourceNotFoundException("Produkten finns inte i dina gillade produkter.");

            _context.LikedProducts.Remove(existingLike);

            await _context.SaveChangesAsync();
        }

        public async Task<List<BasicProductResponse>> GetLikedProductsAsync(int userId)
        {
            var likedProducts = await _context.LikedProducts
                .Include(lp => lp.Product)
                .ThenInclude(p => p.ProductVariants)
                .Where(lp => lp.UserId == userId)
                .ToListAsync();

            var likedProductsResponse = likedProducts.Select(lp => new BasicProductResponse
            {
                Id = lp.Product.Id,
                Name = lp.Product.Name,
                ProductSex = lp.Product.ProductSex,
                ImageUrl = lp.Product.ImageUrl,
                StartPrice = lp.Product.ProductVariants.Count != 0 ? lp.Product.ProductVariants.Min(v => v.Price) : 0,
                IsLiked = true, // All products in this list are liked
                RatingsCount = lp.Product.RatingsCount,
                AverageGrade = Math.Round(lp.Product.AverageGrade, 1)
            }).ToList();

            return likedProductsResponse;
        }
    }
}
