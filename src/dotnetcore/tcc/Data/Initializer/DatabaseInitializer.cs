using Data.Context;
using Entity;
using System;
using System.Linq;

namespace Data.Initializer
{
    public class DatabaseInitializer
    {
        private readonly ProductModelContext _context;
        public DatabaseInitializer(ProductModelContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();

            if (_context.Content.Any())
            {
                return;   // DB is ready
            }

            for (int i = 1; i <= 1000; i++)
            {
                _context.Content.Add(new ProductModel { Name = $"Product Name {i}", LastUpdatedTime = DateTime.Now });
            }

            _context.SaveChanges();
        }
    }
}
