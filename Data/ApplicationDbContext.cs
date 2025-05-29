using Microsoft.EntityFrameworkCore;
using backendcafe.Models;

namespace backendcafe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
      public DbSet<Table> Tables { get; set; }
      public DbSet<TableReservation> TableReservations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(256);
                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasConversion<string>();
                entity.HasIndex(u => u.Username)
                      .IsUnique();
                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(b => b.BranchName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(b => b.Address)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.HasIndex(b => b.BranchName)
                      .IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(c => c.CategoryName)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(c => c.BranchId)
                      .IsRequired();
                entity.HasIndex(c => new { c.CategoryName, c.BranchId })
                      .IsUnique();
                entity.HasOne(c => c.Branch)
                      .WithMany()
                      .HasForeignKey(c => c.BranchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Stock)
                      .IsRequired();
                entity.Property(p => p.Price)
                      .IsRequired();
                entity.Property(p => p.Description)
                      .HasMaxLength(500);
                entity.Property(p => p.IsActive)
                      .IsRequired();
                entity.Property(p => p.CategoryId)
                      .IsRequired();
                entity.Property(p => p.BranchId)
                      .IsRequired();
                entity.HasIndex(p => new { p.Name, p.BranchId })
                      .IsUnique();
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Branch)
                      .WithMany()
                      .HasForeignKey(p => p.BranchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.OpeningTime)
                      .HasConversion(
                          timeOnly => timeOnly.ToTimeSpan(),
                          timeSpan => TimeOnly.FromTimeSpan(timeSpan));
                
                entity.Property(s => s.ClosingTime)
                      .HasConversion(
                          timeOnly => timeOnly.ToTimeSpan(),
                          timeSpan => TimeOnly.FromTimeSpan(timeSpan));
                entity.Property(s => s.WifiPassword).HasMaxLength(20);  
                entity.HasOne(s => s.Branch)
                      .WithOne()
                      .HasForeignKey<Setting>(s => s.BranchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cart>(entity => {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(c => c.Quantity)
                      .IsRequired();
                entity.HasOne(c => c.Branch)
                      .WithMany()
                      .HasForeignKey(c => c.BranchId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Product)
                      .WithMany()
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => new { c.ProductId, c.TransactionId })
                      .IsUnique();
            });

            modelBuilder.Entity<Transaction>(entity => {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnType("integer");
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.TransactionCode)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(t => t.Total)
                      .IsRequired();
                entity.Property(t => t.Status)
                      .IsRequired()
                      .HasMaxLength(20);
                   entity.Property(c => c.CreatedBy)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.CreatedAt)
                      .IsRequired();
                entity.HasOne(t => t.Branch)
                      .WithMany()
                      .HasForeignKey(t => t.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(t => t.TransactionCode)
                      .IsUnique();
            });

            modelBuilder.Entity<Table>(entity =>
            {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                  .ValueGeneratedOnAdd()
                  .HasColumnType("integer");
            entity.Property(t => t.TableNumber)
                  .IsRequired()
                  .HasMaxLength(20);
            entity.Property(t => t.Capacity)
                  .IsRequired();
            entity.Property(t => t.IsAvailable)
                  .IsRequired();
            entity.Property(t => t.BranchId)
                  .IsRequired();
            entity.Property(t => t.Description)
                  .HasMaxLength(200);
            entity.Property(t => t.CreatedAt)
                  .IsRequired();
            entity.Property(t => t.UpdatedAt)
                  .IsRequired();
            
            // Unique constraint untuk kombinasi TableNumber dan BranchId
            entity.HasIndex(t => new { t.TableNumber, t.BranchId })
                  .IsUnique();
            
            // Foreign key relationship
            entity.HasOne(t => t.Branch)
                  .WithMany()
                  .HasForeignKey(t => t.BranchId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TableReservation>(entity =>
{
    entity.HasKey(tr => tr.Id);
    entity.Property(tr => tr.Id)
          .ValueGeneratedOnAdd()
          .HasColumnType("integer");
    entity.Property(tr => tr.ReservationCode)
          .IsRequired()
          .HasMaxLength(50);
    entity.Property(tr => tr.CustomerName)
          .IsRequired()
          .HasMaxLength(100);
    entity.Property(tr => tr.CustomerPhone)
          .HasMaxLength(15);
    entity.Property(tr => tr.CustomerEmail)
          .HasMaxLength(100);
    entity.Property(tr => tr.TableId)
          .IsRequired();
    entity.Property(tr => tr.BranchId)
          .IsRequired();
    entity.Property(tr => tr.ReservationDateTime)
          .IsRequired();
    entity.Property(tr => tr.DurationHours)
          .IsRequired();
    entity.Property(tr => tr.GuestCount)
          .IsRequired();
    entity.Property(tr => tr.Status)
          .IsRequired()
          .HasConversion<string>();
    entity.Property(tr => tr.Notes)
          .HasMaxLength(500);
    entity.Property(tr => tr.CreatedBy)
          .IsRequired()
          .HasMaxLength(100);
    entity.Property(tr => tr.CreatedAt)
          .IsRequired();
    entity.Property(tr => tr.UpdatedAt)
          .IsRequired();
    
    // Unique constraint untuk ReservationCode
    entity.HasIndex(tr => tr.ReservationCode)
          .IsUnique();
    
    // Foreign key relationships
    entity.HasOne(tr => tr.Table)
          .WithMany()
          .HasForeignKey(tr => tr.TableId)
          .OnDelete(DeleteBehavior.Restrict);
    
    entity.HasOne(tr => tr.Branch)
          .WithMany()
          .HasForeignKey(tr => tr.BranchId)
          .OnDelete(DeleteBehavior.Cascade);
});
        }
    }
}