using Microsoft.Extensions.FileProviders;
using MiniExcelLibs;

try
{
    var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//桌面路徑

    var path = Path.Combine(desktop, $"{Guid.NewGuid()}.xlsx");
    //MiniExcel.SaveAs(path, new[]
    //{
    //    new UserAccount(),
    //});



    using PhysicalFileProvider fileProvider = new("C:\\02.GitHub\\Elite.Develop.Kit\\04.Tools\\Elite.File.Parser");
    foreach (var fileInfo in fileProvider.GetDirectoryContents("Files"))
    {
        //有副檔名
        if (Path.HasExtension(fileInfo.PhysicalPath))
        {
            //取得副檔名 .csv .xlsx
            var extension = Path.GetExtension(fileInfo.PhysicalPath);
            if (extension is ".csv" || extension is ".xlsx")
            {
                var rows = await MiniExcel.QueryAsync<UserAccount>(fileInfo.PhysicalPath);
                var contents = rows.ToArray();
            }
        }
    }
    Console.ReadLine();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
class UserAccount
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "dddd";
    public DateTime BoD { get; set; } = DateTime.Now;
    public int Age { get; set; }
    public bool VIP { get; set; }
}