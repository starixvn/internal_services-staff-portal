namespace TicketDesk.Web.Identity.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    public class UserDisplayInfoCache : List<UserDisplayInfo>
    {
        public UserDisplayInfoCache(TicketDeskUserManager manager)
        {
            Manager = manager;
        }

        private TicketDeskUserManager Manager { get; set; }

        public void ClearCache()
        {
            MemoryCache.Default.Remove("user-info-cache");
        }

        public UserDisplayInfo GetUserInfo(string userId)
        {
            return GetCachedUserInfo(Manager).FirstOrDefault(u => u.Id == userId);
        }

        private static IEnumerable<UserDisplayInfo> GetCachedUserInfo(TicketDeskUserManager manager)
        {
            const string key = "user-info-cache";
            var cache = MemoryCache.Default;
            var cacheCollection = cache[key] as IEnumerable<UserDisplayInfo>;

            if (cacheCollection == null || !cacheCollection.Any())
            {
                var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15d) };

                cacheCollection = manager.NapContextHelper.UserService.GetBySite(manager.NapContextHelper.SiteId).Select(u => new UserDisplayInfo
                {
                    Id = u.Id.ToString(),
                    Email = u.Email,
                    DisplayName = u.FirstName + " " + u.LastName
                }).ToList();

                cache.Add(key, cacheCollection, policy);
            }

            return cacheCollection;
        }
    }
}