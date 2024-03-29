using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Device.Gpio;
//using Unosquare.RaspberryIO;
using WebIoT.Filter;
using WebIoT.Hubs;
using WebIoT.Models;
using WebIoT.Playground;

namespace WebIoT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteConfig>(Configuration.GetSection("SiteConfig"));
            services.AddSingleton(_ => new GpioController(PinNumberingScheme.Logical));
            services.AddSingleton<ILedClient, LedClient>();
            services.AddSingleton<IHJR2Client, HJR2Client>();
            services.AddSingleton<IL298NClient, L298NClient>();
            services.AddSingleton<IHcsr04Client, Hcsr04Client>();
            services.AddLoggingFileUI();
            services.AddSignalR();
            services.AddRazorPages();
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });
            //.AddRazorRuntimeCompilation(); // 修改视图后刷新既显示效果 https://www.cnblogs.com/jiyuwu/p/11770241.html
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            try
            {
                //Pi.Init<Unosquare.WiringPi.BootstrapWiringPi>();
            }
            catch
            {

            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dth}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapRazorPages();
            });
            //appLifetime.ApplicationStopped.Register(() =>
            //{
            //    //获取ServiceProvider
            //    //var serviceProvider = services.BuildServiceProvider();
            //    var gpio = serviceProvider.GetService<GpioController>();
            //    gpio.Dispose();
            //});
        }
    }
}
