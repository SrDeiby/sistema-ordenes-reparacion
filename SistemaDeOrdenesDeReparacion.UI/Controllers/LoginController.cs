using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaDeOrdenesDeReparacion.Model.Response;
using SistemaDeOrdenesDeReparacion.SI;

namespace SistemaDeOrdenesDeReparacion.UI.Controllers
{
    public class LoginController : Controller
    {
       
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(string nombre, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Debe ingresar nombre y contraseña.";
                return View();
            }

            using (var client = new HttpClient())
            {
                var apiKey = _configuration.GetValue<string>(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeySectionName);
                var urlapi = _configuration.GetValue<string>("URLAPI"); // lee la URL base desde configuración
                client.BaseAddress = new Uri(urlapi);
                var url = $"{urlapi}api/ServicioDeLogin/IniciarSesion"; // usa la nueva URL
                var request = new
                {
                    Nombre = nombre,
                    Contrasena = contrasena
                };

                client.DefaultRequestHeaders.Add(SistemaDeOrdenesDeReparacion.SI.Autenticacion.ConstantesDeAutenticacion.ApiKeyHeaderName, apiKey);

                var response = await client.PostAsJsonAsync(url, request);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudo conectar con el servidor.";
                    return View();
                }

                var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (!resultado.Exito)
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View();
                }
               
                var rol = resultado.Rol?.ToLowerInvariant();
                HttpContext.Session.SetString("RolUsuario", rol);
                var identificacion = resultado.Identificacion;
                HttpContext.Session.SetString("Identificacion", identificacion);

                if (rol == "administrador")
                    return RedirectToAction("VistaAdministrador", new { usuario = nombre });
                else if (rol == "mecanico")
                    return RedirectToAction("VistaMecanico", "Mecanico");
                else if (rol == "cliente")
                    return RedirectToAction("VistaCliente", "Cliente");
                else
                {
                    ViewBag.Error = "Rol no reconocido.";
                    return View("Index");
                }

            }
        }



        public IActionResult VistaAdministrador()
        {
            
            var rol = HttpContext.Session.GetString("RolUsuario");
            if (rol != "administrador")
            {
                
                return RedirectToAction("Index", "Login");
            }

            
            var nombreCompleto = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.NombreCompleto = nombreCompleto;

            return View();
        }


        [HttpGet]
        public IActionResult Logout()
        {
           
            HttpContext.Session.Clear();
         
            return RedirectToAction("Index", "Login");
        }



    }
}
