using System;
using System.Collections.Generic;
using System.Security.Claims;
using BC.Integration.AppService.TigersWebhookReceiverService.Models;

namespace BC.Integration.AppService.TigersWebhookReceiverService.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
