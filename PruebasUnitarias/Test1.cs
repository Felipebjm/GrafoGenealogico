using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clases;

namespace PruebasUnitarias
{
    [TestClass]
    public class TestGrafoyPersonas
    {
        // Prueba #1 : Verificar que se puedan agregar personas al grafo
        [TestMethod]
        public void DebeAgregarPersonas()
        {
            var grafo = new GrafoPersonas();
            var persona = new Persona("Juan", 1, DateTime.Now, true, "M");

            grafo.AgregarPersona(persona);

            Assert.AreEqual(1, grafo.Personas.Count);
        }

        // Prueba #2 : Verificar que se puedan agregar personas al grafo (Prueba duplicada intencionalmente NO debe funcionar)
        [TestMethod]
        public void DebeAgregarPersonasMALO()
        {
            var grafo = new GrafoPersonas();
            var persona = new Persona("Juan", 1, DateTime.Now, true, "M");

            grafo.AgregarPersona(persona);

            Assert.AreEqual(2, grafo.Personas.Count);
        }

        // Prueba #3 : Verificar que AgregarRelacionBidireccional cree la relación en ambos sentidos
        [TestMethod]
        public void AgregarRelacionBidireccional_CreaRelacionesEnAmbosSentidos()
        {
            var grafo = new GrafoPersonas();
            // Crear dos personas
            var p1 = new Persona("Juan", 1, DateTime.Today.AddYears(-30), true);
            var p2 = new Persona("Ana", 2, DateTime.Today.AddYears(-25), true);

            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2);

            grafo.AgregarRelacionBidireccional(p1, p2);

            // p1 tiene a p2 y p2 tiene a p1
            var vecinosP1 = grafo.ObtenerPersonasRelacionadas(p1).ToList();
            var vecinosP2 = grafo.ObtenerPersonasRelacionadas(p2).ToList();

            Assert.AreEqual(1, vecinosP1.Count, "p1 debe tener exactamente 1 vecino");
            Assert.AreEqual(1, vecinosP2.Count, "p2 debe tener exactamente 1 vecino");
            Assert.AreEqual(p2.Id, vecinosP1[0].Id, "El vecino de p1 debe ser p2");
            Assert.AreEqual(p1.Id, vecinosP2[0].Id, "El vecino de p2 debe ser p1");
        }

        // Prueba #4 : Verificar que no se permita relacion consigo mismo
        [TestMethod]
        public void AgregarRelacionBidireccional_NoPermiteAutoRelacion()
        {
            var grafo = new GrafoPersonas();
            var p = new Persona("Sofaa", 5, DateTime.Today.AddYears(-20), true);

            grafo.AgregarPersona(p);
            grafo.AgregarRelacionBidireccional(p, p);

            var vecinos = grafo.ObtenerPersonasRelacionadas(p).ToList();
            Assert.AreEqual(0, vecinos.Count, "Una persona no debe aparecer como su propio vecino");
        }

        // Prueba #5: Si la persona es null, debe devolver secuencia vacía
        [TestMethod]
        public void ObtenerPersonasRelacionadas_PersonaNull_RetornaVacio()
        {
            var grafo = new GrafoPersonas();

            var resultado = grafo.ObtenerPersonasRelacionadas(null).ToList();

            Assert.AreEqual(0, resultado.Count, "Si se pasa null la secuencia debe estar vacía");
        }

        // Prueba #6: Persona con vecinos devuelve las personas relacionadas en el orden de la lista de adyacencias
        [TestMethod]
        public void ObtenerPersonasRelacionadas_PersonaConVecinos_RetornaVecinosCorrectos()
        {
            var grafo = new GrafoPersonas();
            // Crear personas
            var p1 = new Persona("A", 11, DateTime.Today.AddYears(-40), true);
            var p2 = new Persona("B", 12, DateTime.Today.AddYears(-35), true);
            var p3 = new Persona("C", 13, DateTime.Today.AddYears(-20), true);
            // Agregar personas al grafo
            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2);
            grafo.AgregarPersona(p3);

            // Agregar relaciones dirigidas p3
            grafo.AgregarRelacion(p1.Id, p2.Id);
            grafo.AgregarRelacion(p1.Id, p3.Id);

            var vecinos = grafo.ObtenerPersonasRelacionadas(p1).ToList();

            Assert.AreEqual(2, vecinos.Count, "p1 debe tener 2 vecinos");
            // Verificar que contenga p2 y p3 (y preservar orden si así lo espera la implementación)
            Assert.AreEqual(p2.Id, vecinos[0].Id, "Primer vecino debe ser p2");
            Assert.AreEqual(p3.Id, vecinos[1].Id, "Segundo vecino debe ser p3");
        }

        // Prueba #7: Si la lista de adyacencias contiene ids que no estan en Personas, esos ids se ignoran
        [TestMethod]
        public void ObtenerPersonasRelacionadas_VecinosNoRegistrados_SeIgnoran()
        {
            var grafo = new GrafoPersonas();
            // Crear personas
            var p1 = new Persona("D", 14, DateTime.Today.AddYears(-50), true);
            var p2 = new Persona("E", 15, DateTime.Today.AddYears(-45), true);

            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2);

            // Agregar un id inexistente manualmente (simula inconsistencia)
            var idInexistente = Guid.NewGuid();
            grafo.AgregarRelacion(p1.Id, p2.Id);
            grafo.AgregarRelacion(p1.Id, idInexistente);

            var vecinos = grafo.ObtenerPersonasRelacionadas(p1).ToList();

            // Debe devolver solo p2 (ignorar el id que no corresponde a ninguna Persona)
            Assert.AreEqual(1, vecinos.Count, "Debe ignorar ids que no correspondan a ninguna Persona");
            Assert.AreEqual(p2.Id, vecinos[0].Id, "El único vecino válido debe ser p2");
        }


        // Prueba #8: calcula correctamente la distancia euclidiana entre origen y vecinos
        [TestMethod]
        public void CalcularDistanciasConVecinos_CalculaDistanciasCorrectas()
        {
            var grafo = new GrafoPersonas();

            // origen en (1,2)
            var origen = new Persona("O", 3001, DateTime.Today.AddYears(-30), true, null, 1.0, 2.0);
            // v1 en (4,6) -> dx=3, dy=4 -> distancia=5
            var v1 = new Persona("V1", 3002, DateTime.Today.AddYears(-20), true, null, 4.0, 6.0);
            // v2 en (1,5) -> dx=0, dy=3 -> distancia=3
            var v2 = new Persona("V2", 3003, DateTime.Today.AddYears(-22), true, null, 1.0, 5.0);

            grafo.AgregarPersona(origen);
            grafo.AgregarPersona(v1);
            grafo.AgregarPersona(v2);

            // mantener orden: primero v1 luego v2
            grafo.AgregarRelacion(origen.Id, v1.Id);
            grafo.AgregarRelacion(origen.Id, v2.Id);

            var resultado = grafo.CalcularDistanciasConVecinos(origen).ToList();

            Assert.AreEqual(2, resultado.Count, "Debe devolver tantas entradas como vecinos válidos");

            var tol = 1e-9;

            Assert.AreEqual(v1.Id, resultado[0].persona.Id, "Primer vecino debe ser v1");
            Assert.IsTrue(Math.Abs(resultado[0].distancia - 5.0) < tol, "Distancia a v1 debe ser 5.0");

            Assert.AreEqual(v2.Id, resultado[1].persona.Id, "Segundo vecino debe ser v2");
            Assert.IsTrue(Math.Abs(resultado[1].distancia - 3.0) < tol, "Distancia a v2 debe ser 3.0");
        }


        // Prueba #9: persona sin adyacencias (relaciones) devuelve lista vacía
        [TestMethod]
        public void CalcularDistanciasConVecinos_SinVecinos_RetornaListaVacia()
        {
            var grafo = new GrafoPersonas();
            var origen = new Persona("Or", 1001, DateTime.Today.AddYears(-30), true, null, 0.0, 0.0);

            grafo.AgregarPersona(origen);

            var resultado = grafo.CalcularDistanciasConVecinos(origen);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count, "Debe devolver lista vacía si no hay vecinos");
        }


    }

    [TestClass]
    public class TestEstadisticas
    {
        // Prueba #10: una sola relacion p1,p2 debe devolver ese par y su distancia
        [TestMethod]
        public void ObtenerParMasLejano_UnaRelacion_RetornaEsePar()
        {
            var grafo = new GrafoPersonas();

            var p1 = new Persona("P1", 1, DateTime.Today.AddYears(-30), true, null, 0.0, 0.0);
            var p2 = new Persona("P2", 2, DateTime.Today.AddYears(-25), true, null, 3.0, 4.0); // distancia 5

            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2);

            grafo.AgregarRelacion(p1.Id, p2.Id); // Solo hay una relacion

            var resultado = grafo.ObtenerParMasLejano();

            Assert.IsNotNull(resultado.persona1);
            Assert.IsNotNull(resultado.persona2);
            // El par puede presentarse en orden p1,p2 o p2,p1 
            var ids = new[] { resultado.persona1.Id, resultado.persona2.Id };
            CollectionAssert.AreEquivalent(new[] { p1.Id, p2.Id }, ids.ToList(), "El par retornado debe contener p1 y p2");
            Assert.AreEqual(5.0, resultado.distancia, 1e-9, "La distancia debe ser 5.0 (3-4-5)");
        }
        [TestMethod]

        //Prueba #11: Verificar que ObtenerParMasLejano devuelve el par con mayor distancia correctamente
        public void ObtenerParMasLejano_DebeRetornarParConMayorDistancia()
        {
            // Arrange
            var grafo = new GrafoPersonas();

            // Crear tres personas usando el constructor real
            var a = new Persona("A", 1, DateTime.Now.AddYears(-30), true, null, 0, 0);      
            var b = new Persona("B", 2, DateTime.Now.AddYears(-25), true, null, 3, 4);      // (3,4) distancia a-A = 5
            var c = new Persona("C", 3, DateTime.Now.AddYears(-20), true, null, 10, 0);     // (10,0) distancia a-A = 10

            // Agregar personas al grafo
            grafo.AgregarPersona(a);
            grafo.AgregarPersona(b);
            grafo.AgregarPersona(c);

            grafo.AgregarRelacionBidireccional(a, b);
            grafo.AgregarRelacionBidireccional(a, c);

            // par mas lejano es A,C con distancia 10
            double distanciaEsperada = Math.Sqrt((c.PosX - a.PosX) * (c.PosX - a.PosX) + (c.PosY - a.PosY) * (c.PosY - a.PosY));

            var (persona1, persona2, distancia) = grafo.ObtenerParMasLejano();

            // Assert
            Assert.IsNotNull(persona1, "persona1 no debe ser null");
            Assert.IsNotNull(persona2, "persona2 no debe ser null");

            // Verificar que las dos personas devueltas son A y C (comparando Cedula para mayor robustez)
            bool esParEsperado =
                (persona1.Cedula == a.Cedula && persona2.Cedula == c.Cedula) ||
                (persona1.Cedula == c.Cedula && persona2.Cedula == a.Cedula);

            Assert.IsTrue(esParEsperado, $"Se esperaba el par A-C pero se obtuvo {persona1?.Nombre}-{persona2?.Nombre}");

            // Verificar distancia con tolerancia
            double tolerancia = 1e-9;
            Assert.IsTrue(Math.Abs(distancia - distanciaEsperada) <= tolerancia,
                $"Distancia esperada {distanciaEsperada} pero se obtuvo {distancia}");
        }

        [TestMethod]
        // Prueba #12: Verificar que ObtenerParMasCercano devuelve el par con menor distancia correctamente
        public void ObtenerParMasCercano_DebeRetornarParConMenorDistancia()
        {
            // Arrange
            var grafo = new GrafoPersonas();

            // Crear tres personas con posiciones conocidas
            var a = new Persona("A", 1, DateTime.Now.AddYears(-30), true, null, 0, 0);
            var b = new Persona("B", 2, DateTime.Now.AddYears(-25), true, null, 1, 1);
            var c = new Persona("C", 3, DateTime.Now.AddYears(-20), true, null, 10, 0);

            // Agregar personas al grafo
            grafo.AgregarPersona(a);
            grafo.AgregarPersona(b);
            grafo.AgregarPersona(c);

            grafo.AgregarRelacionBidireccional(a, b);
            grafo.AgregarRelacionBidireccional(a, c);

            // par más cercano es a,b con distancia 
            double distanciaEsperada = Math.Sqrt((b.PosX - a.PosX) * (b.PosX - a.PosX) +
                                                 (b.PosY - a.PosY) * (b.PosY - a.PosY));

            var (persona1, persona2, distancia) = grafo.ObtenerParMasCercano();

            Assert.IsNotNull(persona1, "persona1 no debe ser null");
            Assert.IsNotNull(persona2, "persona2 no debe ser null");

            // Verificar que las dos personas devueltas son a,b
            bool esParEsperado =
                (persona1.Cedula == a.Cedula && persona2.Cedula == b.Cedula) ||
                (persona1.Cedula == b.Cedula && persona2.Cedula == a.Cedula);

            Assert.IsTrue(esParEsperado, $"Se esperaba el par A-B pero se obtuvo {persona1?.Nombre}-{persona2?.Nombre}");

            // Verificar distancia con tolerancia
            double tolerancia = 1e-9;
            Assert.IsTrue(Math.Abs(distancia - distanciaEsperada) <= tolerancia,
                $"Distancia esperada {distanciaEsperada} pero se obtuvo {distancia}");
        }

        [TestMethod]
        // Prueba #13: Verificar que CalcularDistanciaPromedio devuelve el promedio correcto
        public void CalcularDistanciaPromedio_DebeRetornarPromedioCorrecto()
        {
            // Arrange
            var grafo = new GrafoPersonas();

            // Crear tres personas con posiciones conocidas
            var a = new Persona("A", 1, DateTime.Now.AddYears(-30), true, null, 0, 0);   // (0,0)
            var b = new Persona("B", 2, DateTime.Now.AddYears(-25), true, null, 3, 4);   // (3,4) distancia A-B = 5
            var c = new Persona("C", 3, DateTime.Now.AddYears(-20), true, null, 6, 8);   // (6,8) distancia A-C = 10, B-C = 5

            grafo.AgregarPersona(a);
            grafo.AgregarPersona(b);
            grafo.AgregarPersona(c);

            // Establecer relaciones bidireccionales
            grafo.AgregarRelacionBidireccional(a, b);
            grafo.AgregarRelacionBidireccional(a, c);
            grafo.AgregarRelacionBidireccional(b, c);

            // Expected: promedio de distancias (5 + 10 + 5) / 3 = 20 / 3 ≈ 6.666
            double distanciaEsperada = (5 + 10 + 5) / 3.0;

            double distanciaPromedio = grafo.CalcularDistanciaPromedio();

            double tolerancia = 1e-9;
            Assert.IsTrue(Math.Abs(distanciaPromedio - distanciaEsperada) <= tolerancia,
                $"Distancia promedio esperada {distanciaEsperada} pero se obtuvo {distanciaPromedio}");
        }






    }




}
