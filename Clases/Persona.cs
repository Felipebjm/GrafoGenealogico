using System;

namespace Clases
{
    public class Persona
    {
        public Guid Id { get; private set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Vivo { get; set; }
        public string Sexo { get; set; }  // M o F

        public Persona(string nombre, int cedula, DateTime fechaNacimiento, bool vivo, string sexo)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            Vivo = vivo;
            Sexo = sexo.ToUpper();  // Normalizar
        }

        public override string ToString()
        {
            return $"{Nombre} | Cédula: {Cedula} | Nacimiento: {FechaNacimiento:dd/MM/yyyy} | Vivo: {Vivo} | Sexo: {Sexo}";
        }
    }
}
