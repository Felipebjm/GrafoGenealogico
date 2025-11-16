using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Clases
{
    public class GrafoPersonas
    {
        public List<Persona> Personas { get; private set; } = new();
        public Dictionary<Guid, List<(Guid idRelacionado, string parentesco)>> Adyacencias { get; private set; } = new();

        public void AgregarPersona(Persona persona)
        {
            if (persona == null) throw new ArgumentNullException(nameof(persona));
            if (Personas.Any(p => p.Cedula == persona.Cedula))
            {
                Console.WriteLine($" La persona con cédula {persona.Cedula} ya existe.");
                return;
            }

            Personas.Add(persona);
            Adyacencias[persona.Id] = new List<(Guid, string)>();
        }

        public Persona? BuscarPersonaPorNombre(string nombre) =>
            Personas.FirstOrDefault(p =>
                string.Equals(p.Nombre, nombre, StringComparison.OrdinalIgnoreCase));


        public void AgregarRelacionBidireccional(Persona p1, Persona p2, string parentesco)
        {
            if (p1 == null || p2 == null || string.IsNullOrWhiteSpace(parentesco)) return;

            string inverso = ObtenerParentescoInverso(p1, p2, parentesco);

            AgregarRelacion(p1.Id, p2.Id, parentesco);
            AgregarRelacion(p2.Id, p1.Id, inverso);
        }

        private void AgregarRelacion(Guid id1, Guid id2, string parentesco)
        {
            if (!Adyacencias.ContainsKey(id1))
                Adyacencias[id1] = new List<(Guid, string)>();

            if (!Adyacencias[id1].Any(rel => rel.idRelacionado == id2))
                Adyacencias[id1].Add((id2, parentesco));
        }

        private static string ObtenerParentescoInverso(Persona p1, Persona p2, string parentesco)
        {
            parentesco = parentesco.Trim().ToLower();

            return parentesco switch
            {
                "esposo" => "esposa",
                "esposa" => "esposo",
                "mamá" or "papá" => p2.Sexo == "F" ? "hija" : "hijo",
                "hijo" or "hija" => p2.Sexo == "F" ? "mamá" : "papá",
                "hermano" or "hermana" => p2.Sexo == "F" ? "hermana" : "hermano",
                _ => "relación"
            };
        }

        public void MostrarGrafo()
        {
            Console.WriteLine("\n🔗 Relaciones en el grafo:\n");
            foreach (var persona in Personas)
            {
                Console.WriteLine($"{persona.Nombre}:");
                if (Adyacencias.TryGetValue(persona.Id, out var lista))
                {
                    foreach (var (idRelacionado, parentesco) in lista)
                    {
                        Persona? relacionada = Personas.FirstOrDefault(p => p.Id == idRelacionado);
                        if (relacionada != null)
                        {
                            Console.WriteLine($"   → {relacionada.Nombre} ({parentesco})");
                        }

                    }
                }
                Console.WriteLine();
            }
        }
    }
}
