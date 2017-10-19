#pragma warning disable 1591
namespace S22.Mail
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Mime;

    [Serializable]
    public class SerializableContentType
    {
        private SerializableContentType(ContentType contentType)
        {
            Boundary = contentType.Boundary;
            CharSet = contentType.CharSet;
            MediaType = contentType.MediaType;
            Name = contentType.Name;
            Parameters = new StringDictionary();
            foreach (string k in contentType.Parameters.Keys)
            {
                if (!contentType.Parameters.ContainsKey(k))
                {
                    Parameters.Add(k, contentType.Parameters[k]);
                }
            }
        }

        public string Boundary { get; set; }

        public string CharSet { get; set; }

        public string MediaType { get; set; }

        public string Name { get; set; }

        public StringDictionary Parameters { get; }

        public static implicit operator ContentType(SerializableContentType contentType)
        {
            if (contentType == null)
            {
                return null;
            }

            ContentType ct = new ContentType
            {
                Boundary = contentType.Boundary,
                CharSet = contentType.CharSet,
                MediaType = contentType.MediaType,
                Name = contentType.Name
            };

            foreach (string k in contentType.Parameters.Keys)
            {
                ct.Parameters.Add(k, contentType.Parameters[k]);
            }

            return ct;
        }

        public static implicit operator SerializableContentType(ContentType contentType)
        {
            if (contentType == null)
            {
                return null;
            }

            return new SerializableContentType(contentType);
        }
    }
}
#pragma warning restore 1591