using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Clases
{
    public class GrafoPersonas
    {
        //public Ob<Persona> Personas { get; private set; } = new(); // Lista de personas en el grafo
        public ObservableCollection<Persona> Personas { get; private set; } = new(); // Lista de personas en el grafo

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


        // Implementacion del algoritmo de Dijkstra para calcular distancias  desde una persona origen a los otros nodo
        // Retorna un diccionario con las distancias y el nodo previo en el camino mas corto
        public Dictionary<Guid, (double distancia, Guid? previo)> CalcularDistancia(Persona origen)
        {
            if (origen == null)
                throw new ArgumentNullException(nameof(origen));

            // Diccionario de distancias: IdPersona -> (distancia acumulada, Id previo en el camino)
            var distancias = new Dictionary<Guid, (double distancia, Guid? previo)>();

            // Inicializar con infinito
            foreach (var p in Personas)
            {
                distancias[p.Id] = (double.PositiveInfinity, null);
            }

            // Distancia a si mismo = 0
            distancias[origen.Id] = (0, null);
            // Conjunto de nodos visitados
            var visitados = new HashSet<Guid>();

            // Bucle principal de Dijkstra 
            while (true)
            {
                // 1. Elegir el nodo no visitado con menor distancia conocida
                Guid? actualId = null;
                double mejorDist = double.PositiveInfinity;

                foreach (var kvp in distancias)
                {
                    var id = kvp.Key;
                    var (dist, _) = kvp.Value;

                    if (!visitados.Contains(id) && dist < mejorDist)
                    {
                        mejorDist = dist;
                        actualId = id;
                    }
                }

                // Si no hay mas alcanzables se terminamos
                if (actualId == null)
                    break;

                visitados.Add(actualId.Value);

                // 2. Relajar las aristas salientes desde actualId
                if (!Adyacencias.TryGetValue(actualId.Value, out var vecinos))
                    continue;

                var personaActual = BuscarPersonaPorId(actualId.Value);
                if (personaActual == null)
                    continue;

                foreach (var vecinoId in vecinos)
                {
                    var vecino = BuscarPersonaPorId(vecinoId);
                    if (vecino == null)
                        continue;

                    // Peso de la arista = distancia euclidiana entre personaActual y vecino
                    double dx = vecino.PosX - personaActual.PosX;
                    double dy = vecino.PosY - personaActual.PosY;
                    double peso = Math.Sqrt(dx * dx + dy * dy);

                    double nuevaDist = mejorDist + peso;

                    var (distActualVecino, _) = distancias[vecinoId];
                    if (nuevaDist < distActualVecino)
                    {
                        distancias[vecinoId] = (nuevaDist, actualId.Value);
                    }
                }
            }
            return distancias;
        }

        // Devuelve el par de familiares  que estan mas lejos uno del otro
        // Solo se consideran pares que tengan una relación en el grafo 
        public (Persona? persona1, Persona? persona2, double distancia) ObtenerParMasLejano()
        {
            // Si no hay relaciones, no hay nada que calcular
            if (Adyacencias.Count == 0)
                return (null, null, 0);
            // Variables para rastrear el mejor par encontrado
            Persona? mejor1 = null; 
            Persona? mejor2 = null;
            double maxDistancia = -1; // Distancia inicial negativa para asegurar que cualquier distancia valida la supere

            // Para evitar contrar dos veces el mismo par (A,B) y (B,A)
            var paresVisitados = new HashSet<(Guid, Guid)>();

            // Función local para "normalizar" el par de ids (ordenarlos) 
            //Se normalizan los pares para evitar duplicados
            (Guid, Guid) NormalizarPar(Guid a, Guid b)
            {
                return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
            }

            foreach (var kvp in Adyacencias) 
            {
                Guid id1 = kvp.Key;
                var vecinos = kvp.Value;
                 
                
                foreach(var id2 in vecinos) 
                {
                    if (id1 == id2)
                        continue; // Ignorar lazos a sí mismo por seguridad

                    // Normalizar el par para evitar duplicados (A,B) y (B,A)
                    var parNormalizado = NormalizarPar(id1, id2);

                    // Si ya vimos este par, lo saltamos
                    if (!paresVisitados.Add(parNormalizado))
                        continue;

                    var p1 = BuscarPersonaPorId(id1);
                    var p2 = BuscarPersonaPorId(id2);

                    if (p1 == null || p2 == null) continue;

                    // Calcular la distancia entre p1 y p2
                    double dx = p2.PosX - p1.PosX;
                    double dy = p2.PosY - p1.PosY;
                    double distancia = Math.Sqrt(dx * dx + dy * dy);

                    if (distancia > maxDistancia)
                    {
                        maxDistancia = distancia;
                        mejor1 = p1;
                        mejor2 = p2;
                    }

                }
            }
            if (mejor1 == null || mejor2 == null)
                return (null, null, 0);
            return (mejor1, mejor2, maxDistancia);
        }

        // Devuelve el par de familiares  que estan mas cerca uno del otro
        public (Persona? persona1, Persona? persona2, double distancia) ObtenerParMasCercano()
        {
            // Si no hay relaciones, no hay nada que calcular
            if (Adyacencias.Count == 0)
                return (null, null, 0);

            Persona? mejor1 = null;
            Persona? mejor2 = null;
            double minDistancia = double.MaxValue;

            // Para evitar contrar dos veces el mismo par (A,B) y (B,A)
            var paresVisitados = new HashSet<(Guid, Guid)>();

            (Guid, Guid) NormalizarPar(Guid a, Guid b) //Metodo para normalizar pares
            {
                return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
            }

            foreach (var kvp in Adyacencias)
            {
                Guid id1 = kvp.Key;
                var vecinos = kvp.Value;

                foreach (var id2 in vecinos)
                {
                    var parNormalizado = NormalizarPar(id1, id2);

                    if (!paresVisitados.Add(parNormalizado))
                        continue;

                    var p1 = BuscarPersonaPorId(id1);
                    var p2 = BuscarPersonaPorId(id2);

                    if (p1 == null || p2 == null)  
                        continue;

                    double dx = p2.PosX - p1.PosX;
                    double dy = p2.PosY - p1.PosY;
                    double distancia = Math.Sqrt(dx * dx + dy * dy);

                    if (distancia < minDistancia)
                    {
                        minDistancia = distancia;
                        mejor1 = p1;
                        mejor2 = p2;
                    }

                }
            }
            // Si nunca encontro un par valido regresa distancia 0 y nulls
            if (mejor1 == null || mejor2 == null)
                        return (null, null, 0);
            return (mejor1, mejor2, minDistancia);
        }

        public double CalcularDistanciaPromedio()
        {
            if (Adyacencias.Count ==0) // Si no hay relaciones
                return 0;

            double sumaDistancias = 0;
            int cantidadPares = 0;

            //Para evitar contrar dos veces el mismo par 
            var paresVisitados = new HashSet<(Guid, Guid)>();
            (Guid, Guid) NormalizarPar(Guid a, Guid b)
            {
                return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
            }

            foreach (var kvp in Adyacencias)
            {
                Guid id1 = kvp.Key;
                var vecinos = kvp.Value;

                foreach (var id2 in vecinos)
                {
                    if (id1 == id2)
                        continue;
                    var parNormalizado = NormalizarPar(id1, id2);

                    if (!paresVisitados.Add(parNormalizado)) //Evitar procesar un par ya visitado
                        continue;

                    var p1 = BuscarPersonaPorId(id1);
                    var p2 = BuscarPersonaPorId(id2);

                    if (p1 == null || p2 == null) continue;

                    double dx = p2.PosX - p1.PosX;
                    double dy = p2.PosY - p1.PosY;
                    double distancia = Math.Sqrt(dx * dx + dy * dy);
                    
                    sumaDistancias += distancia; // Acumular la distancia
                    cantidadPares++; // Contar el par procesado
                }

            }
            if (cantidadPares == 0)
                return 0;
            return sumaDistancias/cantidadPares; // Retornar la distancia promedio
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


