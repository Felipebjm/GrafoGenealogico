using Clases;
using System.Windows.Controls;

namespace InterfazGrafica.Vistas
{
    public partial class EstadisticasControl : UserControl
    {
        private readonly GrafoPersonas _grafo;
        public EstadisticasControl(GrafoPersonas grafo)
        {
            InitializeComponent();
            _grafo = grafo ?? throw new ArgumentNullException(nameof(grafo));
            CalcularYMostrarEstadisticas();
        }

        // Calcula las estadisticas y las muestra en la interfaz
        private void CalcularYMostrarEstadisticas()
        {
            Limpiar();
            // Si no hay relaciones, muestra eso
            if (_grafo.Adyacencias.Count == 0)
            {
                TxtPromedio.Text = "0";
                TxtCercanoA.Text = "N/A";
                TxtCercanoB.Text = "N/A";
                TxtLejanoA.Text = "N/A";
                TxtLejanoB.Text = "N/A";
                return;
            }

            // Par mas cercano
            var (c1, c2, distCercana) = _grafo.ObtenerParMasCercano();
            SetParCercano(
                c1?.Nombre ?? "N/A",
                c2?.Nombre ?? "N/A"
            );
            // Par más lejano
            var (l1, l2, distLejana) = _grafo.ObtenerParMasLejano();
            SetParLejano(
                l1?.Nombre ?? "N/A",
                l2?.Nombre ?? "N/A"
            );
            // Distancia promedio
            double promedio = _grafo.CalcularDistanciaPromedio();
            SetDistanciaPromedio(promedio.ToString("0.00"));
        }
        /// Establece los nombres del par mas cercano.
        public void SetParCercano(string nombreA, string nombreB)
        {
            TxtCercanoA.Text = nombreA ?? string.Empty;
            TxtCercanoB.Text = nombreB ?? string.Empty;
        }
        /// Establece los nombres del par mss lejano.
        public void SetParLejano(string nombreA, string nombreB)
        {
            TxtLejanoA.Text = nombreA ?? string.Empty;
            TxtLejanoB.Text = nombreB ?? string.Empty;
        }
        /// Establece la distancia promedio entre familiares (como texto).
        public void SetDistanciaPromedio(string distanciaPromedio)
        {
            TxtPromedio.Text = (distanciaPromedio ?? string.Empty) + "km";
        }
        /// Limpia todos los campos (por si hiciera falta resetear).
     
        public void Limpiar()
        {
            TxtCercanoA.Text = string.Empty;
            TxtCercanoB.Text = string.Empty;
            TxtLejanoA.Text = string.Empty;
            TxtLejanoB.Text = string.Empty;
            TxtPromedio.Text = string.Empty;
        }
    }
}
