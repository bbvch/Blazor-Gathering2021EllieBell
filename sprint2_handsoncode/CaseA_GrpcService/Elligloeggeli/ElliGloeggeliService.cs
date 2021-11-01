using Grpc.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaseA_GrpsService.Elligloeggeli
{
    public class EllieGloeggeliService : EllieGloeggeli.EllieGloeggeliBase
    {
        private static ConcurrentDictionary<Subscriber, SubscriberData> Subscribers { get; } = new ConcurrentDictionary<Subscriber, SubscriberData>();


        public override async Task SubscribeForGloeggli(RegisterRequest request, IServerStreamWriter<GloeggliNotification> responseStream, ServerCallContext context)
        {
            var subscriber = new Subscriber(request.ClientId);
            Console.WriteLine($"{nameof(SubscribeForGloeggli)}: SubscriptionRequest from {context.Peer} with clientId {subscriber.SubscriberId}");


            var queue = new AwaitableSenderQueue();
            var cts = new CancellationTokenSource();

            var subscriberRegistered = Subscribers.TryAdd(subscriber, new SubscriberData(cts, queue, request.Name));

            var token = cts.Token;
            while (subscriberRegistered && token.IsCancellationRequested == false)
            {
                var sender = await queue.DequeueAsync(token).ConfigureAwait(false);
                await responseStream.WriteAsync(new GloeggliNotification { SenderClientId = sender.SenderId, Name = sender.Name }).ConfigureAwait(false);

                Console.WriteLine($"{nameof(PublishGloeggeli)}: Notification sent to {context.Peer} with senderId {sender.SenderId}");
            }

            Console.WriteLine($"{nameof(SubscribeForGloeggli)}: Unsubscription from {context.Peer} with clientId {subscriber.SubscriberId}"); 
            Unsubscribe(subscriber);
        }

        public override async Task<EmptyMessage> PublishGloeggeli(GloeggeliRequest request, ServerCallContext context)
        {
            var subscribers = Subscribers.Values.ToList();
            Console.WriteLine($"{nameof(PublishGloeggeli)}: Request from {context.Host} with clientId {request.SenderClientId}");


            var name = Subscribers[new Subscriber(request.SenderClientId)].name;
            var message = new Sender(request.SenderClientId, name);
            foreach (var (_, data) in Subscribers.ToList())
            {
                data.Queue.Enqueue(message);
            }

            return new EmptyMessage();
        }

        public override Task<EmptyMessage> UnsubscribeForGloeggli(RegisterRequest request, ServerCallContext context)
        {
            var subscriber = new Subscriber(request.ClientId);

            Console.WriteLine($"{nameof(UnsubscribeForGloeggli)}: Request from {context.Peer} with clientId {subscriber.SubscriberId}");
            Unsubscribe(subscriber);

            return Task.FromResult(new EmptyMessage());
        }

        public override Task<RegisteredResponse> GetRegistered(EmptyMessage _, ServerCallContext context)
        {
            Console.WriteLine(nameof(GetRegistered));

            var list = Subscribers.Select(x => new RegisterRequest { ClientId= x.Key.SubscriberId, Name = x.Value.name }).ToList();
            var result = new RegisteredResponse();
            result.Registered.AddRange(list);

            return Task.FromResult(result);
        }

        private static void Unsubscribe(Subscriber subscriber)
        {
            _ = Subscribers.TryRemove(subscriber, out _);
        }

        private record struct Subscriber(int SubscriberId);
        private record struct Sender(int SenderId, string Name);
        private record class SubscriberData(CancellationTokenSource CancellationTokenSource, AwaitableSenderQueue Queue, string name);
        private class AwaitableSenderQueue
        {
            private readonly Queue<Sender> queue = new Queue<Sender>();
            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

            public void Enqueue(Sender sender)
            {
                queue.Enqueue(sender);
                semaphore.Release();
            }

            public async Task<Sender> DequeueAsync(CancellationToken cancellationToken)
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                return queue.Dequeue();
            }
        }

    }
}
