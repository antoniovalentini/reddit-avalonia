using System.Threading.Tasks;
using Reddilonia.BusinessLogic;
using Reddit.Client.Dtos;

namespace Reddilonia.FakeData;

public class FakeAuthTokenStorage : IAuthTokenStorage
{
    public OAuthToken? Load()
    {
        return new OAuthToken("FakeToken", "FakeToken", 100000, "scope", "fake-refresh-token");
    }

    public Task StoreToken(OAuthToken token)
    {
        return Task.CompletedTask;
    }
}
