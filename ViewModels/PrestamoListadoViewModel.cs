using BookCore.Helpers;

namespace BookCore.ViewModels
{
    public class PrestamoListadoViewModel
    {
        public int PrestamoId { get; set; }

        public string UsuarioNombreCompleto { get; set; } = string.Empty;

        public string UsuarioCedula { get; set; } = string.Empty;

        public string LibroTitulo { get; set; } = string.Empty;

        public string EjemplarCodigo { get; set; } = string.Empty;

        public DateTime FechaPrestamo { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public string Estado { get; set; } = string.Empty;

        public bool PuedeDevolverse =>
            EstadosPrestamo.EsPendiente(Estado);

        public int DiasAtraso
        {
            get
            {
                if (Estado != EstadosPrestamo.Vencido)
                {
                    return 0;
                }

                return Math.Max(
                    0,
                    (DateTime.Today -
                     FechaVencimiento.Date).Days);
            }
        }
    }
}