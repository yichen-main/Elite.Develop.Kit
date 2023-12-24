try
{//https://www.bilibili.com/video/BV1u94y1x7RQ/?spm_id_from=333.880.my_history.page.click
    //https://mp.weixin.qq.com/s/SmWyeSdwJqgaB8v06cMJYg

    ServiceCollection serviceCollection = new();
    serviceCollection.AddHttpClient();
    serviceCollection.AddHttpClient("testClient", (client) =>
    {
        client.BaseAddress = new("http://localhost:5000");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });
    serviceCollection.AddHttpClient<LinliuSercice>((client) =>
    {
        client.BaseAddress = new("http://localhost:5000");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    serviceCollection.AddSingleton<TestSercice>();

    var service = serviceCollection.BuildServiceProvider();
    var httpClientFactory = service.GetRequiredService<IHttpClientFactory>();

    var httpClient = httpClientFactory.CreateClient();
    var result = await httpClient.GetStringAsync("https://www.baidu.com");

    var testService = service.GetRequiredService<TestSercice>();
    var testResult = await testService.GetStringAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

class TestSercice(IHttpClientFactory clientFactory)
{
    readonly HttpClient _client = clientFactory.CreateClient("testClient");
    public async Task<string> GetStringAsync()
    {
        using var response = await _client.GetAsync("api/Jobs");
        ArgumentNullException.ThrowIfNull(response.Content);

        //.Net 8
        // var stream = response.Content.ReadFromJsonAsAsyncEnumerable<Job>();
        // var stream = httpClient.GetFromJsonAsAsyncEnumerable<Job>("api/Jobs");
        //await foreach (var job in stream)
        //{
        //    Console.WriteLine(job);
        //    Console.WriteLine(DateTimeOffset.Now);
        //}

        return await _client.GetStringAsync("/test");
    }
}
class LinliuSercice(IHttpClientFactory clientFactory)
{
    readonly HttpClient _client = clientFactory.CreateClient("testClient");
    public async Task<string> GetStringAsync()
    {
        return await _client.GetStringAsync("/test");
    }
}
sealed class Job
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public override string ToString() => $"JobId: {Id}, JobTitle: {Title}";
}