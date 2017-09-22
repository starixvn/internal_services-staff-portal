namespace S22.Mail
{
    using System;
    using System.IO;
    using System.Net.Mail;
    using System.Net.Mime;

    [Serializable]
    #pragma warning disable 1591
    public class SerializableLinkedResource
    {
        private SerializableLinkedResource(LinkedResource resource)
        {
            ContentLink = resource.ContentLink;
            ContentId = resource.ContentId;
            ContentStream = new MemoryStream();
            resource.ContentStream.CopyTo(ContentStream);
            resource.ContentStream.Position = 0;
            ContentType = resource.ContentType;
            TransferEncoding = resource.TransferEncoding;
        }

        public Uri ContentLink { get; set; }

        public string ContentId { get; set; }

        public Stream ContentStream { get; private set; }

        public SerializableContentType ContentType { get; set; }

        public TransferEncoding TransferEncoding { get; set; }

        public static implicit operator LinkedResource(SerializableLinkedResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            LinkedResource r = new LinkedResource(resource.ContentStream);

            r.ContentId = resource.ContentId;
            r.ContentType = resource.ContentType;
            r.TransferEncoding = resource.TransferEncoding;
            return null;
        }

        public static implicit operator SerializableLinkedResource(LinkedResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new SerializableLinkedResource(resource);
        }
    }
    #pragma warning restore 1591
}