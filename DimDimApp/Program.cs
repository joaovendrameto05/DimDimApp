using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

var todoApi = app.MapGroup("/todos");

todoApi.MapGet("/", async (AppDbContext db) =>
    await db.Todos.Include(t => t.Categoria).ToListAsync());

todoApi.MapGet("/{id}", async (int id, AppDbContext db) =>
    await db.Todos.Include(t => t.Categoria).FirstOrDefaultAsync(t => t.Id == id)
    is Todo todo ? Results.Ok(todo) : Results.NotFound());

todoApi.MapPost("/", async (Todo todo, AppDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

todoApi.MapPut("/{id}", async (int id, Todo inputTodo, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = inputTodo.Title;
    todo.DueBy = inputTodo.DueBy;
    todo.IsComplete = inputTodo.IsComplete;
    todo.CategoriaId = inputTodo.CategoriaId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

todoApi.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }
    return Results.NotFound();
});

var catApi = app.MapGroup("/categorias");

catApi.MapGet("/", async (AppDbContext db) =>
    await db.Categorias.ToListAsync());

catApi.MapGet("/{id}", async (int id, AppDbContext db) =>
    await db.Categorias.FindAsync(id) is Categoria cat ? Results.Ok(cat) : Results.NotFound());

catApi.MapPost("/", async (Categoria cat, AppDbContext db) =>
{
    db.Categorias.Add(cat);
    await db.SaveChangesAsync();
    return Results.Created($"/categorias/{cat.Id}", cat);
});

catApi.MapPut("/{id}", async (int id, Categoria inputCat, AppDbContext db) =>
{
    var cat = await db.Categorias.FindAsync(id);
    if (cat is null) return Results.NotFound();

    cat.Nome = inputCat.Nome;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

catApi.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Categorias.FindAsync(id) is Categoria cat)
    {
        db.Categorias.Remove(cat);
        await db.SaveChangesAsync();
        return Results.Ok(cat);
    }
    return Results.NotFound();
});

app.Run();
