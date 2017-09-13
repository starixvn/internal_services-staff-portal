namespace TicketDesk.Domain
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;

    public interface IContextHelper
    {
        ICurrentSession CurrentSession();

        ICurrentContext CurrentContext();

        IUserService UserService();

        int GetSiteId();

        string GetEnvironment();

        bool HasRight(string functionKey, Constant.Policy policy);
    }
}