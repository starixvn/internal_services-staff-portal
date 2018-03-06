namespace TicketDesk.Service.Interface.Admin
{
    using System.Linq;
    using Criteria.Shop;
    using Domain.Model;

    public interface IShopService
    {
        IQueryable<Shop> GetListShop(ShopFilterCrietria criteria);

        Shop GetShopById(int id);

        bool CreateNewShop(ShopCriteria criteria);

        bool UpdateShop(int id, ShopCriteria criteria);

        bool CheckExistShopName(string shopName);

        object GetShopSuggestion(string shopName);
    }
}