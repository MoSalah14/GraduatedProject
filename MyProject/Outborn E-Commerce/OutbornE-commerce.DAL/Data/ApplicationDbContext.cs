using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Discount = OutbornE_commerce.DAL.Models.Discount;

namespace OutbornE_commerce.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Header> Headers { get; set; }
        public DbSet<HomeSection> HomeSections { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SEO> SEOs { get; set; }
        public DbSet<ReceivePoints> ReceivePoints { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SMTPServer> SMTPServers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<ContactUsSetup> ContactUsSetups { get; set; }
        public DbSet<InquiryType> InquiryTypes { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<BagItem> BagItems { get; set; }
        public DbSet<DiscountCategory> DiscountCategories { get; set; }
        public DbSet<FlashDeal> FlashDeals { get; set; }

        //start
        public DbSet<NavTitle> NavTitles { get; set; }

        public DbSet<ProductSizeDiscount> ProductSizeDiscounts { get; set; }

        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrder { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<Coupons> Coupons { get; set; }

        public DbSet<UserCoupon> CouponUsers { get; set; }
        public DbSet<Models.Footer> Footer { get; set; }

        public DbSet<ProductColorImage> ProductImage { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<CategorySubCategoryBridge> CategorySubCategoryBridges { get; set; }
        public DbSet<TypeEntity> TypeEntities { get; set; }
        public DbSet<SubCategoryType> SubCategoryTypes { get; set; }
        public DbSet<CityAreas> CityArea { get; set; }
        public DbSet<ShippingPrice> ShippingPrices { get; set; }
        public DbSet<FreeDelivery> FreeDeliveryRules { get; set; }
        public DbSet<ReturnItemReason> ReturnedItemReasons { get; set; }
        public DbSet<ReturnedOrders> ReturnedOrders { get; set; }
        public DbSet<ImageReturned> ImageReturneds { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.ApplyConfiguration(new RoleConfigrations());

            builder.Entity<ReturnedOrders>()
           .HasOne(or => or.Order)
           .WithMany()
           .HasForeignKey(or => or.OrderId);

            builder.Entity<ReturnItemReason>()
                .HasOne(ori => ori.OrderReturn)
                .WithMany(or => or.ReturnItems)
                .HasForeignKey(ori => ori.ReturnOrderId);

            builder.Entity<ReturnedOrders>()
                    .HasOne(or => or.Address)
                    .WithMany()
                    .HasForeignKey(or => or.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Brand>()
           .HasIndex(b => b.BrandNumber)
           .IsUnique();

            builder.Entity<Country>()
            .HasMany(c => c.Cities)
            .WithOne(ci => ci.Country)
            .HasForeignKey(ci => ci.CountryId);

            builder.Entity<ShippingPrice>()
                .HasOne(sp => sp.Country)
                .WithMany(c => c.ShippingPrices)
                .HasForeignKey(sp => sp.CountryId);

            // City Configurations
            builder.Entity<City>()
                .HasMany(ci => ci.CityAreas)
                .WithOne(sa => sa.City)
                .HasForeignKey(sa => sa.CityId);

            // Address Configurations
            builder.Entity<Address>()
                .HasOne(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId);

            builder.Entity<Address>()
                .HasOne(a => a.City)
                .WithMany()
                .HasForeignKey(a => a.CityId);

            builder.Entity<Address>()
                .HasOne(a => a.CityAreas)
                .WithMany()
                .HasForeignKey(a => a.ServiceableAreaId);

            builder.Entity<ProductColorImage>()
             .HasIndex(p => p.ProductColorId)
             .IsUnique(false);

            builder.Entity<ProductColor>()
                .HasMany(c => c.ProductColorImages)
                .WithOne(i => i.Product_Color)
                .HasForeignKey(i => i.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => new { e.NameEn, e.NameAr, e.IsActive, e.IsPreOrder, e.SKU })
                      .IsUnique(false)
                      .IsClustered(false);
            });
            builder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => new { e.NameEn, e.NameAr })
                      .IsUnique(false)
                      .IsClustered(false);
            });

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
            builder.Entity<Color>(entity =>
            {
                entity.HasIndex(e => new { e.NameEn, e.NameAr })
                      .IsUnique(false)
                      .IsClustered(false);
            });
            builder.Entity<Size>(entity =>
            {
                entity.HasIndex(e => new { e.Type, e.Name })
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

            builder.Entity<Blogs>()
                   .HasOne(b => b.BlogCategory)
                   .WithMany(c => c.Blogs)
                   .HasForeignKey(b => b.BlogCategoryId)
                   .OnDelete(DeleteBehavior.Cascade); // Configures cascade delete
            builder.Entity<Order>()
          .HasIndex(o => o.OrderNumber)
          .IsUnique();

            builder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            //builder.Entity<ProductCategory>()
            //    .HasOne(pc => pc.Product)
            //    .WithMany(p => p.ProductCategories)
            //    .HasForeignKey(pc => pc.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<ProductCategory>()
            //    .HasOne(pc => pc.Category)
            //    .WithMany(c => c.ProductCategories)
            //    .HasForeignKey(pc => pc.CategoryId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Category>()
            //.HasOne(c => c.SuperCategory)
            //.WithMany(c => c.Categories)
            //.HasForeignKey(c => c.SuperCategoryID);

            builder.Entity<NewsletterSubscriber>()
           .HasIndex(n => n.Email)
           .IsUnique();

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
            builder.Entity<Ticket>()
           .HasOne(t => t.user)
           .WithMany(u => u.Tickets)
           .HasForeignKey(t => t.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Brand>()
           .HasOne(c => c.ParentBrand)
           .WithMany(c => c.SubBrands)
           .HasForeignKey(c => c.ParentBrandId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
           .HasMany(p => p.ProductColors)
           .WithOne(pc => pc.Product)
           .HasForeignKey(pc => pc.ProductId);

            builder.Entity<Color>()
                .HasMany(c => c.ProductColor)
                .WithOne(pc => pc.Color)
                .HasForeignKey(pc => pc.ColorId);

            builder.Entity<ProductColor>()
                .HasMany(pc => pc.ProductSizes)
                .WithOne(ps => ps.ProductColor)
                .HasForeignKey(ps => ps.ProductColorId);

            builder.Entity<Size>()
                .HasMany(s => s.productSizes)
                .WithOne(ps => ps.Size)
                .HasForeignKey(ps => ps.SizeId);
            builder.Entity<Product>()
       .HasOne(p => p.Brand)
       .WithMany(b => b.Products)
       .HasForeignKey(p => p.BrandId)
       .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserPermission>()
           .HasKey(up => new { up.UserId, up.PermissionId });

            builder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId);

            builder.Entity<UserPermission>()
            .HasOne(up => up.Permission)
            .WithMany()
            .HasForeignKey(up => up.PermissionId);
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

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductSize)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.ProductSizeId);

            builder.Entity<DiscountCategory>()
                .HasOne(dc => dc.Discount)
                .WithMany(d => d.DiscountCategories)
                .HasForeignKey(dc => dc.DiscountId);

            builder.Entity<DiscountCategory>()
                 .HasOne(dc => dc.Category)
                 .WithMany(c => c.DiscountCategories)
                 .HasForeignKey(dc => dc.CategoryId);

            builder.Entity<ProductSizeDiscount>(entity =>
            {
                entity.HasKey(psd => psd.Id);

                entity.HasIndex(e => new { e.StartDate, e.EndDate })
                      .IsUnique(false)
                      .IsClustered(false);

                entity.HasOne(psd => psd.ProductSize)
                      .WithMany(ps => ps.ProductSizeDiscounts)
                      .HasForeignKey(psd => psd.ProductSizeId);

                entity.HasOne(psd => psd.Discount)
                      .WithMany(d => d.ProductSizeDiscounts)
                      .HasForeignKey(psd => psd.DiscountId);
            });
            builder.Entity<CategorySubCategoryBridge>(entity =>
            {
                entity.HasOne(cs => cs.Category)
                    .WithMany(c => c.CategorySubCategories)
                    .HasForeignKey(cs => cs.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(cs => cs.SubCategory)
                    .WithMany(sc => sc.CategorySubCategories)
                    .HasForeignKey(cs => cs.SubCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.ToTable("CategorySubCategoryBridge");
            });

            builder.Entity<SubCategoryType>(entity =>
            {
                entity.HasOne(st => st.SubCategory)
                    .WithMany(sc => sc.SubCategoryTypes)
                    .HasForeignKey(st => st.SubCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(st => st.type)
                    .WithMany(t => t.SubCategoryTypes)
                    .HasForeignKey(st => st.TypeId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.ToTable("SubCategoryType");
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.ProductType)
                      .WithMany(t => t.Products)
                      .HasForeignKey(p => p.ProductTypeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Product");
            });

            builder.Entity<CategorySubCategoryBridge>()
         .HasKey(csb => csb.Id);

            builder.Entity<CategorySubCategoryBridge>()
                .HasOne(csb => csb.Category)
                .WithMany(c => c.CategorySubCategories)
                .HasForeignKey(csb => csb.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CategorySubCategoryBridge>()
            .HasOne(csb => csb.SubCategory)
            .WithMany(sc => sc.CategorySubCategories)
            .HasForeignKey(csb => csb.SubCategoryId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SubCategoryType>()
                .HasKey(sct => sct.Id);

            builder.Entity<SubCategoryType>()
                .HasOne(sct => sct.SubCategory)
                .WithMany(x => x.SubCategoryTypes)
                .HasForeignKey(sct => sct.SubCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SubCategoryType>()
                .HasOne(sct => sct.type)
                .WithMany(t => t.SubCategoryTypes)
                .HasForeignKey(sct => sct.TypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Product>()
              .HasOne(p => p.ProductType)
              .WithMany(t => t.Products)
              .HasForeignKey(p => p.ProductTypeId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SubCategory>()
           .HasIndex(s => s.NameAr)
           .HasDatabaseName("IX_SubCategory_NameAr");

            builder.Entity<SubCategory>()
          .HasIndex(s => s.NameEn)
          .HasDatabaseName("IX_SubCategory_NameEn");

            builder.Entity<Category>()
        .HasIndex(c => c.NameEn)
        .IsUnique()
        .HasDatabaseName("IX_Category_NameEn");

            builder.Entity<Category>()
                .HasIndex(c => c.NameAr)
                .IsUnique()
                .HasDatabaseName("IX_Category_NameAr");

            //     builder.Entity<ReturnItemReason>()
            //.HasOne(rir => rir.OrderItem)
            //.WithOne(oi => oi.ReturnItemReason)
            //.HasForeignKey<ReturnItemReason>(rir => new { rir.OrderId, rir.ProductSizeId })
            //.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ReturnItemReason>()
                 .HasMany(rir => rir.Images)
                 .WithOne(ir => ir.ReturnItemReason)
                 .HasForeignKey(ir => ir.ReturnItemReasonId)
                 .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<ReturnItemReason>()
            //.HasOne(r => r.Address)
            //.WithMany(a => a.ReturnItemReasons)
            //.HasForeignKey(r => r.AddressId)
            //.OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ImageReturned>()
                .Property(ir => ir.ImageUrl)
                .IsRequired();
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Coupons>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Color>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Size>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Order>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Address>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<User>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<ProductSize>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<ProductColor>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<Category>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<SubCategory>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<TypeEntity>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<CategorySubCategoryBridge>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);
            builder.Entity<SubCategoryType>().HasQueryFilter(p => !p.IsDeleted).HasIndex(p => p.IsDeleted);

            base.OnModelCreating(builder);
        }
    }
}