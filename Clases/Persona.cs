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

        // Propiedad calculada: edad en años
        public int Edad => (int)((DateTime.Now - FechaNacimiento).TotalDays / 365.25);

        // Constructor


        public Persona(string nombre, int cedula, DateTime fechaNacimiento, bool estaVivo,
                       string? rutaFoto = null, double posX = 0, double posY = 0)
        {
            //Validaciones
            if(string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
            if (cedula <= 0)
                throw new ArgumentException("La cédula debe ser un número positivo.", nameof(cedula));
            if (fechaNacimiento > DateTime.Now)
                throw new ArgumentException("La fecha de nacimiento no puede ser en el futuro.", nameof(fechaNacimiento));


            Id = Guid.NewGuid();
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            EstaVivo = estaVivo;

            RutaFoto = rutaFoto;
            PosX = posX;
            PosY = posY;
        }

        public override string ToString()
        {
            return $"{Nombre} | Cédula: {Cedula} | Nacimiento: {FechaNacimiento:dd/MM/yyyy} | Vivo: {EstaVivo}";
        }
    }

}
