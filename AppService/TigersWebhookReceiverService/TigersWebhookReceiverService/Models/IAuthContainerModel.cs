using System.Security.Claims;

namespace BC.Integration.AppService.TigersWebhookReceiverService.Models
{

    public interface IAuthContainerModel
    {

        #region Members
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireYears { get; set; }

        Claim[] Claims { get; set; }
        #endregion
    }
}