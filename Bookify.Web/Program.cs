using Bookify.Data;
using Bookify.Data.Entities;
using Bookify.Data.Repositories;
using Bookify.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== Serilog =====
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Console()
	.CreateLogger();
builder.Host.UseSerilog();

// ===== MVC + Razor + Session =====
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHealthChecks();

// ===== DbContext =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? "Data Source=DESKTOP-N3NU5F1\\MSSQL;Initial Catalog=PrHotel1;Integrated Security=True;Encrypt=False;Trust Server Certificate=True";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

// ===== Identity =====
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// ===== UnitOfWork + Repositories + Services =====
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// ===== Stripe =====
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IStripeService, StripeService>();

var app = builder.Build();

// ===== Seed Roles & Users =====
using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

	foreach (var role in new[] { "Admin", "Customer" })
	{
		if (!await roleManager.RoleExistsAsync(role))
		{
			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}

	var adminEmail = "admin@bookify.com";
	var customerEmail = "user@bookify.com";

	var admin = await userManager.FindByEmailAsync(adminEmail);
	if (admin == null)
	{
		admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "Admin" };
		await userManager.CreateAsync(admin, "Admin123!");
		await userManager.AddToRoleAsync(admin, "Admin");
	}

	var customer = await userManager.FindByEmailAsync(customerEmail);
	if (customer == null)
	{
		customer = new ApplicationUser { UserName = customerEmail, Email = customerEmail, EmailConfirmed = true, FullName = "Customer" };
		await userManager.CreateAsync(customer, "User123!");
		await userManager.AddToRoleAsync(customer, "Customer");
	}
}

// ===== Middleware =====
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHealthChecks("/health");
app.MapRazorPages();

app.Run();
