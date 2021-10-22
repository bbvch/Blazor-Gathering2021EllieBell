using System.Runtime.Serialization;

namespace CaseB_SharedLibrary
{
    [DataContract]
    public class GloeggeliNotification
    {
        [DataMember]
        public int SenderId { get; set; }
    }
}
