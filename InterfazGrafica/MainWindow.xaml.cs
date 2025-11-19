using Clases;
using InterfazGrafica.Vistas;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfazGrafica
{
    public partial class MainWindow : Window
    {
        // Instancia del grafo de personas
        private readonly GrafoPersonas _grafo = new GrafoPersonas(); 
        public MainWindow()
        {
            InitializeComponent();

            // Al iniciar, se muestra la pantalla de agregar familiar
            MainContent.Content = new Vistas.AgregarFamiliarControl(_grafo);
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            //Recordar pasar el grafo por parametro cuando exista
            MainContent.Content = new Vistas.AgregarFamiliarControl(_grafo);
        }

        //Boton para ir a la vista del mapa
        private void BtnMapa_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.MapaControl(_grafo);
        }

        //Boton para ir a la vista de estadisticas
        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            var statsControl = new EstadisticasControl(_grafo);

            // Usar los datos reales del grafo
            var (c1, c2, distCercana) = _grafo.ObtenerParMasCercano();
            var (l1, l2, distLejana)  = _grafo.ObtenerParMasLejano();
            double distProm           = _grafo.CalcularDistanciaPromedio();

            statsControl.SetParCercano(
                c1?.Nombre ?? "N/A",
                c2?.Nombre ?? "N/A"
            );

            statsControl.SetParLejano(
                l1?.Nombre ?? "N/A",
                l2?.Nombre ?? "N/A"
            );

            statsControl.SetDistanciaPromedio(
                distProm.ToString("0.00")
            );
            MainContent.Content = statsControl;
        }
    }
}
