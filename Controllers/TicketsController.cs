using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pract6.Models;


namespace pract6.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly WebApiDatabaseContext _context;

        public TicketsController(WebApiDatabaseContext context)
        {
            _context = context;
        }

        // получение всех записей
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? priority,
            [FromQuery] bool? closed)
        {
            IQueryable<Ticket> query = _context.Tickets;

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(t => t.Priority == priority); // фильтрация по приоритету

            if (closed.HasValue)
                query = query.Where(t => t.IsClosed == closed.Value); // фильтрация по статусу

            return Ok(query.ToList());
        }

        // получение записи по Id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if(ticket== null)
                return NotFound();

            return Ok(ticket);
        }

        // добавление записи
        [HttpPost]
        public IActionResult Create([FromBody] Ticket ticket)
        {
            if (ticket == null)
                return BadRequest();

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        // полное изменение записи
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.Id)
                return BadRequest();

            _context.Entry(ticket).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(ticket); //NoContent()
        }

        // частичное изменение записи
        [HttpPatch("{id}")]
        public IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<Ticket> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
                return NotFound();

            patchDoc.ApplyTo(ticket, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.SaveChanges();
            return Ok(ticket); //NoContent()
        }

        // удаление записи
        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
                return NotFound();

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
