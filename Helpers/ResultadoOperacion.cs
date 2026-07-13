namespace BookCore.Helpers
{
    public class ResultadoOperacion
    {
        public bool Exitoso { get; init; }

        public string Mensaje { get; init; } = string.Empty;

        public static ResultadoOperacion Correcto(string mensaje)
        {
            return new ResultadoOperacion
            {
                Exitoso = true,
                Mensaje = mensaje
            };
        }

        public static ResultadoOperacion Fallido(string mensaje)
        {
            return new ResultadoOperacion
            {
                Exitoso = false,
                Mensaje = mensaje
            };
        }
    }
}