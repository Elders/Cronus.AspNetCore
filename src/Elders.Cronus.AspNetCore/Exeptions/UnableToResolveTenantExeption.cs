using System;
using System.Runtime.Serialization;

namespace Elders.Cronus.AspNetCore.Exeptions
{
    [Serializable]
    // Important: This attribute is NOT inherited from Exception, and MUST be specified 
    // otherwise serialization will fail with a SerializationException stating that
    // "Type X in Assembly Y is not marked as serializable."
    public class UnableToResolveTenantExeption : Exception
    {
        public UnableToResolveTenantExeption() { }

        public UnableToResolveTenantExeption(string message) : this(message, null) { }

        public UnableToResolveTenantExeption(string message, Exception innerException) : base(message, innerException) { }

        protected UnableToResolveTenantExeption(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
