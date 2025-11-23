using Clases;
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
using System.IO;

namespace InterfazGrafica.Vistas
{
    public partial class MapaControl : UserControl
    {
        private readonly GrafoPersonas _grafo; // Instancia del grafo de personas
        public Action? OnRefresh { get; set; }

        public MapaControl(GrafoPersonas grafo)
        {
            InitializeComponent();
            _grafo = grafo; // Guardar referencia al grafo

            // Al cargar el control la pantalla intenta refrescar automaticamente
            Loaded += (_, __) => OnRefresh?.Invoke();
            OnRefresh = RefrescarMapa;
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

        //Redibuja todo el grafo en el canvas
        public void RefrescarMapa()
        {
            LimpiarOverlay();
            const double anchoNodo = 60;
            const double altoNodo  = 60;

            // 1.Dibujar relaciones
            DibujarRelaciones(anchoNodo, altoNodo);
            // 2.Dibujar personas
            foreach (var persona in _grafo.Personas)
            {
                DibujarPersona(persona, anchoNodo, altoNodo);
            }
        }

        private void DibujarPersona(Persona persona, double anchoNodo, double altoNodo)
        {
            var imagenPersona = new Image
            {
                Width = anchoNodo,
                Height = altoNodo,
                Stretch = Stretch.UniformToFill,
                ToolTip = $"{persona.Nombre}\nCédula: {persona.Cedula}"
            };
            if (!string.IsNullOrWhiteSpace(persona.RutaFoto) && File.Exists(persona.RutaFoto))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(persona.RutaFoto, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imagenPersona.Source = bitmap;
            }
            Canvas.SetLeft(imagenPersona, persona.PosX);
            Canvas.SetTop(imagenPersona, persona.PosY);
            MapaCanvas.Children.Add(imagenPersona);
        }

        private void DibujarRelaciones(double anchoNodo, double altoNodo)
        {
            var paresVisitados = new HashSet<(Guid, Guid)>();
            (Guid, Guid) Normalizar(Guid a, Guid b)
                => a.CompareTo(b) <= 0 ? (a, b) : (b, a);

            foreach (var kvp in _grafo.Adyacencias)
            {
                var id1 = kvp.Key;
                var p1 = _grafo.BuscarPersonaPorId(id1);
                if (p1 == null) continue;

                foreach(var id2 in kvp.Value)
                {
                    var p2 = _grafo.BuscarPersonaPorId(id2);
                    if (p2 == null) continue;

                    var par = Normalizar(id1, id2);
                    if ( !paresVisitados.Add(par))
                        continue; // Ya dibujado

                    double x1 = p1.PosX + anchoNodo / 2.0;
                    double y1 = p1.PosY + altoNodo / 2.0;
                    double x2 = p2.PosX + anchoNodo / 2.0;
                    double y2 = p2.PosY + altoNodo / 2.0;

                    var linea = new Line // Dibujar linea entre p1 y p2
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                        Stroke = Brushes.White,
                        StrokeThickness = 2,
                        Opacity = 0.8
                    };
                    MapaCanvas.Children.Add(linea);
                }
            }
        }
        private void MapaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
