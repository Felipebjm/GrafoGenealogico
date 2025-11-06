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
        public MainWindow()
        {
            InitializeComponent();

            // Al iniciar, mostramos la pantalla de agregar familiar
            MainContent.Content = new Vistas.AgregarFamiliarControl();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.AgregarFamiliarControl();
        }

        private void BtnMapa_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.MapaControl();
        }

        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.EstadisticasControl();
        }
    }
}
