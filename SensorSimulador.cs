using System.Reactive.Linq;
using System;
using System.Collections.Generic;

public static class SensorSimulator//Este método es public static, por lo tanto se puede llamar sin instanciar la clase.

{
    public static IObservable<SensorData> SimularTemperatura()//Devuelve un IObservable<SensorData>, lo cual significa que retorna un flujo reactivo de datos de tipo SensorData.
    {
        return Observable.Interval(TimeSpan.FromSeconds(1))//Crea un observable que emite un número secuencial (0, 1, 2, …) cada 1 segundo.
                                                           //Es como un temporizador que va generando "ticks" cada segundo.


        .Select(i => new SensorData// transforma cada número emitido por Interval en un nuevo objeto SensorData
        {
            SensorType = "Temp",//indica que es un sensor de temperatura
            Value = 20 + new Random().NextDouble() * 10,// genera un número decimal aleatorio entre 20.0 y 30.0, simulando grados Celsius
            Timestamp = DateTime.Now//guarda el momento exacto de la emisión del dato.
        })
        .Do(data => Console.WriteLine($"[Control] Emitido: {data.SensorType} = {data.Value:F2}°C"))// ejecuta una acción sin modificar el flujo
        .Take(20);//Este operador limita el número total de emisiones a 20.
    }

    public static void SimulateCO2()
    {
        Console.WriteLine("Simulación de sensor de CO2 (valores > 500 ppm)");

        // Random para generar valores entre 300 y 700
        Random random = new Random();

        // Observable que emite un valor cada 2 segundos
        var co2Sensor = Observable.Interval(TimeSpan.FromSeconds(2))
            .Select(_ => random.Next(300, 701)) // Genera número entre 300 y 700
            .Do(value => Console.WriteLine($"Lectura: {value} ppm")) // Logging de cada lectura
            .Where(value => value > 500) // Solo tomamos los que superan los 500 ppm
            .Take(10); // Solo queremos 10 alertas

        // Subscribirse para ejecutar el flujo
        using (co2Sensor.Subscribe(
            alert => Console.WriteLine($"⚠️ ALERTA CO2: {alert} ppm"),
            () => Console.WriteLine("Simulación finalizada.")
        ))
        {
            Console.WriteLine("Presione una tecla para detener...");
            Console.ReadKey();
        }
    }

    public static void SimulateTemperaturaHumedad()
    {
        var temperaturaStream = Observable.Range(1, 5)//Crea un observable que emite 5 valores (1 a 5).
                                                      //No usamos esos valores directamente, los usamos solo como "disparador" para generar lecturas aleatorias.
            .Select(_ => new SensorData //Generamos un SensorData con un valor aleatorio (Temperatura entre 5 y 25, Humedad entre 10 y 50).
            {
                SensorType = "Temperatura",
                Value = new Random().Next(5, 26), // entre 5 y 25
                Timestamp = DateTime.Now
            });

        var humedadStream = Observable.Range(1, 5)
            .Select(_ => new SensorData
            {
                SensorType = "Humedad",
                Value = new Random().Next(10, 51), // entre 10 y 50
                Timestamp = DateTime.Now
            });

        var combinado = temperaturaStream.Zip(humedadStream, (temp, hum) =>
            $"{temp.Value:F1}°C | {hum.Value:F1}%");//Combina los valores de temperatura y humedad de a pares, y genera un string con ambos.
                                                    //El operador Zip permite tomar un valor de un observable A y combinarlo con otro de B, como si los emparejara por orden de llegada.


        using (combinado.Subscribe(resultado =>
            Console.WriteLine($"[Lectura combinada] {resultado}")))//Muestra en consola los valores combinados.
        {
            Console.ReadKey();//Mantiene la consola abierta y luego corta la suscripción automáticamente.


        }
    }

    public static void SimulateSensorConFalla()
    {
        var sensorFalla = Observable
            .Repeat(new SensorData    //Crea un observable que repite indefinidamente el mismo objeto.
                                      //SensorData Define los datos del sensor dañado (tipo, valor, timestamp).
            {
                SensorType = "Sensor Dañado",
                Value = 42.0,
                Timestamp = DateTime.Now
            })
            .Take(5);//Solo toma los primeros 5 valores emitidos

        using (sensorFalla.Subscribe(data =>                        //subscribe: Se suscribe al flujo para imprimir cada valor
            Console.WriteLine($"Tipo: {data.SensorType} | Valor: {data.Value}")))
        {
            Console.ReadKey();//using ()... { Console.ReadKey(); } Muestra los datos y espera una tecla para salir, limpiamente.
        }
    }

    public static void SimulateSensorLluvia()
    {
        var lluviaStream = Observable
            .Interval(TimeSpan.FromMilliseconds(500)) //Emite ticks cada 500 ms
            .Take(200) // 200 emisiones => 100 segundos. 	Limita la duración a 100 segundos.
            .Select(_ => new SensorData  //Genera un objeto SensorData con un valor aleatorio entre 0 y 100 mm.
            {
                SensorType = "Lluvia",
                Value = new Random().NextDouble() * 100,
                Timestamp = DateTime.Now
            })
            .Do(data => Console.WriteLine($"[Lluvia] {data.Value:F2} mm"));//Muestra cada valor emitido en consola

        // Filtrar y contar los que superan 50 mm
        var conteoMayores50 = lluviaStream
            .Where(data => data.Value > 50)//Filtra solo los valores mayores a 50 mm
            .Count();//Cuenta cuántos pasaron el filtro

        // Ejecutar todo
        using (conteoMayores50.Subscribe(total =>
            Console.WriteLine($"\n🌧️  Cantidad de registros mayores a 50mm: {total}")))
        {
            Console.ReadKey();
        }
    }

    public static void SimulateSensorViento()
    {
        var random = new Random();

        var vientoStream = Observable
            .Interval(TimeSpan.FromSeconds(10)) // una lectura cada 10 segundos
            .Take(20) // Emitir durante 200 segundos → 200 / 10 = 20 mediciones.
            .Select(_ => new SensorData
            {
                SensorType = "Viento",
                Value = random.NextDouble() * 170, // valor entre 0 y 170
                Timestamp = DateTime.Now
            })
            .DistinctUntilChanged(data => data.Value) // eliminar valores repetidos
            .Where(data => data.Value >= 10) // descartar valores menores a 10
            .Do(data => Console.WriteLine($"🌬️  {data.Value:F2} km/h - {data.Timestamp}"));

        using (vientoStream.Subscribe())
        {
            Console.ReadKey();
        }
    }


    public static void SimulateSensorPresionYRadiacion()
    {
        var random = new Random();

        var radiacionSolar = Observable
            .Range(1, 10)//Genera 10 emisiones secuenciales (1 al 10).
            .Select(_ => new SensorData //Genera un objeto SensorData con un valor aleatorio.
            {
                SensorType = "Radiación Solar",
                Value = random.NextDouble() * 1200, // 0 - 1200 W/m²
                Timestamp = DateTime.Now
            });

        var presionAtmosferica = Observable
            .Range(1, 10)
            .Select(_ => new SensorData
            {
                SensorType = "Presión Atmosférica",
                Value = 950 + random.NextDouble() * 100, // 950 - 1050 hPa
                Timestamp = DateTime.Now
            });

        var combinado = radiacionSolar
            .Concat(presionAtmosferica) //Combina ambos observables, uno después del otro.
            .Do(data => Console.WriteLine($"{data.SensorType}: {data.Value:F2} - {data.Timestamp}"));//Muestra cada lectura por consola

        using (combinado.Subscribe())//Ejecuta todo el flujo.
        {
            Console.ReadKey(); // Espera para no cerrar la consola
        }

}
    public static void MostrarSensoresFuncionando()
    {
        // 1. Lista de sensores (pueden estar activos aunque tengan fallas)
        var sensores = new List<SensorData>
        {
            new SensorData { SensorType = "Temperatura", Value = 23.5, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Humedad", Value = 45.1, Timestamp = DateTime.Now },
            new SensorData { SensorType = "CO2", Value = 520, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Radiación Solar", Value = 1000, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Presión Atmosférica", Value = 1015, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Lluvia", Value = 60, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Velocidad del Viento", Value = 80, Timestamp = DateTime.Now },
            new SensorData { SensorType = "Sensor en Falla", Value = 42, Timestamp = DateTime.Now } // aún funcionando
        };

        // 2. Convertimos la lista a un observable
        var sensoresObservable = sensores.ToObservable();//Convierte la lista en un flujo reactivo.

        // 3. Seleccionamos solo los nombres de los sensores
        var nombres = sensoresObservable
            .Select(s => s.SensorType)//Extrae el nombre del sensor.
            .Distinct() // Elimina nombres repetidos.
            .Do(nombre => Console.WriteLine($"Sensor funcionando: {nombre}"));//Muestra cada nombre por consola

        // 4. Ejecutamos la secuencia
        using (nombres.Subscribe())
        {
            Console.ReadKey(); // Espera a que el usuario presione una tecla
        }
    }
}

    


