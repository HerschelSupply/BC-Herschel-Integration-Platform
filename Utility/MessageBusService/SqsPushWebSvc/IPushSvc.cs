using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BC.Integration.Utility.SqsPushWebSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPushSvc" in both code and config file together.
    [ServiceContract]
    public interface IPushSvc
    {
        //[OperationContract(IsOneWay = true, Action = "*")]  -When the actual service is definined include the Action = "*" directive.
        [OperationContract(IsOneWay = true, Action = "*")]
        void InitializeProcess(string guid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize);

    }

}
