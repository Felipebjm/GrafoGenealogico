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

namespace InterfazGrafica.Vistas
{
    public partial class MapaControl : UserControl
    {
        /// <summary>
        /// Acción que tu capa superior puede asignar para (re)dibujar nodos/relaciones.
        /// Ahora puede estar null; cuando la asignes, el botón "Refrescar" la invocará.
        /// </summary>
        public Action OnRefresh { get; set; }

        public MapaControl()
        {
            InitializeComponent();

            // Al cargar el control la pantalla intenta refrescar automaticamente
            Loaded += (_, __) => OnRefresh?.Invoke();
        }

        // Handler del boton "Refrescar" para refrescar el dibujo del mapa
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            var old = BtnRefrescar.Content; // Guardar estado previo
            BtnRefrescar.IsEnabled = false; // Evitar click repetidos
            BtnRefrescar.Content = "Refrescando…"; // Feedback visual al usuario

            try
            {
                OnRefresh?.Invoke(); // La UI superior limpia/dibuja en MapaCanvas
            }
            finally
            {
                BtnRefrescar.Content = old;
                BtnRefrescar.IsEnabled = true; // Restaurar estado previo del boton
            }
        }

        
        /// Limpia la capa de dibujo (las lineas, imagenes de personas) sin tocar la imagen de fondo
        public void LimpiarOverlay() => MapaCanvas.Children.Clear();
        
        /// Cambia la imagen del mapa en tiempo de ejecución.
        public void SetMapaSource(string sourceUri)
        {
            ImgMapa.Source = new BitmapImage(new Uri(sourceUri, UriKind.RelativeOrAbsolute));
        }

    }
}
