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
using System.Windows.Shapes;

namespace InterfazGrafica.Vistas
{
    public partial class MapaWindow : Window
    {
        // Coordenadas elegidas por el usuario
        public double CoordenadaX { get; private set; }
        public double CoordenadaY { get; private set; }

        public MapaWindow()
        {
            InitializeComponent();
        }

        private void GridMapa_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Obtener la posición del clic relativa al Grid
            var punto = e.GetPosition(GridMapa);

            CoordenadaX = punto.X;
            CoordenadaY = punto.Y;

            // Cerramos la ventana indicando que se eligió una posición válida
            DialogResult = true;
            Close();
        }
    }
}
