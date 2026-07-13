namespace BookCore.Helpers
{
    public static class EstadosPrestamo
    {
        public const string Activo = "Activo";

        public const string Devuelto = "Devuelto";

        public const string Vencido = "Vencido";

        public static bool EsPendiente(string? estado)
        {
            return estado == Activo ||
                   estado == Vencido;
        }

        public static bool EsValido(string? estado)
        {
            return estado == Activo ||
                   estado == Devuelto ||
                   estado == Vencido;
        }
    }
}