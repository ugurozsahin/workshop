using Data.Context;
using Entity;
using System;
using System.Linq;

namespace Data.Initializer
{
    public static class DbInitializer
    {
        public static void Initialize(ContentContext context)
        {
            context.Database.EnsureCreated();

            // Look for any contents.
            if (context.Content.Any())
            {
                return;   // DB is ready
            }

            var contents = new Content[]
            {
                new Content{Title="Content Title",Body="Content Body", CreatedOn = DateTime.Parse("2017-10-01"), Status = 1},
                new Content{Title="Content Title 2",Body="Content Body 2", CreatedOn = DateTime.Parse("2017-10-01"), Status = 1},
            };

            foreach (Content content in contents)
            {
                context.Content.Add(content);
            }

            context.SaveChanges();
        }
    }
}
