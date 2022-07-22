using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CASS___Construction_Assistance.Data
{
    public class CASSContext : DbContext
    {
        public CASSContext(DbContextOptions<CASSContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Project> Project { get; set; }
    }
}