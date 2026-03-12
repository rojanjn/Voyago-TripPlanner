using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Models;

namespace TripPlanner.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();
    public DbSet<ItineraryItem> ItineraryItems => Set<ItineraryItem>();
    public DbSet<Location> Locations => Set<Location>();
    // public DbSet<Phrase> Phrases => Set<Phrase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        // === Table name mapping part ===
        modelBuilder.Entity<Country>().ToTable("countries");
        modelBuilder.Entity<Itinerary>().ToTable("itineraries");
        modelBuilder.Entity<ItineraryItem>().ToTable("itinerary_items");
        modelBuilder.Entity<Location>().ToTable("locations");
        // modelBuilder.Entity<Phrase>().ToTable("phrases");
        
        // === Relationship mapping part ===
        modelBuilder.Entity<Itinerary>()
            .HasOne(i => i.User)
            .WithMany(u => u.Itineraries)
            .HasForeignKey(I => I.UserId)
            // Cascade => Delete related itineraryitems
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Itinerary>()
            .HasOne(i => i.Country)
            .WithMany(c => c.Itineraries)
            .HasForeignKey(I => I.CountryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItineraryItem>()
            .HasOne(t => t.Itinerary)
            .WithMany(i => i.ItineraryItems)
            .HasForeignKey(I => I.ItineraryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItineraryItem>()
            .HasOne(i => i.Location)
            .WithMany(l => l.ItineraryItems)
            .HasForeignKey(i => i.LocationId)
            // If there are other itineraryitem are using this location, then the location won't be deleted
            .OnDelete(DeleteBehavior.Restrict);
        /*
        modelBuilder.Entity<Phrase>()
            .HasOne(p => p.Country)
            .WithMany(c => c.Phrases)
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
        */
        
        // === Add Seed Data Part ===
        
        modelBuilder.Entity<Country>().HasData(
            new Country
            {
                CountryId = 1,
                CountryName = "Canada",
                CountryLanguage = "English"
            },
            new Country
            {
                CountryId = 2,
                CountryName = "Iran",
                CountryLanguage = "Persian (Farsi)"
            },
            new Country
            {
                CountryId = 3,
                CountryName = "China",
                CountryLanguage = "Mandarin (Simplify Chinese)"
            },
            new Country
            {
                CountryId = 4,
                CountryName = "Taiwan",
                CountryLanguage = "Mandarin (Traditional Chinese)"
            }
        );
        
        modelBuilder.Entity<Location>()
            .HasData(
                new Location { 
                    Id = 1, 
                    Name = "CN Tower", 
                    Address = "290 Bremner Blvd, Toronto, ON M5V 3L9",
                    Latitude = 43.6425662m, 
                    Longitude = -79.3870568m
                    
                }, 
                new Location { 
                    Id = 2, 
                    Name = "Naqsh-e Jahan Square",
                    Address = "Isfahan, Isfahan Province, Iran",
                    Latitude = 32.65745m, 
                    Longitude = 51.677778m
                    
                },
                new Location { 
                    Id = 3, 
                    Name = "Sun Moon Lake", 
                    Address = "Yuchi Township, Nantou County, Taiwan",
                    Latitude = 23.866667m, 
                    Longitude = 120.916667m
                    
                },
                new Location { 
                    Id = 4, 
                    Name = "Fenghuang Ancient City", 
                    Address = "Tuojiang Town, Fenghuang County, Xiangxi Tujia and Miao Autonomous Prefecture of Hunan Province",
                    Latitude = 27.952822m, 
                    Longitude = 109.600989m
                }
            );
        
            /*
            modelBuilder.Entity<Phrase>().HasData(
                new Phrase {
                    PhraseId = 1,
                    CountryId = 1,
                    Content = "Hello",
                    Translation = "Hello"
                },
                new Phrase {
                    PhraseId = 2,
                    CountryId = 1,
                    Content = "Thank you",
                    Translation = "Thank you"
                },
                new Phrase {
                    PhraseId = 3,
                    CountryId = 1,
                    Content = "Où sont les toilettes ?",
                    Translation = "Where is the restroom?"
                },
                new Phrase {
                    PhraseId = 4,
                    CountryId = 1,
                    Content = "Combien ça coûte ?",
                    Translation = "How much is it?"
                },
                new Phrase {
                    PhraseId = 5,
                    CountryId = 2,
                    Content = "درود",
                    Translation = "Hello"
                },
                new Phrase {
                    PhraseId = 6,
                    CountryId = 2,
                    Content = "ممنونم",
                    Translation = "Thank you"
                },
                new Phrase {
                    PhraseId = 7,
                    CountryId = 2,
                    Content = "سرویس بهداشتی کجاست؟",
                    Translation = "Where is the restroom?"
                },
                new Phrase {
                    PhraseId = 8,
                    CountryId = 2,
                    Content = "این چقدر است؟",
                    Translation = "How much is it?"
                },
                new Phrase {
                    PhraseId = 9,
                    CountryId = 3,
                    Content = "你好",
                    Translation = "Hello"
                },
                new Phrase {
                    PhraseId = 10,
                    CountryId = 3,
                    Content = "谢谢",
                    Translation = "Thank you"
                },
                new Phrase {
                    PhraseId = 11,
                    CountryId = 3,
                    Content = "请问卫生间在哪？",
                    Translation = "Where is the restroom?"
                },
                new Phrase {
                    PhraseId = 12,
                    CountryId = 3,
                    Content = "这个多少钱？",
                    Translation = "How much is it?"
                },
                new Phrase {
                    PhraseId = 13,
                    CountryId = 4,
                    Content = "こんにちは",
                    Translation = "Hello"
                },
                new Phrase {
                    PhraseId = 14,
                    CountryId = 4,
                    Content = "ありがとう",
                    Translation = "Thank you"
                }
            );
            */
        
    }
}