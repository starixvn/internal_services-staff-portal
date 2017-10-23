namespace S22.Mail
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    #pragma warning disable 1591
    public sealed class SerializableLinkedResourceCollection : Collection<SerializableLinkedResource>, IDisposable
    {
        public void Dispose()
        {
            // Method intentionally left empty.
        }
    }
    #pragma warning restore 1591
}