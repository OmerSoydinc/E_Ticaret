using Bogus;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Context;
using MiniETicaret.Products.WebAPI.Dtos;
using MiniETicaret.Products.WebAPI.Models;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/seedData", (AplicationDbContext context) =>
{
    for(int i = 0; i < 100; i++)
    {
        Faker faker = new Faker();

        Product product = new()
        {
            Name = faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(faker.Commerce.Price()),
            Stock = faker.Commerce.Random.Int(1,100)   //1 100 aras� random �retir
            //Name = "",
            //Price =0,
            //Stock=0
        };
        context.Products.Add(product);
    }
    context.SaveChanges();

    return Results.Ok(Result<string>.Succeed("Seed (Random) Data Ba�ar�yla �al��t�r�ld�..."));
});

app.MapGet("/getall", async (AplicationDbContext context, CancellationToken cancellationToken) =>
{
    var products = await context.Products.OrderBy(p => p.Name).ToListAsync(cancellationToken);

    Result<List<Product>> response = products;
    return response;
});

app.MapPost("/create", async (CreateProductDto request ,AplicationDbContext context, CancellationToken cancellationToken) =>
{
    bool isNameExists = await context.Products.AnyAsync(p => p.Name == request.Name, cancellationToken);

    if(isNameExists)
    {
        var response = Result<string>.Failure("�r�n Ad� Daha �nce Olu�turulmu�...");
        return Results.BadRequest(response);
    }

    Product product = new()
    {
        Name = request.Name,
        Price = request.Price,
        Stock = request.Stock,
    };

    await context.AddAsync(product, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(Result<string>.Succeed("�r�n Kayd� Ba�ar�yla Olu�turuldu."));
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<AplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
