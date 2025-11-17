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
    }
}
