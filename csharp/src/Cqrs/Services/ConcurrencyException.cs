using System;

namespace Cqrs.Services
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string aggregateName, Guid aggregateId, int attempts, Exception innerException)
            : base(
                string.Format("Unable to process action on {0} with id {1} after {1} attempts.", aggregateName,
                    aggregateId, attempts), innerException)
        {
        }
    }
}