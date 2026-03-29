using Microsoft.EntityFrameworkCore;
using RecordApp.Api.Data;
using RecordApp.Api.Models;
using RecordApp.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<NoteService>();


var app = builder.Build();

// Create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

app.MapGet("/notes", async (NoteService svc) =>
    Results.Ok(await svc.GetAllAsync()));

app.MapPost("/notes", async (Note note, NoteService svc) =>
{
    try
    {
        var created = await svc.CreateAsync(note);
        return Results.Created($"/notes/{created.Id}", created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/notes/{id}", async (int id, Note updated, NoteService svc) =>
{
    try
    {
        var note = await svc.UpdateAsync(id, updated);
        return note is null ? Results.NotFound() : Results.Ok(note);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/notes/{id}", async (int id, NoteService svc) =>
{
    var deleted = await svc.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");