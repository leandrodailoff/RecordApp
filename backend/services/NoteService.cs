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
        note.CreatedAt = DateTime.UtcNow;
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }

    public async Task<Note?> UpdateAsync(int id, Note updated)
    {
        var note = await db.Notes.FindAsync(id);
        if (note is null) return null;

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
}