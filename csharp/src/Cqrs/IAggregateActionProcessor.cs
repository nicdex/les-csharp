using System;
using System.Collections.Generic;

namespace Cqrs
{
    public interface IAggregateActionProcessor
    {
        void Execute<TAggregate>(Guid aggregateId, Func<TAggregate,IEnumerable<object>> action) where TAggregate : IAggregateRoot, new();
    }
}