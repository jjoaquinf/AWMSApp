// See https://aka.ms/new-console-template for more information
using WorkinfWithEFCore.shared;
using Microsoft.EntityFrameworkCore;
using WorkinfWithEFCore.repo.model;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using WorkinfWithEFCore.repo;
using static System.Console;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


Console.WriteLine("Hello, World!");
Console.WriteLine($"Usign {ProjectConstants.DatabaseProvider} as database");
CheckConnection();
//QueryingCategories();
QueryingCategories2();

static void CheckConnection()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());
        WriteLine("Numero de categorias: ");
        IQueryable<Category>? categories = db.Categories;
                //.Include(categories => categories.Products);

        if (categories is null)
        {
            WriteLine("No encontradas categorias");
            return;
        }
        foreach (Category c in categories)
        {
            if (c != null)
            {
                WriteLine($"{c.CategoryName} tiene un total de: {c.Products.Count} productos");
            }
        }
    }
}

static void QueryingCategories()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());
        WriteLine("Prodctos que con precio superior a.");
        string? value;
        decimal price;
        bool loop = false;
        do
        {
            Write("Introduzca un precio: ");
            value= ReadLine();
            loop = decimal.TryParse(value, out price);

        } while (!loop);
        IQueryable<Product> products = db.Products.Where(q => q.Cost > price).OrderByDescending(p => p.Cost);
        if (products is null)
        {
            WriteLine("No encontrado");
            return;
        }
        WriteLine($"Query {products.ToQueryString()}");
        foreach (Product p in products) {
            WriteLine($"{p.ProductId}: {p.ProductName} tiene un precio de {p.Cost} y un stock de {p.Stock}. Discotinuado: {p.Discontinued}");
        }
    }
}

static void QueryingCategories2()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());
        WriteLine("Prodctos que con precio superior a.");
        string? value;
        decimal price;
        bool loop = false;
        do
        {
            Write("Introduzca un nombre: ");
            value = ReadLine();

        } while (value is null);
        IQueryable<Product> products = db.Products.Where(q => q.ProductName.Contains(value)).OrderByDescending(p => p.Cost);
        if (products is null)
        {
            WriteLine("No encontrado");
            return;
        }
        WriteLine($"Query {products.ToQueryString()}");
        foreach (Product p in products)
        {
            WriteLine($"{p.ProductId}: {p.ProductName} tiene un precio de {p.Cost} y un stock de {p.Stock} Discotinuado: {p.Discontinued}");
        }
    }
}