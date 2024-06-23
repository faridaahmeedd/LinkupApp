using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repositories;
using ServicesApp.Repository;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x =>
				x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceOfferRepository, ServiceOfferRepository>();
builder.Services.AddScoped<ITimeSlotsRepository , TimeSlotRepositry>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPayPalRepository, PayPalRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IMLRepository, MLRepository>();
builder.Services.AddScoped<IPayMobRepository,PayMobRepository>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	options.User.RequireUniqueEmail = true;
	options.User.AllowedUserNameCharacters = null;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();


builder.Services.AddIdentityCore<Customer>()
	.AddRoles<IdentityRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Customer, IdentityRole>>()
	.AddEntityFrameworkStores<DataContext>()
	.AddDefaultTokenProviders();

builder.Services.AddIdentityCore<Provider>()
	.AddRoles<IdentityRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Provider, IdentityRole>>()
	.AddEntityFrameworkStores<DataContext>()
	.AddDefaultTokenProviders();


// make [Authorize] check for token instead of cookie
builder.Services.AddAuthentication(options =>{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>{
	options.SaveToken = true;
	options.RequireHttpsMetadata = true;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
		ValidateAudience = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
	};
}).AddGoogle(googleOptions =>
{
	googleOptions.ClientId = builder.Configuration["Google:ClientId"];
	googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline (middleware).
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
