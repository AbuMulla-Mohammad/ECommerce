﻿namespace ECommerce.API.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
