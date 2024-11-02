using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos;

public record OAuthToken(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("refresh_token")] string RefreshToken
)
{
    [JsonIgnore]
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(AccessToken) &&
        !string.IsNullOrWhiteSpace(RefreshToken) &&
        new JwtSecurityTokenHandler().ReadToken(AccessToken) is JwtSecurityToken token &&
        token.ValidTo.ToUniversalTime() > DateTime.UtcNow;
}
