using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Configuration;

namespace BC.Integration.AppService.TigersWebhookReceiverService.Models
{

    public class JWTContainerModel : IAuthContainerModel
    {
        public int ExpireYears { get; set; } = Convert.ToInt32(ConfigurationManager.AppSettings["ExpireYears"]);
        public string SecretKey { get; set; } = ConfigurationManager.AppSettings["SecretKey"];
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public Claim[] Claims { get; set; }

    }
}