﻿using System.Collections.Generic;

namespace Cqrs.Services
{
    public interface IEventStore
    {
        void AppendToStream(string streamId, int expectedVersion, IEnumerable<object> events);
        IEnumerable<object> ReadAll();
        IEnumerable<object> ReadStream(string streamId);
    }
}