using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Corp.Integration.AppService.EpOnRampServiceBC
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOnRamp" in both code and config file together.
    [ServiceContract]
    public interface IOnRamp
    {
        //All on-ramp services that are activated by the scheduler service must expose this service contract.
        //[OperationContract(IsOneWay = true)] //Use this contract if you need to create a service reference for this endpoint.
        [OperationContract(IsOneWay = true)]
        void InitializeProcess(string guid);
    }
}
