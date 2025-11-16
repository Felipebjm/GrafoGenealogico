using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PruebasGrafo
{
    [TestClass]
    public class TestAgregarPersonas
    {
        [TestMethod]
        public void AgregarPersona_DebeAgregarNodoCorrectamente()
        {
            // Arrange
            var grafo = new GrafoPersonas();
            var p = new Persona("Juan", 123, DateTime.Now, true, "M");

            // Act
            grafo.AgregarPersona(p);

            // Assert
            Assert.AreEqual(1, grafo.Personas.Count, "El grafo debe contener 1 persona.");
            Assert.IsTrue(grafo.Adyacencias.ContainsKey(p.Id), "Debe existir una entrada en el diccionario de adyacencias.");
            Assert.AreEqual(0, grafo.Adyacencias[p.Id].Count, "La lista de adyacencias debe iniciar vacía.");
        }

        [TestMethod]
        public void AgregarPersona_NoDebeAgregarDuplicadosPorCedula()
        {
            // Arrange
            var grafo = new GrafoPersonas();
            var p1 = new Persona("Pedro", 111, DateTime.Now, true, "M");
            var p2 = new Persona("Pedro Copia", 111, DateTime.Now, true, "M"); // misma cédula

            // Act
            grafo.AgregarPersona(p1);
            grafo.AgregarPersona(p2); // debería ignorarse

            // Assert
            Assert.AreEqual(1, grafo.Personas.Count, "El grafo NO debe permitir duplicados de cédula.");
        }

        [TestMethod]
        public void BuscarPersonaPorNombre_DebeEncontrarCorrectamente()
        {
            // Arrange
            var grafo = new GrafoPersonas();
            var p = new Persona("Maria", 555, DateTime.Now, true, "F");

            // Act
            grafo.AgregarPersona(p);
            var encontrada = grafo.BuscarPersonaPorNombre("maria"); // case-insensitive

            // Assert
            Assert.IsNotNull(encontrada, "La persona debería encontrarse por nombre.");
            Assert.AreEqual(p.Id, encontrada.Id, "Debe devolver la misma persona.");
        }
    }
}
