using CajaVirtualCobrosMVC.Entidades;
using CajaVirtualCobrosMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace CajaVirtualCobrosMVC.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult ModuloAdministrador()
        {
            return View();
        }

        public IActionResult ModuloAdministrativo()
        {
            return View();
        }

        public IActionResult ModuloGestionUsuarios()
        {
            return View();
        }

        public async Task<IActionResult> EdicionEliminacionUsuario(string numeroCliente)
        {
            ClienteViewModel clienteViewModel = new ClienteViewModel();

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
                var cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cliente != null)
                {
                    string apiURL = "https://localhost:7073/api/usuario/" + cliente.Usuario_Id;
                    var respon = await client.GetAsync(apiURL);

                    if (respon.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Usuario no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var conte = await respon.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<Usuario>(conte, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string apiUrlURol = "https://localhost:7073/api/rol/" + usuario.Rol_Id;
                    var responseURol = await client.GetAsync(apiUrlURol);

                    var contentURol = await responseURol.Content.ReadAsStringAsync();
                    var rolusuario = System.Text.Json.JsonSerializer.Deserialize<Rol>(contentURol, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    usuario.Rol = rolusuario;

                    string apiUrl2 = "https://localhost:7073/api/estadocuenta/" + cliente.Id;
                    var resp = await client.GetAsync(apiUrl2);

                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Estado Cuenta no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var cont = await resp.Content.ReadAsStringAsync();
                    var edoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(cont, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string apiUrlRol = "https://localhost:7073/api/rol/";
                    var responseRol = await client.GetAsync(apiUrlRol);

                    var contentRol = await responseRol.Content.ReadAsStringAsync();
                    var roles = System.Text.Json.JsonSerializer.Deserialize<List<Rol>>(contentRol, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    clienteViewModel.Cliente = cliente;
                    clienteViewModel.Cliente.EstadoCuenta = edoCuenta;
                    clienteViewModel.Usuario = usuario;
                    clienteViewModel.Roles = roles;

                }

                return View(clienteViewModel);
            }
        }

        public async Task<IActionResult> AgregarUsuario()
        {
            ClienteViewModel clienteModel = new ClienteViewModel();

            // Realizar la solicitud al API para obtener los roles de usuario
            var cliente = new HttpClient();

            string apiUrl = "https://localhost:7073/api/rol/";
            var response = await cliente.GetAsync(apiUrl);

            var content = await response.Content.ReadAsStringAsync();
            var roles = System.Text.Json.JsonSerializer.Deserialize<List<Rol>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (roles?.Count > 0)
            {
                clienteModel.Roles = roles;
            }


            return View(clienteModel);
        }

        public async Task<IActionResult> ModuloGestionConcepto()
        {

            HttpClient client = new HttpClient();

            var UrlApi = "https://localhost:7073/api/conceptocobro/";
            var response = await client.GetAsync(UrlApi);

            var content = await response.Content.ReadAsStringAsync();
            var conceptos = System.Text.Json.JsonSerializer.Deserialize<List<ConceptoCobro>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(conceptos);
        }

        [HttpPost]
        public async Task<IActionResult> AltaCliente(ClienteViewModel model)
        {

            if (ModelState.IsValid)
            {
                var cliente = new HttpClient();

                string urlApi = "https://localhost:7073/api/usuario/registro";
                var response = await cliente.PostAsync(urlApi, new StringContent($"{{\"usuarioNombre\":\"{model?.Usuario?.UsuarioNombre}\",\"contrasena\":\"{model?.Usuario?.Contrasena}\",\"rol_id\":\"{model?.Rol_Id}\"}}", Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (usuario != null)
                    {
                        var client = new HttpClient();

                        DateTime fechaHora = DateTime.Now;
                        string cadenaNumeroCliente = fechaHora.ToString("dd") + fechaHora.ToString("MM") + fechaHora.ToString("yyyy") + fechaHora.ToString("hh") + fechaHora.ToString("mm") + fechaHora.ToString("ss") + model?.Rol_Id;

                        var apiUrl = "https://localhost:7073/api/cliente/registro";
                        var respons = await client.PostAsync(apiUrl, new StringContent($"{{\"nombre\":\"{model?.Cliente?.Nombre}\",\"numeroCliente\":\"{cadenaNumeroCliente}\",\"usuario_Id\":\"{usuario.Id}\"}}", Encoding.UTF8, "application/json"));

                        if (respons.IsSuccessStatusCode)
                        {
                            var conten = await respons.Content.ReadAsStringAsync();
                            var nCliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(conten, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            if (nCliente != null)
                            {
                                var clie = new HttpClient();
                                var apiURL = "https://localhost:7073/api/estadocuenta/registro";
                                var resp = await client.PostAsync(apiURL, new StringContent($"{{\"saldo\":\"{"0.0"}\",\"cliente_id\":\"{nCliente.Id}\"}}", Encoding.UTF8, "application/json"));
                                if (resp.IsSuccessStatusCode)
                                {
                                    var cont = await resp.Content.ReadAsStringAsync();
                                    var edoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(cont, new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });
                                    if (edoCuenta != null)
                                    {
                                        model.Cliente = nCliente;
                                        model.Usuario = usuario;
                                        model.Cliente.EstadoCuenta = edoCuenta;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            HttpContext.Session.SetString("ClienteModel", JsonConvert.SerializeObject(model));
            return RedirectToAction("ClienteCreado", "Administrador");
        }

        public IActionResult ClienteCreado()
        {
            var clienteModel = JsonConvert.DeserializeObject<ClienteViewModel>(HttpContext.Session.GetString("ClienteModel"));
            ViewBag.ClienteModel = clienteModel;
            return View();
        }

        public async Task<IActionResult> EditarCliente(string clienteId)
        {
            ClienteViewModel clienteViewModel = new ClienteViewModel();

            if (clienteId == null)
            {
                return View("Error", "Error");
            }
            else
            {
                // Realizar la solicitud al API para obtener el cliente
                var client = new HttpClient();

                string apiUrl = "https://localhost:7073/api/cliente/" + clienteId;
                var response = await client.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Cliente no encontrado";
                    return RedirectToAction("Error", "Cliente");
                }
                var content = await response.Content.ReadAsStringAsync();
                var cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cliente != null)
                {
                    string apiURL = "https://localhost:7073/api/usuario/" + cliente.Usuario_Id;
                    var respon = await client.GetAsync(apiURL);

                    if (respon.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Usuario no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var conte = await respon.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<Usuario>(conte, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string apiUrlURol = "https://localhost:7073/api/rol/" + usuario.Rol_Id;
                    var responseURol = await client.GetAsync(apiUrlURol);

                    var contentURol = await responseURol.Content.ReadAsStringAsync();
                    var rolusuario = System.Text.Json.JsonSerializer.Deserialize<Rol>(contentURol, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    usuario.Rol = rolusuario;

                    string apiUrl2 = "https://localhost:7073/api/estadocuenta/" + cliente.Id;
                    var resp = await client.GetAsync(apiUrl2);

                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Estado Cuenta no encontrado";
                        return RedirectToAction("Error", "Cliente");
                    }
                    var cont = await resp.Content.ReadAsStringAsync();
                    var edoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(cont, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string apiUrlRol = "https://localhost:7073/api/rol/";
                    var responseRol = await client.GetAsync(apiUrlRol);

                    var contentRol = await responseRol.Content.ReadAsStringAsync();
                    var roles = System.Text.Json.JsonSerializer.Deserialize<List<Rol>>(contentRol, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    clienteViewModel.Cliente = cliente;
                    clienteViewModel.Cliente.EstadoCuenta = edoCuenta;
                    clienteViewModel.Usuario = usuario;
                    clienteViewModel.Roles = roles;

                }

                return View(clienteViewModel);
            }
        }

        public async Task<IActionResult> EditadoCliente(ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cliente = new HttpClient();

                var rolId = model.Rol_Id != null ? model.Rol_Id : model.Usuario.Rol_Id;

                string urlApi = "https://localhost:7073/api/usuario/editar/" + model?.Usuario?.Id;
                var response = await cliente.PutAsync(urlApi, new StringContent($"{{\"id\":\"{model?.Usuario?.Id}\",\"usuarioNombre\":\"{model?.Usuario?.UsuarioNombre}\",\"contrasena\":\"{model?.Usuario?.Contrasena}\",\"rol_id\":\"{rolId}\"}}", Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (usuario != null)
                    {
                        var client = new HttpClient();

                        var apiUrl = "https://localhost:7073/api/cliente/editar/"+model?.Cliente?.Id;
                        var respons = await client.PutAsync(apiUrl, new StringContent($"{{\"id\":\"{model?.Cliente?.Id}\",\"nombre\":\"{model?.Cliente?.Nombre}\",\"numeroCliente\":\"{model?.Cliente?.NumeroCliente}\",\"usuario_Id\":\"{usuario.Id}\"}}", Encoding.UTF8, "application/json"));

                        if (respons.IsSuccessStatusCode)
                        {
                            var conten = await respons.Content.ReadAsStringAsync();
                            var nCliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(conten, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            if (nCliente != null)
                            {
                                var clie = new HttpClient();
                                var apiURL = "https://localhost:7073/api/rol/" + usuario.Rol_Id;
                                var resp = await client.GetAsync(apiURL);
                                if (resp.IsSuccessStatusCode)
                                {
                                    var cont = await resp.Content.ReadAsStringAsync();
                                    var rol = System.Text.Json.JsonSerializer.Deserialize<Rol>(cont, new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });
                                    if (rol != null)
                                    {
                                        model.Cliente = nCliente;
                                        model.Usuario = usuario;
                                        model.Usuario.Rol = rol;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            HttpContext.Session.SetString("ClienteModel", JsonConvert.SerializeObject(model));
            return RedirectToAction("ClienteEditado", "Administrador");
        }

        public IActionResult ClienteEditado()
        {
            var clienteModel = JsonConvert.DeserializeObject<ClienteViewModel>(HttpContext.Session.GetString("ClienteModel"));
            ViewBag.ClienteModel = clienteModel;
            return View();
        }

        public async Task<IActionResult> EliminarCliente(int clienteId)
        {
            ClienteViewModel clienteViewModel = await ObtenerModeloCliente(clienteId);

            return View(clienteViewModel);
        }

        public async Task<IActionResult> EliminadoCliente(int clienteId)
        {
            ClienteViewModel clienteViewModel = await ObtenerModeloCliente(clienteId);
            
            if (clienteId != 0)
            {
                var cliente = new HttpClient();

                string urlApi = "https://localhost:7073/api/usuario/" + clienteViewModel.Usuario.Id;
                var response = await cliente.DeleteAsync(urlApi);

                if (response.IsSuccessStatusCode)
                {
                    var apiUrl = "https://localhost:7073/api/cliente/" + clienteViewModel?.Cliente?.Id;
                    var respons = await cliente.DeleteAsync(apiUrl);
                }
            }
            HttpContext.Session.SetString("ClienteModel", JsonConvert.SerializeObject(clienteViewModel));
            return RedirectToAction("ClienteEliminado", "Administrador");
        }

        public IActionResult ClienteEliminado()
        {
            var clienteModel = JsonConvert.DeserializeObject<ClienteViewModel>(HttpContext.Session.GetString("ClienteModel"));
            ViewBag.ClienteModel = clienteModel;
            return View();
        }

        public async Task<IActionResult> CrearConcepto(ConceptoCobro concepto) 
        {
            if (concepto.Nombre != null && concepto.Valor != 0)
            {
                HttpClient client = new HttpClient();
                string urlApi = "https://localhost:7073/api/conceptocobro/";
                var response = await client.PostAsync(urlApi, new StringContent($"{{\"nombre\":\"{concepto.Nombre}\",\"valor\":\"{concepto.Valor}\"}}", Encoding.UTF8, "application/json"));

                return RedirectToAction("ModuloGestionConcepto");
            }

            else 
            {
                return View();
            }
        }

        public async Task<IActionResult> EditarConcepto(ConceptoCobro concepto, int id)
        {
            if (concepto.Nombre != null && concepto.Valor != 0)
            {
                HttpClient client = new HttpClient();
                string urlApi = "https://localhost:7073/api/conceptocobro/editar/" + concepto.Id;
                var response = await client.PutAsync(urlApi, new StringContent($"{{\"id\":\"{concepto.Id}\",\"nombre\":\"{concepto.Nombre}\",\"valor\":\"{concepto.Valor}\"}}", Encoding.UTF8, "application/json"));

                return RedirectToAction("ModuloGestionConcepto");
            }
            else if(id != 0)
            {
                // Realizar la solicitud al API para obtener el cliente
                var client = new HttpClient();

                string apiUrl = "https://localhost:7073/api/conceptocobro/" + id;
                var response = await client.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Cliente no encontrado";
                }
                var content = await response.Content.ReadAsStringAsync();
                var conceptoEditar = System.Text.Json.JsonSerializer.Deserialize<ConceptoCobro>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                return View(conceptoEditar);
            }

            else
            {
                return RedirectToAction("ModuloGestionConcepto");
            }
        }

        public async Task<IActionResult> EliminarConcepto(int id)
        {
            if (id != 0)
            {
                HttpClient client = new HttpClient();
                string urlApi = "https://localhost:7073/api/conceptocobro/" + id;
                var response = await client.DeleteAsync(urlApi);

                if(!response.IsSuccessStatusCode) 
                {
                    ViewBag.Message = "No se ha podido eliminar el concepto";
                }

            }
            return RedirectToAction("ModuloGestionConcepto");
        }

        private async Task<ClienteViewModel> ObtenerModeloCliente(int clienteId) 
        {
            ClienteViewModel clienteViewModel = new ClienteViewModel();

            if (clienteId == 0)
            {
                return clienteViewModel;
            }
            else
            {
                // Realizar la solicitud al API para obtener el cliente
                var client = new HttpClient();

                string apiUrl = "https://localhost:7073/api/cliente/" + clienteId;
                var response = await client.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Cliente no encontrado";
                }
                var content = await response.Content.ReadAsStringAsync();
                var cliente = System.Text.Json.JsonSerializer.Deserialize<Cliente>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cliente != null)
                {
                    string apiURL = "https://localhost:7073/api/usuario/" + cliente.Usuario_Id;
                    var respon = await client.GetAsync(apiURL);

                    if (respon.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Usuario no encontrado";
                    }
                    var conte = await respon.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<Usuario>(conte, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string apiUrlURol = "https://localhost:7073/api/rol/" + usuario.Rol_Id;
                    var responseURol = await client.GetAsync(apiUrlURol);

                    var contentURol = await responseURol.Content.ReadAsStringAsync();
                    var rolusuario = System.Text.Json.JsonSerializer.Deserialize<Rol>(contentURol, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    usuario.Rol = rolusuario;

                    string apiUrl2 = "https://localhost:7073/api/estadocuenta/" + cliente.Id;
                    var resp = await client.GetAsync(apiUrl2);

                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.Message = "Estado Cuenta no encontrado";
                    }
                    var cont = await resp.Content.ReadAsStringAsync();
                    var edoCuenta = System.Text.Json.JsonSerializer.Deserialize<EstadoCuenta>(cont, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    clienteViewModel.Cliente = cliente;
                    clienteViewModel.Cliente.EstadoCuenta = edoCuenta;
                    clienteViewModel.Usuario = usuario;

                }

            }

            return clienteViewModel;
        }

    }
}
