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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Esto es para crear un nuevo grafo cuando exista la clase grafo
        //private GrafoFamilia_grafo = new GrafoFamilia();
        public MainWindow()
        {
            InitializeComponent();

            // Al iniciar, se muestra la pantalla de agregar familiar
            //Recordar pasar el grafo por parametro cuando exista
            MainContent.Content = new Vistas.AgregarFamiliarControl();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            //Recordar pasar el grafo por parametro cuando exista
            MainContent.Content = new Vistas.AgregarFamiliarControl();
        }

        //Boton para ir a la vista del mapa
        private void BtnMapa_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.MapaControl();
        }

        //Boton para ir a la vista de estadisticas
        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.EstadisticasControl();
        }
    }
}
