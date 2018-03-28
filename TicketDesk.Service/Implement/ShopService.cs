namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Criteria.Shop;
    using Domain.Model;
    using Interface;

    public class ShopService : EntityFrameworkBaseService, IShopService
    {
        public bool CheckExistShopName(string shopName)
        {
            return context.Shops.Any(x => x.ShopName == shopName);
        }

        public bool CreateNewShop(Shop shop)
        {
            context.Shops.Add(shop);
            var result = context.SaveChanges();
            return result > 0;
        }

        public IQueryable<Shop> GetListShop(ShopFilter criteria)
        {
            bool isActive = criteria.ActiveState == ActiveState.Yes;
            var shopList = criteria.ActiveState == ActiveState.All ? context.Shops : context.Shops.Where(s => s.IsActive == isActive);

            if (criteria.ShopId != 0)
            {
                shopList = shopList.Where(s => s.ShopId == criteria.ShopId);
            }

            if (criteria.ShopId == 0 && !string.IsNullOrEmpty(criteria.ShopName))
            {
                shopList = shopList.Where(s => s.ShopName.Contains(criteria.ShopName));
            }

            return shopList.OrderBy(x => x.ShopCode);
        }

        public Shop GetShopById(int id)
        {
            return context.Shops.Find(id);
        }

        public object GetShopSuggestion(string shopName)
        {
            return context.Shops.Where(s => s.ShopName.Contains(shopName)).Select(x => new { x.ShopId, x.ShopName }).Take(10);
        }

        public bool UpdateShop(int id, Shop model)
        {
            bool result = false;
            var shop = context.Shops.Find(id);
            if (shop != null)
            {
                shop.ShopName = model.ShopName;
                shop.ShopCode = model.ShopCode;
                shop.ContactName = model.ContactName;
                shop.ContactNumber = model.ContactNumber;
                shop.Email = model.Email;
                shop.Address = model.Address;
                shop.IsActive = model.IsActive;
                shop.LastUpdatedBy = CurrentUsername;
                shop.LastUpdatedDate = DateTime.Now;
                result = context.SaveChanges() > 0;
            }

            return result;
        }

        public IEnumerable<SelectListItem> GetShopSelectList()
        {
            return context.Shops
                .Where(s => s.IsActive)
                .OrderBy(s => s.ShopName)
                .Select(s => new SelectListItem
                {
                    Text = s.ShopName,
                    Value = s.ShopId.ToString()
                });
        }
    }
}