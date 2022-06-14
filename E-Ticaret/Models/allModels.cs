using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Ticaret.Models
{
    public class Category
    {
        public short CategoryId { get; set; }
        [Required]
        [Column(TypeName ="nchar(50)")]
        public string CategoryName { get; set; }
        [Required]
        public bool IsDeleted { get; set; } //kategori silme yetkisi
    }
    public class Brand
    {
        public short BrandId { get; set;}
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string BrandName { get; set; }
    }
    public class City
    {
        public short CityId { get; set; }
        [Required]
        [Column(TypeName = "nchar(20)")]
        public string CityName { get; set; }
    }
    public class Seller
    {
        public int SellerId { get; set;}
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string SellerName { get; set; }
        [Required]
        [Column(TypeName = "nchar(100)")]
        [EmailAddress]
        public string SellerEMail { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [Column(TypeName = "nchar(64)")]
        [DataType(DataType.Password)]
        public string SellerPassword { get; set; }
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("SellerPassword",ErrorMessage ="Şifreler Uyumlu Değil!")]
        public string ConfirmPassword { get; set; }
        [Column(TypeName = "nchar(200)")]
        public string? SellerDescription { get; set; } //? koyduk. Boş olabilir anlamında.
        
        public bool Banned { get; set; }
        [Required]
        public bool IsDeleted { get; set; } //seller silme yetkisi
        public float? SellerRate { get; set; }
        [Required]
        public short CityId { get; set;}
        public City? City { get; set; }
        public List<Product>? Products { get; set; }
    }
    public class Product
    {
        public long ProductId { get; set; }
        [Required]
        [Column(TypeName = "nchar(100)")]
        public string ProductName { get; set; }
        [Required]
        public float ProductPrice { get; set; }
        [NotMapped]
        [MaxLength(5)]
        public IFormFile[]? Image { get; set; }
        [Column(TypeName = "nchar(200)")]
        public string? Description { get; set; }
        public float? ProductRate { get; set; }
        [Required]
        public short CategoryId { get; set; }
        [Required]
        public bool IsDeleted { get; set; } //product silme yetkisi
        [Required]
        public short BrandId { get; set;}
        [Required]
        public int SellerId { get; set; }
        public Seller? Seller { get; set; }
        public Category? Category { get; set; } //? koyduk. Null olabilir anlamında.
        public Brand? Brand { get; set; }
        
    }
    public class Customer
    {
        public long CustomerId { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string CustomerName { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string CustomerSurname { get; set; }
        [Required]
        [Column(TypeName = "nchar(100)")]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string CustomerPhone { get; set; }
        [Column(TypeName = "nchar(64)")]
        [DataType(DataType.Password)]
        public string? CustomerPassword { get; set; }
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("CustomerPassword", ErrorMessage = "Şifreler Uyumlu Değil!")]
        public string? ConfirmPassword { get; set; }
        [Required]
        [Column(TypeName = "nchar(200)")]
        public string CustomerAddress { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public List<Order>? Orders { get; set; }

    }
    public class PaymentMethod
    {
        public short PaymentMethodId { get; set; }
        [Required]
        [Column(TypeName = "nchar(40)")]
        public string PaymentMethodName { get; set; }
    }
    public class Order
    {
        public long OrderId { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public float OrderPrice { get; set; }
        [Required]
        public bool AllDelivered { get; set; }
        [Required]
        public bool Cancelled { get; set; }
       
        public short? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public bool PaymentComplete { get; set; }
        [Required]
        public long CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
        [Required]
        public bool IsCart { get; set; }
    }
    public class ItemStatus
    {
        public short ItemStatusId { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string ItemStatusName { get; set; }

    }
    public class OrderDetailStatus
    {
        public long OrderDetailStatusId { get; set; }
        [Required]
        public long OrderDetailId { get; set; } 
        public OrderDetail? OrderDetail { get; set; }
        [Required]
        public DateTime ChangeItemStatus { get; set; }
        [Required]
        public short ItemStatusId { get; set; }
        public ItemStatus? ItemStatus { get; set; }
    }
    public class OrderDetail
    {
        public long OrderDetailId { get; set; }
        [Required]
        public long OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public long ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public byte Count { get; set; }
        [Required]
        public float Price { get; set; }
        public List<OrderDetailStatus>? OrderDetailStatues { get; set; }

    }
}
