using System;
using System.Globalization;

class Program
{
    static readonly string[] Parentescos =
    {
        "hijo", "hija", "esposo", "esposa", "mamá", "papá", "hermano", "hermana"
    };

    static void Main()
    {
        Console.WriteLine("=== Sistema de Relaciones Familiares (Grafo con listas de adyacencia) ===\n");
        var grafo = new GrafoPersonas();

        int cantidad = LeerEntero("¿Cuántas personas desea ingresar? ");

        // --- Ingreso de personas ---
        for (int i = 0; i < cantidad; i++)
        {
            Console.WriteLine($"\nPersona #{i + 1}");
            var persona = CrearPersona();
            grafo.AgregarPersona(persona);
        }

        // --- Ingreso de relaciones ---
        Console.WriteLine("\n--- Definir relaciones ---");
        int numRelaciones = LeerEntero("¿Cuántas relaciones desea definir? ");

        for (int i = 0; i < numRelaciones; i++)
        {
            Console.WriteLine($"\nRelación #{i + 1}:");
            string n1 = LeerTexto("Nombre de la primera persona: ");
            string n2 = LeerTexto("Nombre de la segunda persona: ");

            var p1 = grafo.BuscarPersonaPorNombre(n1);
            var p2 = grafo.BuscarPersonaPorNombre(n2);

            if (p1 == null || p2 == null)
            {
                Console.WriteLine(" Una de las personas no existe. Intente de nuevo.");
                i--;
                continue;
            }

            MostrarParentescos();
            int opcion = LeerEntero("Seleccione el número del parentesco: ", 1, Parentescos.Length);

            grafo.AgregarRelacionBidireccional(p1, p2, Parentescos[opcion - 1]);
        }

        // --- Mostrar grafo final ---
        grafo.MostrarGrafo();
        Console.WriteLine("\n Grafo generado correctamente.");
    }

    // -------------------- MÉTODOS AUXILIARES --------------------

    static Persona CrearPersona()
    {
        string nombre = LeerTexto("Nombre: ");
        int cedula = LeerEntero("Cédula: ");
        DateTime fecha = LeerFecha("Fecha de nacimiento (dd/MM/yyyy): ");
        bool vivo = LeerTexto("¿Está vivo? (s/n): ").ToLower() == "s";
        string sexo = LeerSexo("Sexo (M/F): ");
        return new Persona(nombre, cedula, fecha, vivo, sexo);
    }

    static void MostrarParentescos()
    {
        Console.WriteLine("Tipos de parentesco disponibles:");
        for (int i = 0; i < Parentescos.Length; i++)
            Console.WriteLine($"{i + 1}. {Parentescos[i]}");
    }

    static int LeerEntero(string mensaje, int min = int.MinValue, int max = int.MaxValue)
    {
        int valor;
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out valor) && valor >= min && valor <= max)
                return valor;
            Console.WriteLine(" Valor inválido. Intente de nuevo.");
        }
    }

    static string LeerTexto(string mensaje)
    {
        string valor;
        do
        {
            Console.Write(mensaje);
            valor = Console.ReadLine()?.Trim() ?? "";
        } while (string.IsNullOrEmpty(valor));
        return valor;
    }

    static DateTime LeerFecha(string mensaje)
    {
        DateTime fecha;
        while (true)
        {
            Console.Write(mensaje);
            if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fecha))
                return fecha;
            Console.WriteLine(" Fecha inválida. Formato correcto: dd/MM/yyyy");
        }
    }

    static string LeerSexo(string mensaje)
    {
        string sexo;
        do
        {
            Console.Write(mensaje);
            sexo = Console.ReadLine()?.Trim().ToUpper();
        } while (sexo != "M" && sexo != "F");
        return sexo;
    }
}

