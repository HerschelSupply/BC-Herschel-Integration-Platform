﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApp.BaozunOnRamp {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BaozunOnRamp.IOnRamp")]
    public interface IOnRamp {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IOnRamp/InitializeProcess")]
        void InitializeProcess(string guid);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IOnRamp/InitializeProcess")]
        System.Threading.Tasks.Task InitializeProcessAsync(string guid);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOnRampChannel : TestApp.BaozunOnRamp.IOnRamp, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OnRampClient : System.ServiceModel.ClientBase<TestApp.BaozunOnRamp.IOnRamp>, TestApp.BaozunOnRamp.IOnRamp {
        
        public OnRampClient() {
        }
        
        public OnRampClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OnRampClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnRampClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnRampClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void InitializeProcess(string guid) {
            base.Channel.InitializeProcess(guid);
        }
        
        public System.Threading.Tasks.Task InitializeProcessAsync(string guid) {
            return base.Channel.InitializeProcessAsync(guid);
        }
    }
}
