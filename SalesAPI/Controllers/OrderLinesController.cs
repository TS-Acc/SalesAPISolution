using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Models;

namespace SalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLinesController : ControllerBase
    {
        private readonly SalesDbContext _context;

        public OrderLinesController(SalesDbContext context)
        {
            _context = context;
        }

        private async Task<IActionResult> RecalculateOrderTotal(int orderId) // using LINQ and query syntax
        {
            Order? order = await _context.Orders.FindAsync(orderId);

            order.Total = (from ol in _context.OrderLines
                           join i in _context.Items
                           on ol.ItemId equals i.Id
                           where ol.OrderId == orderId
                           select new
                           {
                               lineTotal = ol.Quantity * i.Price
                           }).Sum(x => x.lineTotal);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<IActionResult> XRecalculateOrderTotal(int orderId)
        {
            // read the order to be updated
            Order? order = await _context.Orders.FindAsync(orderId);
            // check of the order is found
            if (order is null)
            {
                return NotFound();
            }
            // get all the orderlines for the order
            List<OrderLine> orderLines = await _context.OrderLines
                                                       .Include(orderLines => orderLines.Item)
                                                       .Where(orderLines => orderLines.OrderId == orderId)
                                                       .ToListAsync();
            // create a collection to store the product of quantity times price
            // and sum to linetotal to get the grand total
            decimal grandTotal = 0m;
            foreach (OrderLine ordLineInst in orderLines)
            {
                decimal lineTotal = ordLineInst.Quantity * ordLineInst.Item.Price;
                grandTotal += lineTotal;
            }
            //update the order.Total with the grandTotal
            order.Total = grandTotal;
            int changed = await _context.SaveChangesAsync();
            // if change failed throw exception
            if (changed != 1)
            {
                throw new Exception("Recalculate failed!");
            }
            return Ok();
        }

        // GET: api/OrderLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderLine>>> GetOrderLines()
        {
            return await _context.OrderLines.ToListAsync();
        }

        // GET: api/OrderLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderLine>> GetOrderLine(int id)
        {
            var orderLine = await _context.OrderLines.FindAsync(id);

            if (orderLine == null)
            {
                return NotFound();
            }

            return orderLine;
        }

        // PUT: api/OrderLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderLine(int id, OrderLine orderLine)
        {
            if (id != orderLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await RecalculateOrderTotal(orderLine.OrderId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderLineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OrderLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderLine>> PostOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Add(orderLine);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderLine.OrderId);

            return CreatedAtAction("GetOrderLine", new { id = orderLine.Id }, orderLine);
        }

        // DELETE: api/OrderLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderLine(int id)
        {
            var orderLine = await _context.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return NotFound();
            }
            int orderId = orderLine.OrderId;

            _context.OrderLines.Remove(orderLine);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderId);

            return NoContent();
        }

        private bool OrderLineExists(int id)
        {
            return _context.OrderLines.Any(e => e.Id == id);
        }
    }
}
