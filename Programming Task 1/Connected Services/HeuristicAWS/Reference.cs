﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllocationsApplication.HeuristicAWS {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HeuristicAWS.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Heuristic", ReplyAction="http://tempuri.org/IService/HeuristicResponse")]
        WcfsServiceLibrary.AllocationsData Heuristic(int deadline, WcfsServiceLibrary.ConfigData cd);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IService/Heuristic", ReplyAction="http://tempuri.org/IService/HeuristicResponse")]
        System.IAsyncResult BeginHeuristic(int deadline, WcfsServiceLibrary.ConfigData cd, System.AsyncCallback callback, object asyncState);
        
        WcfsServiceLibrary.AllocationsData EndHeuristic(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : AllocationsApplication.HeuristicAWS.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class HeuristicCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public HeuristicCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public WcfsServiceLibrary.AllocationsData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((WcfsServiceLibrary.AllocationsData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<AllocationsApplication.HeuristicAWS.IService>, AllocationsApplication.HeuristicAWS.IService {
        
        private BeginOperationDelegate onBeginHeuristicDelegate;
        
        private EndOperationDelegate onEndHeuristicDelegate;
        
        private System.Threading.SendOrPostCallback onHeuristicCompletedDelegate;
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<HeuristicCompletedEventArgs> HeuristicCompleted;
        
        public WcfsServiceLibrary.AllocationsData Heuristic(int deadline, WcfsServiceLibrary.ConfigData cd) {
            return base.Channel.Heuristic(deadline, cd);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginHeuristic(int deadline, WcfsServiceLibrary.ConfigData cd, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginHeuristic(deadline, cd, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public WcfsServiceLibrary.AllocationsData EndHeuristic(System.IAsyncResult result) {
            return base.Channel.EndHeuristic(result);
        }
        
        private System.IAsyncResult OnBeginHeuristic(object[] inValues, System.AsyncCallback callback, object asyncState) {
            int deadline = ((int)(inValues[0]));
            WcfsServiceLibrary.ConfigData cd = ((WcfsServiceLibrary.ConfigData)(inValues[1]));
            return this.BeginHeuristic(deadline, cd, callback, asyncState);
        }
        
        private object[] OnEndHeuristic(System.IAsyncResult result) {
            WcfsServiceLibrary.AllocationsData retVal = this.EndHeuristic(result);
            return new object[] {
                    retVal};
        }
        
        private void OnHeuristicCompleted(object state) {
            if ((this.HeuristicCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.HeuristicCompleted(this, new HeuristicCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void HeuristicAsync(int deadline, WcfsServiceLibrary.ConfigData cd) {
            this.HeuristicAsync(deadline, cd, null);
        }
        
        public void HeuristicAsync(int deadline, WcfsServiceLibrary.ConfigData cd, object userState) {
            if ((this.onBeginHeuristicDelegate == null)) {
                this.onBeginHeuristicDelegate = new BeginOperationDelegate(this.OnBeginHeuristic);
            }
            if ((this.onEndHeuristicDelegate == null)) {
                this.onEndHeuristicDelegate = new EndOperationDelegate(this.OnEndHeuristic);
            }
            if ((this.onHeuristicCompletedDelegate == null)) {
                this.onHeuristicCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnHeuristicCompleted);
            }
            base.InvokeAsync(this.onBeginHeuristicDelegate, new object[] {
                        deadline,
                        cd}, this.onEndHeuristicDelegate, this.onHeuristicCompletedDelegate, userState);
        }
    }
}
