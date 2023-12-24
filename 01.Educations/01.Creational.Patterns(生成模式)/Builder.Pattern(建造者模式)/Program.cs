
//組裝戴爾電腦
DellBuilder dellBuilder = new ();
Director.InitBuild(dellBuilder);
var dellResult = dellBuilder.GetComputer();

//組裝三星電腦
Director.InitBuild(new SanxingBuilder());


/// <summary>
/// 概念: 如導演的按順序的組織建造
/// </summary>
class Director
{
    public static void InitBuild(Builder builder)
    {
        //按順序建造: 主機板 => 處理器 => 記憶體 => 螢幕
        builder.BuildMainbord().BuildCpu().BuildMemory().BuildScreen();
    }
}

/// <summary>
/// 電腦
/// </summary>
class Computer
{
    public string Cpu { get; set; } = string.Empty;
    public string Memory { get; set; } = string.Empty;
    public string Mainbord { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
}
abstract class Builder
{
    public abstract Builder BuildCpu();
    public abstract Builder BuildMemory();
    public abstract Builder BuildMainbord();
    public abstract Builder BuildScreen();
    public abstract Computer GetComputer();
}

/// <summary>
/// 戴爾電腦
/// </summary>
class DellBuilder : Builder
{
    readonly Computer _computer = new();
    public override Builder BuildCpu()
    {
        _computer.Cpu = "Dell Cpu";
        Console.WriteLine(_computer.Cpu);
        return this;
    }
    public override Builder BuildMemory()
    {
        _computer.Memory = "Dell Memory";
        Console.WriteLine(_computer.Memory);
        return this;
    }
    public override Builder BuildMainbord()
    {
        _computer.Mainbord = "Dell Mainbord";
        Console.WriteLine(_computer.Mainbord);
        return this;
    }
    public override Builder BuildScreen()
    {
        _computer.Screen = "Dell Screen";
        Console.WriteLine(_computer.Screen);
        return this;
    }
    public override Computer GetComputer() => _computer;
}

/// <summary>
/// 三星電腦
/// </summary>
class SanxingBuilder : Builder
{
    readonly Computer _computer = new();
    public override Builder BuildCpu()
    {
        _computer.Cpu = "Sanxing Cpu";
        Console.WriteLine(_computer.Cpu);
        return this;
    }
    public override Builder BuildMemory()
    {
        _computer.Memory = "Sanxing Memory";
        Console.WriteLine(_computer.Memory);
        return this;
    }
    public override Builder BuildMainbord()
    {
        _computer.Mainbord = "Sanxing Mainbord";
        Console.WriteLine(_computer.Mainbord);
        return this;
    }
    public override Builder BuildScreen()
    {
        _computer.Screen = "Sanxing Screen";
        Console.WriteLine(_computer.Screen);
        return this;
    }
    public override Computer GetComputer() => _computer;
}