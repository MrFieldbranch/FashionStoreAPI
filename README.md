# FashionStoreAPI

This is the backend REST API for the Fashion Store project, built with ASP.NET Core. 
It handles authentication, user profiles, products, categories, and orders in an 
online dummy fashion store. It uses Entity Framework Core for database operations.

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- PostgreSQL Server
- JWT Authentication
- BCrypt for password hashing

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Run Locally
```bash
dotnet build
dotnet ef database update
dotnet run
```
The API will run at https://localhost:8000.
Connection string and JWT settings are configured in 
`appsettings.Development.json` when running locally.

### Endpoints
- `POST /api/registration` - Register a new user.

- `POST /api/login` - Login and receive JWT token.

- `GET /api/products/{productId}` - Get a single product.
- `GET /api/products/most-popular?sex=Male` - Get the most popular products by sex.

- `GET /api/categories/sex/{sex}/allcategories` - Get all categories (without products) based on sex.
- `GET /api/categories/{categoryId}/sex/{sex}/products` - Get products by category based on sex.
- `GET /api/categories/allcategories` - Get all categories (without products). Only admin uses this endpoint.
- `GET /api/categories/{categoryId}/products` - Get products by category. Only admin uses this endpoint.

- `POST /api/likedproducts/{productId}` - Add a product to liked products for the logged-in user.
- `DELETE /api/likedproducts/{productId}` - Remove a product from liked products for the logged-in user.
- `GET /api/likedproducts` - Get all liked products for the logged-in user.

- `POST /api/shoppingbasket/items` - Add a product to the shopping basket for the logged-in user.
- `DELETE /api/shoppingbasket/items` - Remove a product from the shopping basket for the logged-in user.
- `GET /api/shoppingbasket/items` - Get all items in the shopping basket for the logged-in user.
- `GET /api/shoppingbasket/items/totalprice` - Get the total price of items in the shopping basket for the logged-in user.
- `PUT /api/shoppingbasket/items/{productvariantid}/quantity` - Update the quantity of an item in the shopping basket for the logged-in user.

- `POST /api/orders` - Create an order for the logged-in user. This will also clear the shopping basket, and all the liked products, that are in the basket.
- `GET /api/orders/{orderId}` - Get a specific order by ID.
- `GET /api/orders/allorders` - Get all orders for the logged-in user.

### Admin Endpoints
These endpoints require a valid JWT token with the Admin role.
- `POST /api/products/{categoryId}` - Create a new product.
- `PUT /api/products/{productId}` - Update an existing product.

- `POST /api/categories` - Create a new category.

- `POST /api/products/{productId}/productvariants` - Create a new product variant (size).
- `PUT /api/products/{productId}/productvariants` - Update an existing product variant (size).

### Authentication Notes
Most endpoints require a valid JWT token in the Authorization header as:
Authorization: Bearer {token}
User identity is extracted from the token (via claims) in many endpoints to determine which user is sending the request.

### Database
The database is PostgreSQL.
Ensure your appsettings.Development.json contains a valid connection string like:
<pre> "ConnectionStrings": {
   "DefaultConnection": "Host=localhost;Port=5432;Database=FashionStoreDb;Username=yourusername;Password=yourpassword"
}
</pre>

### Security
Passwords are securely hashed using BCrypt.
JWT tokens are issued for secure, stateless authentication.

### License
This project is licensed under the MIT License.