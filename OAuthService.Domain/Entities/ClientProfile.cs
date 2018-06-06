using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuthService.Domain.Entities
{
    public class ClientProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ClientId { get; set; }

        public string FaviconUrl { get; set; }

        public string FaviconLocalUrl { get; set; }
    }
}
