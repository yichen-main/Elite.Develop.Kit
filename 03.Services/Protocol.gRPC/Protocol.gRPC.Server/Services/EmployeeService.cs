using GrpcServer.HumanResource;

namespace gRPC.Server.Services;
public class EmployeeService : Employee.EmployeeBase
{
    public override Task<EmployeeModel> GetEmployee(EmployeeRequest request, ServerCallContext context)
    {
        EmployeeModel result = new()
        {
            Id = request.Id,
            Name = "Johnny",
            EmployeeType = EmployeeType.FirstLevel,
            PhoneNumbers =
            {
                new EmployeeModel.Types.PhoneNumber
                {
                    Value = "0912345678"
                }
            },
            ModifiedTime = new DateTime(2019, 10, 03, 17, 45, 00).Ticks
        };
        return Task.FromResult(result);
    }
    public override async Task GetEmployees(EmployeeRequest request, IServerStreamWriter<EmployeeModel> responseStream, ServerCallContext context)
    {
        List<EmployeeModel> datas = new()
        {
            new()
            {
                Id = 1,
                Name = "Johnny",
                EmployeeType = EmployeeType.FirstLevel,
                PhoneNumbers =
                {
                    new EmployeeModel.Types.PhoneNumber
                    {
                        Value = "0912345678"
                    }
                },
                ModifiedTime = new DateTime(2019, 10, 3, 17, 45, 00).Ticks
            },
            new()
            {
                Id = 2,
                Name = "Mary",
                EmployeeType = EmployeeType.SecondLevel,
                PhoneNumbers =
                {
                    new EmployeeModel.Types.PhoneNumber
                    {
                        Value = "0923456789"
                    }
                },
                ModifiedTime = new DateTime(2019, 10, 4, 9, 21, 00).Ticks
            },
            new()
            {
                Id = 3,
                Name = "Tom",
                EmployeeType = EmployeeType.LastLevel,
                PhoneNumbers =
                {
                    new EmployeeModel.Types.PhoneNumber
                    {
                        Value = "0934567890"
                    }
                },
                ModifiedTime = new DateTime(2019, 10, 5, 10, 34, 00).Ticks
            }
        };
        foreach (var data in datas)
        {
            await responseStream.WriteAsync(data);
        }
    }
    public override Task<EmployeeAddedResult> AddEmployee(EmployeeModel request, ServerCallContext context)
    {
        request.ModifiedTime = DateTime.Now.Ticks;
        return Task.FromResult(new EmployeeAddedResult { IsSuccess = true });
    }
    public override async Task<EmployeeAddedResult> AddEmployees(IAsyncStreamReader<EmployeeModel> requestStream, ServerCallContext context)
    {
        List<EmployeeModel> datas = new();
        while (await requestStream.MoveNext())
        {
            datas.Add(requestStream.Current);
            DateTime dateTime = new(requestStream.Current.ModifiedTime);
        }
        return new EmployeeAddedResult { IsSuccess = true };
    }
}