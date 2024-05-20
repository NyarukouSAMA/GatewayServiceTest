using Microsoft.IdentityModel.Tokens;

namespace ExtService.GateWay.API.Abstractions.Resolvers
{
    public interface ISigningKeyResolver
    {
        IEnumerable<SecurityKey> GetSigningKey(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters);
    }
}
