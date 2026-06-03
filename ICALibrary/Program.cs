
using DimainLayer.Contex;
using DimainLayer.Repository;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using ServiceLayer.BusinesLayer;

var builder = WebApplication.CreateBuilder(args);

// فعال کردن AllowSynchronousIO در Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibraryDBContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMemberShipCardRepository,MemberShipCardRepository>();
builder.Services.AddScoped<IReservationRepository,ReservationRepository>();
builder.Services.AddScoped<IBookRepository,BookRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMemberShipCardService, MembershipCardService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();    
builder.Services.AddScoped<IBookService, BookService>();    
builder.Services.AddScoped<IReservationService, ReservationService>();

//چک کردن اتوماتیک دیرکرد
builder.Services.AddScoped<OverdueCheckerService>();
//builder.Services.AddHostedService<OverdueBackgroundService>();


builder.Services.AddDistributedMemoryCache(); // برای ذخیره‌سازی موقت
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // مدت زمان اعتبار Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();


app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
