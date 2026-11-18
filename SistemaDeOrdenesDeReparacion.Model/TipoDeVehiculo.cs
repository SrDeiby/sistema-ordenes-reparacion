using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.Model
{
    public enum TipoDeVehiculo
    {
        Automoviles = 1,
        SUV = 2,
        [Display(Name = "Pick Up")]
        PickUps = 3,
        Autobuses = 4,
        Trolebuses = 5,
        Microbuses = 6,
        Camiones = 7,
        Trailer = 8,
        Furgonetas = 9,
        Ambulancias = 10,
        [Display(Name = "Camiones Bomberos")]
        CamionesBomberos = 11,
        [Display(Name = "Vehículos Militares")]
        VehiculosMilitares = 12,
        [Display(Name = "Maquinaria Agrícola")]
        MaquinariaAgricola = 13,
        [Display(Name = "Maquinaria Contrucción")]
        MaquinariaConstruccion = 14 
    }
}
