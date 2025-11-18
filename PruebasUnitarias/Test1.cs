using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clases;

namespace PruebasUnitarias
{
    [TestClass]
    public class Test1
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

    }
}
