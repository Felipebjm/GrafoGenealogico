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


    }

    [TestClass]
    public class TestEstadisticas
    {
        // Prueba #8: una sola relación p1,p2 debe devolver ese par
        [TestMethod]
        public void ObtenerParMasLejano_UnaRelacion_RetornaEsePar()
        {
            var grafo = new GrafoPersonas();

            var p1 = new Persona("P1", 1, DateTime.Today.AddYears(-30), true, null, 0.0, 0.0);
            var p2 = new Persona("P2", 2, DateTime.Today.AddYears(-25), true, null, 3.0, 4.0); // distancia 5

            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2);

            grafo.AgregarRelacion(p1.Id, p2.Id); // Solo hay una relación

            var resultado = grafo.ObtenerParMasLejano();

            Assert.IsNotNull(resultado.persona1);
            Assert.IsNotNull(resultado.persona2);

            // El par puede presentarse en orden p1,p2 o p2,p1
            var ids = new[] { resultado.persona1!.Id, resultado.persona2!.Id };
            CollectionAssert.AreEquivalent(new[] { p1.Id, p2.Id }, ids.ToList(),
                "El par retornado debe contener p1 y p2");
        }

        //Prueba #9: Verificar que ObtenerParMasLejano devuelve el par con mayor distancia correctamente
        [TestMethod]
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

            // Act
            var (persona1, persona2) = grafo.ObtenerParMasLejano();

            // Assert
            Assert.IsNotNull(persona1, "persona1 no debe ser null");
            Assert.IsNotNull(persona2, "persona2 no debe ser null");

            // Verificar que las dos personas devueltas son A y C (comparando Cédula para mayor robustez)
            bool esParEsperado =
                (persona1!.Cedula == a.Cedula && persona2!.Cedula == c.Cedula) ||
                (persona1!.Cedula == c.Cedula && persona2!.Cedula == a.Cedula);

            Assert.IsTrue(esParEsperado,
                $"Se esperaba el par A-C pero se obtuvo {persona1?.Nombre}-{persona2?.Nombre}");
        }

        [TestMethod]
        // Prueba #10: Verificar que ObtenerParMasCercano devuelve el par con menor distancia correctamente
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

            // Act: el par más cercano debe ser A-B
            var (persona1, persona2) = grafo.ObtenerParMasCercano();

            // Assert
            Assert.IsNotNull(persona1, "persona1 no debe ser null");
            Assert.IsNotNull(persona2, "persona2 no debe ser null");

            // Verificar que las dos personas devueltas son A y B (sin importar el orden)
            bool esParEsperado =
                (persona1!.Cedula == a.Cedula && persona2!.Cedula == b.Cedula) ||
                (persona1!.Cedula == b.Cedula && persona2!.Cedula == a.Cedula);

            Assert.IsTrue(esParEsperado,
                $"Se esperaba el par A-B pero se obtuvo {persona1?.Nombre}-{persona2?.Nombre}");
        }

        [TestMethod]
        // Prueba #11: Verificar que CalcularDistanciaPromedio devuelve el promedio correcto
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
