using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Facade
    {
        public void ConsumeThePattern()
        {
            FRentAnApartmentFacade fRentAnApartmentFacade = new FRentAnApartmentFacade();
            fRentAnApartmentFacade.RentAHead();
        }
    }

    public class FRentAnApartmentFacade
    {
        SaveSomeMoney saveSomeMoney;
        LookForAvailableApartments lookForAvailableApartments;
        TalkToBrooker talkToBrooker;
        MakeAReview makeAReview;
        SignTheContract signTheContract;
        Relocate relocate;

        public FRentAnApartmentFacade()
        {
            saveSomeMoney = new SaveSomeMoney();
            lookForAvailableApartments = new LookForAvailableApartments();
            talkToBrooker = new TalkToBrooker();
            makeAReview = new MakeAReview();
            signTheContract = new SignTheContract();
            relocate = new Relocate();
        }
        public void RentAHead()
        {
            if (saveSomeMoney.EnoughSavedMoney())
            {
                bool found = lookForAvailableApartments.FoundOnline();
                if (!found)
                {
                    talkToBrooker.FindABroker();
                    if (makeAReview.Review())
                    {
                        signTheContract.Sign();
                        relocate.MovingMyStuff();
                    }
                }
            }
        }
    }
    public class SaveSomeMoney
    {
        public bool EnoughSavedMoney()
        {
            Console.WriteLine("I have saved 2000 Euro.");
            return true;
        }
    }
    public class LookForAvailableApartments
    {
        readonly List<Apartment> onlineApartments = new List<Apartment> { new Apartment("A"), new Apartment("B")};

        public bool FoundOnline()
        {
            Console.WriteLine("Looking online");
            foreach (var apartment in onlineApartments)
            {
                if (apartment.Address.Contains("YES"))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class TalkToBrooker
    {
        public void FindABroker()
        {
            Console.WriteLine("Hey Broker");
        }
    }
    public class MakeAReview
    {
        public bool Review()
        {
            Console.WriteLine("Review...");
            return true;
        }
    }
    public class SignTheContract
    {
        public void Sign()
        {
            Console.WriteLine("Signing contract");
        }
    }
    public class Relocate
    {
        public void MovingMyStuff()
        {
            Console.WriteLine("Moving My Stuff...");
        }
    }
    public class Apartment
    {
        public string Address { set; get; }
        public bool Convenient { set; get; }
        public Apartment(string address)
        {
            this.Address = address;
        }
    }
}
