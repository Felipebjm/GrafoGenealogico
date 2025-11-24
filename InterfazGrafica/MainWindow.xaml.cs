using Clases;
using InterfazGrafica.Vistas;
using System.Windows;

namespace InterfazGrafica
{
    public partial class MainWindow : Window
    {
        // Instancia del grafo de personas
        private readonly GrafoPersonas _grafo = new GrafoPersonas(); 
        public MainWindow()
        {
            InitializeComponent();

            // Al iniciar, se muestra el user control de agregar familiar
            MainContent.Content = new Vistas.AgregarFamiliarControl(_grafo);
        }

        // Boton para ir a la vista de agregar familiar
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
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
            MainContent.Content = statsControl;
        }
    }
}
