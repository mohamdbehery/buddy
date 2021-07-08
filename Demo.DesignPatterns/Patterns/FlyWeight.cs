using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class FlyWeight
    {
        public void ConsumeThePattern()
        {
            Process proc = Process.GetCurrentProcess();

            //List<NormalCar> cars = new List<NormalCar>();
            List<FWCar> cars = new List<FWCar>();
            for (int i = 0; i < 10000000; i++)
            {
                //cars.Add(new NormalCar("Toyota")); // costs 1800 MB
                cars.Add(new FWCar("Toyota"));       // costs 680 MB
            }
            Console.WriteLine($"Normal way for creating {cars.Count} cars, cost: {proc.PrivateMemorySize64}");
            Console.WriteLine(CarObjectsFactory.carObjects.Count); // only one object
        }
    }

    public class NormalCar
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public NormalCar(string name)
        {
            this.Name = name;
            this.Description = name + " Description";
            this.Model = this.Name + this.Description;
            this.ID = DateTime.Now.GetHashCode().ToString("x"); // Extrinsic not shared
        }
    }

    public class FWCar
    {
        public string ID { get; set; }
        public ICarObjects CarObjects { get; set; }
        public FWCar(string name)
        {
            this.CarObjects = CarObjectsFactory.GetCarObjects(name);
            this.ID = DateTime.Now.GetHashCode().ToString("x"); // Extrinsic not shared
        }
    }

    public interface ICarObjects
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CarSharedObjects : ICarObjects
    {
        // moving the Intrinsic props to shared class
        public string Name { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public CarSharedObjects(string name)
        {
            this.Name = name;
            this.Description = name + " Description";
            this.Model = this.Name + this.Description;
        }
    }

    public static class CarObjectsFactory
    {
        public static Dictionary<string, CarSharedObjects> carObjects = new Dictionary<string, CarSharedObjects>();
        public static ICarObjects GetCarObjects(string name)
        {
            if (!carObjects.ContainsKey(name))
                carObjects.Add(name, new CarSharedObjects(name));
            return carObjects[name];
        }
    }
}
