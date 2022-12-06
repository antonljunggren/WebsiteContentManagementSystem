using Core.Persistance;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var cosmoDbConn = builder.Configuration.GetConnectionString("cosmo-connection") ?? "";
            var cosmoDbName = builder.Configuration["cosmo-db-name"] ?? "";

            builder.Services.AddPooledDbContextFactory<CmsContext>(opt =>
                opt.UseCosmos(cosmoDbConn, cosmoDbName));

            builder.Services.AddSingleton<ImageProcessingService>();

            builder.Services.AddScoped<PhotographService>();

            var blobConn = builder.Configuration.GetConnectionString("blob-storage-connection") ?? "";
            var blobContainer = builder.Configuration["blob-storage-container"] ?? "";

            builder.Services.AddSingleton(typeof(AzureBlobFileService), new AzureBlobFileService(blobConn, blobContainer));

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}