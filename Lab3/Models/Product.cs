using System;
using System.Collections.Generic;

namespace Lab3.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Color { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int AvailableQuantity { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? ImageUrl { get; set; }
}
