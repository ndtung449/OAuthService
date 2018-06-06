namespace OAuthService.Core.Services
{
    public interface ISecretGenerator
    {
        string Create();

        string Hash(string secret);
    }
}
