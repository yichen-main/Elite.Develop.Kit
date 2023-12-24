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

////�ˬd���S��sqlite�ɮסA�S���N�s�W�A�üW�[�@�����
//app.MapGet("insert", async () =>
//{
//    try
//    {
//        if (Directory.CreateDirectory(filePath).Exists)
//        {

//        }

//        //�s��sqlite��Ʈw
//        using SqliteConnection connection = new($"Data Source={dbPath}");

//        StringBuilder SQL = new();

//        //��䤣��sqlite�ɮ׮ɡA�إ߷s��A�s��Ыث�N�|����sqlite�ɮפF
//        if (!File.Exists(dbPath))
//        {
//            //�ջy�k�A�s�ئW��Student����
//            SQL.Append("CREATE TABLE Student( \n");

//            //Id���]�w�Ʀr���O��PKey�A�åB�۰ʻ��W
//            SQL.Append("Id INTEGER PRIMARY KEY AUTOINCREMENT, \n");

//            //Name���]�w��VARCHAR(32)�����\�Onull
//            SQL.Append("Name VARCHAR(32) NOT NULL, \n");

//            //Age���]�w��int
//            SQL.Append("Age INTEGER) \n");

//            //����sql�y�k
//            //await connection.ExecuteAsync(SQL.ToString());

//            await connection.ExecuteAsync("""
//            CREATE TABLE Student(
//              Id INTEGER PRIMARY KEY NOT NULL,
//              Name VARCHAR(32) NOT NULL,
//              AGE INTEGER NOT NULL,
//              BIRTHDAY VARCHAR(32) NOT NULL
//            );
//            """);

//            //�M���r�ꤺ����
//            SQL.Clear();
//        }
//        //�ջy�k
//        SQL.Append("INSERT INTO Student (Name, Age) VALUES (@Name, @Age);");

//        //�إ�SQL�ѼƤƭn�ϥΪ��ܼ�
//        DynamicParameters parameters = new();

//        //�Ѽ�1
//        parameters.Add("Name", "BillHuang");

//        //�Ѽ�2
//        parameters.Add("Age", 20);

//        //����y�k�Ainsert�@����ƨ�Student
//        var Result = await connection.ExecuteAsync(SQL.ToString(), parameters);

//        return Result; //�^�ǰ��榨�\���ƶq
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e);
//        return default;
//    }
//});

////���oStudent�Ҧ����
//app.MapGet("select", async () =>
//{
//    try
//    {
//        //�s��sqlite��Ʈw
//        using SqliteConnection connection = new($"Data Source={dbPath}");
//        StringBuilder SQL = new();

//        //�ջy�k
//        SQL.Append("SELECT * FROM Student");

//        //����A�åB�N���浲�G�s���j���O
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
