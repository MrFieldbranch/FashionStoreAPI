﻿using FashionStoreAPI.Enums;

namespace FashionStoreAPI.DTOs
{
    public class UpdateExistingProductRequest
    {
        public required string Name { get; set; }

        public Sex ProductSex { get; set; } 
        public required string Description { get; set; }
        public required string ImageUrl { get; set; }
        public required string Color { get; set; }
    }
}
