using Microsoft.EntityFrameworkCore;
using RecordApp.Api.Data;
using RecordApp.Api.Models;
using RecordApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// --------------------
// DATABASE
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!);

// --------------------
// SERVICES
// --------------------
builder.Services.AddScoped<NoteService>();

// --------------------
// CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());
});

var app = builder.Build();

app.MapHealthChecks("/health/live");

app.MapHealthChecks("/health/ready");

// --------------------
// ENVIRONMENT PIPELINE
// --------------------

// Dev-only behavior
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Running in DEVELOPMENT mode");
}

// Create DB only in dev (opcional pero recomendado)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// --------------------
// RUN (CLEAN)
// --------------------
app.Run();