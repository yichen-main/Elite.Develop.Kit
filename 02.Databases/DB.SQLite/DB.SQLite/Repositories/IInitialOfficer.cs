using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace SQLite.Repositories;
internal interface IInitialOfficer
{
    ValueTask RunAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialOfficer(ISQLiteHelper SQLHelper) : IInitialOfficer
{
    readonly ISQLiteHelper _SQLHelper = SQLHelper;
    public async ValueTask RunAsync()
    {
        try
        {
           await _SQLHelper.CreateTable();
        }
        catch(Exception e) 
        {

        }
    }
    public string FormatVersion(int major, int minor, int build, int revision)
    {
        DefaultInterpolatedStringHandler handler = new(literalLength: 3 /*字元數量*/, formattedCount: 4 /*變數數量*/);
        handler.AppendFormatted(major);
        handler.AppendLiteral(".");
        handler.AppendFormatted(minor);
        handler.AppendLiteral(".");
        handler.AppendFormatted(build);
        handler.AppendLiteral(".");
        handler.AppendFormatted(revision);
        return handler.ToStringAndClear();
    }
}