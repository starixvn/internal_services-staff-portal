namespace TicketDesk.Domain
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;

    public interface IContextHelper
    {
        int SiteId { get; }

        string Environment { get; }

        ICurrentSession CurrentSession { get; set; }

        ICurrentContext CurrentContext { get; set; }

        IUserService UserService { get; }

        bool HasRight(string functionKey, Constant.Policy policy);
    }
}