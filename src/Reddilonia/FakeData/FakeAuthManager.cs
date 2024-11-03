using System;
using Reddilonia.BusinessLogic;

namespace Reddilonia.FakeData;

public class FakeAuthManager : IAuthManager
{
    public event EventHandler<AuthSuccessEventArgs>? AuthSuccess;

    public void Start()
    {
    }

    public void OpenBrowser()
    {
    }

    public void Stop()
    {
    }

    public void Dispose()
    {
    }
}
