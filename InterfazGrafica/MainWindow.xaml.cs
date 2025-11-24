using Clases;
using InterfazGrafica.Vistas;
using System.Windows;

namespace InterfazGrafica
{
    public partial class MainWindow : Window
    {
<<<<<<< HEAD
        // Sistema de estadísticas de la familia
        private EstadisticasFamilia _estadisticasFamilia;
        private EstadisticasControl? _estadisticasControl;

=======
        // Instancia del grafo de personas
        private readonly GrafoPersonas _grafo = new GrafoPersonas(); 
>>>>>>> origin/feature/felipeUI
        public MainWindow()
        {
            InitializeComponent();

            // Inicializar el sistema de estadísticas
            _estadisticasFamilia = new EstadisticasFamilia();
            
            // Agregar algunos datos de prueba
            AgregarDatosDePrueba();
            
            // Al iniciar, se muestra la pantalla de agregar familiar
<<<<<<< HEAD
            MainContent.Content = new Vistas.AgregarFamiliarControl();
=======
            MainContent.Content = new Vistas.AgregarFamiliarControl(_grafo);
>>>>>>> origin/feature/felipeUI
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            //Recordar pasar el grafo por parametro cuando exista
            MainContent.Content = new Vistas.AgregarFamiliarControl(_grafo);
        }

        //Boton para ir a la vista del mapa
        private void BtnMapa_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Vistas.MapaControl(_grafo);
        }

        //Boton para ir a la vista de estadisticas
        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            // Crear la vista de estadísticas con los datos reales
            _estadisticasControl = new EstadisticasControl(_estadisticasFamilia);

            // Mostrar la vista de estadísticas en el área principal
            MainContent.Content = _estadisticasControl;
        }

        /// <summary>
        /// Método público para agregar una persona desde otros controles
        /// </summary>
        public void AgregarPersonaAFamilia(Persona persona)
        {
            _estadisticasFamilia.AgregarPersona(persona);
            
            // Si las estadísticas están actualmente visibles, actualizarlas
            if (_estadisticasControl != null && MainContent.Content == _estadisticasControl)
            {
                _estadisticasControl.ActualizarEstadisticas();
            }
        }

        /// <summary>
        /// Obtiene la instancia de estadísticas para uso de otros controles
        /// </summary>
        public EstadisticasFamilia ObtenerEstadisticasFamilia()
        {
            return _estadisticasFamilia;
        }

        /// <summary>
        /// Agrega algunos datos de prueba para demostrar el funcionamiento del sistema
        /// </summary>
        private void AgregarDatosDePrueba()
        {
            // Crear algunas personas de ejemplo
            var ana = new Persona("Ana", "García", new DateTime(1985, 3, 15), 100, 200);
            var juan = new Persona("Juan", "Pérez", new DateTime(1982, 7, 22), 150, 180);
            var sofia = new Persona("Sofía", "López", new DateTime(1990, 11, 8), 300, 400);
            var pedro = new Persona("Pedro", "Martín", new DateTime(1978, 5, 30), 500, 100);
            
            // Agregar una persona fallecida
            var abuela = new Persona("María", "González", new DateTime(1950, 2, 14), 200, 300);
            abuela.FechaFallecimiento = new DateTime(2020, 8, 10);

            // Agregar todas las personas al sistema
            _estadisticasFamilia.AgregarPersona(ana);
            _estadisticasFamilia.AgregarPersona(juan);
            _estadisticasFamilia.AgregarPersona(sofia);
            _estadisticasFamilia.AgregarPersona(pedro);
            _estadisticasFamilia.AgregarPersona(abuela);
=======
            var statsControl = new EstadisticasControl(_grafo);
            MainContent.Content = statsControl;
>>>>>>> origin/feature/felipeUI
        }
    }
}
