using System;
using Microsoft.EntityFrameworkCore;

class WeatherContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<WeatherRecord> WeatherRecords { get; set; }

    public WeatherContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=weather.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relacja: City 1 --- * WeatherRecord
        modelBuilder.Entity<WeatherRecord>()
            .HasOne(r => r.City)
            .WithMany(c => c.WeatherRecords)
            .HasForeignKey(r => r.CityId);
    }
}
