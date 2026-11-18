namespace SistemaDeOrdenesDeReparacion.BL
{
    public interface IServicioDeCorreo
    {
        void Envie(string destinatario, string asunto, string mensaje);
    }
}
