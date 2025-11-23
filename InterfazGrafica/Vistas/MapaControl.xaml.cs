using Clases;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace InterfazGrafica.Vistas
{
    public partial class MapaControl : UserControl
    {
        private readonly GrafoPersonas _grafo; // Instancia del grafo de personas
        public Action? OnRefresh { get; set; }
        // Tamano de las fotos en el mapa
        private const double ANCHO_NODO = 60;
        private const double ALTO_NODO = 60;

        public MapaControl(GrafoPersonas grafo)
        {
            InitializeComponent();
            _grafo = grafo; // Guardar referencia al grafo

            // Al cargar el control la pantalla intenta refrescar automaticamente
            Loaded += (_, __) => OnRefresh?.Invoke();
            OnRefresh = RefrescarMapa;
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
            DibujarRelaciones(ANCHO_NODO, ALTO_NODO);
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
                ToolTip = $"{persona.Nombre}\nCédula: {persona.Cedula}",
                Tag = persona
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
            imagenPersona.MouseEnter += ImagenPersona_MouseEnter; // Para mostrar al pasar el mouse
            imagenPersona.MouseLeftButtonDown += ImagenPersona_MouseLeftButtonDown;
            imagenPersona.MouseLeave += (s, e) => LimpiarEtiquetasDistancia();
            MapaCanvas.Children.Add(imagenPersona);

            var etiquetaNombre = new TextBlock
            {
                Text = persona.Nombre,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Width = anchoNodo,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(etiquetaNombre, persona.PosX);
            Canvas.SetTop(etiquetaNombre, persona.PosY + altoNodo + 2);
            MapaCanvas.Children.Add(etiquetaNombre);
        }

        // Handlers
        private void ImagenPersona_MouseEnter(object sender, MouseEventArgs e) // Mostrar distancias al pasar el mouse
        {
            if (sender is Image img && img.Tag is Persona persona)
                MostrarDistanciasParaPersona(persona);
        }
        private void ImagenPersona_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Mostrar distancias al hacer click
        {
            if (sender is Image img && img.Tag is Persona persona)
                MostrarDistanciasParaPersona(persona);
        }

        private void MostrarDistanciasParaPersona(Persona origen)
        {
            // 1. Quitar etiquetas de distancia anteriores
            LimpiarEtiquetasDistancia();

            // 2. Obtener distancias mínimas por grafo desde "origen"
            var distancias = _grafo.CalcularDistancia(origen);

            foreach (var kvp in distancias)
            {
                Guid idDestino = kvp.Key;
                var (distanciaTotal, previoId) = kvp.Value;

                // Saltar el propio origen o nodos sin camino
                if (idDestino == origen.Id || double.IsInfinity(distanciaTotal) || previoId == null)
                    continue;

                var destino = _grafo.BuscarPersonaPorId(idDestino);
                var previo = _grafo.BuscarPersonaPorId(previoId.Value);

                if (destino == null || previo == null)
                    continue;

                // 3. Colocar la etiqueta sobre la ÚLTIMA arista del camino:
                // la que une "previo" -> "destino"
                double x1 = previo.PosX + ANCHO_NODO / 2.0;
                double y1 = previo.PosY + ALTO_NODO / 2.0;
                double x2 = destino.PosX + ANCHO_NODO / 2.0;
                double y2 = destino.PosY + ALTO_NODO / 2.0;

                double xMid = (x1 + x2) / 2.0;
                double yMid = (y1 + y2) / 2.0;

                var etiqueta = new TextBlock
                {
                    Text = distanciaTotal.ToString("0.0"),
                    FontSize = 12,
                    Foreground = Brushes.Yellow,
                    Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                    Padding = new Thickness(3, 1, 3, 1),
                    Tag = "DistanciaLabel"
                };

                // Pequeño offset para que no se pegue exactamente a la línea
                Canvas.SetLeft(etiqueta, xMid + 4);
                Canvas.SetTop(etiqueta, yMid + 4);

                MapaCanvas.Children.Add(etiqueta);
            }
        }

        private void LimpiarEtiquetasDistancia()
        {
            var aEliminar = new List<UIElement>();

            foreach (UIElement child in MapaCanvas.Children)
            {
                if (child is TextBlock tb && tb.Tag is string tag && tag == "DistanciaLabel")
                {
                    aEliminar.Add(child);
                }
            }
            foreach (var el in aEliminar)
            {
                MapaCanvas.Children.Remove(el);
            }

        }



        // / Dibuja las relaciones entre personas como lineas en el canvas
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
                        Stroke = Brushes.Black,
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
