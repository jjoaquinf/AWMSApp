namespace AWMSApp.Repository
{
    using DBData.model;
    public interface ICustomerRepository

    {
        Task<Customer?> CreateAsync(Customer customer);
        Task<Customer?> DeleteAsync(string id);
        Task<Customer?> GetCustomerAsync(string id);
        Task<Customer?> UpdateAsync(string id, Customer customer);
        Task<IEnumerable<Customer>> GetCustomersAsync();

        Task<IEnumerable<Customer>> FindCustomer(string querySearch);
    }
}

