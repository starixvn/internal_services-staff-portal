namespace SSP.Website
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return string.IsNullOrWhiteSpace(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}