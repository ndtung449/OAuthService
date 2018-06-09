namespace OAuthService.Domain
{
    public class Constants
    {
        public class Validation
        {
            public const int IdMaxLength = 256;
            public const int UriMaxLength = 1000;
            public const int NameMaxLength = 200;
            public const int UserNameMinLength = 1;
            public const int UserNameMaxLength = 200;
            public const int PasswordMaxLength = 100;
            public const int PasswordMinLength = 6;
            public const int DisplayNameMaxLength = 200;
            public const int DescriptionMaxLength = 1000;
            public const int EmailMaxLength = 256;
            public const int ClaimTypeMaxLength = 250;
            public const int ClaimValueMaxLength = 250;
        }
        
        public class ApiConfig
        {
            public const int SecretLength = 30;
            public const int ClientSecretLifeTimeInYear = 1;
            public const int ApiSecretLifeTimeInYear = 1;
        }
    }
}
