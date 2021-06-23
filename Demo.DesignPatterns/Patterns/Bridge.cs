using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Bridge
    {
        public void ConsumeThePattern()
        {
            PaymentBridge paymentBridge = new NbeGateway();
            paymentBridge.PaymentMethod = new CardMethod();
            paymentBridge.ProcessGateway();
        }
    }

    public interface IPaymentMethod
    {
        void InitializePaymentMethod();
    }

    public class CardMethod : IPaymentMethod
    {
        public void InitializePaymentMethod()
        {
            Console.WriteLine("Card Init");
        }
    }
    public class BuzonMethod : IPaymentMethod
    {
        public void InitializePaymentMethod()
        {
            Console.WriteLine("Buzon Init");
        }
    }

    public abstract class PaymentBridge
    {
        public abstract void ProcessGateway();
        public IPaymentMethod PaymentMethod;
    }

    public class NbeGateway: PaymentBridge
    {
        public override void ProcessGateway()
        {
            PaymentMethod.InitializePaymentMethod();
            Console.WriteLine("Process NBE");
        }
    }

    public class MigsGateway : PaymentBridge
    {
        public override void ProcessGateway()
        {
            PaymentMethod.InitializePaymentMethod();
            Console.WriteLine("Process MIGS");
        }
    }
}
