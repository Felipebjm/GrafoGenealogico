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
            MainContent.Content = statsControl;
        }
    }
}
