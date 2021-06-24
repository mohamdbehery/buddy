using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class AbstractFactory
    {
        public void ConsumeThePattern()
        {
            IAFCreator aFCreator = new HeroFactory();
            IAFBike aFBike = aFCreator.GetBike("Sports");
            IAFScooter aFScooter = aFCreator.GetScooter("Regular");
        }
    }
    public interface IAFCreator
    {
        IAFBike GetBike(string type);
        IAFScooter GetScooter(string type);
    }
    class HeroFactory : IAFCreator
    {
        public IAFBike GetBike(string type)
        {
            switch (type)
            {
                case "Sports":
                    return new AFBikeA();
                case "Regular":
                    return new AFBikeB();
                default:
                    throw new ApplicationException(string.Format("Vehicle '{0}' cannot be created", type));
            }
        }

        public IAFScooter GetScooter(string type)
        {
            switch (type)
            {
                case "Sports":
                    return new AFScooterA();
                case "Regular":
                    return new AFScooterB();
                default:
                    throw new ApplicationException(string.Format("Vehicle '{0}' cannot be created", type));
            }
        }
    }

    public interface IAFScooter { }

    public class AFScooterA : IAFScooter { }
    public class AFScooterB : IAFScooter { }

    public interface IAFBike { }

    public class AFBikeA : IAFBike { }
    public class AFBikeB : IAFBike { }
}
