using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfsLibrary
{
   [DataContract]

   public class TimeoutFault
    {
        [DataMember]
        public string Message { get; set; }

        public TimeoutFault(string message)
        {
            Message = message;
        }
    }

    [DataContract]

    public class ConfigData
    {
        [DataMember]
        public Double Duration { get; set; }

        [DataMember]
        public int NumTasks { get; set; }

        [DataMember]
        public int NumProcs { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public int[] ProcsRam { get; set; }

        [DataMember]
        public int[] TaskRam { get; set; }

        [DataMember]
        public double[] TaskRuntime { get; set; }

        [DataMember]
        public int[] TaskDownloadSpeed { get; set; }

        [DataMember]
        public int[] ProcessorDownloadSpeed { get; set; }

        [DataMember]
        public int[] TaskUploadSpeed { get; set; }

        [DataMember]
        public int[] ProcessorUploadSpeed { get; set; }

        [DataMember]
        public double[] Runtimes{ get; set; }

        [DataMember]
        public double[] Energies { get; set; }

        [DataMember]
        public double[] LocalCommunication { get; set; }

        [DataMember]
        public double[] RemoteCommunication { get; set; }

        [DataMember]
        public double[] ProcsFreq { get; set; }

        [DataMember]
        public double[] TaskFreq { get; set; }

    }

    [DataContract]

    public class AllocationsData
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Double Energy { get; set; }

        [DataMember]
        public int Count { get; set; }

    }
}
