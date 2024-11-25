using System;
using Reddilonia.BusinessLogic;

namespace Reddilonia.FakeData;

public class FakeAuthManager : IAuthManager
{
    public event EventHandler<AuthSuccessEventArgs>? AuthSuccess;

    public void Start()
    {
    }

    public string GetAuthUrl()
    {
        return "https://www.reddit.com";
    }

    public void Stop()
    {
    }

    public void Dispose()
    {
    }
}
