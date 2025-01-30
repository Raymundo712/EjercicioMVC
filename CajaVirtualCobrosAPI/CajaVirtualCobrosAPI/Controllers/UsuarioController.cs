using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public UsuarioController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> Get()
        {
            return await context.Usuarios.ToListAsync();

        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Usuario>> Get(int id)
        {
            var entidad = await context.Usuarios.FirstOrDefaultAsync(x=>x.Id == id);

            if(entidad == null) 
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Get([FromBody] Usuario usuario)
        {
            var entidad = await context.Usuarios.Include(x=>x.Cliente).FirstOrDefaultAsync(x => x.UsuarioNombre == usuario.UsuarioNombre && x.Contrasena == usuario.Contrasena);

            if (entidad == null)
            {
                return NotFound("Usuario o contraseña incorrectos");
            }

            return entidad;
        }


        [HttpPost("registro")]
        public async Task<ActionResult<Usuario>> Post([FromBody] Usuario usuario) 
        {
            var existeUsuarioConElMismoNombre = await context.Usuarios.AnyAsync(x => x.UsuarioNombre == usuario.UsuarioNombre);

            if (existeUsuarioConElMismoNombre)
            {
                return BadRequest($"Ya existe un usuario con el nombre {usuario.UsuarioNombre}");
            }
            context.Add(usuario);
            await context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPut]
        [Route("editar/{id:int}")]
        public async Task<ActionResult<Usuario>> Put(Usuario usuario, int id)
        {
            if (usuario.Id != id)
            {
                return BadRequest("El id del Usuario no coincide con el id de la URL");
            }

            var existe = await context.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Update(usuario);
            await context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Usuario() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }


    }
}
