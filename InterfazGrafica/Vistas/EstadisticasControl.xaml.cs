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
    public partial class EstadisticasControl : UserControl
    {
        public EstadisticasControl()
        {
            InitializeComponent();
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
        }
    }
}
