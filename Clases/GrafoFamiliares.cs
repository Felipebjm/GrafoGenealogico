using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Clases
{
    public class GrafoPersonas
    {
        // Lista de personas en el grafo
        public ObservableCollection<Persona> Personas { get; private set; } = new(); 

        // Diccionario de adyacencias (relaciones) entre personas
        public Dictionary<Guid, List<Guid>> Adyacencias { get; private set; } = new(); 

        public void AgregarPersona(Persona persona) // Agregar una persona al grafo
        {
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));
            if (Personas.Any(p => p.Cedula == persona.Cedula)) // Verificar si la persona ya existe por cedula
            {
                return;
            }

            Personas.Add(persona); // Agregar la persona al grafo
            if (!Adyacencias.ContainsKey(persona.Id)) // Verificar si ya tiene adyacencias
            {
                Adyacencias[persona.Id] = new List<Guid>(); // Inicializar la lista de adyacencias para la nueva persona
            }
        }
        //Buscar persona por id
        public Persona? BuscarPersonaPorId(Guid id) =>
            Personas.FirstOrDefault(p => p.Id == id);
        
        // Agregar relacion bidireccional entre dos personas
        public void AgregarRelacionBidireccional(Persona p1, Persona p2)
        {
            if (p1 == null || p2 == null) // Validar que las personas no sean nulas
                return;
            if (p1.Id == p2.Id)  // No permitir relaciones consigo mismo
                return;            

            AgregarRelacion(p1.Id, p2.Id); // Agregar la relacion en ambos sentidon
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
                Adyacencias[id1].Add(id2); // Agregar la relacion a la lista de adyacencias
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

        // Implementacion del algoritmo de Dijkstra para calcular distancias  desde una persona origen a los otros nodo
        // Retorna un diccionario con las distancias y el nodo previo en el camino mas corto
        // Clave:Guid, Valor: tupla(distancia acumulada, Id previo en el camino) 
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

                // Buscar el nodo con la distancia minima
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

                // Si no hay mas alcanzables se termina
                if (actualId == null)
                    break;

                visitados.Add(actualId.Value); // Marcar como visitado

                // 2. Actualizar las aristas salientes desde actualId
                // Buscar vecinos en el grafo
                if (!Adyacencias.TryGetValue(actualId.Value, out var vecinos))
                    continue;

                var personaActual = BuscarPersonaPorId(actualId.Value);
                if (personaActual == null)
                    continue;

                // Para cada vecino calcula el peso de la arista y actualiza la distancia si es mejor
                foreach (var vecinoId in vecinos)
                {
                    var vecino = BuscarPersonaPorId(vecinoId);
                    if (vecino == null)
                        continue;

                    // Peso de la arista = distancia euclidiana entre personaActual y vecino
                    double dx = vecino.PosX - personaActual.PosX;
                    double dy = vecino.PosY - personaActual.PosY;
                    double peso = Math.Sqrt(dx * dx + dy * dy);

                    double nuevaDist = mejorDist + peso; // Distancia acumulada hasta el vecino

                    var (distActualVecino, _) = distancias[vecinoId]; 
                    if (nuevaDist < distActualVecino) // Si la nueva distancia es mejor actualiza
                    {
                        distancias[vecinoId] = (nuevaDist, actualId.Value); // Actualizar distancia y previo
                    }
                }
            }
            return distancias; // Retornar el diccionario de distancias
        }

        // Devuelve el par de familiares  que estan mas lejos uno del otro
        // Solo se consideran pares que tengan una relación en el grafo 
        public (Persona? persona1, Persona? persona2) ObtenerParMasLejano()
        {
            // Si no hay relaciones no calcula nada
            if (Adyacencias.Count == 0)
                return (null, null);
            // Variables para rastrear el mejor par encontrado
            Persona? mejor1 = null; 
            Persona? mejor2 = null;
            double maxDistancia = -1; // Distancia inicial negativa para asegurar que cualquier distancia valida la supere

            // Para evitar contrar dos veces el mismo par (A,B) y (B,A)
            // HashSet es una estructura que no permite duplicados
            var paresVisitados = new HashSet<(Guid, Guid)>(); //Hash de tuplas de guids

            // Metodo para "normalizar" el par de ids (ordenarlos) 
            //Se normalizan los pares para evitar duplicados
            // El menor id (guid) siempre va primero
            (Guid, Guid) NormalizarPar(Guid a, Guid b)
            {
                return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
            }

            foreach (var kvp in Adyacencias) //kvp es un par clave-valor del diccionario
            {
                Guid id1 = kvp.Key; // Id de la persona actual
                var vecinos = kvp.Value; // Lista de ids de personas relacionadas

                foreach (var id2 in vecinos) // Se recorren los vecinos del id1
                {
                    if (id1 == id2)
                        continue; // Ignorar lazos a si 

                    // Normalizar el par para evitar duplicados (A,B) y (B,A)
                    var parNormalizado = NormalizarPar(id1, id2);

                    // Si ya sevio este lo salta
                    if (!paresVisitados.Add(parNormalizado))
                        continue;
                    // Buscar las personas correspondientes
                    var p1 = BuscarPersonaPorId(id1); 
                    var p2 = BuscarPersonaPorId(id2);

                    if (p1 == null || p2 == null) continue;

                    // Calcular la distancia entre p1 y p2 (distancia entre puntos)
                    double dx = p2.PosX - p1.PosX;
                    double dy = p2.PosY - p1.PosY;
                    double distancia = Math.Sqrt(dx * dx + dy * dy);

                    if (distancia > maxDistancia)
                    {
                        // Actualizar el mejor par encontrado y la distancia
                        maxDistancia = distancia;
                        mejor1 = p1;
                        mejor2 = p2;
                    }
                }
            }
            if (mejor1 == null || mejor2 == null) // Si nunca encontro un par valido regresa distancia 0 y nulls
                return (null, null);
            return (mejor1, mejor2); // Retornar el mejor par encontrado
        }

        // Devuelve el par de familiares  que estan mas cerca uno del otro
        // Funcionamiento similar a ObtenerParMasLejano
        public (Persona? persona1, Persona? persona2) ObtenerParMasCercano()
        {
            // Si no hay relaciones, no hay nada que calcular
            if (Adyacencias.Count == 0)
                return (null, null);

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

                    if (distancia < minDistancia) // Actualizar el mejor par encontrado
                    {
                        minDistancia = distancia;
                        mejor1 = p1;
                        mejor2 = p2;
                    }
                }
            }
            // Si nunca encontro un par valido regresa distancia 0 y nulls
            if (mejor1 == null || mejor2 == null)
                        return (null, null);
            return (mejor1, mejor2);
        }

        public double CalcularDistanciaPromedio()
        {
            if (Adyacencias.Count ==0) // Si no hay relaciones no hace nada
                return 0;

            double sumaDistancias = 0; // Suma acumulada de distancias
            int cantidadPares = 0; // Contador de pares procesados

            //Para evitar contrar dos veces el mismo par 
            var paresVisitados = new HashSet<(Guid, Guid)>();
            (Guid, Guid) NormalizarPar(Guid a, Guid b)
            {
                return a.CompareTo(b) <= 0 ? (a, b) : (b, a);
            }

            foreach (var kvp in Adyacencias) // Recorrer todos los nodos y sus vecinos
            {
                Guid id1 = kvp.Key; // Id de la persona actual
                var vecinos = kvp.Value; // Lista de ids de personas relacionadas

                foreach (var id2 in vecinos) // Recorrer los vecinos del id1
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
                    double distancia = Math.Sqrt(dx * dx + dy * dy); // Calcular la distancia entre p1 y p2

                    sumaDistancias += distancia; // Acumular la distancia
                    cantidadPares++; // Contar el par procesado
                }

            }
            if (cantidadPares == 0)
                return 0;
            return sumaDistancias/cantidadPares; // Retornar la distancia promedio
        }
    }
}


