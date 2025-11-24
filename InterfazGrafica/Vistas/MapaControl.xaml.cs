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

        public MapaControl(GrafoPersonas grafo) // Constructor 
        {
            InitializeComponent();
            _grafo = grafo; // Guardar referencia al grafo

            // Al cargar el control la pantalla intenta refrescar automaticamente
            Loaded += (_, __) => OnRefresh?.Invoke();
            OnRefresh = RefrescarMapa;
        }
        
        /// Limpia la capa de dibujo (las lineas, imagenes de personas) sin tocar la imagen de fondo
        public void LimpiarOverlay() => MapaCanvas.Children.Clear();
        
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

        // Dibuja una persona en el canvas
        private void DibujarPersona(Persona persona, double anchoNodo, double altoNodo)
        {
            var imagenPersona = new Image // Crear imagen de la persona
            {
                Width = anchoNodo,
                Height = altoNodo,
                Stretch = Stretch.UniformToFill,
                ToolTip = $"{persona.Nombre}\nCédula: {persona.Cedula}",
                Tag = persona
            };
            if (!string.IsNullOrWhiteSpace(persona.RutaFoto) && File.Exists(persona.RutaFoto)) // Cargar foto si existe
            {
                var bitmap = new BitmapImage(); // Crear bitmap desde la ruta
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(persona.RutaFoto, UriKind.Absolute); // Establecer la fuente de la imagen
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imagenPersona.Source = bitmap; // Asignar la imagen al control
            }
            Canvas.SetLeft(imagenPersona, persona.PosX); // Posicionar en el canvas
            Canvas.SetTop(imagenPersona, persona.PosY);

            // Para mostrar las distancias al dar click el mouse
            imagenPersona.MouseLeftButtonDown += ImagenPersona_MouseLeftButtonDown;
            imagenPersona.MouseLeave += (s, e) => LimpiarEtiquetasDistancia();
            MapaCanvas.Children.Add(imagenPersona); // Agregar al canvas

            var etiquetaNombre = new TextBlock // Etiqueta con el nombre
            {
                Text = persona.Nombre,
                Foreground = Brushes.White, // Color del texto
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Width = anchoNodo,
                TextAlignment = TextAlignment.Center // Centrar el texto
            };
            Canvas.SetLeft(etiquetaNombre, persona.PosX); // Posicionar debajo de la imagen
            Canvas.SetTop(etiquetaNombre, persona.PosY + altoNodo + 2); // 2 pixels debajo
            MapaCanvas.Children.Add(etiquetaNombre); 
        }

        // Handler
        // Mostrar distancias al hacer click
        private void ImagenPersona_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag is Persona persona) // Obtener la persona del tag
                MostrarDistanciasParaPersona(persona); // Mostrar distancias desde esa persona
        }

        private void MostrarDistanciasParaPersona(Persona origen)
        {
            // 1. Quitar etiquetas de distancia anteriores
            LimpiarEtiquetasDistancia();

            // 2. Obtener distancias minimas por grafo desde el origen
            var distancias = _grafo.CalcularDistancia(origen);

            foreach (var kvp in distancias) // Recorrer cada destino y su distancia
            {
                Guid idDestino = kvp.Key; // Nodo destino
                var (distanciaTotal, previoId) = kvp.Value; // Distancia total y nodo previo en el camino

                // Saltar el propio origen o nodos sin camino
                if (idDestino == origen.Id || double.IsInfinity(distanciaTotal) || previoId == null)
                    continue;

                var destino = _grafo.BuscarPersonaPorId(idDestino); // Nodo destino
                var previo = _grafo.BuscarPersonaPorId(previoId.Value); // Nodo previo en el camino

                if (destino == null || previo == null)
                    continue;

                // 3. Colocar la etiqueta sobre la ÚLTIMA arista del camino:
                // la que une previo al destino
                // Calcular punto medio de la linea entre previo y destino
                double x1 = previo.PosX + ANCHO_NODO / 2.0; 
                double y1 = previo.PosY + ALTO_NODO / 2.0; 
                double x2 = destino.PosX + ANCHO_NODO / 2.0; 
                double y2 = destino.PosY + ALTO_NODO / 2.0;
                // Punto medio
                double xMid = (x1 + x2) / 2.0; 
                double yMid = (y1 + y2) / 2.0;

                var etiqueta = new TextBlock // Crear etiqueta con la distancia
                {
                    Text = $"{distanciaTotal:0.0} km",
                    FontSize = 12,
                    Foreground = Brushes.Yellow,
                    Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                    Padding = new Thickness(3, 1, 3, 1),
                    Tag = "DistanciaLabel"
                };

                // offset para que no se pegue exactamente a la línea
                Canvas.SetLeft(etiqueta, xMid + 4);
                Canvas.SetTop(etiqueta, yMid + 4);

                MapaCanvas.Children.Add(etiqueta);
            }
        }
        // Elimina todas las etiquetas de distancia del canvas
        private void LimpiarEtiquetasDistancia()
        {
            // Buscar y eliminar todas las etiquetas con el tag DistanciaLabel
            var aEliminar = new List<UIElement>(); // Lista temporal para evitar modificar la coleccion mientras se itera

            foreach (UIElement child in MapaCanvas.Children)
            {
                if (child is TextBlock tb && tb.Tag is string tag && tag == "DistanciaLabel") 
                {
                    aEliminar.Add(child);
                }
            }
            foreach (var el in aEliminar) // Eliminar las etiquetas encontradas
            {
                MapaCanvas.Children.Remove(el);
            }

        }

        // / Dibuja las relaciones entre personas como lineas en el canvas
        private void DibujarRelaciones(double anchoNodo, double altoNodo)
        {
            var paresVisitados = new HashSet<(Guid, Guid)>(); // Para evitar dibujar dos veces la misma relacion
            (Guid, Guid) Normalizar(Guid a, Guid b)
                => a.CompareTo(b) <= 0 ? (a, b) : (b, a);

            foreach (var kvp in _grafo.Adyacencias) // Recorrer cada persona y sus relaciones
            {
                var id1 = kvp.Key; // Id de la persona origen
                var p1 = _grafo.BuscarPersonaPorId(id1); // Obtener la persona origen
                if (p1 == null) continue;

                foreach(var id2 in kvp.Value) // Recorrer cada relacion de la persona
                {
                    var p2 = _grafo.BuscarPersonaPorId(id2);
                    if (p2 == null) continue;

                    var par = Normalizar(id1, id2);
                    if ( !paresVisitados.Add(par))
                        continue; // Ya dibujado

                    // Calcular puntos centrales de cada persona
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
    }
}
