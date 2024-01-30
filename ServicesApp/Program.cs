using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
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
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceOfferRepository, ServiceOfferRepository>();
builder.Services.AddScoped<ITimeSlotsRepository , TimeSlotRepositry>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

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


//[Authoriz] used JWT Token in Chck Authantiaction
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["JWT:ValidAudience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
	};
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder => {
		builder.AllowAnyOrigin();
		builder.AllowAnyMethod();
		builder.AllowAnyHeader();
	});
});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });
});

builder.Services.AddSwaggerGen(swagger =>
{
	//This is to generate the Default UI of Swagger Documentation    
	swagger.SwaggerDoc("v2", new OpenApiInfo
	{
		Version = "v1",
		Title = "Linkup App",
		Description = "Linkup"
	});

	// To Enable authorization using Swagger (JWT)    
	swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
	});

	swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		 {
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		 }
	});
});

var app = builder.Build();


// Configure the HTTP request pipeline (middleware).
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo v1"));
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
