namespace Decorator.Pattern_裝飾器模式_.Repositories;
internal interface IFlavoredCoffee
{
    void Create();
}
//https://cloud.tencent.com/developer/article/1998631

[Dependency(ServiceLifetime.Singleton)]
file sealed class FlavoredCoffee : IFlavoredCoffee
{
    public void Create()
    {
        var beverage = new Espresso();
        Console.WriteLine($"{beverage.Description} $ {beverage.Cost()}");

        Beverage beverage2 = new HouseBlend();
        beverage2 = new Mocha(beverage2);
        beverage2 = new Mocha(beverage2);
        beverage2 = new Whip(beverage2);
        Console.WriteLine($"{beverage2.Description} $ {beverage2.Cost()}");
    }

    /// <summary>
    /// 飲料
    /// </summary>
    abstract class Beverage
    {
        public virtual string Description { get; protected set; } = "Unknown Beverage";
        public abstract double Cost();
    }

    /// <summary>
    /// 調味品裝飾器
    /// </summary>
    abstract class CondimentDecorator : Beverage
    {
        public abstract override string Description { get; }
    }

    /// <summary>
    /// 濃咖啡
    /// </summary>
    class Espresso : Beverage
    {
        public Espresso() => Description = "Espresso";
        public override double Cost() => 1.99;
    }

    /// <summary>
    /// 三合一咖啡
    /// </summary>
    class HouseBlend : Beverage
    {
        public HouseBlend() => Description = "HouseBlend";
        public override double Cost() => .89;
    }

    /// <summary>
    /// 摩卡
    /// </summary>
    class Mocha(Beverage beverage) : CondimentDecorator
    {
        readonly Beverage beverage = beverage;
        public override string Description => $"{beverage.Description}, Mocha";
        public override double Cost() => .20 + beverage.Cost();
    }

    /// <summary>
    /// 奶泡
    /// </summary>
    class Whip(Beverage beverage) : CondimentDecorator
    {
        readonly Beverage beverage = beverage;
        public override string Description => $"{beverage.Description}, Whip";
        public override double Cost() => .15 + beverage.Cost();
    }
}