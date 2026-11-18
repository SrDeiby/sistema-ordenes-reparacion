using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.Model;
using SistemaDeOrdenesDeReparacion.SI.Autenticacion;
using SistemaDeOrdenesDeReparacion.SI.Controllers;

public class OrdenesDeTrabajoController : Controller
{

    private readonly IConfiguration _configuration;

    public OrdenesDeTrabajoController(IConfiguration configuration)
    {

        _configuration = configuration;
    }

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
                ? "api/ServiciosDeOrdenesDeTrabajo/ObtenerListaDeOrdenesDeTrabajo"
                : $"api/ServiciosDeOrdenesDeTrabajo/ObtenerOrdenPorNombre?nombre={Uri.EscapeDataString(Nombre)}";

            try
            {
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = $"Error al obtener las órdenes: {response.ReasonPhrase}";
                    return View(new List<SistemaDeOrdenesDeReparacion.Model.ResumenDeOrdenesViewModel>());
                }

                var ordenes = await response.Content.ReadFromJsonAsync<List<SistemaDeOrdenesDeReparacion.Model.ResumenDeOrdenesViewModel>>();

                return View(ordenes ?? new List<SistemaDeOrdenesDeReparacion.Model.ResumenDeOrdenesViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al conectar con el API: {ex.Message}";
                return View(new List<SistemaDeOrdenesDeReparacion.Model.ResumenDeOrdenesViewModel>());
            }
        }
    }


    [HttpGet]
    public async Task<IActionResult> Crear()
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

                var endpoint = "api/ServiciosDeOrdenesDeTrabajo/PrepararFormularioDeVistaOrden";

                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var modelo = await response.Content.ReadFromJsonAsync<SistemaDeOrdenesDeReparacion.Model.CrearOrdenViewModel>();

                    if (modelo == null)
                    {
                        TempData["Mensaje"] = "No se pudieron obtener los datos del formulario.";
                        return RedirectToAction("Index");
                    }

                    return View(modelo);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al cargar el formulario: {response.ReasonPhrase}. {errorContent}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al conectar con el servicio: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SistemaDeOrdenesDeReparacion.Model.CrearOrdenViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            try
            {
                var endpoint = "api/ServiciosDeOrdenesDeTrabajo/NuevaOrdenDeTrabajo";

                var ordenTrabajo = new OrdenesDeTrabajoViewModel
                {
                    IdCliente = modelo.Orden.IdCliente,
                    FechaFinalizacion = modelo.Orden.FechaFinalizacion,
                    TipoDeVehiculo = modelo.Orden.TipoDeVehiculo,
                    TipoDeCombustionInterna = modelo.Orden.TipoDeCombustionInterna,
                    Marca = modelo.Orden.Marca,
                    Modelo = modelo.Orden.Modelo,
                    AñoFabricacion = modelo.Orden.AñoFabricacion,
                    DescripcionProblema = modelo.Orden.DescripcionProblema
                };

                var response = await client.PostAsJsonAsync(endpoint, ordenTrabajo);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Orden creada exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al crear la orden: {response.ReasonPhrase}. {errorContent}";
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al conectar con el servicio: {ex.Message}";
                return View(modelo);
            }
        }
    }





    public async Task<IActionResult> Editar(int id)
    {
        if (id <= 0)
        {
            ViewBag.Error = "ID inválido.";
            return View(null);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            var endpoint = $"api/ServiciosDeOrdenesDeTrabajo/EditarOrdenesDeTrabajo?id={id}";

            try
            {
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener la orden con ID {id}.";
                    return View(null);
                }

                var dto = await response.Content.ReadFromJsonAsync<EditarOrdenesDeTrabajoDto>();

                if (dto == null || dto.Orden == null)
                {
                    ViewBag.Error = "Los datos de la orden están incompletos.";
                    return View(null);
                }

                var viewModel = new EditarOrdenesDeTrabajoViewModel
                {
                    Orden = dto.Orden,
                    Clientes = new SelectList(dto.Clientes, "Id", "Nombre", dto.Orden.IdCliente),
                    TiposVehiculo = new SelectList(dto.TiposVehiculo, "Key", "Value", dto.Orden.TipoDeVehiculo),
                    TiposCombustion = new SelectList(dto.TiposCombustion, "Key", "Value", dto.Orden.TipoDeCombustionInterna)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al conectar con el servicio: {ex.Message}";
                return View(null);
            }
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(SistemaDeOrdenesDeReparacion.Model.EditarOrdenesDeTrabajoViewModel request)
    {
        await PoblarListasDesplegablesAsync(request);

        if (request == null || request.Orden == null)
        {
            ModelState.AddModelError(string.Empty, "Los datos de la orden son inválidos.");
            return View(request);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            try
            {
                var endpoint = "api/ServiciosDeOrdenesDeTrabajo/ActualizarOrdenesDeTrabajo";

                var ordenActualizada = new OrdenesDeTrabajoViewModel
                {
                    Id = request.Orden.Id,
                    IdCliente = request.Orden.IdCliente,
                    FechaFinalizacion = request.Orden.FechaFinalizacion,
                    TipoDeVehiculo = request.Orden.TipoDeVehiculo,
                    TipoDeCombustionInterna = request.Orden.TipoDeCombustionInterna,
                    Marca = request.Orden.Marca,
                    Modelo = request.Orden.Modelo,
                    AñoFabricacion = request.Orden.AñoFabricacion,
                    DescripcionProblema = request.Orden.DescripcionProblema
                };

                var response = await client.PutAsJsonAsync(endpoint, ordenActualizada);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al editar la orden: {error}");

                    await PoblarListasDesplegablesAsync(request);
                    return View(request);
                }

                TempData["Mensaje"] = "Orden actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al conectar con el servicio: {ex.Message}");

                await PoblarListasDesplegablesAsync(request);
                return View(request);
            }
        }
    }


    [HttpGet]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (id <= 0)
        {
            ViewBag.Error = "ID inválido.";
            return View(null);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            var endpoint = $"api/ServiciosDeOrdenesDeTrabajo/CancelarOrdenesDeTrabajo?id={id}";

            try
            {
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener la orden con ID {id} para cancelar.";
                    return View(null);
                }

                var modelo = await response.Content.ReadFromJsonAsync<SistemaDeOrdenesDeReparacion.Model.CancelarOrdenViewModel>();

                if (modelo == null || modelo.Id == null)
                {
                    ViewBag.Error = "Los datos de la orden están incompletos.";
                    return View(null);
                }

                return View(modelo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al conectar con el servicio: {ex.Message}";
                return View(null);
            }
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(SistemaDeOrdenesDeReparacion.Model.CancelarOrdenViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            try
            {
                var endpoint = "api/ServiciosDeOrdenesDeTrabajo/CancelarOrdenesDeTrabajo";

                var response = await client.PostAsJsonAsync(endpoint, model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al cancelar la orden: {error}");
                    return View(model);
                }

                TempData["Mensaje"] = "Orden cancelada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al conectar con el servicio: {ex.Message}");
                return View(model);
            }
        }
    }





    // GET: Iniciar Orden
    public async Task<IActionResult> Iniciar(int id)
    {
        if (id <= 0)
        {
            ViewBag.Error = "ID inválido.";
            return View(null);
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            try
            {
                var endpointOrden = $"api/ServiciosDeOrdenesDeTrabajo/ObtenerVistaParaOrdenesDeTrabajo?id={id}";

                var response = await client.GetAsync(endpointOrden);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se pudo obtener la orden con ID {id} para iniciar.";
                    return View(null);
                }

                var modelo = await response.Content.ReadFromJsonAsync<SistemaDeOrdenesDeReparacion.Model.IniciarOrdenViewModel>();

                var endpointMecanicos = "api/ServicioDeMecanico/ObtenerListaDeMecanicos";
                var mecanicosResponse = await client.GetAsync(endpointMecanicos);

                if (mecanicosResponse.IsSuccessStatusCode)
                {
                    var mecanicos = await mecanicosResponse.Content.ReadFromJsonAsync<List<MecanicoDto>>();
                    modelo.Mecanicos = new SelectList(
                        mecanicos.Select(m => new
                        {
                            m.Id,
                            Texto = $"{m.Identificacion} - {m.Nombre} {m.Apellidos}"
                        }),
                        "Id",
                        "Texto"
                    );
                }

                if (modelo == null || modelo.Id == null)
                {
                    ViewBag.Error = "Los datos de la orden están incompletos.";
                    return View(null);
                }

                return View(modelo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al conectar con el servicio: {ex.Message}";
                return View(null);
            }
        }
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int id, int mecanicoId)
    {
        if (mecanicoId <= 0)
        {
            ViewBag.ErrorMessage = "Debe seleccionar un mecánico.";

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI");
                client.BaseAddress = new Uri(urlapi);

                client.DefaultRequestHeaders.Add(
                    SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var endpointOrden = $"api/ServiciosDeOrdenesDeTrabajo/ObtenerVistaParaOrdenesDeTrabajo?id={id}";
                var ordenResponse = await client.GetAsync(endpointOrden);

                var endpointMecanicos = "api/ServicioDeMecanico/ObtenerListaDeMecanicos";
                var mecanicosResponse = await client.GetAsync(endpointMecanicos);

                if (!ordenResponse.IsSuccessStatusCode || !mecanicosResponse.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Error al cargar los datos para iniciar la orden.";
                    return RedirectToAction("Index");
                }

                var orden = await ordenResponse.Content.ReadFromJsonAsync<IniciarOrdenViewModel>();
                var listaMecanicos = await mecanicosResponse.Content.ReadFromJsonAsync<List<MecanicoDto>>();

                orden.Mecanicos = new SelectList(listaMecanicos, "Id", "NombreCompleto");
                return View(orden);
            }
        }

        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            var inicio = new IniciarOrdenDto
            {
                Id = id,
                MecanicoId = mecanicoId
            };

            var endpoint = "api/ServiciosDeOrdenesDeTrabajo/IniciarOrdenesDeTrabajo";

            try
            {
                var response = await client.PostAsJsonAsync(endpoint, inicio);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Orden iniciada exitosamente.";
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Mensaje"] = $"Error al iniciar la orden: {error}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al conectarse con el servicio: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }


    private async Task PoblarListasDesplegablesAsync(SistemaDeOrdenesDeReparacion.Model.EditarOrdenesDeTrabajoViewModel viewModel)
    {
        using (var client = new HttpClient())
        {
            var apiKey = _configuration.GetValue<string>(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
            var urlapi = _configuration.GetValue<string>("URLAPI");
            client.BaseAddress = new Uri(urlapi);

            client.DefaultRequestHeaders.Add(
                SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

            var endpointClientes = "api/ServiciosDeOrdenesDeTrabajo/ObtenerClientesParaDropdown";
            var clientesResponse = await client.GetAsync(endpointClientes);

            if (clientesResponse.IsSuccessStatusCode)
            {
                var clientesDto = await clientesResponse.Content.ReadFromJsonAsync<IEnumerable<ClienteDto>>();
                viewModel.Clientes = new SelectList(clientesDto, "Id", "Nombre", viewModel.Orden.IdCliente);
            }

            var endpointVehiculos = "api/ServiciosDeOrdenesDeTrabajo/ObtenerTiposVehiculo";
            var vehiculosResponse = await client.GetAsync(endpointVehiculos);

            if (vehiculosResponse.IsSuccessStatusCode)
            {
                var tiposVehiculoDto = await vehiculosResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                viewModel.TiposVehiculo = new SelectList(tiposVehiculoDto, "Key", "Value", viewModel.Orden.TipoDeVehiculo.ToString());
            }

            var endpointCombustion = "api/ServiciosDeOrdenesDeTrabajo/ObtenerTiposCombustion";
            var combustionResponse = await client.GetAsync(endpointCombustion);

            if (combustionResponse.IsSuccessStatusCode)
            {
                var tiposCombustionDto = await combustionResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                viewModel.TiposCombustion = new SelectList(tiposCombustionDto, "Key", "Value", viewModel.Orden.TipoDeCombustionInterna.ToString());
            }
        }
    }

}



