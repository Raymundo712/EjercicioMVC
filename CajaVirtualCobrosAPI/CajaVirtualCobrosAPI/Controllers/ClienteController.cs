using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ClienteController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet()]
        public async Task<List<Cliente>> Get()
        {
            return await context.Clientes.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> Get(int id)
        {
            var entidad = await context.Clientes.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost]
        [Route("registro")]
        public async Task<ActionResult<Cliente>> Post([FromBody] Cliente cliente)
        {
            var existeClienteParaUsuario = await context.Clientes.AnyAsync(x => x.Usuario_Id == cliente.Usuario_Id);

            if (existeClienteParaUsuario)
            {
                return BadRequest($"Ya existe un cliente vinculado con este usuario");
            }
            context.Add(cliente);
            await context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpGet]
        [Route("buscar/{numeroCliente}")]
        public async Task<ActionResult<Cliente>> Get(string numeroCliente)
        {
            var entidad = await context.Clientes.Include(x => x.EstadoCuenta).FirstOrDefaultAsync(x => x.NumeroCliente == numeroCliente);

            if (entidad == null)
            {
                return NotFound($"No existe un cliente con el numero de cliente {numeroCliente}");
            }
            return Ok(entidad);
        }




        [HttpPut]
        [Route("editar/{id:int}")]
        public async Task<ActionResult<Cliente>> Put(Cliente cliente, int id)
        {
            if (cliente.Id != id)
            {
                return BadRequest("El id del Cliente no coincide con el id de la URL");
            }

            var existe = await context.Clientes.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Update(cliente);
            await context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Clientes.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Cliente() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
