using Microsoft.AspNetCore.Mvc;
using CajaVirtualCobrosMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CajaVirtualCobrosMVC.Entidades;

namespace CajaVirtualCobrosMVC.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri("https://localhost:7073/api/usuario/");

                // Realizar una llamada al endpoint de autenticación del API
                var response = await client.PostAsync("login", new StringContent($"{{\"usuarioNombre\":\"{model.Username}\",\"contrasena\":\"{model.Password}\"}}", Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    // Autenticación exitosa
                    var content = await response.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (usuario != null)
                    {
                        if (usuario.Rol_Id == 1)
                        {
                            if (usuario.Cliente != null)
                            {

                                var cliente = await ObtenerCliente(usuario.Cliente.NumeroCliente);
                                if (cliente != null)
                                {
                                    return RedirectToAction("Cliente", "Cliente", cliente);
                                }

                                else
                                {
                                    ViewBag.Message = "Error al obtener los datos del cliente";
                                    return RedirectToAction("Error", "Cliente");
                                }
                            }

                        }
                        else if (usuario.Rol_Id == 2)
                        {
                            if (usuario.Cliente != null)
                            {
                                var cliente = await ObtenerCliente(usuario.Cliente.NumeroCliente);
                                return RedirectToAction("ModuloCajero", "Cajero", cliente);
                            }
                            else 
                            {
                                return RedirectToAction("ModuloCajero", "Cajero", null);
                            }
                        }
                        else if (usuario.Rol_Id == 3)
                        {
                            if (usuario.Cliente != null)
                            {
                                var cliente = await ObtenerCliente(usuario.Cliente.NumeroCliente);
                                return RedirectToAction("ModuloAdministrador", "Administrador", cliente);
                            }
                            else
                            {
                                return RedirectToAction("ModuloAdministrador", "Administrador", null);
                            }
                        }
                    }
                }
                else
                {
                    // Autenticación fallida
                    ViewBag.Message = "Usuario o contraseña incorrectos.";
                }

            }

            return View();
        }

        public async Task<Cliente> ObtenerCliente(string numeroCliente) 
        {
            Cliente? cliente = new Cliente();
            if (numeroCliente != string.Empty)
            {
                string numeroCuenta = numeroCliente;

                //Codigo para consumir API de cliente 
                var client2 = new HttpClient();

                string apiUrl = "https://localhost:7073/api/cliente/buscar/" + numeroCuenta;
                var clientResponse = await client2.GetAsync(apiUrl);
                if (clientResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var clientContent = await clientResponse.Content.ReadAsStringAsync();
                    cliente = JsonSerializer.Deserialize<Cliente>(clientContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                }

                else
                {
                    ViewBag.Message = "No se encontro el cliente con este numero de cuenta";
                }

            }
            else
            {
                ViewBag.Message = "Usuario no tiene numero de cuenta";
            }

            return cliente;

        }

    }

}
