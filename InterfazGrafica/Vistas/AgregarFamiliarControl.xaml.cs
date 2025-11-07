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
        // Más adelante podés recibir el grafo por constructor
        // private GrafoFamilia _grafo;

        public AgregarFamiliarControl()
        {
            InitializeComponent();
        }

        private void BtnElegirPosicion_Click(object sender, RoutedEventArgs e)
        {
            // Aquí abrirías la ventana "SeleccionarPosicionWindow"
            // y cuando el usuario haga clic, te devuelve X/Y
            // y vos las ponés en TxtX.Text y TxtY.Text
        }

        private void BtnSeleccionarFoto_Click(object sender, RoutedEventArgs e)
        {
            // Aquí abrís un OpenFileDialog y cargas la imagen en ImgFoto.Source
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Aquí leés todos los campos,
            // creás un objeto Familiar y se lo pasás al grafo
            // (esto lo haremos cuando tengamos el proyecto Core referenciado)
        }
    }
}
