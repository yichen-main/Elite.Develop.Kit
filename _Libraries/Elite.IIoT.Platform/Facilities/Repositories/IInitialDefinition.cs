using Rely = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.IIoT.Platform.Facilities.Repositories;
internal interface IInitialDefinition
{
    void DefineTokenContent(in TokenValidationParameters tokenContent);
}

[Rely(ServiceLifetime.Singleton)]
file sealed class InitialDefinition : IInitialDefinition
{
    public void DefineTokenContent(in TokenValidationParameters tokenContent) => IIoTHost.TokenContent = tokenContent;
}