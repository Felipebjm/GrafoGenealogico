using System;
<<<<<<< HEAD
=======
using System.Linq;
>>>>>>> origin/feature/felipeUI

namespace Clases
{
    public class Persona
    {
<<<<<<< HEAD
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; } 
        public DateTime FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public double PosicionX { get; set; }
        public double PosicionY { get; set; }
        public string? RutaFoto { get; set; }
        public string? LugarNacimiento { get; set; }
        
        // Propiedades calculadas
        public bool EstaVivo => !FechaFallecimiento.HasValue;
        
        public int Edad 
        { 
            get 
            {
                var fechaReferencia = FechaFallecimiento ?? DateTime.Now;
                return (int)((fechaReferencia - FechaNacimiento).Days / 365.25);
            } 
        }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        // Constructor
        public Persona()
        {
            Id = Guid.NewGuid().ToString();
            Nombre = string.Empty;
            Apellido = string.Empty;
        }

        public Persona(string nombre, string apellido, DateTime fechaNacimiento, double x = 0, double y = 0)
        {
            Id = Guid.NewGuid().ToString();
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
            PosicionX = x;
            PosicionY = y;
        }

        // Método para calcular distancia euclidiana con otra persona
        public double CalcularDistancia(Persona otraPersona)
        {
            if (otraPersona == null) return 0;
            
            double deltaX = PosicionX - otraPersona.PosicionX;
            double deltaY = PosicionY - otraPersona.PosicionY;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
=======
        // Atributos
        public Guid Id { get; private set; }          // Identificador unico
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool EstaVivo { get; set; }

        // Para la interfaz (fotos y ubicacion)
        public string? RutaFoto { get; set; }         // Ruta a la imagen de la persona
        public double PosX { get; set; }              // Coordenada X para visualización
        public double PosY { get; set; }              // Coordenada Y para visualización
        public int? AnioFallecimiento { get; set; }   // Año de fallecimiento (null si esta vivo)

        // Propiedad calculada: edad en años
        public int Edad => (int)((DateTime.Now - FechaNacimiento).TotalDays / 365.25);

        // Constructor


        public Persona(string nombre, int cedula, DateTime fechaNacimiento, bool estaVivo,
                       string? rutaFoto = null, double posX = 0, double posY = 0, int? anioFallecimiento = null)
        {
            //Validaciones
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException();
            if (cedula <= 0)
                throw new ArgumentException();
            if (fechaNacimiento > DateTime.Now)
                throw new ArgumentException();
            // Validaciones relacionada con el fallecimientos
            if (!estaVivo)
            {
                if (!anioFallecimiento.HasValue)
                    throw new ArgumentException();
                if (anioFallecimiento.Value < fechaNacimiento.Year)
                    throw new ArgumentException();
                if (anioFallecimiento.Value > DateTime.Now.Year)
                    throw new ArgumentException();
            }
            else
            {
                anioFallecimiento = null;
            }

            Id = Guid.NewGuid();
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            EstaVivo = estaVivo;

            RutaFoto = rutaFoto;
            PosX = posX;
            PosY = posY;
            AnioFallecimiento = anioFallecimiento;
>>>>>>> origin/feature/felipeUI
        }

        public override string ToString()
        {
<<<<<<< HEAD
            return NombreCompleto;
=======
            return $"{Nombre} | Cédula: {Cedula} | Nacimiento: {FechaNacimiento:dd/MM/yyyy} | Vivo: {EstaVivo}";
>>>>>>> origin/feature/felipeUI
        }
    }

}
