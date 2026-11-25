using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;

namespace SmartShop.Admin.Security;

public sealed class BoKiemTraPhienQuanTri(ILoggerFactory loggerFactory, PhienQuanTri sessions)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState state, CancellationToken token)
        => Task.FromResult(sessions.PhienHopLe(state.User));
}
