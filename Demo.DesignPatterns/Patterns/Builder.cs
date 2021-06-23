using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Builder
    {
        public void ConsumeThePattern()
        {
            Product product = new Product("Shampoo");
            product.setPicture("test").setCount(120).setFactory("Johnson");
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public int Count { get; set; }
        public string Factory { get; set; }
        public Product(string name)
        {
            this.Name = name;
        }

        public Product setPicture(string picturePath)
        {
            this.PicturePath = picturePath;
            return this;
        }
        public Product setCount(int count)
        {
            this.Count = count;
            return this;
        }
        public Product setFactory(string factory)
        {
            this.Factory = factory;
            return this;
        }
    }
}
