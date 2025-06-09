public abstract class Sensor
{
public string Name { get; }
protected Sensor(string name) => Name = name;
public abstract IObservable<SensorData> GetStream();
}