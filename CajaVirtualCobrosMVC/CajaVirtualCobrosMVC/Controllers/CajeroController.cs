using CajaVirtualCobrosMVC.Entidades;
using CajaVirtualCobrosMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace CajaVirtualCobrosMVC.Controllers
{

    public class CajeroController : Controller
    {
        private ConceptosEstadoCuentaCliente _estadoCuentaCliente;

        public IActionResult ModuloCajero(Cliente cliente)
        {
            return View(cliente);
        }

        public IActionResult ModuloCaja() 
        {
            return View();
        }

        public async Task<IActionResult> CapturaMovimientos(string numeroCliente) 
        {
            ConceptosEstadoCuentaCliente cuentaCliente = new ConceptosEstadoCuentaCliente();
            Cliente? cliente = new Cliente();
            EstadoCuenta? estadoCuenta = new EstadoCuenta();
            List<ConceptoCobro>? conceptosCobro = new List<ConceptoCobro>();

            if (numeroCliente == null)
            {
                return View("Error", "Error");
            }
            else 
            {
                // Realizar la solicitud al API para obtener el cliente
                var client = new HttpClient();

                string apiUrl = "https://localhost:7073/api/cliente/buscar/" + numeroCliente;
                var response = await client.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Cliente no encontrado";
                    return RedirectToAction("Error", "Cliente");
                }
                var content = await response.Content.ReadAsStringAsync();
                cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cliente != null) 
                {
                    // Realizar la solicitud al API para obtener el estado de cuenta del cliente
                    var client2 = new HttpClient();

                    string apiUrl2 = "https://localhost:7073/api/estadocuenta/" + cliente.Id;
                    var response2 = await client.GetAsync(apiUrl2);

                    // Almacenar los datos del estado de cuenta en una variable para pasarlos a la vista
                    if (response2.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Estado de Cuenta no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var content2 = await response2.Content.ReadAsStringAsync();
                    estadoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(content2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // Realizar la solicitud al API para obtener los conceptos de cobro
                var client3 = new HttpClient();

                string apiUrl3 = "https://localhost:7073/api/conceptocobro/";
                var response3 = await client.GetAsync(apiUrl3);

                var content3 = await response3.Content.ReadAsStringAsync();
                conceptosCobro = System.Text.Json.JsonSerializer.Deserialize<List<ConceptoCobro>>(content3, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cliente != null && estadoCuenta != null && conceptosCobro?.Count > 0) 
                {
                    cuentaCliente.Cliente = cliente;
                    cuentaCliente.EstadoCuenta = estadoCuenta;
                    cuentaCliente.EstadoCuenta.Movimientos = new List<Movimiento>();
                    cuentaCliente.Conceptos = conceptosCobro;

                }
            }

            return View(cuentaCliente);
        }

        public async Task<IActionResult> ProcesarMovimientosCuenta([FromBody] Movimiento[] movimientos)
        {
            _estadoCuentaCliente = new ConceptosEstadoCuentaCliente();
            _estadoCuentaCliente.Movimientos = new List<Movimiento>(); 

            decimal totalAbonos = 0;
            decimal totalCobros = 0;
            foreach (var movimiento in movimientos)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7073/api/movimiento/");

                    string fechaFormateada = movimiento.FechaMovimiento.ToString("yyyy-MM-dd");
                    movimiento.Abono = decimal.Parse(movimiento.Abono.ToString("0.0"));

                    var response = await client.PostAsync("registrar", new StringContent($"{{\"estadocuenta_id\":\"{movimiento.EstadoCuenta_Id}\",\"fechamovimiento\":\"{fechaFormateada}\",\"conceptocobro_id\":{(movimiento.ConceptoCobro_Id.HasValue ? movimiento.ConceptoCobro_Id.Value.ToString() : "null")},\"abono\":\"{movimiento.Abono}\"}}", Encoding.UTF8, "application/json"));
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        if (movimiento.ConceptoCobro_Id != null)
                        {
                            string apiUrl = "https://localhost:7073/api/conceptocobro/" + movimiento.ConceptoCobro_Id;
                            var response2 = await client.GetAsync(apiUrl);

                            if (response2.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                ViewBag.Message = "Concepto Cobro no encontrado";
                                return RedirectToAction("Error", "Cajero");
                            }
                            var content = await response2.Content.ReadAsStringAsync();
                            var concepto = System.Text.Json.JsonSerializer.Deserialize<ConceptoCobro>(content, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (concepto != null)
                            {
                                totalCobros = totalCobros + concepto.Valor;
                                movimiento.ConceptoCobro = concepto;
                                _estadoCuentaCliente.Movimientos.Add(movimiento);
                            }
                        }
                        else
                        {
                            totalAbonos = totalAbonos + movimiento.Abono;
                            _estadoCuentaCliente.Movimientos.Add(movimiento);
                        }
                    }
                }

            }

            using (var client = new HttpClient())
            {
                string apiUrl = "https://localhost:7073/api/estadocuenta/" + movimientos[0].EstadoCuenta_Id;
                var response = await client.GetAsync(apiUrl);

                var content = await response.Content.ReadAsStringAsync();
                var estadoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (estadoCuenta != null) 
                {
                    var saldoCobros = estadoCuenta.Saldo + totalCobros;
                    var saldo = saldoCobros - totalAbonos;

                    string apiUrl2 = "https://localhost:7073/api/estadocuenta/actualizar/" + estadoCuenta.Id;

                    var response2 = await client.PutAsync(apiUrl2, new StringContent($"{{\"saldo\":\"{saldo}\"}}", Encoding.UTF8, "application/json"));
                    
                    if (response2.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Estado de cuenta no encontrado";
                        return RedirectToAction("Error", "Cajero");
                    }
                    estadoCuenta.Saldo = saldo;

                    _estadoCuentaCliente.EstadoCuenta = estadoCuenta;

                    string apiUrl3 = "https://localhost:7073/api/cliente/" + estadoCuenta.Cliente_Id;
                    var response3 = await client.GetAsync(apiUrl3);

                    if (response3.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Cliente no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var content2 = await response3.Content.ReadAsStringAsync();
                    var cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(content2, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (cliente != null)
                    {
                        _estadoCuentaCliente.Cliente = cliente;
                    }
                }

                HttpContext.Session.SetString("EstadoCuentaCliente", JsonConvert.SerializeObject(_estadoCuentaCliente));
                return RedirectToAction("DetalleOperacionCaja", "Cajero");
            }
        }

        [HttpGet]
        public IActionResult DetalleOperacionCaja()
        {
            var estadoCuentaCliente = JsonConvert.DeserializeObject<ConceptosEstadoCuentaCliente>(HttpContext.Session.GetString("EstadoCuentaCliente"));
            ViewBag.EstadoCuentaCliente = estadoCuentaCliente;
            return View();
        }
    }
}
