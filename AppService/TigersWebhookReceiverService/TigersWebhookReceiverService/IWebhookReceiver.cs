using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BC.Integration.AppService.TigersWebhookReceiverService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOnRamp" in both code and config file together.
    [ServiceContract]
    
    public interface IWebhookReceiver
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "InitializeProcess", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void InitializeProcess();


    }


}
