using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Clases;
using Microsoft.Win32; //Necesario para el OpenFileDialog
using System.IO;

namespace InterfazGrafica.Vistas
{
    public partial class AgregarFamiliarControl : UserControl
    {
        // Mas adelante hay que pasar el grafo por parametro;
        private readonly GrafoPersonas _grafo;
        private string? _rutaFotoSeleccionada;
        

        private Persona? _ultimoFamiliarCreado; //Para saber cual fue el ultimo creado
        public AgregarFamiliarControl(GrafoPersonas grafo)
        {
            InitializeComponent();
            _grafo = grafo;
            // Cargar la lista de familiares del grafo en el ComboBox
            CmbFamiliares.ItemsSource = _grafo.Personas;
        }

        private void BtnElegirPosicion_Click(object sender, RoutedEventArgs e)
        {
            // Aqui se va a abrir un mapa donde el usuario escoje la posicion
            // y cuando el usuario haga clic, te devuelve X/Y
            var mapaWindow = new MapaWindow();
            bool? resultado = mapaWindow.ShowDialog();

            // Si el usuario hizo clic en el mapa y la ventana devolvio OK
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
                // Filtrar el formato de archivo
                Title = "Seleccionar foto",
                Filter = "Imágenes (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif"

            };

            bool? result = OpenFileDialog.ShowDialog(); // Mostrar el dialog 

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
        private void BtnGuardar_Click(object sender, RoutedEventArgs e) //Btn para guardar el familiar que se esta creando
        {
            try
            {
                // 1. Leer y validar datos basicos
                // Validaciones
                string nombre = TxtNombre.Text.Trim();
                if (string.IsNullOrWhiteSpace(nombre)) //Nombre no puede estar vacio
                {
                    MessageBox.Show("El nombre no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TxtCedula.Text.Trim(), out int cedula) || cedula <= 0) //Cedula debe ser un numero positivo
                {
                    MessageBox.Show("La cédula debe ser un número positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DpFechaNacimiento.SelectedDate == null) //Fecha de nacimiento debe estar seleccionada
                {
                    MessageBox.Show("Seleccione una fecha de nacimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime fechaNacimiento = DpFechaNacimiento.SelectedDate.Value; //fecha de nacimiento seleccionada

                bool estaVivo = !(ChkNoEstaVivo.IsChecked ?? false); //Checkbox para saber si esta vivo o no
                int? anioFallecimiento = null;
                // Validaciones del año de fallecimiento
                if (!estaVivo && !string.IsNullOrWhiteSpace(TxtAnoFallecimiento.Text))
                {
                    if (!int.TryParse(TxtAnoFallecimiento.Text.Trim(), out int anoF))
                    {
                        MessageBox.Show("El año de fallecimiento debe ser un número.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (anoF < fechaNacimiento.Year)
                    {
                        MessageBox.Show("El año de fallecimiento no puede ser menor al año de nacimiento.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (anoF > DateTime.Now.Year)
                    {
                        MessageBox.Show("El año de fallecimiento no puede ser en el futuro.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    anioFallecimiento = anoF;
                }

                // 2. Leer coordenadas 
                double posX = 0; // Inicializar en 0 por defecto
                double posY = 0;
                double.TryParse(TxtX.Text.Trim(), out posX); //Si el usuario ingreso coordenadas, intentar parsearlas
                double.TryParse(TxtY.Text.Trim(), out posY);

                if (posX == 0 && posY == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una ubicación en el mapa.",
                        "Ubicación requerida",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // 3. Verificar que no exista otra persona con la misma cedula en el grafo
                if (_grafo.Personas.Any(p => p.Cedula == cedula))
                {
                    MessageBox.Show("Ya existe una persona con esa cédula en el grafo.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 4. Validad que el usiario tenga una foto seleccionada
                if (string.IsNullOrWhiteSpace(_rutaFotoSeleccionada) || !File.Exists(_rutaFotoSeleccionada))
                {
                    MessageBox.Show("Debe seleccionar una foto para el familiar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 5. Crear el objeto Persona
                var nuevaPersona = new Persona(
                    nombre: nombre,
                    cedula: cedula,
                    fechaNacimiento: fechaNacimiento,
                    estaVivo: estaVivo,
                    rutaFoto: _rutaFotoSeleccionada,
                    posX: posX,
                    posY: posY,
                    anioFallecimiento: anioFallecimiento
                );

                // 5. Agregar al grafo
                _grafo.AgregarPersona(nuevaPersona);

                // Guarda como ultimo familiar creado
                _ultimoFamiliarCreado = nuevaPersona;

                MessageBox.Show("Familiar agregado correctamente.\nAhora puedes usar 'Conectar familiar' para relacionarlo.",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // 6. Limpiar formulario 
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar el familiar:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void LimpiarFormulario() //Metodo para limpiar el formulario despues de guardar un familiar
        { 
            TxtNombre.Text = string.Empty;
            TxtCedula.Text = string.Empty;
            DpFechaNacimiento.SelectedDate = null;
            ChkNoEstaVivo.IsChecked = false;
            TxtAnoFallecimiento.Text = string.Empty;
            TxtX.Text = string.Empty;
            TxtY.Text = string.Empty;
            _rutaFotoSeleccionada = null;
            ImgFoto.Source = null;
        }
        private void ChkNoEstaVivo_Checked(object sender, RoutedEventArgs e) //Checkbox para saber si el familiar esta vivo o no
        {
            TxtAnoFallecimiento.IsEnabled = true;
        }

        private void ChkNoEstaVivo_Unchecked(object sender, RoutedEventArgs e) //Checkbox para saber si el familiar esta vivo o no
        {
            TxtAnoFallecimiento.IsEnabled = false;
            TxtAnoFallecimiento.Text = string.Empty;
        }
        private void BtnConectar_Click(object sender, RoutedEventArgs e) //Btn para conecta el familiar que se esta creando con uno de comboBox
        {
            if (_ultimoFamiliarCreado == null)
            {
                MessageBox.Show("Primero debe agregar un familiar antes de conectarlo.",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (CmbFamiliares.SelectedItem is not Persona seleccionado)
            {
                MessageBox.Show("Seleccione un familiar en la lista para conectarlo.",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Evitar conectarlo consigo mismo
            if (seleccionado.Id == _ultimoFamiliarCreado.Id)
            {
                MessageBox.Show("No se puede conectar un familiar consigo mismo.",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _grafo.AgregarRelacionBidireccional(_ultimoFamiliarCreado, seleccionado);

            MessageBox.Show($"Se ha conectado a {_ultimoFamiliarCreado.Nombre} con {seleccionado.Nombre}.",
                            "Conexión creada", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
