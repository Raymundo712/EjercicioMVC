using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/estadocuenta")]
    public class EstadoCuentaController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public EstadoCuentaController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<List<EstadoCuenta>> Get()
        {
            return await context.EstadosCuenta
                .Include(x => x.Movimientos)
                .ToListAsync();
        }

        [HttpGet("cliente/{id:int}")]
        public async Task<ActionResult<EstadoCuenta>> GetByClienteId(int id)
        {
            var entidad = await context.EstadosCuenta
                .Include(x => x.Movimientos)
                .FirstOrDefaultAsync(x => x.Cliente_Id == id);
            if (entidad == null)
            {
                return NotFound();
            }

            return entidad;
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstadoCuenta>> Get(int id)
        {
            var entidad = await context.EstadosCuenta
                .Include(x => x.Movimientos)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost]
        [Route("registro")]
        public async Task<ActionResult<EstadoCuenta>> Post([FromBody] EstadoCuenta estadoCuenta) 
        {
            var existeEstadoCuentaParaCliente = await context.EstadosCuenta.AnyAsync(x => x.Cliente_Id == estadoCuenta.Cliente_Id);

            if (existeEstadoCuentaParaCliente)
            {
                return BadRequest($"Ya existe un cliente vinculado con este cliente");
            }
            context.Add(estadoCuenta);
            await context.SaveChangesAsync();
            return Ok(estadoCuenta);
        }

        [HttpPut]
        [Route("actualizar/{id:int}")]
        public async Task<ActionResult<EstadoCuenta>> Put([FromBody] EstadoCuenta cuenta, int id)
        {
            var estadoCuenta = await context.EstadosCuenta.FindAsync(id);
            if (estadoCuenta == null)
            {
                return NotFound();
            }

            estadoCuenta.Saldo = cuenta.Saldo;

            context.Entry(estadoCuenta).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.EstadosCuenta.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new EstadoCuenta() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
