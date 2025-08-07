using FashionStoreAPI.Data;
using FashionStoreAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<CategoriesService>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<ProductVariantsService>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<LikedProductsService>();
builder.Services.AddScoped<ShoppingBasketService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<RatingRemindersService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://fancy-fashion.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        var signingSecret = builder.Configuration["JWT:SigningSecret"]
            ?? throw new InvalidOperationException("JWT SigningSecret är inte konfigurerad.");

        var signingKey = Convert.FromBase64String(signingSecret);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey)
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
