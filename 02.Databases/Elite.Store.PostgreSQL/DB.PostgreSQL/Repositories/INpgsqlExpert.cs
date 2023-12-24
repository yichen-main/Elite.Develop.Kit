namespace PostgreSQL.Repositories;
internal interface INpgsqlExpert
{
    ValueTask RunAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class NpgsqlExpert(TableContext context) : INpgsqlExpert
{
    public async ValueTask RunAsync()
    {
        try
        {
            //查詢全部資料
            //foreach (var row in await ListAsync()) 
            //{
            //    var utcTime = row.CreateTime.ToUniversalTime();
            //}

            //分頁查詢
            {
                //var pageNo = 1; //要取得的頁面號碼
                //var pageSize = 10;  //每頁顯示的資料數量          
                //var skip = (pageNo - 1) * pageSize;  //要跳過的資料數量
                //var query = context.AtomUserInfo.OrderByDescending(item => item.CreateTime);  //時間降序排序
                //var page = await query.Skip(skip).Take(pageSize).ToArrayAsync();  //取得分頁資料
            }

            var userID = "e57011be-8ce9-46aa-8526-26e4d391027d";

            //新增資料
            {
                // await AddAsync(userID);
                context.AtomUserInfo.First(item => item.Id == Guid.Parse(userID)).Books.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Content = "EEEE",
                    CreateTime = DateTime.UtcNow
                });         
                var GG = await context.SaveChangesAsync();
            }

            //更新資料
            {
                // await UpdateAsync(userID);
            }

            //刪除資料
            {
                // await DeleteAsync(userID);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    public async ValueTask<IEnumerable<AtomUserInfo>> ListAsync()
    {
        return await context.AtomUserInfo.ToListAsync();
    }
    public async ValueTask<AtomUserInfo?> GetAsync(Guid id)
    {
        return await context.AtomUserInfo.FirstOrDefaultAsync(item => item.Id == id);
    }
    public async ValueTask AddAsync(string id)
    {
        await context.AtomUserInfo.AddAsync(new()
        {
            Id = Guid.Parse(id),
            RoleType = RoleType.Manager,
            UserName = "Test-444",
            CreateTime = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
    public async ValueTask UpdateAsync(string id)
    {
        var result = await GetAsync(Guid.Parse(id));
        if (result is not null)
        {
            result.UserName = "1234";
            await context.SaveChangesAsync();
        }
    }
    public async ValueTask DeleteAsync(string id)
    {
        var result = await GetAsync(Guid.Parse(id));
        if (result is not null)
        {
            context.AtomUserInfo.Remove(result);
            await context.SaveChangesAsync();
        }
    }
}