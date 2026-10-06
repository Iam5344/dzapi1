using Microsoft.AspNetCore.Mvc;
using WebApplication66.Models;

namespace WebApplication66.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private static readonly List<OrderItem> OrderItems = new List<OrderItem>
        {
            new OrderItem { Id = 1, OrderId = 1, ProductName = "Клавіатура", Quantity = 1, UnitPrice = 1200.00m },
            new OrderItem { Id = 2, OrderId = 1, ProductName = "Миша", Quantity = 2, UnitPrice = 450.00m },
            new OrderItem { Id = 3, OrderId = 2, ProductName = "Монітор", Quantity = 1, UnitPrice = 8500.00m }
        };

        [HttpGet]
        public IActionResult GetOrderItems()
        {
            return Ok(OrderItems);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOrderItemById(int id)
        {
            var item = OrderItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound("OrderItem not found");
            }

            return Ok(item);
        }

        [HttpGet("search")]
        public IActionResult SearchOrderItems([FromQuery] string? productName, [FromQuery] int? orderId)
        {
            var query = OrderItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(i => i.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase));
            }

            if (orderId.HasValue)
            {
                query = query.Where(i => i.OrderId == orderId.Value);
            }

            return Ok(query.ToList());
        }

        [HttpPost]
        public IActionResult CreateOrderItem([FromBody] OrderItemDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ProductName) || dto.OrderId <= 0 || dto.Quantity <= 0 || dto.UnitPrice <= 0)
            {
                return BadRequest("Усі поля є обов'язковими, а значення OrderId, Quantity та UnitPrice повинні бути більше 0.");
            }

            int newId = OrderItems.Count > 0 ? OrderItems.Max(i => i.Id) + 1 : 1;

            var newItem = new OrderItem
            {
                Id = newId,
                OrderId = dto.OrderId,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            OrderItems.Add(newItem);

            return CreatedAtAction(nameof(GetOrderItemById), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOrderItem(int id, [FromBody] OrderItemDto dto)
        {
            var existingItem = OrderItems.FirstOrDefault(i => i.Id == id);
            if (existingItem == null)
            {
                return NotFound("OrderItem not found");
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.ProductName) || dto.OrderId <= 0 || dto.Quantity <= 0 || dto.UnitPrice <= 0)
            {
                return BadRequest("Усі поля є обов'язковими, а значення OrderId, Quantity та UnitPrice повинні бути більше 0.");
            }

            existingItem.OrderId = dto.OrderId;
            existingItem.ProductName = dto.ProductName;
            existingItem.Quantity = dto.Quantity;
            existingItem.UnitPrice = dto.UnitPrice;

            return Ok(existingItem);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOrderItem(int id)
        {
            var item = OrderItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound("OrderItem not found");
            }

            OrderItems.Remove(item);

            return NoContent();
        }
    }
}
