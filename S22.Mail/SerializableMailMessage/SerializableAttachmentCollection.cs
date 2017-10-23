#pragma warning disable 1591
namespace S22.Mail
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    public sealed class SerializableAttachmentCollection : Collection<SerializableAttachment>, IDisposable
    {
        public void Dispose()
        {
            // Method intentionally left empty.
        }
    }
}
#pragma warning restore 1591