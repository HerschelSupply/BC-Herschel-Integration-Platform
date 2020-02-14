using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BC.Integration.AppService.EpOffRampServiceTigersBC
{
    //****DO NOT**** change the class or method name as it will cause a contact mismatch when the service is called from the Push Service.
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWcfMessagingService" in both code and config file together.
    [ServiceContract]
    public interface IWcfMessagingService
    {
        [OperationContract]
        bool InitializeProcess(string message);
    }
}
