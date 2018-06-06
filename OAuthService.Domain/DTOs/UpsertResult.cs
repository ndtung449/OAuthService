namespace OAuthService.Domain.DTOs
{
    public class UpsertResult
    {
        public bool IsNew { get; protected set; }

        public static UpsertResult Updated()
        {
            return new UpsertResult();
        }

        public static UpsertResult Created()
        {
            return new UpsertResult
            {
                IsNew = true
            };
        }
    }
}
