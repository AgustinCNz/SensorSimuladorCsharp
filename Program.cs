class Program
{
    static void Main()
    {
        bool salir = false;

        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("=== Simulador de Sensores Inteligentes ===");
            Console.WriteLine("1. Simular sensor de CO2");
            Console.WriteLine("2. Simular temperatura y humedad");
            Console.WriteLine("3. Simular sensor con falla");
            Console.WriteLine("4. Simular sensor de lluvia");
            Console.WriteLine("5. Simular sensor de viento");
            Console.WriteLine("6. Simular sensor de presión y radiación");
            Console.WriteLine("7. Mostrar sensores funcionando");
            Console.WriteLine("8. Sensor de Humedad Extra");
            Console.WriteLine("9. Sensor de combinacion de Co2 y Temperatura");
            Console.WriteLine("10. Mostrar ranking de sensores");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            string? opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    SensorSimulator.SimulateCO2();
                    break;
                case "2":
                    SensorSimulator.SimulateTemperaturaHumedad();
                    break;
                case "3":
                    SensorSimulator.SimulateSensorConFalla();
                    break;
                case "4":
                    SensorSimulator.SimulateSensorLluvia();
                    break;
                case "5":
                    SensorSimulator.SimulateSensorViento();
                    break;
                case "6":
                    SensorSimulator.SimulateSensorPresionYRadiacion();
                    break;
                case "7":
                    SensorSimulator.MostrarSensoresFuncionando();
                    break;
                case "8":
                    SensorSimulator.SimulateHumedadExtra();
                    break;
                case "9":
                    SensorSimulator.SimulateCO2yTemperaturaConjuntos();
                    break;
                    case "10":
                    SensorSimulator.MostrarRankingSensores();
                    break;
                case "0":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

            if (!salir)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("¡Hasta luego!");
    }
}

