using Microsoft.EntityFrameworkCore;
using RecordApp.Api.Data;
using RecordApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=records.db"));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

// POST /notes — Create a note
app.MapPost("/notes", async (Note note, AppDbContext db) =>
{
    note.CreatedAt = DateTime.UtcNow;
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/notes/{note.Id}", note);
});

// GET /notes — List all notes
app.MapGet("/notes", async (AppDbContext db) =>
{
    var notes = await db.Notes
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
    return Results.Ok(notes);
});

// DELETE /notes/{id} — Delete a note
app.MapDelete("/notes/{id}", async (int id, AppDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();

    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// PUT /notes/{id} — Edit a note (optional MVP)
app.MapPut("/notes/{id}", async (int id, Note updated, AppDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();

    note.Title = updated.Title;
    note.Content = updated.Content;
    note.Color = updated.Color;

    await db.SaveChangesAsync();
    return Results.Ok(note);
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");