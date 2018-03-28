namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Criteria.Shop;
    using Domain.Model;

    public interface IShopService
    {
        IQueryable<Shop> GetListShop(ShopFilterCrietria criteria);

        Shop GetShopById(int id);

        bool CreateNewShop(Shop shop);

        bool UpdateShop(int id, Shop shop);

        bool CheckExistShopName(string shopName);

        object GetShopSuggestion(string shopName);

        IEnumerable<SelectListItem> GetShopSelectList();
    }
}