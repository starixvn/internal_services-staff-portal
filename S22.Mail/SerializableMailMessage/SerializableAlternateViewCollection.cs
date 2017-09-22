#pragma warning disable 1591
namespace S22.Mail
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    public class SerializableAlternateViewCollection : Collection<SerializableAlternateView>, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
#pragma warning restore 1591