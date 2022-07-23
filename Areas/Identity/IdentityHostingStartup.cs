using System;
using CASS___Construction_Assistance.Areas.Identity.Data;
using CASS___Construction_Assistance.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CASS___Construction_Assistance.Areas.Identity.IdentityHostingStartup))]
namespace CASS___Construction_Assistance.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CassUserContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CassUserContextConnection")));

                services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<CassUserContext>();
            });
        }
    }
}