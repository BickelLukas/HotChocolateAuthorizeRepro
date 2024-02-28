using HotChocolate.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("BookPolicy", b => b.RequireAssertion(x => true));
    c.AddPolicy("AuthorPolicy", b => b.RequireAssertion(x => false));
});

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddGlobalObjectIdentification();

var app = builder.Build();

app.UseAuthorization();

app.MapGraphQL();

app.Run();

public class Query
{
    private static List<Author> authors = [new Author() { Id = 1, Name = "Greg" }, new Author() { Id = 2, Name = "George" }];

    private static List<Book> books = [new Book() { Id = 1, Title = "Gregs Book", Author = authors[0] }, new Book() { Id = 2, Title = "Georges Book", Author = authors[1] }];

    public IEnumerable<Book> Books => books;
    [NodeResolver]
    public Book? Book(int id) => books.SingleOrDefault(x => x.Id == id);

    public IEnumerable<Author> Authors => authors;
    [NodeResolver]
    public Author? Author(int id) => authors.SingleOrDefault(x => x.Id == id);
}

[Authorize("BookPolicy", ApplyPolicy.Validation)]
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Author Author { get; set; }
}

[Authorize("AuthorPolicy", ApplyPolicy.Validation)]
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}