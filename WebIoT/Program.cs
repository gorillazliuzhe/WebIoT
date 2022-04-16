using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Device.Gpio;
using WebIoT.Filter;
using WebIoT.Hubs;
using WebIoT.Models;
using WebIoT.Playground;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFile();

// Add services to the container.
builder.Services.Configure<SiteConfig>(builder.Configuration.GetSection("SiteConfig"));
builder.Services.AddSingleton(_ => new GpioController(PinNumberingScheme.Logical));
builder.Services.AddSingleton<ILedClient, LedClient>();
builder.Services.AddSingleton<IHJR2Client, HJR2Client>();
builder.Services.AddSingleton<IL298NClient, L298NClient>();
builder.Services.AddSingleton<IHcsr04Client, Hcsr04Client>();
builder.Services.AddLoggingFileUI();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=car}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.MapRazorPages(); // ÈÕÖ¾
app.Run();

//namespace WebIoT
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                    webBuilder.UseUrls("http://localhost:5000/");
//                    webBuilder.ConfigureLogging(builder => builder.AddFile());
//                });
//    }
//}
