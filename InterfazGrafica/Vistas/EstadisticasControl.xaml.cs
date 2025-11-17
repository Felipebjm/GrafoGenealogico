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
using Clases;

namespace InterfazGrafica.Vistas
{
    public partial class EstadisticasControl : UserControl
    {
        private EstadisticasFamilia _estadisticasFamilia;

        public EstadisticasControl()
        {
            InitializeComponent();
            _estadisticasFamilia = new EstadisticasFamilia();
        }

        public EstadisticasControl(EstadisticasFamilia estadisticasFamilia)
        {
            InitializeComponent();
            _estadisticasFamilia = estadisticasFamilia ?? new EstadisticasFamilia();
            ActualizarEstadisticas();
        }

       
        /// Establece los nombres del par más cercano.
        /// Ejemplo: SetParCercano("Ana", "Juan");
        public void SetParCercano(string nombreA, string nombreB)
        {
            TxtCercanoA.Text = nombreA ?? string.Empty;
            TxtCercanoB.Text = nombreB ?? string.Empty;
        }

  
        /// Establece los nombres del par mss lejano.
       
        public void SetParLejano(string nombreA, string nombreB)
        {
            TxtLejanoA.Text = nombreA ?? string.Empty;
            TxtLejanoB.Text = nombreB ?? string.Empty;
        }
        /// Establece la distancia promedio entre familiares (como texto).

        public void SetDistanciaPromedio(string distanciaPromedio)
        {
            TxtPromedio.Text = distanciaPromedio ?? string.Empty;
        }

        
        /// Limpia todos los campos (por si hiciera falta resetear).
        public void Limpiar()
        {
            TxtCercanoA.Text = string.Empty;
            TxtCercanoB.Text = string.Empty;
            TxtLejanoA.Text = string.Empty;
            TxtLejanoB.Text = string.Empty;
            TxtPromedio.Text = string.Empty;
            TxtTotalPersonas.Text = string.Empty;
            TxtVivosYFallecidos.Text = string.Empty;
            TxtEdadPromedio.Text = string.Empty;
            TxtMasJoven.Text = string.Empty;
            TxtMasViejo.Text = string.Empty;
        }

        /// <summary>
        /// Actualiza todas las estadísticas basándose en los datos actuales
        /// </summary>
        public void ActualizarEstadisticas()
        {
            var estadisticas = _estadisticasFamilia.CalcularEstadisticas();

            // Actualizar campos existentes
            if (estadisticas.ParMasCercano != null)
            {
                SetParCercano(estadisticas.ParMasCercano.PersonaA.NombreCompleto, 
                             estadisticas.ParMasCercano.PersonaB.NombreCompleto);
            }
            else
            {
                SetParCercano("N/A", "N/A");
            }

            if (estadisticas.ParMasLejano != null)
            {
                SetParLejano(estadisticas.ParMasLejano.PersonaA.NombreCompleto,
                            estadisticas.ParMasLejano.PersonaB.NombreCompleto);
            }
            else
            {
                SetParLejano("N/A", "N/A");
            }

            SetDistanciaPromedio($"{estadisticas.DistanciaPromedio:F1} km");

            // Actualizar nuevos campos
            SetTotalPersonas(estadisticas.TotalPersonas);
            SetVivosYFallecidos(estadisticas.PersonasVivas, estadisticas.PersonasFallecidas);
            SetEdadPromedio(estadisticas.EdadPromedio);
            
            if (estadisticas.PersonaMasJoven != null && estadisticas.PersonaMasVieja != null)
            {
                SetEdadesExtremas(estadisticas.PersonaMasJoven, estadisticas.PersonaMasVieja);
            }
        }

        /// <summary>
        /// Agrega una nueva persona y actualiza las estadísticas
        /// </summary>
        public void AgregarPersona(Persona persona)
        {
            _estadisticasFamilia.AgregarPersona(persona);
            ActualizarEstadisticas();
        }

        /// <summary>
        /// Remueve una persona y actualiza las estadísticas
        /// </summary>
        public void RemoverPersona(string personaId)
        {
            _estadisticasFamilia.RemoverPersona(personaId);
            ActualizarEstadisticas();
        }

        /// <summary>
        /// Establece el total de personas
        /// </summary>
        public void SetTotalPersonas(int total)
        {
            TxtTotalPersonas.Text = total.ToString();
        }

        /// <summary>
        /// Establece la información de vivos y fallecidos
        /// </summary>
        public void SetVivosYFallecidos(int vivos, int fallecidos)
        {
            TxtVivosYFallecidos.Text = $"{vivos} vivos, {fallecidos} fallecidos";
        }

        /// <summary>
        /// Establece la edad promedio
        /// </summary>
        public void SetEdadPromedio(double edadPromedio)
        {
            TxtEdadPromedio.Text = $"{edadPromedio:F1} años";
        }

        /// <summary>
        /// Establece las edades extremas (más joven y más viejo)
        /// </summary>
        public void SetEdadesExtremas(Persona masJoven, Persona masViejo)
        {
            TxtMasJoven.Text = $"{masJoven.NombreCompleto} ({masJoven.Edad})";
            TxtMasViejo.Text = $"{masViejo.NombreCompleto} ({masViejo.Edad})";
        }

        /// <summary>
        /// Obtiene la instancia de EstadisticasFamilia para uso externo
        /// </summary>
        public EstadisticasFamilia ObtenerEstadisticasFamilia()
        {
            return _estadisticasFamilia;
        }
    }
}
