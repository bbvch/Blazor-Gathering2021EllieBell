using CaseB_SharedLibrary;
using ProtoBuf.Grpc;
using System.Collections.Concurrent;

namespace CaseB_GrpcService.Elligloeggeli
{
    public class EllieGloeggeliService : IEllieGloeggeliService
    {
        private static readonly ConcurrentDictionary<Subscriber, SubscriberData> SubscriberQueues = new ConcurrentDictionary<Subscriber, SubscriberData>();

        public async IAsyncEnumerable<GloeggeliNotification> SubscribeForGloeggli(RegisterRequest request, CallContext context)
        {
            var subscriber = new Subscriber(request.ClientId);
            Console.WriteLine($"{nameof(SubscribeForGloeggli)}: Request from {context.ServerCallContext?.Host} with clientId {subscriber.SubscriberId}");

            var queue = new AwaitableSubscriberQueue();
            var intenalSource = CancellationTokenSource.CreateLinkedTokenSource(context.CallOptions.CancellationToken);

            SubscriberQueues.TryAdd(subscriber, new SubscriberData(intenalSource, queue));


            CancellationToken token = intenalSource.Token;
            while (token.IsCancellationRequested == false)
            {
                Subscriber sender;
                try
                {
                    sender = await queue.DequeueAsync(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                Console.WriteLine($"{nameof(SubscribeForGloeggli)}: Notified {context.ServerCallContext?.Host} with clientId {sender.SubscriberId}");

                yield return new GloeggeliNotification { SenderId = sender.SubscriberId };
            }


            Console.WriteLine($"{nameof(SubscribeForGloeggli)}: Ended request from {context.ServerCallContext?.Host} with clientId {subscriber.SubscriberId}");
            SubscriberQueues.TryRemove(subscriber, out _);
        }

        public Task<EmptyResponse> UnsubscribeForGloeggli(RegisterRequest request, CallContext context)
        {
            var subscriber = new Subscriber(request.ClientId);
            Console.WriteLine($"{nameof(UnsubscribeForGloeggli)}: Request from {context.ServerCallContext?.Host}  with clientId {subscriber.SubscriberId}");

            var canRemove = SubscriberQueues.TryRemove(subscriber, out var data);
            if (canRemove)
            {
                data?.CancellationTokenSource.Cancel();
            };

            return Task.FromResult(new EmptyResponse());
        }

        public Task<EmptyResponse> PublishGloeggeli(GloeggeliRequest request, CallContext context)
        {
            var subscriber = new Subscriber(request.ClientId);
            Console.WriteLine($"{nameof(PublishGloeggeli)}: Request from {context.ServerCallContext?.Host}  with clientId {subscriber.SubscriberId}");

            var subscribers = SubscriberQueues.ToList();

            foreach (var (_, data) in subscribers)
            {
                data.Queue.Enqueue(subscriber);
            }


            return Task.FromResult(new EmptyResponse());
        }

        private record struct Subscriber(int SubscriberId);
        private record class SubscriberData(CancellationTokenSource CancellationTokenSource, AwaitableSubscriberQueue Queue);
        private class AwaitableSubscriberQueue
        {
            private readonly Queue<Subscriber> queue = new Queue<Subscriber>();
            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

            public void Enqueue(Subscriber subscriber)
            {
                queue.Enqueue(subscriber);
                semaphore.Release();
            }

            public async Task<Subscriber> DequeueAsync(CancellationToken cancellationToken)
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                return queue.Dequeue();
            }
        }
    }
}
