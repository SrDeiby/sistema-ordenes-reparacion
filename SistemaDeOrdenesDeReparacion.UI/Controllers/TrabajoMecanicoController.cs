using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;
using SistemaDeOrdenesDeReparacion.UI.Models;
using System.Diagnostics;


namespace SistemaDeOrdenesDeReparacion.UI.Controllers
{
    public class TrabajoMecanicoController : Controller
    {

        private readonly IConfiguration _configuration;

      
        public TrabajoMecanicoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            ? "api/ServicioDeTrabajoMecanico/ObtengaLaListaDeOrdenesEnProceso"
            : $"api/ServicioDeTrabajoMecanico/FiltreLaListaDeOrdenes?filtro={Uri.EscapeDataString(nombre)}";

        var response = await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("Error", errorModel);
        }

        var ordenesDeTrabajo = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajos>>();
        return View(ordenesDeTrabajo);
    }
}

        public async Task<IActionResult> OrdenesCanceladas(string nombre)
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
                    ? "api/ServicioDeTrabajoMecanico/ObtengaLaListaDeOrdenesCanceladas"
                    : $"api/ServicioDeTrabajoMecanico/FiltreLaListaDeOrdenesCanceladas?filtro={Uri.EscapeDataString(nombre)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }

                var ordenesDeTrabajo = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajos>>();
                return View(ordenesDeTrabajo);
            }
        }


        public async Task<IActionResult> MostrarRepuestos(int idOrden, string filtro)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                string endpoint = string.IsNullOrEmpty(filtro)
                    ? "api/ServicioDeTrabajoMecanico/ObtengaLosRepuestos"
                    : $"api/ServicioDeTrabajoMecanico/FiltreRepuestoPorNombre?filtro={Uri.EscapeDataString(filtro)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }

                var repuestos = await response.Content.ReadFromJsonAsync<List<InventarioDeRepuestos>>();
                ViewBag.IdOrden = idOrden;
                return View(repuestos);
            }
        }

        public async Task<IActionResult> MostrarServicios(int idOrden, string filtro)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                string endpoint = string.IsNullOrEmpty(filtro)
                    ? "api/ServicioDeTrabajoMecanico/ObtengaLosServicios"
                    : $"api/ServicioDeTrabajoMecanico/FiltreServiciosPorNombre?filtro={Uri.EscapeDataString(filtro)}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    };

                    return View("Error", errorModel);
                }

                var servicios = await response.Content.ReadFromJsonAsync<List<InventarioDeServicios>>();
                ViewBag.IdOrden = idOrden;
                return View(servicios);
            }
        }



        public async Task<IActionResult> Asignar(int idServicio, int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpointServicio = $"api/ServicioDeTrabajoMecanico/ServicioPorId?id={idServicio}";
                var servicioResponse = await client.GetAsync(endpointServicio);

                var endpointOrden = $"api/ServicioDeTrabajoMecanico/OrdenPorId?id={idOrden}";
                var ordenResponse = await client.GetAsync(endpointOrden);

                if (!servicioResponse.IsSuccessStatusCode || !ordenResponse.IsSuccessStatusCode)
                    return NotFound();

                var servicio = await servicioResponse.Content.ReadFromJsonAsync<InventarioDeServicios>();
                var orden = await ordenResponse.Content.ReadFromJsonAsync<OrdenesDeTrabajos>();

                var model = new OrdenesDeTrabajoInventarioDeServicios
                {
                    Id_InventarioDeServicios = idServicio,
                    Id_OrdenesDeTrabajo = idOrden
                };

                ViewBag.NombreServicio = servicio.Nombre;
                ViewBag.NombreOrden = $"Orden #{orden.Id}";

                return View(model);
            }
        }



        // POST: TrabajoMecanicoController/Asignar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(OrdenesDeTrabajoInventarioDeServicios model)
        {
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

                // 1. Obtener el servicio para calcular el total
                var endpointServicio = $"api/ServicioDeTrabajoMecanico/ServicioPorId?id={model.Id_InventarioDeServicios}";
                var servicioResponse = await client.GetAsync(endpointServicio);

                if (!servicioResponse.IsSuccessStatusCode)
                    return View("Error");

                var servicio = await servicioResponse.Content.ReadFromJsonAsync<InventarioDeServicios>();
                model.Total = model.Cantidad * servicio.Precio;

                // 2. Enviar el modelo a la API para asignar
                var endpointAsignar = "api/ServicioDeTrabajoMecanico/AsignarServicio";
                var asignacionResponse = await client.PostAsJsonAsync(endpointAsignar, model);

                if (!asignacionResponse.IsSuccessStatusCode)
                    return View("Error");
            }

            return RedirectToAction("MostrarServicios", new { idOrden = model.Id_OrdenesDeTrabajo });
        }




        // GET: TrabajoMecanicoController/Asignar/5
        public async Task<IActionResult> AsigneUnRepuesto(int idRepuesto, int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                // Obtener el repuesto por ID
                var endpointRepuesto = $"api/ServicioDeTrabajoMecanico/RepuestoPorId?id={idRepuesto}";
                var repuestoResponse = await client.GetAsync(endpointRepuesto);
                if (!repuestoResponse.IsSuccessStatusCode)
                    return View("Error");

                var repuesto = await repuestoResponse.Content.ReadFromJsonAsync<InventarioDeRepuestos>();

                // Obtener la orden por ID
                var endpointOrden = $"api/ServicioDeTrabajoMecanico/OrdenPorId?id={idOrden}";
                var ordenResponse = await client.GetAsync(endpointOrden);
                if (!ordenResponse.IsSuccessStatusCode)
                    return View("Error");

                var orden = await ordenResponse.Content.ReadFromJsonAsync<OrdenesDeTrabajos>();

                if (repuesto == null || orden == null)
                    return NotFound();

                var model = new OrdenesDeTrabajoInventarioDeRepuestos
                {
                    Id_InventarioDeRepuestos = idRepuesto,
                    Id_OrdenesDeTrabajos = idOrden
                };

                ViewBag.NombreServicio = repuesto.Nombre;
                ViewBag.NombreOrden = $"Orden #{orden.Id}";

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsigneUnRepuesto(OrdenesDeTrabajoInventarioDeRepuestos model)
        {
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

                // 1. Obtener el repuesto para calcular el total
                var endpointRepuesto = $"api/ServicioDeTrabajoMecanico/RepuestoPorId?id={model.Id_InventarioDeRepuestos}";
                var repuestoResponse = await client.GetAsync(endpointRepuesto);
                if (!repuestoResponse.IsSuccessStatusCode)
                    return View("Error");

                var repuesto = await repuestoResponse.Content.ReadFromJsonAsync<InventarioDeRepuestos>();
                model.Total = model.Cantidad * repuesto.PrecioUnitario;

                // 2. Enviar modelo a la API para asignarlo
                var endpointAsignar = "api/ServicioDeTrabajoMecanico/AsignarRepuesto";
                var asignacionResponse = await client.PostAsJsonAsync(endpointAsignar, model);
                if (!asignacionResponse.IsSuccessStatusCode)
                    return View("Error");
            }

            return RedirectToAction("MostrarRepuestos", new { idOrden = model.Id_OrdenesDeTrabajos });
        }


        public async Task<IActionResult> MostrarRepuestosAsignados(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeTrabajoMecanico/RepuestosAsignados?idOrden={idOrden}";
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var lista = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeRepuestos>>();

                ViewBag.IdOrden = idOrden;
                return View(lista);
            }
        }

        public async Task<IActionResult> MostrarRepuestosAsignadosACliente(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeTrabajoMecanico/RepuestosAsignados?idOrden={idOrden}";
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var lista = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeRepuestos>>();

                ViewBag.IdOrden = idOrden;
                return View(lista);
            }
        }


        public async Task<IActionResult> MostrarServiciosAsignados(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeTrabajoMecanico/ServiciosAsignados?idOrden={idOrden}";
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var lista = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeServicios>>();

                ViewBag.IdOrden = idOrden;
                return View(lista);
            }
        }



        public async Task<IActionResult> MostrarServiciosAsignadosACliente(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeTrabajoMecanico/ServiciosAsignados?idOrden={idOrden}";
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var lista = await response.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeServicios>>();

                ViewBag.IdOrden = idOrden;
                return View(lista);
            }
        }



        public async Task<IActionResult> VerResumen(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpoint = $"api/ServicioDeTrabajoMecanico/ResumenDeOrden?idOrden={idOrden}";
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudo obtener el resumen desde la API.";
                    return View("Error");
                }

                var resumen = await response.Content.ReadFromJsonAsync<ResumenDeCostoViewModel>();
                if (resumen == null)
                {
                    ViewBag.Error = "No se pudo obtener el resumen.";
                    return View("Error");
                }

                ViewBag.IdOrden = idOrden;
                ViewBag.TotalRepuestos = resumen.TotalRepuestos;
                ViewBag.TotalServicios = resumen.TotalServicios;
                ViewBag.Total = resumen.Total;

                return View();
            }
        }


        public async Task<IActionResult> MostrarDetalleDeOrdenCancelada(int idOrden)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var apiKey = _configuration.GetValue<string>(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                    var urlapi = _configuration.GetValue<string>("URLAPI");
                    client.BaseAddress = new Uri(urlapi);

                    client.DefaultRequestHeaders.Add(
                        SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                    var endpoint = $"api/ServicioDeTrabajoMecanico/OrdenCanceladaPorId?idOrden={idOrden}";
                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "No se pudo obtener el detalle de la orden cancelada";
                        return RedirectToAction("OrdenesCanceladas");
                    }

                    var orden = await response.Content.ReadFromJsonAsync<OrdenesDeTrabajos>();

                    if (orden == null)
                    {
                        TempData["Error"] = "Orden cancelada no encontrada";
                        return RedirectToAction("OrdenesCanceladas");
                    }

                    return View(orden);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al cargar el detalle de la orden: " + ex.Message;
                    return RedirectToAction("OrdenesCanceladas");
                }
            }
        }



        public async Task<IActionResult> OrdenesClientes()
        {
            var identificacionEnSession = HttpContext.Session.GetString("Identificacion");
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(identificacionEnSession) || rol != "cliente")
            {
                ViewBag.Error = "Debe iniciar sesión como cliente para ver esta información.";
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

                    // 1. Obtener el cliente por la identificación de sesión
                    var endpointCliente = $"api/ServicioDeLogin/ClientePorIdentificacionUsuario/{identificacionEnSession}";
                    var clienteResponse = await client.GetAsync(endpointCliente);
                    if (!clienteResponse.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudo obtener el cliente desde la API.";
                        return View("Error");
                    }

                    var cliente = await clienteResponse.Content.ReadFromJsonAsync<Cliente>();
                    if (cliente == null)
                    {
                        ViewBag.Error = "Cliente no encontrado.";
                        return View("Error");
                    }

                    // 2. Obtener órdenes por cliente
                    var endpointOrdenes = $"api/ServicioDeTrabajoMecanico/OrdenesPorCliente?idCliente={cliente.Id}";
                    var ordenesResponse = await client.GetAsync(endpointOrdenes);
                    if (!ordenesResponse.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "No se pudieron obtener las órdenes del cliente.";
                        return View("Error");
                    }

                    var ordenesDelCliente = await ordenesResponse.Content.ReadFromJsonAsync<List<OrdenesDeTrabajos>>();
                    ViewBag.IdCliente = cliente.Id;
                    ViewBag.NombreCompleto = cliente.Nombre + " " + cliente.Apellidos;
                    return View(ordenesDelCliente);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener las órdenes del cliente: " + ex.Message;
                return View("Error");
            }
        }


        public async Task<IActionResult> Detalle(int idOrden)
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                // Obtener la orden
                var endpointOrden = $"api/ServicioDeTrabajoMecanico/OrdenPorId?id={idOrden}";
                var ordenResponse = await client.GetAsync(endpointOrden);
                if (!ordenResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudo obtener la orden de trabajo.";
                    return View("Error");
                }

                var orden = await ordenResponse.Content.ReadFromJsonAsync<OrdenesDeTrabajos>();
                if (orden == null)
                {
                    ViewBag.Error = "Orden no encontrada.";
                    return View("Error");
                }

                // Obtener los repuestos asignados
                var endpointRepuestos = $"api/ServicioDeTrabajoMecanico/RepuestosAsignados?idOrden={idOrden}";
                var repuestosResponse = await client.GetAsync(endpointRepuestos);

                // Obtener los servicios asignados
                var endpointServicios = $"api/ServicioDeTrabajoMecanico/ServiciosAsignados?idOrden={idOrden}";
                var serviciosResponse = await client.GetAsync(endpointServicios);

                if (!repuestosResponse.IsSuccessStatusCode || !serviciosResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudieron obtener los detalles de la orden.";
                    return View("Error");
                }

                var repuestos = await repuestosResponse.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeRepuestos>>();
                var servicios = await serviciosResponse.Content.ReadFromJsonAsync<List<OrdenesDeTrabajoInventarioDeServicios>>();

                var totalRepuestos = repuestos.Sum(r => r.Total);
                var totalServicios = servicios.Sum(s => s.Total);

                var viewModel = new ResumenDeOrdenDelClienteViewModel
                {
                    Orden = orden,
                    Repuestos = repuestos,
                    Servicios = servicios,
                    TotalRepuestos = totalRepuestos,
                    TotalServicios = totalServicios
                };

                return View(viewModel);
            }
        }



    }
}
