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

        public MapaWindow() // Constructor
        {
            InitializeComponent();
        }

        // Evento que se dispara al hacer clic en el Grid del mapa
        private void GridMapa_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) 
        {
            // Obtener la posicion del clic relativa al Grid
            var punto = e.GetPosition(GridMapa);

            CoordenadaX = punto.X;
            CoordenadaY = punto.Y;

            // Cierra la ventana indicando que se eligio una posicion valida
            DialogResult = true;
            Close();
        }
    }
}
