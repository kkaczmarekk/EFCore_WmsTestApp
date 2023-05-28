using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Persistence.Common.Db.EventRunner;

namespace WmsApp.Persistence.Items
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ItemDbContext>
    {
        private string _connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=WmsApp_Test;Trusted_Connection=true;Trust Server Certificate=true";

        public ItemDbContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<ItemDbContext>();

            optionBuilder.UseSqlServer(_connectionString, dbOptions =>
                dbOptions.MigrationsHistoryTable("_ItemMigrationHistoryTable"));

            return new ItemDbContext(optionBuilder.Options, new EventRunner());
        }
    }
}
