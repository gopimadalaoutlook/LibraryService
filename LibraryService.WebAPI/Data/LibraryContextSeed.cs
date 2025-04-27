namespace LibraryService.WebAPI.Data
{
    public class LibraryContextSeed
    {
        public static async Task SeedAsync(LibraryContext context, CancellationToken cancellationToken = default)
        {
            // Seed Library data if the table is empty
            if (!context.Libraries.Any())
            {
                var libraries = new List<Library>
                {
                  new Library { Name = "Central Library", Location = "Downtown" },
                  new Library { Name = "East Branch", Location = "Eastside" },
                  new Library { Name = "West Branch", Location = "Westside" }
                };

                await context.Libraries.AddRangeAsync(libraries, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            // Seed Book data if the table is empty
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                   new Book { Name = "The Great Gatsby", Category = "Fiction", LibraryId = 1 },
                   new Book { Name = "1984", Category = "Dystopian", LibraryId = 1 },
                   new Book { Name = "To Kill a Mockingbird", Category = "Fiction", LibraryId = 2 },
                   new Book { Name = "The Catcher in the Rye", Category = "Fiction", LibraryId = 2 },
                   new Book { Name = "Moby Dick", Category = "Classic", LibraryId = 3 },
                   new Book { Name = "War and Peace", Category = "Historical Fiction", LibraryId = 3 }
                };

                await context.Books.AddRangeAsync(books, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
