using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32; //Necesario para el OpenFileDialog

namespace InterfazGrafica.Vistas
{
    /// <summary>
    /// Interaction logic for AgregarFamiliarControl.xaml
    /// </summary>
    public partial class AgregarFamiliarControl : UserControl
    {
        // Mas adelante hay que pasar el grafo por parametro
        // private GrafoFamilia _grafo;


        public AgregarFamiliarControl()
        {
            InitializeComponent();
        }

        private void BtnElegirPosicion_Click(object sender, RoutedEventArgs e)
        {
            // Aqui se va a abrir un mapa donde el usuario escoje la posicion"
            // y cuando el usuario haga clic, te devuelve X/Y
            // esto es para ponerlo en TxtX.Text y TxtY.Text
        }

        private void BtnSeleccionarFoto_Click(object sender, RoutedEventArgs e)
        {
            // Esto va a ser para abrir un OpenFileDialog y cargar la imagen en ImgFoto.Source
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Aqui se leen todos los campos,
            // Secrea un objeto Familiar y se le pasa al grafo (cuando exista)
           
        }

        private void ChkNoEstaVivo_Checked(object sender, RoutedEventArgs e)
        {
            TxtAnoFallecimiento.IsEnabled = true;
        }

        private void ChkNoEstaVivo_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtAnoFallecimiento.IsEnabled = false;
            TxtAnoFallecimiento.Text = string.Empty;
        }
        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
