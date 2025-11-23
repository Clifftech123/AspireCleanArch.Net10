using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AspireCleanArch.Domain.Entities;
using AspireCleanArch.Domain.Entities.ValueObjects;

namespace AspireCleanArch.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Order aggregate root
    /// </summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table configuration
            builder.ToTable("Orders");

            // Primary key
            builder.HasKey(o => o.Id);

            // Indexes for performance
            builder.HasIndex(o => o.OrderNumber).IsUnique();
            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.OrderDate);

            // Value conversions for UserId (Guid)
            builder.Property(o => o.UserId)
                .IsRequired();

            // Order Number
            builder.Property(o => o.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            // Status enum
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // Configure Money value objects as owned types
            builder.OwnsOne(o => o.SubtotalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("SubtotalAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                money.Property(m => m.Currency)
                    .HasColumnName("SubtotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(o => o.TaxAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TaxAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                money.Property(m => m.Currency)
                    .HasColumnName("TaxCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(o => o.ShippingAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("ShippingAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                money.Property(m => m.Currency)
                    .HasColumnName("ShippingCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(o => o.DiscountAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("DiscountAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                money.Property(m => m.Currency)
                    .HasColumnName("DiscountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(o => o.TotalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                money.Property(m => m.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Configure ShippingAddress as owned type
            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("ShippingStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("ShippingCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("ShippingState")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.ZipCode)
                    .HasColumnName("ShippingZipCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("ShippingCountry")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.RecipientName)
                    .HasColumnName("RecipientName")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.PhoneNumber)
                    .HasColumnName("RecipientPhone")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.DeliveryInstructions)
                    .HasColumnName("DeliveryInstructions")
                    .HasMaxLength(500);
            });

            // Other properties
            builder.Property(o => o.PaymentId);

            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(o => o.CourierService)
                .HasMaxLength(100);

            builder.Property(o => o.CustomerNotes)
                .HasMaxLength(1000);

            builder.Property(o => o.InternalNotes)
                .HasMaxLength(1000);

            builder.Property(o => o.CancellationReason)
                .HasMaxLength(500);

            // Dates
            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.PaymentConfirmedDate);
            builder.Property(o => o.ShippedDate);
            builder.Property(o => o.DeliveredDate);
            builder.Property(o => o.CompletedDate);
            builder.Property(o => o.CancelledDate);

            // Configure OrderItems as owned collection
            builder.OwnsMany(o => o.Items, orderItem =>
            {
                orderItem.ToTable("OrderItems");
                orderItem.WithOwner(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId);

                orderItem.HasKey(oi => oi.Id);

                orderItem.Property(oi => oi.ProductId).IsRequired();
                orderItem.Property(oi => oi.VendorId).IsRequired();
                
                orderItem.Property(oi => oi.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                orderItem.Property(oi => oi.ProductSku)
                    .HasMaxLength(100);

                orderItem.Property(oi => oi.Quantity).IsRequired();
                orderItem.Property(oi => oi.TaxRate).IsRequired();

                // Money value objects in OrderItem
                orderItem.OwnsOne(oi => oi.UnitPrice, money =>
                {
                    money.Property(m => m.Amount)
                        .HasColumnName("UnitPriceAmount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                        
                    money.Property(m => m.Currency)
                        .HasColumnName("UnitPriceCurrency")
                        .HasMaxLength(3)
                        .IsRequired();
                });

                orderItem.OwnsOne(oi => oi.TotalPrice, money =>
                {
                    money.Property(m => m.Amount)
                        .HasColumnName("TotalPriceAmount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                        
                    money.Property(m => m.Currency)
                        .HasColumnName("TotalPriceCurrency")
                        .HasMaxLength(3)
                        .IsRequired();
                });

                orderItem.OwnsOne(oi => oi.TaxAmount, money =>
                {
                    money.Property(m => m.Amount)
                        .HasColumnName("TaxAmount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                        
                    money.Property(m => m.Currency)
                        .HasColumnName("TaxCurrency")
                        .HasMaxLength(3)
                        .IsRequired();
                });

                // Indexes for OrderItems
                orderItem.HasIndex(oi => oi.OrderId);
                orderItem.HasIndex(oi => oi.ProductId);
                orderItem.HasIndex(oi => oi.VendorId);
            });

            // Ignore domain events collection (not persisted)
            builder.Ignore(o => o.DomainEvents);

            // BaseEntity properties
            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt);
            builder.Property(o => o.DeletedAt);
            
            builder.Property(o => o.IsDeleted)
                .IsRequired();

            // Soft delete query filter
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
