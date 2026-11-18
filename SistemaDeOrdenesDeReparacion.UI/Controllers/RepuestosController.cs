

using Microsoft.AspNetCore.Mvc;



using SistemaDeOrdenesDeReparacion.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaDeOrdenesDeReparacion.DA;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;
using SistemaDeOrdenesDeReparacion.UI.Models;
using System.Diagnostics;

namespace SistemaDeOrdenesDeReparacion.Controllers
{

    public class RepuestosController : Controller
    {
        private readonly IConfiguration _configuration;

        public RepuestosController(IConfiguration configuration)
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
                    ? "api/ServicioDeRepuestos/ObtengaLaLista"
                    : $"api/ServicioDeRepuestos/FiltrarRepuestoPorNombre?nombre={Uri.EscapeDataString(nombre)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }

                var repuesto = await response.Content.ReadFromJsonAsync<List<Model.InventarioDeRepuestos>>();
                return View(repuesto);
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

                var endpoint = $"api/ServicioDeRepuestos/BuscarPorId/{id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener el repuesto con ID {id}.";
                    return View(null);
                }

                var repuestos = await response.Content.ReadFromJsonAsync<Model.InventarioDeRepuestos>();
                return View(repuestos);
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
        public async Task<IActionResult> Create(Model.InventarioDeRepuestos repuestos)
        {
            if (!ModelState.IsValid)
                return View(repuestos);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeRepuestos/AgregarRepuesto";

                var response = await client.PostAsJsonAsync(endpoint, repuestos);

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al agregar repuesto: {mensaje}");
                    return View(repuestos);
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

                var endpoint = $"api/ServicioDeRepuestos/BuscarPorId/{id}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener el repuesto con ID {id}.";
                    return View(null);
                }

                var repuestos = await response.Content.ReadFromJsonAsync<Model.InventarioDeRepuestos>();
                return View(repuestos);
            }
        }


        // POST: ServiciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Model.InventarioDeRepuestos repuesto)
        {
            if (!ModelState.IsValid)
                return View(repuesto);

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = "api/ServicioDeRepuestos/ActualizarRepuesto";

                var response = await client.PutAsJsonAsync(endpoint, repuesto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al editar el repuesto: {error}");
                    return View(repuesto);
                }

                return RedirectToAction(nameof(Index));
            }
        }

    }

}