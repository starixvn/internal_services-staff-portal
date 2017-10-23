namespace TicketDesk.Web.Client.Controllers
{
    using Domain;
    using Domain.Infrastructure;
    using Domain.Model;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using IO;
    using Localization.Controllers;
    using Models.Ticket;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("ticket-activity")]
    [Route("{action}")]
    [ValidateInput(false)]
    [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
    public class TicketActivityController : BaseController
    {
        public TicketActivityController(TdDomainContext context)
        {
            Context = context;
        }

        private TdDomainContext Context { get; }

        [Route("load-activity")]
        public async Task<ActionResult> LoadActivity(TicketActivity activity, int ticketId, Guid? tempId)
        {
            var ticket = await Context.Tickets.FindAsync(ticketId).ConfigureAwait(false);

            Context.TicketActions.IsTicketActivityValid(ticket, activity);
            ViewBag.CommentRequired = activity.IsCommentRequired();
            ViewBag.Activity = activity;
            ViewBag.TempId = tempId ?? Guid.NewGuid();
            ViewBag.IsEditorDefaultHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";
            if (activity == TicketActivity.EditTicketInfo)
            {
                await SetProjectInfoForModelAsync().ConfigureAwait(false);
            }

            return PartialView("_ActivityForm", ticket);
        }

        private async Task SetProjectInfoForModelAsync()
        {
            var projectCount = await Context.Projects.CountAsync().ConfigureAwait(false);
            var isMulti = projectCount > 1;
            ViewBag.IsMultiProject = isMulti;
        }

        [Route("activity-buttons")]
        public ActionResult ActivityButtons(int ticketId)
        {
            // WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            var activities = Context.TicketActions.GetValidTicketActivities(ticket);
            return PartialView("_ActivityButtons", activities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add-comment")]
        public async Task<ActionResult> AddComment(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.AddComment(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.AddComment).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("assign")]
        public async Task<ActionResult> Assign(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment, string assignedTo, string priority)
        {
            var activityFn = Context.TicketActions.Assign(comment, assignedTo, priority);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.Assign).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel-more-info")]
        public async Task<ActionResult> CancelMoreInfo(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.CancelMoreInfo(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.CancelMoreInfo).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("close")]
        public async Task<ActionResult> Close(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.Close(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.Close).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit-ticket-info")]
        public async Task<ActionResult> EditTicketInfo(
            int ticketId,
            int projectId,
            [ModelBinder(typeof(SummernoteModelBinder))] string comment,
            string title,
            string details,
            string priority,
            string ticketType,
            string category,
            string owner,
            string tagList)
        {
            string detailsWithoutHtmlWhenEmpty = details.StripHtmlWhenEmpty();
            var projectName = await Context.Projects.Where(p => p.ProjectId == projectId).Select(s => s.ProjectName).FirstOrDefaultAsync().ConfigureAwait(false);
            var activityFn = Context.TicketActions.EditTicketInfo(
                comment,
                projectId,
                projectName,
                title,
                detailsWithoutHtmlWhenEmpty,
                priority,
                ticketType,
                category,
                owner,
                tagList,
                Context.TicketDeskSettings);

            return await PerformTicketAction(ticketId, activityFn, TicketActivity.EditTicketInfo).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("force-close")]
        public async Task<ActionResult> ForceClose(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.ForceClose(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.ForceClose).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("give-up")]
        public async Task<ActionResult> GiveUp(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.GiveUp(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.GiveUp).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("modify-attachments")]
        public async Task<ActionResult> ModifyAttachments(
            int ticketId,
            [ModelBinder(typeof(SummernoteModelBinder))] string comment,
            Guid tempId,
            string deleteFiles)
        {
            var demoMode = (ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"] ?? "false")
                .Equals("true", StringComparison.InvariantCultureIgnoreCase);
            if (demoMode)
            {
                return new EmptyResult();
            }

            // most of this action is performed directly against the storage provider, outside the business domain's control.
            // All the business domain has to do is record the activity log and comments
            Action<Ticket> activityFn = ticket =>
           {
               // TODO: it might make sense to move the string building part of this over to the TicketDeskFileStore too?
               var sb = new StringBuilder(comment);
               if (!string.IsNullOrEmpty(deleteFiles))
               {
                   sb.AppendLine();
                   sb.AppendLine("<dl><dt>");
                   sb.AppendLine(Strings.RemovedFiles);
                   sb.AppendLine("</dt>");

                   var files = deleteFiles.Split(',');
                   foreach (var file in files)
                   {
                       TicketDeskFileStore.DeleteAttachment(file, ticketId.ToString(CultureInfo.InvariantCulture), false);
                       sb.AppendLine($"<dd>    {file}</dd>");
                   }

                   sb.AppendLine("</dl>");
               }

               var filesAdded = ticket.CommitPendingAttachments(tempId).ToArray();
               if (filesAdded.Any())
               {
                   sb.AppendLine();
                   sb.AppendLine("<dl><dt>");
                   sb.AppendLine(Strings.NewFiles);
                   sb.AppendLine("</dt>");

                   foreach (var file in filesAdded)
                   {
                       sb.AppendLine($"<dd>    {file}</dd>");
                   }

                   sb.AppendLine("</dl>");
               }

               // perform the simple business domain functions
               var domainActivityFn = Context.TicketActions.ModifyAttachments(sb.ToString());
               domainActivityFn(ticket);
           };

            return await PerformTicketAction(ticketId, activityFn, TicketActivity.ModifyAttachments).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("pass")]
        public async Task<ActionResult> Pass(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment, string assignedTo, string priority)
        {
            var activityFn = Context.TicketActions.Pass(comment, assignedTo, priority);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.Pass).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("reassign")]
        public async Task<ActionResult> ReAssign(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment, string assignedTo, string priority)
        {
            var activityFn = Context.TicketActions.ReAssign(comment, assignedTo, priority);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.ReAssign).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("request-more-info")]
        public async Task<ActionResult> RequestMoreInfo(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.RequestMoreInfo(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.RequestMoreInfo).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("reopen")]
        public async Task<ActionResult> ReOpen(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment, bool assignToMe = false)
        {
            var activityFn = Context.TicketActions.ReOpen(comment, assignToMe);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.ReOpen).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("resolve")]
        public async Task<ActionResult> Resolve(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment)
        {
            var activityFn = Context.TicketActions.Resolve(comment);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.ReOpen).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("supply-more-info")]
        public async Task<ActionResult> SupplyMoreInfo(int ticketId, [ModelBinder(typeof(SummernoteModelBinder))] string comment, bool reactivate = false)
        {
            var activityFn = Context.TicketActions.SupplyMoreInfo(comment, reactivate);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.SupplyMoreInfo).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("take-over")]
        public async Task<ActionResult> TakeOver(
            int ticketId,
            [ModelBinder(typeof(SummernoteModelBinder))] string comment,
            string priority)
        {
            var activityFn = Context.TicketActions.TakeOver(comment, priority);
            return await PerformTicketAction(ticketId, activityFn, TicketActivity.TakeOver).ConfigureAwait(false);
        }

        private async Task<ActionResult> PerformTicketAction(int ticketId, Action<Ticket> activityFn, TicketActivity activity)
        {
            var ticket = await Context.Tickets.FindAsync(ticketId).ConfigureAwait(false);
            TryValidateModel(ticket);
            try
            {
                ticket?.PerformAction(activityFn);
            }
            catch (SecurityException ex)
            {
                ModelState.AddModelError("Security", ex);
            }

            // save changes catches lastupdatedby and date automatically
            var result = await Context.SaveChangesAsync().ConfigureAwait(false);
            if (result > 0)
            {
                // standard success case
                return new EmptyResult();
            }

            // fail case, return the view and let the client/view sort out the errors
            ViewBag.CommentRequired = activity.IsCommentRequired();
            ViewBag.Activity = activity;
            ViewBag.IsEditorDefaultHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";
            return PartialView("_ActivityForm", ticket);
        }

        [Route("get-ticketfeedback-info")]
        public ActionResult GetTicketFeedbackInfo(int ticketId, int categoryId)
        {
            SqlParameter categoryParam = new SqlParameter("@CategoryId", categoryId);
            var customFields = Context.Database.SqlQuery<TicketFeedbackCustomField>("TicketFeedbackCustomFields_Get @CategoryId", categoryParam).ToList();

            foreach (var field in customFields)
            {
                field.Value = Context.CustomFieldValues.FirstOrDefault(x => (x.TicketId == ticketId) && (x.CustomFieldId == field.CustomFieldId))?.Value;
            }

            ViewData.TemplateInfo.HtmlFieldPrefix = "CustomFields";
            return PartialView("~/Views/Ticket/_FeedbackCustomField.cshtml", customFields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit-ticket-feedback")]
        public async Task<ActionResult> EditTicketFeedback(int ticketId,
            [ModelBinder(typeof(SummernoteModelBinder))] string comment,
            Ticket ticket,
            IList<TicketFeedbackCustomField> customFields)
        {
            ticket.Details = ticket.Details.StripHtmlWhenEmpty();
            var projectName = await Context.Projects.Where(p => p.ProjectId == ticket.ProjectId).Select(s => s.ProjectName).FirstOrDefaultAsync().ConfigureAwait(false);
            var activityFn = Context.TicketActions.EditTicketInfo(
                comment,
                ticket.ProjectId,
                projectName,
                ticket.Title,
                ticket.Details,
                ticket.Priority,
                ticket.TicketType,
                ticket.Category,
                ticket.Owner,
                ticket.TagList,
                Context.TicketDeskSettings);

            foreach (var field in customFields)
            {
                var customFieldValue = Context.CustomFieldValues.FirstOrDefault(x => (x.TicketId == ticketId) && (x.CustomFieldId == field.CustomFieldId));
                if (customFieldValue != null)
                {
                    customFieldValue.Value = field.Value;
                }
            }

            return await PerformTicketAction(ticketId, activityFn, TicketActivity.EditTicketInfo).ConfigureAwait(false);
        }
    }
}