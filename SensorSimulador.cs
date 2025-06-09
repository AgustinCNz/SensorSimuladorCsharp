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
    public static void SimulateHumedadExtra()
    {
        // Crea un flujo de 10 valores secuenciales (del 1 al 10)
        var humedadExtra = Observable.Range(1, 10)
            // Por cada valor emitido, genera un objeto SensorData
            .Select(_ => new SensorData
            {
                SensorType = "Humedad Extra",
                Value = new Random().Next(40, 91), // Valor aleatorio entre 40 y 90
                Timestamp = DateTime.Now
            })
            // Imprime cada valor generado sin modificar el flujo
            .Do(data => Console.WriteLine($"[Humedad Extra] {data.Value}%"));

        // Ejecuta la suscripción para procesar y mostrar los datos
        using (humedadExtra.Subscribe())
        {
            Console.ReadKey(); // Espera a que el usuario presione una tecla antes de cerrar
        }
    }

    public static void SimulateCO2yTemperaturaConjuntos()
    {
        var random = new Random();

        // Flujo que emite un valor de CO2 aleatorio cada 2 segundos
        var co2Stream = Observable.Interval(TimeSpan.FromSeconds(2))
            .Select(_ => random.Next(300, 701)); // Valores entre 300 y 700 ppm

        // Flujo que emite un valor de temperatura aleatorio cada 2 segundos
        var tempStream = Observable.Interval(TimeSpan.FromSeconds(2))
            .Select(_ => 20 + random.NextDouble() * 15); // Temperatura entre 20 y 35°C

        // Une cada valor de CO2 con uno de Temperatura usando Zip
        var combinado = co2Stream.Zip(tempStream, (co2, temp) => new { CO2 = co2, Temperatura = temp })
            .Take(10) // Limita a 10 pares de medición
            .Do(d => Console.WriteLine($"Lectura combinada → CO2: {d.CO2} ppm | Temp: {d.Temperatura:F1} °C"))
            .Where(d => d.CO2 > 500 && d.Temperatura > 28); // Filtra solo cuando ambos valores son altos

        // Ejecuta la simulación y muestra alerta si se cumple la condición
        using (combinado.Subscribe(
            d => Console.WriteLine($"⚠️ ALERTA DOBLE → CO2: {d.CO2} ppm, Temp: {d.Temperatura:F1} °C"),
            () => Console.WriteLine("Fin de la simulación conjunta.")
        ))
        {
            Console.ReadKey(); // Espera para no cerrar la consola
        }
    }
    // Método para mostrar el ranking de sensores según sus emisiones altas VERSION 1
    // public static void MostrarRankingSensores()
    // {
    //     var random = new Random();

    //     // Definimos umbrales para cada sensor
    //     double umbralTemp = 28.0;
    //     double umbralHum = 60.0;
    //     double umbralCO2 = 500.0;

    //     // Simula 20 lecturas de temperatura entre 20 y 35
    //     var temperatura = Observable.Range(1, 20)
    //         .Select(_ => 20 + random.NextDouble() * 15);

    //     // Simula 20 lecturas de humedad entre 30 y 80
    //     var humedad = Observable.Range(1, 20)
    //         .Select(_ => 30 + random.NextDouble() * 50);

    //     // Simula 20 lecturas de CO2 entre 300 y 700
    //     var co2 = Observable.Range(1, 20)
    //         .Select(_ => random.Next(300, 701));

    //     // Cuenta cuántas veces cada sensor supera su umbral
    //     var conteoTemp = temperatura.Where(v => v > umbralTemp).Count();
    //     var conteoHum = humedad.Where(v => v > umbralHum).Count();
    //     var conteoCO2 = co2.Where(v => v > umbralCO2).Count();

    //     // Combina los conteos y genera un ranking
    //     Observable.Zip(conteoTemp, conteoHum, conteoCO2, (temp, hum, co2) =>
    //     {
    //         var resultados = new List<(string sensor, int cantidad)>
    //         {
    //                 ("Temperatura", temp),
    //                 ("Humedad", hum),
    //                 ("CO2", co2)
    //         };

    //         Console.WriteLine("\n📊 Ranking de Sensores por emisiones altas:");

    //         // Ordena de mayor a menor cantidad de alertas y las muestra
    //         foreach (var item in resultados.OrderByDescending(x => x.cantidad))
    //         {
    //             Console.WriteLine($"{item.sensor}: {item.cantidad} emisiones altas");
    //         }

    //         return true;
    //     })
    //     .Subscribe(_ =>
    //     {
    //         Console.WriteLine("\nPresione una tecla para continuar...");
    //     });

    //     Console.ReadKey(); // Espera antes de cerrar
    // }
        
        
        // Método para mostrar el ranking de sensores según sus emisiones altas VERSION 2
        public static void MostrarRankingSensores()
    {
        var random = new Random();

        // Umbrales definidos por el profesor
        double umbralTemp = 28.0;
        double umbralHum = 60.0;
        double umbralCO2 = 500.0;

        // === Lecturas de Temperatura ===
        Console.WriteLine("=== Lecturas de Temperatura ===");
        var tempLecturas = Observable.Range(1, 20) // Genera 20 emisiones (del 1 al 20)
            .Select(_ => 20 + random.NextDouble() * 15) // Valor aleatorio entre 20 y 35
            .Do(v => Console.WriteLine($"🌡️ Temperatura: {v:F1} °C")); // Imprime cada lectura

        // === Lecturas de Humedad ===
        Console.WriteLine("\n=== Lecturas de Humedad ===");
        var humLecturas = Observable.Range(1, 20)
            .Select(_ => 30 + random.NextDouble() * 50) // Valor aleatorio entre 30 y 80
            .Do(v => Console.WriteLine($"💧 Humedad: {v:F1} %")); // Imprime cada lectura

        // === Lecturas de CO2 ===
        Console.WriteLine("\n=== Lecturas de CO2 ===");
        var co2Lecturas = Observable.Range(1, 20)
            .Select(_ => random.Next(300, 701)) // Valor aleatorio entre 300 y 700 ppm
            .Do(v => Console.WriteLine($"🟣 CO2: {v} ppm")); // Imprime cada lectura

        // Cuenta cuántas lecturas de temperatura superan el umbral
        var conteoTemp = tempLecturas.Where(v => v > umbralTemp).Count();

        // Cuenta cuántas lecturas de humedad superan el umbral
        var conteoHum = humLecturas.Where(v => v > umbralHum).Count();

        // Cuenta cuántas lecturas de CO2 superan el umbral
        var conteoCO2 = co2Lecturas.Where(v => v > umbralCO2).Count();

        // Combina los tres conteos cuando todos hayan finalizado
        Observable.Zip(conteoTemp, conteoHum, conteoCO2, (temp, hum, co2) =>
        {
            // Crea una lista con los resultados de cada sensor
            var resultados = new List<(string sensor, int cantidad)>
            {
            ("Temperatura", temp),
            ("Humedad", hum),
            ("CO2", co2)
            };

            // Muestra los sensores ordenados de mayor a menor por cantidad de emisiones altas
            Console.WriteLine("\n📊 Ranking de Sensores por emisiones altas:");
            foreach (var item in resultados.OrderByDescending(x => x.cantidad))
            {
                Console.WriteLine($"{item.sensor}: {item.cantidad} emisiones altas");
            }

            return true; // Devuelve algo solo para completar el Zip (no se usa)
        })
        .Subscribe(_ =>
        {
            // Una vez que termina todo, espera una tecla
            Console.WriteLine("\nPresione una tecla para continuar...");
        });

        // Detiene la consola para que no se cierre automáticamente
        Console.ReadKey();
    }




}

    


