namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using System.Linq;
    using System.Web.Mvc;

    [RoutePrefix("auto-complete")]
    [TdAuthorize(Roles = "TdInternalUsers,TdHelpDeskUsers,TdAdministrators")]
    public class AutoCompleteController : Controller
    {
        public AutoCompleteController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("tag-list")]
        [HttpGet]
        public ActionResult TagList(string term)
        {
            // TODO: cache a complete distinct taglist on app start to improve performance, keep this action synchronous
            var tags = Context.TicketTags
                        .Where(tag => tag.TagName.StartsWith(term))
                        .GroupBy(tag => tag.TagName)
                        .Select(g => g.FirstOrDefault())
                        .Take(10); // limit to 10

            // pull data, then convert to format expected by select2
            return new JsonCamelCaseResult
            {
                Data = tags.Select(t => new { Id = t.TagName, Text = t.TagName }).ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}