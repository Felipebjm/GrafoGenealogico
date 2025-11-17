using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Clases;

namespace InterfazGrafica.Vistas
{
    /// <summary>
    /// Interaction logic for AgregarFamiliarControl.xaml
    /// </summary>
    public partial class AgregarFamiliarControl : UserControl
    {
        // Mas adelante hay que pasar el grafo por parametro
        // private GrafoFamilia _grafo;

        private string? _rutaFotoSeleccionada;
        public string? RutaFotoSeleccionada => _rutaFotoSeleccionada;


        public AgregarFamiliarControl()
        {
            InitializeComponent();
        }

        private void BtnElegirPosicion_Click(object sender, RoutedEventArgs e)
        {
            // Aqui se va a abrir un mapa donde el usuario escoje la posicion"
            // y cuando el usuario haga clic, te devuelve X/Y
            // esto es para ponerlo en TxtX.Text y TxtY.Text
            // Crear y mostrar la ventana del mapa como diálogo
            var mapaWindow = new MapaWindow();
            bool? resultado = mapaWindow.ShowDialog();

            // Si el usuario hizo clic en el mapa y la ventana devolvió OK
            if (resultado == true)
            {
                // Guardar las coordenadas en los TextBox
                TxtX.Text = mapaWindow.CoordenadaX.ToString("0"); // sin decimales
                TxtY.Text = mapaWindow.CoordenadaY.ToString("0");
            }
        }

        private void BtnSeleccionarFoto_Click(object sender, RoutedEventArgs e)
        {
            // Esto va a ser para abrir un OpenFileDialog y cargar la imagen en ImgFoto.Source
            // Crear y configurar el OpenFileDialog
            var OpenFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar foto",
                Filter = "Imágenes (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif"

            };

            bool? result = OpenFileDialog.ShowDialog();

            if (result == true)
            {
                //Guarda la ruta de la foto seleccionada
                _rutaFotoSeleccionada = OpenFileDialog.FileName;

                //Cargar la imagen en el control ImgFoto
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_rutaFotoSeleccionada);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgFoto.Source = bitmap;


            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Por ahora, crear una persona con datos de ejemplo
                // hasta que el XAML tenga todos los controles necesarios
                var nuevaPersona = CrearPersonaDeEjemplo();

                // Obtener la ventana principal y agregar la persona
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.AgregarPersonaAFamilia(nuevaPersona);

                // Confirmar al usuario
                MessageBox.Show($"Persona '{nuevaPersona.NombreCompleto}' agregada exitosamente.\n" +
                              $"Las estadísticas se han actualizado automáticamente.", 
                              "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la persona: {ex.Message}", 
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Crea una persona de ejemplo (temporal hasta completar la integración con el formulario)
        /// </summary>
        private Persona CrearPersonaDeEjemplo()
        {
            var nombres = new[] { "Carlos", "María", "José", "Ana", "Luis", "Carmen", "Diego", "Laura" };
            var apellidos = new[] { "González", "Rodríguez", "García", "López", "Martínez", "Hernández", "Pérez", "Sánchez" };
            
            var random = new Random();
            var nombre = nombres[random.Next(nombres.Length)];
            var apellido = apellidos[random.Next(apellidos.Length)];
            
            var persona = new Persona(
                nombre, 
                apellido, 
                DateTime.Today.AddYears(-random.Next(20, 80)),
                random.Next(50, 600), 
                random.Next(50, 400)
            );

            // Algunas personas podrían estar fallecidas
            if (random.Next(10) < 2) // 20% de probabilidad
            {
                persona.FechaFallecimiento = DateTime.Today.AddYears(-random.Next(1, 10));
            }

            return persona;
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
