using System;

namespace Clases
{
    public class Persona
    {
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
        }

        public override string ToString()
        {
            return NombreCompleto;
        }
    }
}
