namespace S22.Mail
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    #pragma warning disable 1591
    public class SerializableMailAddressCollection : Collection<SerializableMailAddress>
    {
    #pragma warning restore 1591
    }
}