using System;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Services
{
    public class AggregateActionProcessor : IAggregateActionProcessor
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMessageBus _messageBus;

        public AggregateActionProcessor(IAggregateRootRepository aggregateRootRepository, IMessageBus messageBus)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _messageBus = messageBus;
        }

        public void Execute<TAggregate>(Guid aggregateId, Func<TAggregate,IEnumerable<object>> action) where TAggregate : IAggregateRoot, new()
        {
            //TODO: make this configurable
            int remainingAttempts = 10;
            IEnumerable<object> events = new object[0];
            Exception saveException;
            do
            {
                try
                {
                    var wot = _aggregateRootRepository.Get<TAggregate>(aggregateId);
                    var expectedVersion = wot.Version;
                    events = action(wot).ToList();
                    _aggregateRootRepository.Save(wot.AggregateId, expectedVersion, events);
                    saveException = null;
                }
                catch (WrongExpectedVersionException ex)
                {
                    saveException = ex;
                }
            }
            while (saveException != null && --remainingAttempts > 0);

            if (saveException != null)
            {
                throw new ConcurrencyException(typeof(TAggregate).Name, aggregateId, 10, saveException);
            }

            _messageBus.Publish(events);
        }
    }
}