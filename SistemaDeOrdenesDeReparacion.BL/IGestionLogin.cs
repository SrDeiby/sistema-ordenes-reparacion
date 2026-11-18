using SistemaDeOrdenesDeReparacion.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeOrdenesDeReparacion.BL
{
    public interface IGestionLogin
    {
        Model.Usuario InicieSesionAdministrador(string nombre, string contrasena);

        void EnvieCorreoDeInicioDeSesionAlAdministrador(Model.Usuario administrador);

        bool InicieUnaSesionDelUsuario(string nombre, string contrasena);

        void envieCorreoElectronicoDeSesion(string correoElectronico, string asunto, string mensaje);

        Model.Mecanico ObtengaMecanicoPorNombreyContrasenaDeUsuario(string nombre, string contrasena);
        Model.Cliente ObtengaClientePorNombreyContrasenaDeUsuario(string nombre, string contrasena);

        bool CambieLaContrasenaDelUsuario(int id, string nuevaContrasena);

        Model.Mecanico ObtengaMecanicoPorIdentificacionDelUsuario(string id);

        Model.Mecanico ObtengaMecanicoPorIdDelUsuario(int id);

        string ObtengaIdentificacionDelMecanicoPorElIdUsuario(int id);

        bool VerifiqueLaContrasena(string nombre, string contrasena);

        Model.Cliente ObtengaClientePorIdentificacionDelUsuario(String identificacion);

        string ObtengaIdentifiacionDelMecanicoPorNombreYContrasenaDeUsuario(string nombre, string contrasena);
        string ObtengaIdentificacionDelClientePorNombreYContrasenaDeUsuario(string nombre, string contrasena);

        Model.Usuario ObtengaUsuarioPorIdDelMecanico(int id);

        Model.Usuario ObtengaUsuarioPorIdentificacionDelMecanico(string identificacionMecanico);
        Model.Usuario ObtengaUsuarioPorId(int id);

        int ObtengaIdDeUsuarioPorNombreYContrasena(string nombre, string contrasena);

        bool SiElUsuarioSeBloqueaDespuesDeDosIntentos(string nombre, string contrasena);

        Model.Usuario ObtengaUsuarioPorIdentificacionDelCliente(String identificacionCliente);

        Model.Cliente ObtengaClientePorIdUsuario(int id);

        Model.Usuario ObtengaUsuarioPorIdDelCliente(int id);

        LoginResponse VerificarCredenciales(string nombre, string contrasena);


    }
}
