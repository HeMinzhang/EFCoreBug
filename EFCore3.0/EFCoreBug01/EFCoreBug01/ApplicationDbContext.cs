using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreBug01
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CdKey> CdKeys { get;set; }
        public DbSet<KeyValue> keyValues { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlConnection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\loong\Documents\EFCoreBug01.mdf;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;MultiSubnetFailover=True";
            optionsBuilder.UseSqlServer(sqlConnection);
        }
    }
}
