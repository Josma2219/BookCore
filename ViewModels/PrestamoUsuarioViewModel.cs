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

        // Cantidad total de días otorgados para el préstamo.
        public int DiasTotalesPrestamo
        {
            get
            {
                return Math.Max(
                    1,
                    (
                        FechaVencimiento.Date -
                        FechaPrestamo.Date
                    ).Days);
            }
        }

        // Días que ya han transcurrido desde que se prestó el libro.
        public int DiasTranscurridos
        {
            get
            {
                DateTime fechaFinal =
                    FechaDevolucion?.Date
                    ?? DateTime.Today;

                int dias = (
                    fechaFinal -
                    FechaPrestamo.Date
                ).Days;

                return Math.Clamp(
                    dias,
                    0,
                    DiasTotalesPrestamo);
            }
        }

        // Porcentaje visual que ocupará la barra.
        public int PorcentajeTranscurrido
        {
            get
            {
                if (Estado == EstadosPrestamo.Vencido ||
                    Estado == EstadosPrestamo.Devuelto)
                {
                    return 100;
                }

                double porcentaje =
                    DiasTranscurridos *
                    100.0 /
                    DiasTotalesPrestamo;

                return Math.Clamp(
                    (int)Math.Round(porcentaje),
                    0,
                    100);
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

        // Clase de Bootstrap utilizada para colorear la barra.
        public string ClaseBarra
        {
            get
            {
                if (Estado == EstadosPrestamo.Vencido)
                {
                    return "bg-danger";
                }

                if (Estado == EstadosPrestamo.Devuelto)
                {
                    return "bg-success";
                }

                if (DiasRestantes == 0)
                {
                    return "bg-info";
                }

                if (DiasRestantes <= 3)
                {
                    return "bg-warning";
                }

                return "bg-success";
            }
        }

        public string ClaseTextoPlazo
        {
            get
            {
                if (Estado == EstadosPrestamo.Vencido)
                {
                    return "text-danger";
                }

                if (DiasRestantes == 0)
                {
                    return "text-info";
                }

                if (DiasRestantes <= 3)
                {
                    return "text-warning";
                }

                return "text-success";
            }
        }
    }
}