using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeOrdenesDeReparacion.BL;
using SistemaDeOrdenesDeReparacion.DA;
using SistemaDeOrdenesDeReparacion.Model;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public class GestionDeOrdenesDeTrabajo : IGestionDeOrdenesDeTrabajo
    {
        private readonly DBContexto _context;

        public GestionDeOrdenesDeTrabajo(DBContexto context)
        {
            _context = context;
        }


       
        public List<ClienteResumen> ObtenerListaClientesParaSeleccion()
        {
            return _context.Clientes
                .Select(c => new ClienteResumen
                {
                    Id = c.Id,
                    Identificacion = c.Identificacion,
                    Nombre = c.Nombre,
                    Apellidos = c.Apellidos
                })
                .OrderBy(c => c.Nombre)
                .ToList();
        }

        
        public bool AgregarOrden(OrdenesDeTrabajoViewModel request)
        {
            try
            {
            
                if (!ValidarDatosOrden(request))
                    return false;

                if (request.FechaFinalizacion <= DateTime.Today)
                {
                    return false;
                }

               
                if (request.FechaFinalizacion <= DateTime.Today)
                {
                    throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha actual");
                }

               
                var nuevaOrden = new SistemaDeOrdenesDeReparacion.Model.OrdenesDeTrabajos
                {
                    IdCliente = request.IdCliente,
                    DescripcionDelProblema = request.DescripcionProblema,
                    FechaDeRegistro = DateTime.Now,
                    FechaAproximadaDeFinalizacion = request.FechaFinalizacion, 
                    Estado = EstadoDeOrdenDeTrabajo.Registrado,
                    TipoDeVehiculo = (TipoDeVehiculo)request.TipoDeVehiculo,
                    TipoDeCombustionInterna = (TipoDeCombustion)request.TipoDeCombustionInterna,
                    Marca = request.Marca?.Trim(), 
                    Modelo = request.Modelo?.Trim(),
                    AñoDeFabricacion = request.AñoFabricacion,
                    MotivoDeCancelacion = null
                };

             
                _context.OrdenesDeTrabajos.Add(nuevaOrden);
                return _context.SaveChanges() > 0;
            }
            catch (ArgumentException)
            {
                
                throw;
            }
            catch (Exception ex)
            {
                string mensajeCompleto = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Error al agregar orden: {mensajeCompleto}", ex);
            }
        }


        private bool ValidarDatosOrden(OrdenesDeTrabajoViewModel request)
        {
            if (request.IdCliente <= 0)
                throw new ArgumentException("Debe seleccionar un cliente válido");
            if (!_context.Clientes.Any(c => c.Id == request.IdCliente))
                throw new ArgumentException("El cliente seleccionado no existe");

            if (request.FechaFinalizacion <= DateTime.Today)
                throw new ArgumentException("La fecha de finalización debe ser posterior a hoy");



            if (!Enum.IsDefined(typeof(TipoDeVehiculo), request.TipoDeVehiculo))
                throw new ArgumentException("Debe seleccionar un tipo de vehículo válido");
            if (!Enum.IsDefined(typeof(TipoDeCombustion), request.TipoDeCombustionInterna))
                throw new ArgumentException("Debe seleccionar un tipo de combustión válido");
            if (string.IsNullOrWhiteSpace(request.DescripcionProblema))
                throw new ArgumentException("La descripción del problema es requerida");
            if (string.IsNullOrWhiteSpace(request.Marca))
                throw new ArgumentException("La marca es requerida");
            if (string.IsNullOrWhiteSpace(request.Modelo))
                throw new ArgumentException("El modelo es requerido");
            if (string.IsNullOrWhiteSpace(request.AñoFabricacion))
                throw new ArgumentException("El año de fabricación es requerido");
            if (!ValidarAñoFabricacion(request.AñoFabricacion))
                throw new ArgumentException("El año de fabricación debe ser válido");

            return true;
        }

        private bool ValidarAñoFabricacion(string año)
        {
            if (!int.TryParse(año, out int añoInt))
                return false;

            return añoInt >= 1900 && añoInt <= DateTime.Now.Year + 1;
        }

        public Dictionary<string, string> ObtenerTiposVehiculo()
        {
            return Enum.GetValues<TipoDeVehiculo>()
                       .ToDictionary(t => ((int)t).ToString(), t => ObtenerDescripcionTipoVehiculo(t));
        }
        public Dictionary<string, string> ObtenerTiposCombustion()
        {
            return Enum.GetValues<TipoDeCombustion>()
                       .ToDictionary(t => ((int)t).ToString(), t => ObtenerDescripcionTipoCombustion(t));
        }

        private string ObtenerDescripcionTipoVehiculo(TipoDeVehiculo tipo)
        {
            return tipo switch
            {
                TipoDeVehiculo.Automoviles => "Automóviles",
                TipoDeVehiculo.SUV => "SUV",
                TipoDeVehiculo.PickUps => "Pick-ups",
                TipoDeVehiculo.Autobuses => "Autobuses",
                TipoDeVehiculo.Trolebuses => "Trolebuses",
                TipoDeVehiculo.Microbuses => "Microbuses",
                TipoDeVehiculo.Camiones => "Camiones",
                TipoDeVehiculo.Trailer => "Tráiler",
                TipoDeVehiculo.Furgonetas => "Furgonetas",
                TipoDeVehiculo.Ambulancias => "Ambulancias",
                TipoDeVehiculo.CamionesBomberos => "Camiones de bomberos",
                TipoDeVehiculo.VehiculosMilitares => "Vehículos militares",
                TipoDeVehiculo.MaquinariaAgricola => "Maquinaria agrícola",
                TipoDeVehiculo.MaquinariaConstruccion => "Maquinaria de construcción",
                _ => tipo.ToString()
            };
        }

        private string ObtenerDescripcionTipoCombustion(TipoDeCombustion tipo)
        {
            return tipo switch
            {
                TipoDeCombustion.Gasolina => "Gasolina",
                TipoDeCombustion.Diesel => "Diésel",
                TipoDeCombustion.GLP => "Gas licuado de petróleo (GLP)",
                TipoDeCombustion.GNV => "Gas natural vehicular (GNV)",
                TipoDeCombustion.Electrico => "Eléctrico",
                _ => tipo.ToString()
            };
        }
        public void PrepararDatosVistaOrden(CrearOrdenViewModel modelo)
        {
            modelo.Clientes = ObtenerListaClientesParaSeleccion()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                }).ToList();

            modelo.TiposVehiculo = ObtenerTiposVehiculo()
                .Select(tv => new SelectListItem
                {
                    Value = tv.Key,
                    Text = tv.Value
                }).ToList();

            modelo.TiposCombustion = ObtenerTiposCombustion()
                .Select(tc => new SelectListItem
                {
                    Value = tc.Key,
                    Text = tc.Value
                }).ToList();
        }


        public CrearOrdenViewModel PrepararFormularioDeCreacion()
        {
            var clientes = ObtenerListaClientesParaSeleccion()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                }).ToList();

            var tiposVehiculo = ObtenerTiposVehiculo()
                .Select(tv => new SelectListItem
                {
                    Value = tv.Key,
                    Text = tv.Value
                }).ToList();

            var tiposCombustion = ObtenerTiposCombustion()
                .Select(tc => new SelectListItem
                {
                    Value = tc.Key,
                    Text = tc.Value
                }).ToList();

            return new CrearOrdenViewModel
            {
                Orden = new OrdenesDeTrabajoViewModel(),
                Clientes = clientes,
                TiposVehiculo = tiposVehiculo,
                TiposCombustion = tiposCombustion
            };
        }

        public List<ResumenDeOrdenesViewModel> ObtenerTodasLasOrdenes()
        {
            return _context.OrdenesDeTrabajos
                .Join(_context.Clientes,
                      orden => orden.IdCliente,
                      cliente => cliente.Id,
                      (orden, cliente) => new ResumenDeOrdenesViewModel
                      {
                          Id = orden.Id,
                          Nombre = cliente.Nombre,
                          Apellidos = cliente.Apellidos,
                          Identificacion = cliente.Identificacion,
                          TipoDeVehiculo = orden.TipoDeVehiculo,
                          Marca = orden.Marca,
                          Modelo = orden.Modelo,
                          TipoDeCombustionInterna = orden.TipoDeCombustionInterna,
                          AñoDeFabricacion = orden.AñoDeFabricacion
                      })
                .ToList();
        }
        public List<Model.ResumenDeOrdenesViewModel> FiltrarOrdenesPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<Model.ResumenDeOrdenesViewModel>();

            string nombreBusqueda = nombre.Trim().ToLower();

            return _context.OrdenesDeTrabajos
                .Join(_context.Clientes,
                      orden => orden.IdCliente,
                      cliente => cliente.Id,
                      (orden, cliente) => new { Orden = orden, Cliente = cliente })
                .Where(x => x.Cliente.Nombre.ToLower().Contains(nombreBusqueda))
                .Select(x => new Model.ResumenDeOrdenesViewModel
                {
                    Id = x.Orden.Id,
                    Nombre = x.Cliente.Nombre,
                    Apellidos = x.Cliente.Apellidos,
                    Identificacion = x.Cliente.Identificacion,
                    TipoDeVehiculo = x.Orden.TipoDeVehiculo,
                    Marca = x.Orden.Marca,
                    Modelo = x.Orden.Modelo,
                    TipoDeCombustionInterna = x.Orden.TipoDeCombustionInterna,
                    AñoDeFabricacion = x.Orden.AñoDeFabricacion
                })
                .ToList();
        }





        public OrdenesDeTrabajoViewModel ObtenerOrdenParaEdicion(int id)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == id);

                if (orden == null)
                    return null;

                return new OrdenesDeTrabajoViewModel
                {
                    Id = orden.Id,
                    IdCliente = orden.IdCliente,
                    FechaFinalizacion = orden.FechaAproximadaDeFinalizacion ?? DateTime.Today,
                    TipoDeVehiculo = (int?)orden.TipoDeVehiculo ?? 0,
                    TipoDeCombustionInterna = (int?)orden.TipoDeCombustionInterna ?? 0,
                    Marca = orden.Marca ?? string.Empty,
                    Modelo = orden.Modelo ?? string.Empty,
                    AñoFabricacion = orden.AñoDeFabricacion ?? string.Empty,
                    DescripcionProblema = orden.DescripcionDelProblema ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la orden: {ex.Message}", ex);
            }
        }


        public EditarOrdenesDeTrabajoViewModel PrepararFormularioDeEdicion(int id)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == id);


                var clientes = ObtenerListaClientesParaSeleccion()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                    }).ToList();

                var tiposVehiculo = ObtenerTiposVehiculo()
                    .Select(tv => new SelectListItem
                    {
                        Value = tv.Key,
                        Text = tv.Value
                    }).ToList();

                var tiposCombustion = ObtenerTiposCombustion()
                    .Select(tc => new SelectListItem
                    {
                        Value = tc.Key,
                        Text = tc.Value
                    }).ToList();

                return new EditarOrdenesDeTrabajoViewModel
                {
                    Orden = ObtenerOrdenParaEdicion(id), 
                    Clientes = new SelectList(clientes, "Value", "Text", orden.IdCliente),
                    TiposVehiculo = new SelectList(tiposVehiculo, "Value", "Text", ((int)orden.TipoDeVehiculo).ToString()),
                    TiposCombustion = new SelectList(tiposCombustion, "Value", "Text", ((int)orden.TipoDeCombustionInterna).ToString())
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al preparar formulario de edición: {ex.Message}", ex);
            }
        }



        public bool ActualizarOrden(OrdenesDeTrabajos NuevoDato)
        {
            var existente = _context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == NuevoDato.Id);
            existente.IdCliente = NuevoDato.IdCliente;
            existente.FechaAproximadaDeFinalizacion = NuevoDato.FechaAproximadaDeFinalizacion;
            existente.TipoDeVehiculo = NuevoDato.TipoDeVehiculo;
            existente.TipoDeCombustionInterna = NuevoDato.TipoDeCombustionInterna;
            existente.Marca = NuevoDato.Marca;
            existente.Modelo = NuevoDato.Modelo;
            existente.AñoDeFabricacion = NuevoDato.AñoDeFabricacion;
            existente.DescripcionDelProblema = NuevoDato.DescripcionDelProblema;

            var cambios = _context.SaveChanges();
            Console.WriteLine($"Cambios guardados: {cambios}");

            return cambios > 0;
        }



        public bool OrdenExiste(int id)
        {
            try
            {
                return _context.OrdenesDeTrabajos.Any(o => o.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de orden: {ex.Message}", ex);
            }
        }

        public OrdenesDeTrabajos ConvertirViewModelAEntidad(OrdenesDeTrabajoViewModel vm)
        {
            return new OrdenesDeTrabajos
            {
                Id = vm.Id,
                IdCliente = vm.IdCliente,
                FechaAproximadaDeFinalizacion = vm.FechaFinalizacion,
                TipoDeVehiculo = (TipoDeVehiculo)vm.TipoDeVehiculo,
                TipoDeCombustionInterna = (TipoDeCombustion)vm.TipoDeCombustionInterna,
                Marca = vm.Marca,
                Modelo = vm.Modelo,
                AñoDeFabricacion = vm.AñoFabricacion,
                DescripcionDelProblema = vm.DescripcionProblema
            };
        }








        public CancelarOrdenViewModel PrepararFormularioDeCancelacion(int id)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos
                    .Include(o => o.Cliente) 
                    .FirstOrDefault(o => o.Id == id);



                if (orden.Estado == EstadoDeOrdenDeTrabajo.Cancelado || orden.Estado == EstadoDeOrdenDeTrabajo.Finalizado)
                {
                    return null; 
                }

                return new CancelarOrdenViewModel
                {
                    Id = orden.Id,
                    NumeroOrden = $"ORD-{orden.Id:D6}",
                    Cliente = $"{orden.Cliente?.Nombre} {orden.Cliente?.Apellidos}", 
                    Descripcion = orden.DescripcionDelProblema
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al preparar formulario de cancelación: {ex.Message}", ex);
            }
        }

        public List<OrdenEnProcesoViewModel> ObtenerOrdenesEnProcesoParaVista()
        {
            try
            {
                var ordenes = _context.OrdenesDeTrabajos
                    .Include(o => o.Cliente)
                    .Include(o => o.Mecanico)
                    .Where(o => o.Estado == EstadoDeOrdenDeTrabajo.EnProceso)
                    .ToList();

                var resultado = ordenes.Select(o => new OrdenEnProcesoViewModel
                {
                    IdentificacionCliente = o.Cliente?.Identificacion,
                    NombreCliente = o.Cliente?.Nombre,
                    ApellidosCliente = o.Cliente?.Apellidos,

                    IdentificacionMecanico = o.Mecanico?.Identificacion,
                    NombreMecanico = o.Mecanico?.Nombre,
                    ApellidosMecanico = o.Mecanico?.Apellidos,

                    TipoDeVehiculo = o.TipoDeVehiculo.ToString(),
                    Marca = o.Marca,
                    Modelo = o.Modelo,
                    TipoDeCombustion = o.TipoDeCombustionInterna.ToString(),
                    AñoDeFabricacion = o.AñoDeFabricacion
                }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener órdenes en proceso: {ex.Message}", ex);
            }
        }


        public bool CancelarOrden(int id, string motivoCancelacion)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == id);

                if (orden == null)
                {
                    Console.WriteLine("Orden no encontrada.");
                    return false;
                }

                if (orden.Estado == EstadoDeOrdenDeTrabajo.Cancelado)
                {
                    Console.WriteLine("La orden ya está cancelada.");
                    return false;
                }

                if (orden.Estado == EstadoDeOrdenDeTrabajo.Finalizado)
                {
                    Console.WriteLine("No se puede cancelar una orden completada.");
                    return false;
                }

                Console.WriteLine($"Cancelando orden ID: {id}");
                Console.WriteLine($"Motivo: {motivoCancelacion}");

                orden.Estado = EstadoDeOrdenDeTrabajo.Cancelado; 
                orden.MotivoDeCancelacion = motivoCancelacion;


                var cambios = _context.SaveChanges();
                Console.WriteLine($"Cambios guardados: {cambios}");

                return cambios > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cancelar orden: {ex.Message}");
                return false;
            }
        }

        public bool PuedeCancelarse(int id)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == id);

                if (orden == null)
                    return false;

             
                return orden.Estado != EstadoDeOrdenDeTrabajo.Cancelado &&
                       orden.Estado != EstadoDeOrdenDeTrabajo.Finalizado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar si se puede cancelar: {ex.Message}", ex);
            }
        }



        public IniciarOrdenViewModel PrepararFormularioDeInicio(int id)
        {
            try
            {
                var orden = _context.OrdenesDeTrabajos
                    .Include(o => o.Cliente)
                    .FirstOrDefault(o => o.Id == id);

                if (orden == null)
                    return null;

                return new IniciarOrdenViewModel
                {
                    Id = orden.Id,
                    TipoVehiculo = orden.TipoDeVehiculo.ToString(),
                    Marca = orden.Marca,
                    Modelo = orden.Modelo,
                    TipoCombustion = orden.TipoDeCombustionInterna.ToString(),
                    AnioFabricacion = orden.AñoDeFabricacion,
                    DescripcionProblema = orden.DescripcionDelProblema,

                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al preparar formulario de inicio: {ex.Message}", ex);
            }
        }



        public OrdenesDeTrabajos ObtenerOrdenPorId(int id)
        {
            return _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .Include(o=> o.Mecanico)
                .FirstOrDefault(o => o.Id == id);
        }

        public List<SelectListItem> ObtenerMecanicos()
        {
            return _context.Mecanicos
                .AsEnumerable()  
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Identificacion} - {m.Nombre} {m.Apellidos}"
                })
                .OrderBy(m => m.Text)
                .ToList();
        }

        public bool IniciarOrden(int ordenId, int mecanicoId)
        {
            var orden = _context.OrdenesDeTrabajos.Find(ordenId);
            if (orden != null && orden.Estado == EstadoDeOrdenDeTrabajo.Registrado)
            {
                orden.Estado = EstadoDeOrdenDeTrabajo.EnProceso;
                orden.Id_Mecanico = mecanicoId;
              
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public void Agregue(Model.OrdenesDeTrabajoInventarioDeServicios automovil)
        {
             automovil.InventarioDeServicio = null;
            _context.OrdenesDeTrabajoInventarioDeServicios.Add(automovil);
            _context.SaveChanges();
        }

        public void AgregueRepuesto(Model.OrdenesDeTrabajoInventarioDeRepuestos automovil)
        {
            _context.OrdenesDeTrabajoInventarioDeRepuestos.Add(automovil);
            _context.SaveChanges();
        }



        public List<OrdenesDeTrabajos> ObtengaLaLista()
        {
            var resultado = _context.OrdenesDeTrabajos
                .Include(o => o.Mecanico);

            return resultado.ToList();
        }



        public List<OrdenesDeTrabajos> ObtengaLaListaDeOrdenesEnProceso()
        {
            var lista = _context.OrdenesDeTrabajos
     .Include(o => o.Mecanico)
     .Include(o => o.Cliente)
     .Where(o => o.Estado == EstadoDeOrdenDeTrabajo.EnProceso)
     .ToList();

            return lista;
        }

        public List<OrdenesDeTrabajos> ObtengaLaListaDeOrdenesCanceladas()
        {
            var lista = _context.OrdenesDeTrabajos
     .Include(o => o.Cliente)
     .Where(o => o.Estado == EstadoDeOrdenDeTrabajo.Cancelado)
     .ToList();

            return lista;
        }

        public List<OrdenesDeTrabajoInventarioDeRepuestos> ObtengaLosRepuestosAsignados(int idOrden)
        {
            return _context.OrdenesDeTrabajoInventarioDeRepuestos
        .Where(r => r.Id_OrdenesDeTrabajos == idOrden)
        .Include(r => r.InventarioDeRepuesto)
        .ToList();
        }

        public List<OrdenesDeTrabajoInventarioDeServicios> ObtengaLosServiciosAsignados(int idOrden)
        {
            return _context.OrdenesDeTrabajoInventarioDeServicios
                .Where(o => o.Id_OrdenesDeTrabajo == idOrden)
                .Include(o => o.InventarioDeServicio) 
                .ToList();
        }


        public List<InventarioDeRepuestos> ObtengaLosRepuestos()
        {
            var resultado = from c in _context.InventarioDeRepuestos
                            select c;
            return resultado.ToList();
        }

        public List<InventarioDeServicios> ObtengaLosServicios()
        {
            var resultado = from c in _context.InventarioDeServicios
                            select c;
            return resultado.ToList();
        }

        public InventarioDeServicios ObtenerServicioPorId(int id)
        {
            return _context.InventarioDeServicios.FirstOrDefault(s => s.Id == id);
        }

        public InventarioDeRepuestos ObtenerRepuestoPorId(int id)
        {
            return _context.InventarioDeRepuestos.FirstOrDefault(s => s.Id == id);
        }

        public void AsigneServicio(Model.OrdenesDeTrabajoInventarioDeServicios servicios)
        {
            try
            {
                _context.OrdenesDeTrabajoInventarioDeServicios.Add(servicios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log o lanzar para ver el error
                throw new Exception("Error al asignar servicio: " + ex.Message, ex);
            }
        }


        public void AsigneRepuesto(OrdenesDeTrabajoInventarioDeRepuestos model)
        {
            model.InventarioDeRepuesto = null;
            _context.OrdenesDeTrabajoInventarioDeRepuestos.Add(model);
            _context.SaveChanges();
        }

        public Model.OrdenesDeTrabajos ObtengaElDetalleDeOrdenesCanceladas(int id)
        {
            List<Model.OrdenesDeTrabajos> lista;
            lista = ObtengaLaListaDeOrdenesCanceladas();
            foreach (var ordenCancelada in lista)
            {
                if (ordenCancelada.Id == id)
                    return ordenCancelada;
            }

            return null;
        }

        public List<Model.OrdenesDeTrabajos> ObtengaOrdenesPorIdCliente(int idCliente)
        {
            List<Model.OrdenesDeTrabajos> lista = ObtengaLaLista();
            List<Model.OrdenesDeTrabajos> resultado = new List<Model.OrdenesDeTrabajos>();

            foreach (var orden in lista)
            {
                if (orden.IdCliente == idCliente)
                {
                    resultado.Add(orden);
                }
            }

            return resultado;
        }

        public Model.OrdenesDeTrabajos ObtengaLaOrdenPorId(int idOrden)
        {
            return ObtengaLaLista().FirstOrDefault(o => o.Id == idOrden);
        }

        public ResumenDeCostoViewModel ObtenerResumenDeOrden(int idOrden)
        {
            try
            {
                var repuestos = ObtengaLosRepuestosAsignados(idOrden);
                var servicios = ObtengaLosServiciosAsignados(idOrden);

                var totalRepuestos = repuestos.Sum(r => r.Total);
                var totalServicios = servicios.Sum(s => s.Total);

                return new ResumenDeCostoViewModel
                {
                    TotalRepuestos = totalRepuestos,
                    TotalServicios = totalServicios
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener resumen de la orden {idOrden}: {ex.Message}", ex);
            }
        }

        public List<OrdenesDeTrabajos> FiltreOrdenesDeTrabajoPorOrdenes(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<OrdenesDeTrabajos>();

            string nombreBusqueda = nombre.Trim().ToLower();

            var ordenesFiltradas = _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)     
                .Include(o => o.Mecanico)    
                .Where(o => o.Estado == EstadoDeOrdenDeTrabajo.EnProceso && 
                           o.Cliente.Nombre.ToLower().Contains(nombreBusqueda))
                .ToList();

            return ordenesFiltradas;
        }


        public List<OrdenesDeTrabajos> FiltreOrdenesDeTrabajoPorOrdenesCanceladas(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<OrdenesDeTrabajos>();

            string nombreBusqueda = nombre.Trim().ToLower();

            var ordenesFiltradas = _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)     // ← Incluir Cliente
                .Include(o => o.Mecanico)    // ← Incluir Mecánico
                .Where(o => o.Estado == EstadoDeOrdenDeTrabajo.Cancelado && // ← SOLO órdenes en proceso
                           o.Cliente.Nombre.ToLower().Contains(nombreBusqueda))
                .ToList();

            return ordenesFiltradas;
        }



        }
}


    

