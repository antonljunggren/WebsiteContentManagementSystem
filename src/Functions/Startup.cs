using Core.Persistance;
using Core.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Functions.Startup))]

namespace Functions
{
    public sealed class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var cosmoDbConn = Environment.GetEnvironmentVariable("ConnectionStrings:cosmo-connection") ?? "";
            var cosmoDbName = Environment.GetEnvironmentVariable("cosmo-db-name") ?? "";

            var blobConn = Environment.GetEnvironmentVariable("ConnectionStrings:blob-storage-connection") ?? "";
            var blobContainer = Environment.GetEnvironmentVariable("blob-storage-container") ?? "";

            builder.Services.AddPooledDbContextFactory<CmsContext>(opt =>
               opt.UseCosmos(cosmoDbConn, cosmoDbName));

            builder.Services.AddSingleton<ImageProcessingService>();

            builder.Services.AddScoped<PhotographService>();

            builder.Services.AddSingleton(typeof(AzureBlobFileService), new AzureBlobFileService(blobConn, blobContainer));
        }
    }
}
