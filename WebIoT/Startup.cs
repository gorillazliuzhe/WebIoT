using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unosquare.RaspberryIO;
using WebIoT.Hubs;
using WebIoT.Peripherals;
using WebIoT.Peripherals.AM2302;
using WebIoT.Peripherals.HJIR;
using WebIoT.Peripherals.L298N;

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

            services.AddSingleton<LedClient>();
            services.AddSingleton<L298NClient>();
            services.AddSingleton<AM2302Client>();
            services.AddSingleton<HJIR2LeftClient>();
            services.AddSingleton<HJR2RightClient>();
            services.AddSingleton<UltrasonicHcsr04Client>();            
            //services.AddSingleton<InfraredSensor>(_ => new InfraredSensor(Pi.Gpio[BcmPin.Gpio25], true));
            services.AddSignalR();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                Pi.Init<Unosquare.WiringPi.BootstrapWiringPi>();
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
                    pattern: "{controller=Car}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
