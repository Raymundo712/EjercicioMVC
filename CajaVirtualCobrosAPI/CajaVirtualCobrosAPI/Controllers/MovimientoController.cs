using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/movimiento")]
    public class MovimientoController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public MovimientoController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("movestadocta/{idEstadoCuenta:int}")]
        public async Task<ActionResult<List<Movimiento>>> Get(int idEstadoCuenta)
        {
            var listMovimientos = await context.Movimientos.Include(x => x.ConceptoCobro).Where(x => x.EstadoCuenta_Id == idEstadoCuenta).ToListAsync();
            if (listMovimientos.Count == 0)
            {
                return NotFound();
            }

            return Ok(listMovimientos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Movimiento>> Get([FromBody] int idEstadoCuenta, int idMovimiento)
        {
            var entidad = await context.Movimientos.FirstOrDefaultAsync(x => x.EstadoCuenta_Id == idEstadoCuenta && x.Id == idMovimiento);

            if (entidad == null)
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost]
        [Route("registrar")]
        public async Task<ActionResult> Post([FromBody] Movimiento movimiento) 
        {
            context.Add(movimiento);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Movimiento>> Put(Movimiento movimiento, int id)
        {
            if (movimiento.Id != id)
            {
                return BadRequest("El id del Movimiento no coincide con el id de la URL");
            }

            var existe = await context.Movimientos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Update(movimiento);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Movimientos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Movimiento() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }


    }
}
