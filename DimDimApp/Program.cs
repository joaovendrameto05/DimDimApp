using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// BLOCO DE DIAGNÓSTICO: Impede que a API feche se o banco falhar
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Database.CanConnect())
    {
        Console.WriteLine("AVISO: A API subiu, mas NÃO conseguiu conectar ao banco da Azure.");
    }
    else
    {
        Console.WriteLine("SUCESSO: Conexão com banco da Azure estabelecida!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERRO CRÍTICO NA PARTIDA: {ex.Message}");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

app.MapPost("/categorias", async (Categoria cat, AppDbContext db) => {
    db.Categorias.Add(cat);
    await db.SaveChangesAsync();
    return Results.Created($"/categorias/{cat.Id}", cat);
});

app.MapPut("/categorias/{id}", async (int id, Categoria cat, AppDbContext db) => {
    var existing = await db.Categorias.FindAsync(id);
    if (existing == null) return Results.NotFound();
    existing.Nome = cat.Nome;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/categorias/{id}", async (int id, AppDbContext db) => {
    var cat = await db.Categorias.FindAsync(id);
    if (cat == null) return Results.NotFound();
    db.Categorias.Remove(cat);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/todos", async (AppDbContext db) => await db.Todos.ToListAsync());

app.MapPost("/todos", async (Todo todo, AppDbContext db) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (int id, Todo todo, AppDbContext db) => {
    var existing = await db.Todos.FindAsync(id);
    if (existing == null) return Results.NotFound();
    existing.Title = todo.Title;
    existing.Description = todo.Description;
    existing.IsComplete = todo.IsComplete;
    existing.CategoriaId = todo.CategoriaId;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (int id, AppDbContext db) => {
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Todo> Todos => Set<Todo>();
}

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<Todo> Todos { get; set; } = new();
}
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
