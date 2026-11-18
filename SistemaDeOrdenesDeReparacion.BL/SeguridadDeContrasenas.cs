namespace SistemaDeOrdenesDeReparacion.BL
{
    /// <summary>
    /// Hash y verificación de contraseñas con BCrypt.
    /// Acepta contraseñas antiguas guardadas en texto plano para poder migrarlas
    /// al formato con hash la próxima vez que el usuario inicie sesión.
    /// </summary>
    public static class SeguridadDeContrasenas
    {
        private const int FactorDeTrabajo = 10;

        public static string Hashee(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena, FactorDeTrabajo);
        }

        public static bool Verifique(string contrasenaIngresada, string? contrasenaAlmacenada)
        {
            if (string.IsNullOrEmpty(contrasenaIngresada) || string.IsNullOrEmpty(contrasenaAlmacenada))
                return false;

            if (EsUnHash(contrasenaAlmacenada))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(contrasenaIngresada, contrasenaAlmacenada);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    return false;
                }
            }

            // Contraseña heredada en texto plano (datos anteriores a esta versión).
            return contrasenaAlmacenada == contrasenaIngresada;
        }

        public static bool EsUnHash(string? contrasenaAlmacenada)
        {
            return contrasenaAlmacenada != null
                && contrasenaAlmacenada.Length == 60
                && (contrasenaAlmacenada.StartsWith("$2a$")
                    || contrasenaAlmacenada.StartsWith("$2b$")
                    || contrasenaAlmacenada.StartsWith("$2y$"));
        }
    }
}
