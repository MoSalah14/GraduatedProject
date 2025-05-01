using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OutbornE_commerce.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<BagItem> BagItems { get; set; }
        public DbSet<HomeSection> HomeSections { get; set; }

        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrder { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<Coupons> Coupons { get; set; }

        public DbSet<UserCoupon> CouponUsers { get; set; }

        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ShippingPrice> ShippingPrices { get; set; }
  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.ApplyConfiguration(new RoleConfigrations());

            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.NameEn, e.NameAr })
                      .IsUnique(false)
                      .IsClustered(false);
            });
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => new { e.OrderNumber })
                      .IsUnique(true)
                      .IsClustered(false);
                entity.HasIndex(e => new { e.OrderStatus, e.ShippedStatus, e.PaymentStatus, e.PaymentMethod })
                      .IsUnique(false)
                      .IsClustered(false);
            });



            builder.Entity<User>(entity =>
            {
                entity.HasIndex(e => new { e.Email, e.FullName, e.UserName })
                      .IsUnique(false)
                      .IsClustered(false);
            });

            builder.Entity<WishList>()
           .HasKey(w => new { w.UserId, w.ProductId });

            builder.Entity<WishList>()
          .HasOne(w => w.ProductWishList)
          .WithMany(p => p.WishLists)
          .HasForeignKey(w => w.ProductId);
            builder.Entity<WishList>()
     .HasOne(w => w.UserWishList)
     .WithMany(p => p.WishLists)
     .HasForeignKey(w => w.UserId);

            builder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallets>(w => w.UserId);

            builder.Entity<Wallets>()
                .HasMany(w => w.Transactions)
                .WithOne(t => t.UserWallet)
                .HasForeignKey(t => t.UserWalletId);

            builder.Entity<UserCoupon>()
           .HasKey(cu => new { cu.CouponId, cu.UserId });

            builder.Entity<Order>()
            .HasOne(o => o.Delivery)
            .WithOne(d => d.Order)
            .HasForeignKey<DeliveryOrder>(d => d.OrderId);



            builder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => e.ReviewId);

                entity.Property(e => e.Rating)
                .IsRequired(false).HasDefaultValue(0)
                .HasMaxLength(5);

                entity.Property(e => e.Comment)
                  .IsRequired(false);

                entity.Property(e => e.ReviewDate)
                  .IsRequired()
                  .HasDefaultValueSql("GETDATE()"); // Automatically sets the date to current date

                entity.HasOne(d => d.Product)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade); // If the product is deleted, delete its reviews

                entity.HasOne(d => d.User)
                      .WithMany(u => u.Users_Reviews)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, delete their reviews
            });

            builder.Entity<Order>()
           .HasMany(o => o.OrderItems)
           .WithOne(oi => oi.Order)
           .HasForeignKey(oi => oi.OrderId)
           .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
           .Property(o => o.UserId)
           .IsRequired();

            builder.Entity<OrderItem>()
              .HasKey(e => new { e.OrderId, e.ProductSizeId });

            builder.Entity<OrderItem>()
           .Property(oi => oi.ItemPrice)
           .HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
               .HasOne(oi => oi.Order)
               .WithMany(o => o.OrderItems)
               .HasForeignKey(oi => oi.OrderId);






            builder.Entity<Category>()
        .HasIndex(c => c.NameEn)
        .IsUnique()
        .HasDatabaseName("IX_Category_NameEn");

            builder.Entity<Category>()
                .HasIndex(c => c.NameAr)
                .IsUnique()
                .HasDatabaseName("IX_Category_NameAr");

            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Coupons>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Order>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Address>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Category>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);

            base.OnModelCreating(builder);
        }
    }
}