using AutoMapper;
using CajaVirtualCobrosAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CajaVirtualCobrosAPI.Controllers
{
    [ApiController]
    [Route("api/conceptocobro")]
    public class ConceptoCobroController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ConceptoCobroController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet()]
        public async Task<List<ConceptoCobro>> Get()
        {
            return await context.Conceptos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConceptoCobro>> Get(int id)
        {
            var entidad = await context.Conceptos.FirstOrDefaultAsync(x=>x.Id == id);

            if(entidad == null) 
            {
                return NotFound();
            }

            return entidad;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ConceptoCobro conceptoCobro) 
        {
            context.Add(conceptoCobro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("editar/{id:int}")]
        public async Task<ActionResult<ConceptoCobro>> Put(ConceptoCobro conceptoCobro, int id)
        {
            if (conceptoCobro.Id != id)
            {
                return BadRequest("El id del Concepto de Cobro no coincide con el id de la URL");
            }

            var existe = await context.Conceptos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Update(conceptoCobro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Conceptos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            try
            {
                context.Remove(new ConceptoCobro() { Id = id });
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo eliminar el registro. Error: " + ex.Message);
            }
        }
    }
}
