using DBData.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBData
{
    public static class NorthwindContextExtensions
    {
        public static IServiceCollection AddNorthwindContext(this IServiceCollection services, string realativePath=".")
        {
            string databasePath = System.IO.Path.Combine(realativePath, "Northwind.db");
            services.AddDbContext<NorthWindContext>(options =>
                options.UseSqlite($"Data Source{databasePath}"));
            return services;
        }
    }
}
