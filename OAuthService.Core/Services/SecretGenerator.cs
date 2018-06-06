using IdentityModel;
using OAuthService.Core;

namespace OAuthService.Core.Services
{
    public class SecretGenerator : ISecretGenerator
    {
        public string Create()
        {
            return CryptoRandom.CreateRandomKeyString(Constants.SecretLength);
        }

        public string Hash(string secret)
        {
            return secret.ToSha512();
        }
    }
}
