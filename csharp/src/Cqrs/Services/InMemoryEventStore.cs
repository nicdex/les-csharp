using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Services
{
    public class InMemoryEventStore : IEventStore
    {
        public void AppendToStream(string streamId, int expectedVersion, IEnumerable<object> events)
        {
            var lockObject = GetOrCreateLockObject(streamId);
            lock (lockObject)
            {
                var version = _eventStore.Count(x => x.StreamId == streamId);
                if (version != expectedVersion)
                    throw new WrongExpectedVersionException(streamId, expectedVersion, version);

                foreach (var @event in events)
                {
                    _eventStore.Enqueue(new EventData {StreamId = streamId, Event = @event});
                }
            }
        }

        public IEnumerable<object> ReadAll()
        {
            return _eventStore.Select(x => x.Event);
        }

        public IEnumerable<object> ReadStream(string streamId)
        {
            return _eventStore.Where(x => x.StreamId == streamId).Select(x => x.Event);
        }

        private readonly ConcurrentQueue<EventData> _eventStore;
        private static readonly ConcurrentQueue<EventData> EventStore = new ConcurrentQueue<EventData>();

        public InMemoryEventStore() : this(EventStore)
        {
        }

        private InMemoryEventStore(ConcurrentQueue<EventData> eventStore)
        {
            _eventStore = eventStore;
        }

        public class EventData
        {
            public string StreamId { get; set; }
            public object Event { get; set; }
        }

        private static readonly ConcurrentDictionary<string, object> LockObjects = new ConcurrentDictionary<string, object>();

        private static object GetOrCreateLockObject(string id)
        {
            object lockObject;
            while (!LockObjects.TryGetValue(id, out lockObject))
            {
                lockObject = new Object();
                if (LockObjects.TryAdd(id, lockObject)) break;
            }
            return lockObject;
        }
    }
}