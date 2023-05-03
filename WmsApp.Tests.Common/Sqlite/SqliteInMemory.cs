using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WmsApp.Persistence.Items.Tests.Integration.SqlLite
{
    public class SqliteInMemory<TContext, TOptions> : IDisposable
        where TContext : TOptions
        where TOptions : DbContext
    {
        SqliteConnection _connection;
        DbContextOptions<TOptions> _options;

        public SqliteInMemory()
        {
            _connection = new SqliteConnection();
            _connection.Open();

            _options = new DbContextOptionsBuilder<TOptions>()
                .UseSqlite(_connection)
                .Options;
        }

        public TContext GetContext()
        {
            return (TContext)Activator.CreateInstance(typeof(TContext), new object[] { _options });
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
