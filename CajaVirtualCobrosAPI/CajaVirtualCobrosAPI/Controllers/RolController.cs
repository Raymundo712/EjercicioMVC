using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/rol")]
    public class RolController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public RolController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet()]
        public async Task<List<Rol>> Get()
        {
            return await context.Roles.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Rol>> Get(int id)
        {
            var entidad = await context.Roles.FirstOrDefaultAsync(x=>x.Id == id);

            if(entidad == null) 
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Rol rol) 
        {
            var existeRolConElMismoNombre = await context.Roles.AnyAsync(x => x.Nombre == rol.Nombre);

            if (existeRolConElMismoNombre)
            {
                return BadRequest($"Ya existe un rol con el nombre {rol.Nombre}");
            }
            context.Add(rol);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Rol>> Put(Rol rol, int id)
        {
            if (rol.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Roles.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Update(rol);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Roles.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Rol() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
