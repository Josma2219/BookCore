using BookCore.Helpers;

namespace BookCore.ViewModels
{
    public class PrestamoUsuarioViewModel
    {
        public int PrestamoId { get; set; }

        public int LibroId { get; set; }

        public string LibroTitulo { get; set; }
            = string.Empty;

        public string EjemplarCodigo { get; set; }
            = string.Empty;

        public DateTime FechaPrestamo { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public string Estado { get; set; }
            = string.Empty;

        public bool EstaPendiente =>
            EstadosPrestamo.EsPendiente(Estado);

        public int DiasRestantes
        {
            get
            {
                if (Estado != EstadosPrestamo.Activo)
                {
                    return 0;
                }

                return Math.Max(
                    0,
                    (
                        FechaVencimiento.Date -
                        DateTime.Today
                    ).Days);
            }
        }

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
                    (
                        DateTime.Today -
                        FechaVencimiento.Date
                    ).Days);
            }
        }

        public string TextoPlazo
        {
            get
            {
                if (Estado == EstadosPrestamo.Vencido)
                {
                    return $"{DiasAtraso} día(s) de atraso";
                }

                if (Estado == EstadosPrestamo.Devuelto)
                {
                    return "Devuelto";
                }

                if (DiasRestantes == 0)
                {
                    return "Vence hoy";
                }

                return $"{DiasRestantes} día(s) restantes";
            }
        }
    }
}