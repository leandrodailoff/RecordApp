using Microsoft.EntityFrameworkCore;
using RecordApp.Api.Data;
using RecordApp.Api.Models;

namespace RecordApp.Services;

public class NoteService(AppDbContext db)
{
    public async Task<List<Note>> GetAllAsync() =>
        await db.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();

    public async Task<Note?> GetByIdAsync(int id) =>
        await db.Notes.FindAsync(id);

    public async Task<Note> CreateAsync(Note note)
    {
        Validate(note);
        note.CreatedAt = DateTime.UtcNow;
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }

    public async Task<Note?> UpdateAsync(int id, Note updated)
    {
        var note = await db.Notes.FindAsync(id);
        if (note is null) return null;

        Validate(updated);
        note.Title = updated.Title;
        note.Content = updated.Content;
        note.Color = updated.Color;

        await db.SaveChangesAsync();
        return note;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var note = await db.Notes.FindAsync(id);
        if (note is null) return false;

        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return true;
    }

    private static void Validate(Note note)
    {
        if (string.IsNullOrWhiteSpace(note.Title))
            throw new ArgumentException("Title is required.");

        if (note.Title.Length > 100)
            throw new ArgumentException("Title cannot exceed 100 characters.");

        if (note.Content.Length > 2000)
            throw new ArgumentException("Content cannot exceed 2000 characters.");
    }
}