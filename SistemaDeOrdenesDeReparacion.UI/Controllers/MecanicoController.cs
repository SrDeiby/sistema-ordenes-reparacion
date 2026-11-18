using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SistemaDeOrdenesDeReparacion.UI.Models;
using System.Diagnostics;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;

namespace SistemaDeOrdenesDeReparacion.UI.Controllers
{
    public class MecanicoController : Controller
    {

        private readonly IConfiguration _configuration;

        public MecanicoController(IConfiguration configuration)
        {
          
            _configuration = configuration;
        }

        // GET: Mecanico/Index
        public async Task<IActionResult> Index(string Nombre)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                string endpoint = string.IsNullOrEmpty(Nombre)
                    ? "api/ServicioDeMecanico/ObtenerListaDeMecanicos"
                    : $"api/ServicioDeMecanico/ObtenerMecanicoPorNombre?nombre={Uri.EscapeDataString(Nombre)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }

                var mecanicos = await response.Content.ReadFromJsonAsync<List<Mecanico>>();
                return View(mecanicos);
            }
        }


        // GET: Mecanico/VistaMecanico
        [HttpGet]
        public async Task<ActionResult> VistaMecanico()
        {
            var identificacionEnSession = HttpContext.Session.GetString("Identificacion");
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(identificacionEnSession) || rol != "mecanico")
                return RedirectToAction("Index", "Login");

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

                    var endpoint = $"api/ServicioDeLogin/ObtengaMecanicoPorIdentificacion/{identificacionEnSession}";

                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo conectar con el servidor.";
                        return View("Error");
                    }

                    var mecanico = await response.Content.ReadFromJsonAsync<Mecanico>();

                    if (mecanico == null)
                    {
                        ViewBag.Error = "No se encontró información del Mecánico.";
                        return View("Error");
                    }

                    var nombreCompleto = $"{mecanico.Nombre} {mecanico.Apellidos}";
                    ViewBag.NombreCompleto = nombreCompleto;

                    return View(mecanico);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }



        // GET: MecanicoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MecanicoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MecanicoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mecanico mecanico)
        {
            if (!ModelState.IsValid)
                return View(mecanico);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeMecanico/RegistrarMecanico";

                var response = await client.PostAsJsonAsync(endpoint, mecanico);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"{mensaje}");
                    return View(mecanico);
                }

                return RedirectToAction(nameof(Index));
            }
        }





        public async Task<IActionResult> Edit(int id)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeMecanico/ObtenerMecanicoPorId?id={id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Error al obtener el mecánico.";
                    return View("Error");
                }

                var mecanico = await response.Content.ReadFromJsonAsync<Mecanico>();
                if (mecanico == null)
                {
                    ViewBag.Error = "No se encontró información del mecánico.";
                    return View("Error");
                }

                return View(mecanico);
            }
        }


        // POST: MecanicoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string identificacion, string nombre, string apellidos, string correoelectronico)
        {
            var model = new Mecanico
            {
                Id = id,
                Identificacion = identificacion,
                Nombre = nombre,
                Apellidos = apellidos,
                CorreoElectronico = correoelectronico,
            };

            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeMecanico/EditarMecanico";

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

            if (string.IsNullOrEmpty(identificacionEnSession) || rol != "mecanico")
            {
                ViewBag.Error = "Debes iniciar sesión como mecánico para poder cambiar tu contraseña.";
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

                    var endpoint = $"api/ServicioDeLogin/ObtengaUsuarioPorIdentificacionDelMecanico/{identificacionEnSession}";

                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo conectar con el servidor.";
                        return View("Error");
                    }

                    var usuario = await response.Content.ReadFromJsonAsync<Usuario>();

                    if (usuario == null)
                    {
                        ViewBag.Error = "No se encontró información del Mecánico.";
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
        public async Task<IActionResult> VerificarContrasena(VerificarContrasenaViewModel model)
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


        //
        // GET: Mecanico/CambiarContrasena/5
        //
        [HttpGet]
        public async Task<ActionResult> CambiarContrasena(int id)
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
                        ViewBag.Error = "No se encontró información del Mecánico.";
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
        // POST: Mecanico/CambiarContrasena/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(int id, string nuevaContrasena1, string nuevaContrasena2)
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

                    return RedirectToAction("VistaMecanico");
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
