using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiDataSubmit.Models
{
    public class MultiDataDB:DbContext
    {
        public MultiDataDB(DbContextOptions<MultiDataDB> options)
            : base(options)
        {

        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasMany(e => e.OrderItems).WithOne(e => e.Product).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Order>().HasMany(e => e.OrderItems).WithOne(e => e.Order).OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }


    [Table("OrderItem")]
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [ForeignKey("Order"), Display(Name = "Order")]
        public long OrderId { get; set; }

        [ForeignKey("Product"), Display(Name = "Product")]
        public long ProductId { get; set; }

        [Column(TypeName = "money"), DataType(DataType.Currency), Display(Name = "Unit Price"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        public long Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }

    [Table("Order")]
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required(AllowEmptyStrings = true), Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [ForeignKey("Customer"), Display(Name = "Customer")]
        public long CustomerId { get; set; }

        [Column(TypeName = "money"), DataType(DataType.Currency), Display(Name = "Total Amount"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal TotalAmount { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual IList<OrderItem> OrderItems { get; set; }
    }
    [Table("Product")]
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [ForeignKey("Supplier"), Display(Name = "Supplier")]
        public long SupplierId { get; set; }

        [Column(TypeName = "money"), DataType(DataType.Currency), Display(Name = "Unit Price"), DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        public string Package { get; set; }

        [Display(Name = "Is Discontinued")]
        public bool IsDiscontinued { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }

    [Table("Customer")]
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Display(Name = "Firs tName")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [ForeignKey("City"), Display(Name = "City")]
        public long CityId { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual City City { get; set; }
    }
    [Table("City")]
    public class City
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Display(Name = "City")]
        public string Name { get; set; }

        [ForeignKey("Country"), Display(Name = "Country")]
        public long CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
    [Table("Country")]
    public class Country
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Display(Name = "Country")]
        public string Name { get; set; }

        public virtual IList<City> Cities { get; set; }
    }

    [Table("Supplier")]
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Display(Name = "Contact Title")]
        public string ContactTitle { get; set; }

        [ForeignKey("City"), Display(Name = "City")]
        public long CityId { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
