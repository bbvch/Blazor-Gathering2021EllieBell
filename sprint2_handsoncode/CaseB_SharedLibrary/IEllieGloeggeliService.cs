using ProtoBuf.Grpc;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CaseB_SharedLibrary
{
    [ServiceContract]
    public interface IEllieGloeggeliService
    {
        [OperationContract]
        IAsyncEnumerable<GloeggeliNotification> SubscribeForGloeggli(RegisterRequest request, CallContext context = default);

        [OperationContract]
        Task<EmptyResponse> UnsubscribeForGloeggli(RegisterRequest request, CallContext context);


        [OperationContract]
        Task<EmptyResponse> PublishGloeggeli(GloeggeliRequest request, CallContext context);
    }
}
