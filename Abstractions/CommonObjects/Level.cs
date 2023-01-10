namespace Abstractions.CommonObjects
{
    public class Level
    {
        public Level(decimal price, decimal quantity)
        {
            Price = price;
            Quantity = quantity;
        }

        public decimal Price { get; }

        public decimal Quantity { get; }
    }
}
