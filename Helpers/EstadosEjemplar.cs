namespace BookCore.Helpers
{
    public static class EstadosEjemplar
    {
        public const string Disponible = "Disponible";

        public const string Prestado = "Prestado";

        public const string Danado = "Dañado";

        public const string Baja = "Baja";

        public static bool EsValido(string? estado)
        {
            return estado == Disponible ||
                   estado == Prestado ||
                   estado == Danado ||
                   estado == Baja;
        }

        public static bool PuedeSeleccionarseManualmente(
            string? estado)
        {
            // Prestado solo lo pondrá el módulo de préstamos.
            return estado == Disponible ||
                   estado == Danado ||
                   estado == Baja;
        }
    }
}