using ProtoBuf;
using System.Runtime.Serialization;

namespace CaseB_SharedLibrary
{

    [DataContract]
    public class RegisterRequest
    {
        [DataMember]
        public int ClientId { get; set; }
    }
}
