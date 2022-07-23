using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CASS___Construction_Assistance.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CASS___Construction_Assistance.Data
{
    public class CassUserContext : IdentityDbContext<User>
    {
        public CassUserContext(DbContextOptions<CassUserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
