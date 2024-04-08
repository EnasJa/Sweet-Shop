using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Add session services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession(); // Enable session middleware


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = null;
//        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
//    });
//builder.Services.AddSession(); // Add session services
//builder.Services.AddHttpContextAccessor();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.UseSession(); // Enable session middleware


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

