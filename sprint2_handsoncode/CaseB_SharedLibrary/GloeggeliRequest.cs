using System.Runtime.Serialization;

namespace CaseB_SharedLibrary
{
    [DataContract]
    public class GloeggeliRequest
    {
        [DataMember]
        public int ClientId { get; set; }
    }
}
