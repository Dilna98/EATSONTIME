using System;
using System.Collections.Generic;

namespace EATSONTIME.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string RestaurantName { get; set; }
        public string ItemsSummary { get; set; }
        public List<OrderItemDetail> Items { get; set; } = new List<OrderItemDetail>();
    }

    public class OrderItemDetail
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class MenuViewModel
    {
        public int FoodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
    }

    public class CartViewModel
    {
        public int CartId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string RestaurantName { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
