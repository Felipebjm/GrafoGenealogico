using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clases;

namespace PruebasUnitarias
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void DebeAgregarPersonas()
        {
            var grafo = new GrafoPersonas();
            var persona = new Persona("Juan", 1, DateTime.Now, true, "M");

            grafo.AgregarPersona(persona);

            Assert.AreEqual(1, grafo.Personas.Count);
        }
    }
}
