using Microsoft.EntityFrameworkCore;
using RecordApp.Api.Models;

namespace RecordApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Note> Notes => Set<Note>();
}