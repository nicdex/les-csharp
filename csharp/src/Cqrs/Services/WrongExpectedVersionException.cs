using System;

namespace Cqrs.Services
{
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string streamId, int expectedVersion, int version)
            : base(string.Format("Stream {0} expected version {1} but was {2}.", streamId, expectedVersion, version))
        {
        }
    }
}