using System.ComponentModel.DataAnnotations;
using Domain.Attributes;

namespace Domain.Model;

public class Product
{
    public int ID { get; set; }

    [Required]
    [EncryptColumn]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int ProductID { get; set; }

    public double Price { get; set; }
}
