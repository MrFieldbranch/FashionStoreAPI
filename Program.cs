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

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5174") // Lägg till sen: production url.
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

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
