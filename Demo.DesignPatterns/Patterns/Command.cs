using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Command
    {
        public void ConsumeThePattern()
        {
            COProduct cOProduct = new COProduct("Car");
            ProductInvoker productInvoker = new ProductInvoker(new ProductCommand(cOProduct, PriceAction.Increase, 200));
            productInvoker.Invoke();
        }
    }

    // Receiver
    public class COProduct
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public COProduct(string name)
        {
            this.Name = name;
        }

        public void IncreasePrice(int amount)
        {
            this.Price += amount; 
            Console.WriteLine($"Product ({this.Name}): price after increasing: {this.Price} $");
        }

        public void DecreasePrice(int amount)
        {
            this.Price -= amount;
            Console.WriteLine($"Product ({this.Name}): price after decreasing: {this.Price} $");
        }
    }

    public enum PriceAction
    {
        Increase = 1,
        Decrease = 2
    }

    public interface ICommand
    {
        void ExecuteCommand();
    }

    // Command
    public class ProductCommand : ICommand
    {
        private readonly COProduct _product;
        private readonly PriceAction _priceAction;
        private readonly int _amount;
        public ProductCommand(COProduct cOProduct, PriceAction priceAction, int amount)
        {
            this._product = cOProduct;
            this._priceAction = priceAction;
            this._amount = amount;
        }
        public void ExecuteCommand()
        {
            if (_priceAction == PriceAction.Increase)
                _product.IncreasePrice(_amount);
            else
                _product.DecreasePrice(_amount);
        }
    }

    // Invoker
    public class ProductInvoker
    {
        public readonly List<ICommand> _commands;
        public readonly ICommand _command;
        public ProductInvoker(ICommand command)
        {
            _commands = new List<ICommand>();
            _command = command;
        }

        public void Invoke()
        {
            _commands.Add(_command);
            _command.ExecuteCommand();
        }
    }
}
