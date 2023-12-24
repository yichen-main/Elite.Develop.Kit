try
{
    using var application = AbpApplicationFactory.Create<AppModule>();
    await application.InitializeAsync();

    var service = application.ServiceProvider.GetService<IInitialOfficer>();
    if (service is not null) await service.RunAsync();

    Console.WriteLine("Press ENTER to stop application...");
    Console.ReadLine();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

var filePath = ".\\LiteDB";
var dbPath = $"{filePath}\\machines.db";

//https://www.ruyut.com/2021/12/sqlite-crud.html

////檢查有沒有sqlite檔案，沒有就新增，並增加一筆資料
//app.MapGet("insert", async () =>
//{
//    try
//    {
//        if (Directory.CreateDirectory(filePath).Exists)
//        {

//        }

//        //連接sqlite資料庫
//        using SqliteConnection connection = new($"Data Source={dbPath}");

//        StringBuilder SQL = new();

//        //當找不到sqlite檔案時，建立新表，新表創建後就會產生sqlite檔案了
//        if (!File.Exists(dbPath))
//        {
//            //組語法，新建名為Student的表
//            SQL.Append("CREATE TABLE Student( \n");

//            //Id欄位設定數字型別為PKey，並且自動遞增
//            SQL.Append("Id INTEGER PRIMARY KEY AUTOINCREMENT, \n");

//            //Name欄位設定為VARCHAR(32)不允許是null
//            SQL.Append("Name VARCHAR(32) NOT NULL, \n");

//            //Age欄位設定為int
//            SQL.Append("Age INTEGER) \n");

//            //執行sql語法
//            //await connection.ExecuteAsync(SQL.ToString());

//            await connection.ExecuteAsync("""
//            CREATE TABLE Student(
//              Id INTEGER PRIMARY KEY NOT NULL,
//              Name VARCHAR(32) NOT NULL,
//              AGE INTEGER NOT NULL,
//              BIRTHDAY VARCHAR(32) NOT NULL
//            );
//            """);

//            //清除字串內的值
//            SQL.Clear();
//        }
//        //組語法
//        SQL.Append("INSERT INTO Student (Name, Age) VALUES (@Name, @Age);");

//        //建立SQL參數化要使用的變數
//        DynamicParameters parameters = new();

//        //參數1
//        parameters.Add("Name", "BillHuang");

//        //參數2
//        parameters.Add("Age", 20);

//        //執行語法，insert一筆資料到Student
//        var Result = await connection.ExecuteAsync(SQL.ToString(), parameters);

//        return Result; //回傳執行成功的數量
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e);
//        return default;
//    }
//});

////取得Student所有資料
//app.MapGet("select", async () =>
//{
//    try
//    {
//        //連接sqlite資料庫
//        using SqliteConnection connection = new($"Data Source={dbPath}");
//        StringBuilder SQL = new();

//        //組語法
//        SQL.Append("SELECT * FROM Student");

//        //執行，並且將執行結果存為強型別
//        var result = await connection.QueryAsync<Student>(SQL.ToString());
//        return result;
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e);
//        return default;
//    }
//});
//app.Run();
