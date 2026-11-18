using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;
using SistemaDeOrdenesDeReparacion.UI.Models;
using System.Diagnostics;

namespace SistemaDeOrdenesDeReparacion.UI.Controllers
{
    public class ServiciosController : Controller
    {

        private readonly IConfiguration _configuration;

        public ServiciosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        // GET: ServiciosController

        [HttpGet]
        public async Task<IActionResult> Index(string nombre)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                string endpoint = string.IsNullOrEmpty(nombre)
                    ? "api/ServicioDeServicios/ObtengaLaLista"
                    : $"api/ServicioDeServicios/FiltrarServicioPorNombre?nombre={Uri.EscapeDataString(nombre)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }

                var servicios = await response.Content.ReadFromJsonAsync<List<Model.InventarioDeServicios>>();
                return View(servicios);
            }
        }






        // GET: ServiciosController/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeServicios/ObtengaElServicio/{id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener el servicio con ID {id}.";
                    return View(null);
                }

                var servicio = await response.Content.ReadFromJsonAsync<Model.InventarioDeServicios>();
                return View(servicio);
            }
        }


        // GET: ServiciosController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiciosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Model.InventarioDeServicios servicios)
        {
            if (!ModelState.IsValid)
                return View(servicios);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeServicios/Agregue";

                var response = await client.PostAsJsonAsync(endpoint, servicios);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al agregar servicio: {mensaje}");
                    return View(servicios);
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

                var endpoint = $"api/ServicioDeServicios/ObtengaElServicio/{id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener el servicio con ID {id}.";
                    return View(null);
                }

                var servicio = await response.Content.ReadFromJsonAsync<Model.InventarioDeServicios>();
                return View(servicio);
            }
        }


        // POST: ServiciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Model.InventarioDeServicios servicio)
        {
            if (!ModelState.IsValid)
                return View(servicio);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeServicios/EditeElServicio";

                var response = await client.PutAsJsonAsync(endpoint, servicio);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al editar el servicio: {error}");
                    return View(servicio);
                }

                return RedirectToAction(nameof(Index));
            }
        }



    }
}
