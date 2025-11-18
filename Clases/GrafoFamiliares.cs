using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Clases
{
    public class GrafoPersonas
    {
        public List<Persona> Personas { get; private set; } = new(); // Lista de personas en el grafo

        // Diccionario de adyacencias (relaciones) entre personas
        public Dictionary<Guid, List<Guid>> Adyacencias { get; private set; } = new(); 

        public void AgregarPersona(Persona persona)
        {
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));
            if (Personas.Any(p => p.Cedula == persona.Cedula)) // Verificar si la persona ya existe por cedula
            {
                Console.WriteLine("La persona con cedula " + persona.Cedula + " ya existe en el grafo.");
                return;
            }

            Personas.Add(persona); // Agregar la persona al grafo
            if (!Adyacencias.ContainsKey(persona.Id))
            {
                Adyacencias[persona.Id] = new List<Guid>(); // Inicializar la lista de adyacencias para la nueva persona
            }
        }
        //Buscar persona por nombre
        public Persona? BuscarPersonaPorNombre(string nombre) =>
        Personas.FirstOrDefault(p =>
            string.Equals(p.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        //Buscar persona por id
        public Persona? BuscarPersonaPorId(Guid id) =>
            Personas.FirstOrDefault(p => p.Id == id);
        //Buscar persona por cedula
        public Persona? BuscarPersonaPorCedula(int cedula) =>
           Personas.FirstOrDefault(p => p.Cedula == cedula);


        // Agregar relacion bidireccional entre dos personas
        public void AgregarRelacionBidireccional(Persona p1, Persona p2)
        {
            if (p1 == null || p2 == null) // Validar que las personas no sean nulas
                return;
            if (p1.Id == p2.Id)  // No permitir relaciones consigo mismo
                return;            

            AgregarRelacion(p1.Id, p2.Id);
            AgregarRelacion(p2.Id, p1.Id);
        }

        // Agregar relacion unidireccional entre dos personas
        public void AgregarRelacion(Guid id1, Guid id2)
        {
            if (!Adyacencias.ContainsKey(id1)) // Si la persona no tiene adyacencias, inicializar la lista
            {
                Adyacencias[id1] = new List<Guid>();
            }
            if (!Adyacencias[id1].Contains(id2)) // Evitar duplicados
            {
                Adyacencias[id1].Add(id2);
            }
        }

        //Retornar los familiares de una persona
        public IEnumerable<Persona> ObtenerPersonasRelacionadas(Persona persona) //Enumerable para retornar una lista de personas relacionadas
        {
            if (persona == null) // Validar que la persona no sea nula
                yield break;

            if (!Adyacencias.TryGetValue(persona.Id, out var vecinos)) // Obtener los vecinos de la persona
                yield break;

            foreach (var id in vecinos) // Recorrer los vecinos
            {
                var relacionada = Personas.FirstOrDefault(p => p.Id == id);
                if (relacionada != null)
                    yield return relacionada; // Retornar la persona relacionada
            }
        }
        // Eliminar relacion bidireccional entre dos personas
        public void EliminarRelacionBidereccional(Persona p1, Persona p2)
        {
            if (p1 == null || p2 == null)
                return;
            EliminarRelacion(p1.Id, p2.Id);
            EliminarRelacion(p2.Id, p1.Id);
        }

        // Eliminar relacion unidireccional entre dos personas
        public void EliminarRelacion(Guid id1, Guid id2)
        {
            if (Adyacencias.TryGetValue(id1, out var lista)) // Obtener la lista de adyacencias
            {
                lista.Remove(id2);
            }
        }

        public void EliminarPersona(Persona persona)
        {
            if (persona == null)
            {
                return;
            }
            Personas.RemoveAll(p => p.Id == persona.Id); // Eliminar la persona del grafo(de la lista de personas)
            Adyacencias.Remove(persona.Id); // Eliminar las adyacencias de la persona (de la lista de adyacencias)
            foreach(var kvp in Adyacencias) // Eliminar las adyacencias de la persona en las listas de adyacencias de otras personas
            {
                kvp.Value.Remove(persona.Id);
            }
        }

        public void MostrarGrafo()
        {
            Console.WriteLine("\nRelaciones en el grafo:\n");
            foreach (var persona in Personas)
            {
                Console.WriteLine($"{persona.Nombre}:");
                if (Adyacencias.TryGetValue(persona.Id, out var lista))
                {
                    foreach (var idRelacionado in lista)
                    {
                        Persona? relacionada = Personas.FirstOrDefault(p => p.Id == idRelacionado);
                        if (relacionada != null)
                        {
                            Console.WriteLine($"   → {relacionada.Nombre}");
                        }
                    }
                }
                Console.WriteLine();

            }

        }
    }
}


