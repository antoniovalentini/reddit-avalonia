using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.FakeData;

public class FakeRedditAuthClient : IRedditAuthClient
{
    public OAuthToken CurrentOAuthToken { get; }

    public FakeRedditAuthClient()
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var token = handler.CreateEncodedJwt(
                "issuer",
                "audience",
                new ClaimsIdentity(),
                DateTime.Now,
                DateTime.Now.AddHours(1),
                DateTime.Now,
                new SigningCredentials(new SymmetricSecurityKey(Encoding.Default.GetBytes("this_is_a_fake_secret_key_for_testing_purposes_only")), SecurityAlgorithms.HmacSha256Signature));
            CurrentOAuthToken = new OAuthToken(token, "bearer", 86400, "*", "refresh-token");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            CurrentOAuthToken = new OAuthToken("", "", 86400, "*", "");
        }
    }

    public Task<OAuthToken> ExchangeCode(string code)
    {
        return Task.FromResult(CurrentOAuthToken);
    }

    public Task<OAuthToken> GetScriptAppToken()
    {
        return Task.FromResult(new OAuthToken(CurrentOAuthToken.AccessToken, "bearer", 86400, "*", "refresh-token"));
    }

    public Task<OAuthToken> RefreshToken(string refreshToken)
    {
        return Task.FromResult(CurrentOAuthToken);
    }
}
