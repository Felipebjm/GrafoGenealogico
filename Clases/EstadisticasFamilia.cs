using System;
using System.Collections.Generic;
using System.Linq;

namespace Clases
{
    public class EstadisticasFamilia
    {
        public class ParDistancia
        {
            public Persona PersonaA { get; set; }
            public Persona PersonaB { get; set; }
            public double Distancia { get; set; }

            public ParDistancia(Persona personaA, Persona personaB, double distancia)
            {
                PersonaA = personaA;
                PersonaB = personaB;
                Distancia = distancia;
            }
        }

        public class EstadisticasCalculadas
        {
            public int TotalPersonas { get; set; }
            public ParDistancia? ParMasCercano { get; set; }
            public ParDistancia? ParMasLejano { get; set; }
            public double DistanciaPromedio { get; set; }
            public double EdadPromedio { get; set; }
            public int PersonasVivas { get; set; }
            public int PersonasFallecidas { get; set; }
            public Persona? PersonaMasJoven { get; set; }
            public Persona? PersonaMasVieja { get; set; }
        }

        private List<Persona> _personas;

        public EstadisticasFamilia()
        {
            _personas = new List<Persona>();
        }

        public void AgregarPersona(Persona persona)
        {
            if (persona != null && !_personas.Any(p => p.Id == persona.Id))
            {
                _personas.Add(persona);
            }
        }

        public void RemoverPersona(string personaId)
        {
            _personas.RemoveAll(p => p.Id == personaId);
        }

        public EstadisticasCalculadas CalcularEstadisticas()
        {
            var estadisticas = new EstadisticasCalculadas();

            if (_personas.Count == 0)
            {
                return estadisticas;
            }

            // Total de personas
            estadisticas.TotalPersonas = _personas.Count;

            // Personas vivas y fallecidas
            estadisticas.PersonasVivas = _personas.Count(p => p.EstaVivo);
            estadisticas.PersonasFallecidas = _personas.Count(p => !p.EstaVivo);

            // Edad promedio
            if (_personas.Any())
            {
                estadisticas.EdadPromedio = _personas.Average(p => p.Edad);
            }

            // Persona más joven y más vieja
            estadisticas.PersonaMasJoven = _personas.OrderBy(p => p.Edad).FirstOrDefault();
            estadisticas.PersonaMasVieja = _personas.OrderByDescending(p => p.Edad).FirstOrDefault();

            // Calcular distancias si hay al menos 2 personas
            if (_personas.Count >= 2)
            {
                var parejas = new List<ParDistancia>();

                for (int i = 0; i < _personas.Count; i++)
                {
                    for (int j = i + 1; j < _personas.Count; j++)
                    {
                        var persona1 = _personas[i];
                        var persona2 = _personas[j];
                        var distancia = persona1.CalcularDistancia(persona2);

                        parejas.Add(new ParDistancia(persona1, persona2, distancia));
                    }
                }

                if (parejas.Any())
                {
                    // Par más cercano (menor distancia)
                    estadisticas.ParMasCercano = parejas.OrderBy(p => p.Distancia).First();

                    // Par más lejano (mayor distancia)
                    estadisticas.ParMasLejano = parejas.OrderByDescending(p => p.Distancia).First();

                    // Distancia promedio
                    estadisticas.DistanciaPromedio = parejas.Average(p => p.Distancia);
                }
            }

            return estadisticas;
        }

        public List<Persona> ObtenerPersonas()
        {
            return new List<Persona>(_personas);
        }

        public int ContarPersonas()
        {
            return _personas.Count;
        }
    }
}