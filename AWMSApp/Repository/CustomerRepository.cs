using DBData.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace AWMSApp.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly NorthWindContext dbContext;
        private static ConcurrentDictionary<string, Customer>? customersCache;
        
        public CustomerRepository(NorthWindContext dbContext)
        {
            this.dbContext = dbContext;
            if (customersCache == null)
            {
                customersCache = new ConcurrentDictionary<string, Customer>(dbContext.Customers.ToDictionary(c => c.CustomerId));
            }
        }

        private Customer UpdateCache(string key, Customer customer)
        {
            if (customersCache == null) return customer;
            if (customersCache.TryGetValue(key, out Customer? oldCustomer))
            {
                if (customersCache.TryUpdate(key, customer, oldCustomer))
                {
                    return customer;
                }
            }
            return null;
        }
        public async Task<Customer?> CreateAsync(Customer customer)
        {
            customer.CustomerId = customer.CustomerId.ToUpper();
            EntityEntry<Customer> newEntry= await dbContext.Customers.AddAsync(customer);
            int rows = await dbContext.SaveChangesAsync();
            if (rows > 0)
            {
                if (customersCache == null) return customer;

                return customersCache.AddOrUpdate(customer.CustomerId, customer, UpdateCache);
            } else
            {
                return null;
            }

        }

        public async Task<Customer?> DeleteAsync(string id)
        {
            id = id.ToUpper();
            Customer? customer = dbContext.Customers.Find(id);
            if (customer is null)
                return null;
            dbContext.Customers.Remove(customer);
            int rows = await dbContext.SaveChangesAsync();
            if (rows > 0)
            {
                if (customersCache == null) return null;
                customersCache.TryRemove(id, out _);
                return customer;
            }
            else
            {
                return null;
            }
        }

        public Task<Customer?> GetCustomerAsync(string id)
        {
            id = id.ToUpper();
            if (customersCache == null) return null;
            customersCache.TryGetValue(id, out Customer? customer) ;
            return Task.FromResult(customer);
        }

        public Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return Task.FromResult(customersCache == null ? Enumerable.Empty<Customer>() : customersCache.Values);
        }

        public Task<Customer?> UpdateAsync(string id, Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Customer>> FindCustomer(string querySearch)
        {
            if (string.IsNullOrWhiteSpace(querySearch))
                return await dbContext.Customers.ToListAsync();

            var parameter = Expression.Parameter(typeof(Customer), "c");
            Expression? combinedExpression = null;

            var criteria = querySearch.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var criterio in criteria)
            {
                var parts = criterio.Split(':');
                if (parts.Length != 2)
                    continue;

                var property = parts[0].Trim();
                var value = parts[1].Trim();

                var propertyInfo = typeof(Customer).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    continue;

                var propertyAccess = Expression.Property(parameter, propertyInfo);
                var valueExpression = Expression.Constant(value);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var containsExpression = Expression.Call(propertyAccess, containsMethod!, valueExpression);
                combinedExpression = combinedExpression == null
                            ? containsExpression
                            : Expression.AndAlso(combinedExpression, containsExpression);
            }

            if (combinedExpression == null)
                return Enumerable.Empty<Customer>();
            var lambda = Expression.Lambda<Func<Customer, bool>>(combinedExpression, parameter);

            return await dbContext.Customers.Where(lambda).ToListAsync();
        }
    }
}
