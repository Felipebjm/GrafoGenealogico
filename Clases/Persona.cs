using System;
using System.Linq;

namespace Clases
{
    public class Persona
    {
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
        }

        public override string ToString()
        {
            return $"{Nombre} | Cédula: {Cedula} | Nacimiento: {FechaNacimiento:dd/MM/yyyy} | Vivo: {EstaVivo}";
        }
    }

}
