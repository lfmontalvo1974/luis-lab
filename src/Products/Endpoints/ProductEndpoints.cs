using DataEntities;
using Microsoft.EntityFrameworkCore;
using Products.Data;

namespace Products.Endpoints;

/// <summary>
/// Group of endpoints for managing products in the application. This class defines the routes and handlers for CRUD operations on products, including retrieving all products, retrieving a product by ID, creating a new product, updating an existing product, and deleting a product.
/// </summary>
public static class ProductEndpoints
{
    public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product");

        group.MapGet("/", async (ProductDataContext db) =>
        {
            return await db.Product.ToListAsync();
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, ProductDataContext db) =>
        {
            return await db.Product.FindAsync(id)
                is Product model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (Product product, ProductDataContext db) =>
        {
            db.Product.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Product/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapPut("/{id}", async (int id, Product product, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Name, product.Name)
                    .SetProperty(m => m.Description, product.Description)
                    .SetProperty(m => m.Price, product.Price)
                    .SetProperty(m => m.ImageUrl, product.ImageUrl)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", async (int id, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
