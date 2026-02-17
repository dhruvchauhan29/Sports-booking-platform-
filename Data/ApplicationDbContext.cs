using Microsoft.EntityFrameworkCore;
using SportsBookingPlatform.Entities;

namespace SportsBookingPlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GamePlayer> GamePlayers { get; set; }
    public DbSet<Waitlist> Waitlists { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Role).HasConversion<string>();
            
            entity.HasOne(e => e.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);
        });

        // Venue configuration
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId);
            entity.Property(e => e.ApprovalStatus).HasConversion<string>();
            
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Venues)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Court configuration
        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId);
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Venue)
                .WithMany(v => v.Courts)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Discount configuration
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId);
            entity.Property(e => e.Scope).HasConversion<string>();
            entity.Property(e => e.PercentOff).HasPrecision(5, 2);
            
            entity.HasOne(e => e.Venue)
                .WithMany(v => v.Discounts)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Court)
                .WithMany(c => c.Discounts)
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Slot configuration
        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.CurrentPrice).HasPrecision(18, 2);
            entity.HasIndex(e => new { e.CourtId, e.StartTime, e.EndTime });
            
            entity.HasOne(e => e.Court)
                .WithMany(c => c.Slots)
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.FinalPrice).HasPrecision(18, 2);
            entity.Property(e => e.RefundAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.IdempotencyKey).IsUnique();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Slot)
                .WithMany(s => s.Bookings)
                .HasForeignKey(e => e.SlotId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Wallet configuration
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
        });

        // WalletTransaction configuration
        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
            entity.HasIndex(e => e.IdempotencyKey).IsUnique();
            
            entity.HasOne(e => e.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(e => e.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId);
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.OwnedGames)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GamePlayer configuration
        modelBuilder.Entity<GamePlayer>(entity =>
        {
            entity.HasKey(e => e.GamePlayerId);
            entity.HasIndex(e => new { e.GameId, e.UserId }).IsUnique();
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.GamePlayers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Waitlist configuration
        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => e.WaitlistId);
            entity.HasIndex(e => new { e.GameId, e.UserId }).IsUnique();
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Waitlist)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Rating configuration
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.RatedUser)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(e => e.RatedUserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Venue)
                .WithMany(v => v.Ratings)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Court)
                .WithMany(c => c.Ratings)
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
