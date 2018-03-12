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

        public bool CreateNewShop(ShopCriteria criteria)
        {
            var newShop = new Shop
            {
                ShopName = criteria.ShopName,
                ShopCode = criteria.ShopCode,
                ContactName = criteria.ContactName,
                ContactNumber = criteria.ContactNumber,
                Email = criteria.Email,
                Address = criteria.Address,
                IsActive = criteria.IsActive,
                LastUpdatedBy = criteria.ModifiedBy,
                LastUpdatedDate = DateTime.Now
            };
            context.Shops.Add(newShop);
            var result = context.SaveChanges();
            return result > 0;
        }

        public IQueryable<Shop> GetListShop(ShopFilterCrietria criteria)
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

            return shopList;
        }

        public Shop GetShopById(int id)
        {
            return context.Shops.Find(id);
        }

        public object GetShopSuggestion(string shopName)
        {
            return context.Shops.Where(s => s.ShopName.Contains(shopName)).Select(x => new { x.ShopId, x.ShopName }).Take(10);
        }

        public bool UpdateShop(int id, ShopCriteria criteria)
        {
            bool result = false;
            var shop = context.Shops.Find(id);
            if (shop != null)
            {
                shop.ShopName = criteria.ShopName;
                shop.ShopCode = criteria.ShopCode;
                shop.ContactName = criteria.ContactName;
                shop.ContactNumber = criteria.ContactNumber;
                shop.Email = criteria.Email;
                shop.Address = criteria.Address;
                shop.IsActive = criteria.IsActive;
                shop.LastUpdatedBy = criteria.ModifiedBy;
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