using CajaVirtualCobrosMVC.Entidades;
using CajaVirtualCobrosMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CajaVirtualCobrosMVC.Controllers
{
    public class ClienteController : Controller
    {
        public Cliente? Client { get; set; }

        public IActionResult Cliente(Cliente cliente)
        {
            if (cliente != null)
            {
                ClienteViewModel clienteViewModel = new ClienteViewModel()
                {
                    Cliente = cliente
                };
                return View(clienteViewModel);
            }

            else 
            {
                return RedirectToAction("Error");
            }

        }

        public async Task<ActionResult> ObtenerEstadoCuenta(int clienteId)
        {
            // Realizar la solicitud al API para obtener el estado de cuenta del cliente
            var client = new HttpClient();

            string apiUrl = "https://localhost:7073/api/estadocuenta/cliente/" + clienteId;
            var response = await client.GetAsync(apiUrl);

            // Almacenar los datos del estado de cuenta en una variable para pasarlos a la vista
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.Message = "Estado de Cuenta no encontrado";
                return RedirectToAction("Error", "Cliente");
            }
            var content = await response.Content.ReadAsStringAsync();
            var estadoCuenta = JsonSerializer.Deserialize<EstadoCuenta>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (estadoCuenta == null)
            {
                ViewBag.Message = "Error al obtener los datos del estado de cuenta";
                return RedirectToAction("Error", "Cliente");
            }

            //obtener los movimientos
            var client2 = new HttpClient();

            string apiUrl2 = "https://localhost:7073/api/movimiento/movestadocta/" + estadoCuenta.Id;
            var response2 = await client2.GetAsync(apiUrl2);

            // Almacenar los datos del estado de cuenta en una variable para pasarlos a la vista
            if (response2.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content2 = await response2.Content.ReadAsStringAsync();
                List<Movimiento>? movimientos = JsonSerializer.Deserialize<List<Movimiento>>(content2, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                estadoCuenta.Movimientos = movimientos;
            }

            //pasarle el modelo completo a la vista
            return View(estadoCuenta);
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}
