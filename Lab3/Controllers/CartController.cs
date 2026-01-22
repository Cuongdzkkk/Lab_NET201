using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Lab3.Controllers
{
    public class CartController : Controller
    {
        private readonly InventoryContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(InventoryContext context)
        {
            _context = context;
        }

        // GET: /Cart/Add/5
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var cart = GetCart();
            if (cart.ContainsKey(product.ProductId))
            {
                cart[product.ProductId]++;
            }
            else
            {
                cart[product.ProductId] = 1;
            }

            SaveCart(cart);

            // Return lightweight view or redirect to previous page if passed
            // For now, back to Shop
            return RedirectToAction("Shop", "Product");
        }

        [HttpGet]
        public IActionResult GetCount()
        {
            var cart = GetCart();
            int count = cart.Values.Sum();
            return Json(new { count = count });
        }

        private Dictionary<int, int> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString(CartSessionKey);
            return sessionCart == null 
                ? new Dictionary<int, int>() 
                : JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();
        }

        private void SaveCart(Dictionary<int, int> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }
    }
}
