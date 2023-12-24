namespace Decorator.Pattern_裝飾器模式_.Repositories;
internal interface ILoginProcess
{
    void Create();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class LoginProcess : ILoginProcess
{
    public void Create()
    {
        //點一杯 烏龍奶茶 加上 芋圓 和 珍珠
        OolongTea oolongTea = new();
        Taro taro = new();
        Pearl pearl = new();

        //顛倒加回去 

        //芋圓 加 烏龍茶
        taro.SetComponent(oolongTea);

        //珍珠 加 芋圓
        pearl.SetComponent(taro);


        //印出
        Console.WriteLine(pearl.Cost());
    }

    /// <summary>
    /// 奶茶
    /// </summary>
    abstract class MilkTea
    {
        /// <summary>
        /// 成本
        /// </summary>
        public abstract double Cost();
    }

    /// <summary>
    /// 裝飾器
    /// </summary>
    abstract class TeaDecorator : MilkTea
    {
        MilkTea _milkTea;

        /// <summary>
        /// 輸入組件
        /// </summary>
        public void SetComponent(MilkTea milkTea) => _milkTea = milkTea;
        public override double Cost() => _milkTea.Cost();
    }

    /// <summary>
    /// 裝飾器: 芋圓
    /// </summary>
    class Taro : TeaDecorator
    {
        readonly double price = 1;
        public override double Cost()
        {
            //父類(茶)的價格加上"芋圓"的價格
            var value = base.Cost() + price;
            Console.WriteLine($"芋圓加上茶的價格: {value}");
            return value;
        }
    }

    /// <summary>
    /// 裝飾器: 珍珠
    /// </summary>
    class Pearl : TeaDecorator
    {
        readonly double price = 2;
        public override double Cost()
        {
            //父類(茶)的價格加上"珍珠"的價格
            var value = base.Cost() + price;
            Console.WriteLine($"珍珠加上茶的價格: {value}");
            return value;
        }
    }

    /// <summary>
    /// 紅茶
    /// </summary>
    class BlackTea : MilkTea
    {
        readonly double price = 10;
        public override double Cost()
        {
            Console.WriteLine($"紅茶的價格: {price}元");
            return price;
        }
    }

    /// <summary>
    /// 烏龍茶
    /// </summary>
    class OolongTea : MilkTea
    {
        readonly double price = 15;
        public override double Cost()
        {
            Console.WriteLine($"烏龍茶的價格: {price}元");
            return price;
        }
    }
}