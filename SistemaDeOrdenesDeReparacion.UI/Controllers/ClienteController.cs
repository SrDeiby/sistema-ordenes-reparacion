using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;
using SistemaDeOrdenesDeReparacion.UI.Models;

namespace SistemaDeOrdenesDeReparacion.UI.Controllers
{
    public class ClienteController : Controller
    {
       
      
        private readonly IConfiguration _configuration;


        public ClienteController( IConfiguration configuration)
        {
           
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult VistaCliente()
        {
            var identificacionEnSession = HttpContext.Session.GetString("Identificacion");
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(identificacionEnSession) || rol != "cliente")
                return RedirectToAction("Index", "Login");

            try
            {

                using (var client = new HttpClient())
                {
                    var apiKey = _configuration.GetValue<string>(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    client.DefaultRequestHeaders.Add(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);
                     

                var urlapi = _configuration.GetValue<string>("URLAPI"); // lee la URL base desde configuración
                    client.BaseAddress = new Uri(urlapi);
                    var url = $"{urlapi}api/ServicioDeLogin/ObtengaClientePorIdentificacion/{identificacionEnSession}"; // usa la nueva URL

                  //  var url = $"https://localhost:7102/api/ServicioDeLogin/ObtengaClientePorIdentificacion/{identificacionEnSession}";
                    var response = client.GetAsync(url);


                    if (!response.Result.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo conectar con el servidor.";
                        return View("Error");
                    }

                    var cliente = response.Result.Content.ReadFromJsonAsync<Cliente>().Result;

                    if (cliente == null)
                    {
                        ViewBag.Error = "No se encontró información del Cliente.";
                        return View("Error");

                    }

                    var nombreCompleto = $"{cliente.Nombre} {cliente.Apellidos}";
                    ViewBag.NombreCompleto = nombreCompleto;

                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }


        public async Task<IActionResult> Index(string Nombre)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                client.DefaultRequestHeaders.Add(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);


                var urlapi = _configuration.GetValue<string>("URLAPI"); // lee la URL base desde configuración
                client.BaseAddress = new Uri(urlapi);
                string url = string.IsNullOrEmpty(Nombre)
                    ? $"{urlapi}api/ServicioDelCliente/ObtenerListaDeClientes"
                    : $"{urlapi}api/ServicioDelCliente/ObtenerClientePorNombre?nombre={Nombre}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }


                var cliente = await response.Content.ReadFromJsonAsync<List<Cliente>>();
                return View(cliente);
            }
        }
        // GET: Cliente/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDelCliente/RegistrarCliente";

                var response = await client.PostAsJsonAsync(endpoint, cliente);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"{mensaje}");
                    return View(cliente);
                }

                return RedirectToAction(nameof(Index));
            }
        }



        public async Task<IActionResult> Edit(int id)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    ConstantesDeAutenticacion.ApiKeySectionName);

                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDelCliente/ObtenerClientePorId?id={id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Error al obtener el cliente.";
                    return View("Error");
                }

                var cliente = await response.Content.ReadFromJsonAsync<Cliente>();
                if (cliente == null)
                {
                    return View("Error");
                }

                return View(cliente);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string identificacion, string nombre, string apellidos, string correoelectronico)
        {
            var model = new Cliente
            {
                Id = id,
                Identificacion = identificacion,
                Nombre = nombre,
                Apellidos = apellidos,
                CorreoElectronico = correoelectronico
            };

            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDelCliente/EditarCliente";

                var response = await client.PutAsJsonAsync(endpoint, model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"{error}");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
        }




        [HttpGet]
        public async Task<ActionResult> VerificarContrasena()
        {
            var identificacionEnSession = HttpContext.Session.GetString("Identificacion");
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(identificacionEnSession) || rol != "cliente")
            {
                ViewBag.Error = "Debes iniciar sesión como cliente para poder cambiar tu contraseña.";
                return View("Error");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var apiKey = _configuration.GetValue<string>(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    var urlapi = _configuration.GetValue<string>("URLAPI");
                    client.BaseAddress = new Uri(urlapi);

                    client.DefaultRequestHeaders.Add(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                    var endpoint = $"api/ServicioDeLogin/ObtengaUsuarioPorIdentificacionDelCliente/{identificacionEnSession}";

                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo conectar con el servidor.";
                        return View("Error");
                    }

                    var usuario = await response.Content.ReadFromJsonAsync<Usuario>();

                    if (usuario == null)
                    {
                        ViewBag.Error = "No se encontró información del Cliente.";
                        return View("Error");
                    }

                    var modelo = new VerificarContrasenaViewModel
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Contrasena = string.Empty
                    };

                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error: " + ex.Message;
                return View("Error");
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerificarContrasena(VerificarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var apiKey = _configuration.GetValue<string>(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    var urlapi = _configuration.GetValue<string>("URLAPI");
                    client.BaseAddress = new Uri(urlapi);

                    client.DefaultRequestHeaders.Add(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                    var endpoint = "api/ServicioDeLogin/VerifiqueContrasena";

                    var request = new
                    {
                        Nombre = model.Nombre,
                        Contrasena = model.Contrasena
                    };

                    var response = await client.PostAsJsonAsync(endpoint, request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = "Error al verificar contraseña: " + error;
                        return View(model);
                    }

                    return RedirectToAction("CambiarContrasena", new { id = model.Id });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al verificar la contraseña: " + ex.Message;
                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> CambiarContrasena(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var apiKey = _configuration.GetValue<string>(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    var urlapi = _configuration.GetValue<string>("URLAPI");
                    client.BaseAddress = new Uri(urlapi);

                    client.DefaultRequestHeaders.Add(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                    var endpoint = $"api/ServicioDeLogin/ObtengaUsuarioPorId/{id}";

                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo conectar con el servidor.";
                        return View("Error");
                    }

                    var usuario = await response.Content.ReadFromJsonAsync<Usuario>();

                    if (usuario == null)
                    {
                        ViewBag.Error = "No se encontró información del Cliente.";
                        return View("Error");
                    }

                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error: " + ex.Message;
                return View("Error");
            }
        }


        //
        // POST: Cliente/CambiarContrasena/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarContrasena(int id, string nuevaContrasena1, string nuevaContrasena2)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena1) || string.IsNullOrWhiteSpace(nuevaContrasena2))
            {
                ViewBag.Error = "Los campos no pueden ir vacíos.";
                return View();
            }

            if (nuevaContrasena1 != nuevaContrasena2)
            {
                ViewBag.Error = "Las contraseñas no coinciden. Por favor, inténtalo otra vez.";
                return View();
            }

            if (nuevaContrasena1.Length < 8)
            {
                ViewBag.Error = "La nueva contraseña debe tener al menos 8 caracteres.";
                return View();
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var apiKey = _configuration.GetValue<string>(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    var urlapi = _configuration.GetValue<string>("URLAPI");
                    client.BaseAddress = new Uri(urlapi);

                    client.DefaultRequestHeaders.Add(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                    var endpoint = "api/ServicioDeLogin/CambieContrasenaDelUsuario";

                    var request = new
                    {
                        Id = id,
                        NuevaContrasena = nuevaContrasena1
                    };

                    var response = await client.PostAsJsonAsync(endpoint, request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var mensajeError = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = "No se pudo cambiar la contraseña: " + mensajeError;
                        return View();
                    }

                    return RedirectToAction("VistaCliente");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al actualizar la contraseña: " + ex.Message;
                return View();
            }
        }


    }
}
