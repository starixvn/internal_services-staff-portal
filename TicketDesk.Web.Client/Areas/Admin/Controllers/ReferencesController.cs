namespace TicketDesk.Web.Client.Areas.Admin.Controllers
{
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using TicketDesk.Core.Enumerations;
    using TicketDesk.Domain;
    using TicketDesk.Domain.Infrastructure;
    using TicketDesk.Domain.Model;
    using TicketDesk.Web.Client.Models.Admin;

    public class ReferencesController : Controller
    {
        private readonly TdDomainContext Context = new TdDomainContext();

        public ActionResult TVSchedules()
        {
            var tvSchedulesViewModel = new ListReferencesViewModel
            {
                References = Context.References.Where(x => x.ReferenceType == ReferenceType.TvSchedule).ToList()
            };

            return View(tvSchedulesViewModel);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.TicketCenter, Policy = Constant.Policy.Read)]
        public ActionResult TVSchedules(ListReferencesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var references = viewModel.References;

                if (references != null)
                {
                    RemoveOldReference(ReferenceType.TvSchedule);

                    foreach (Reference reference in references)
                    {
                        SaveEachReference(reference, ReferenceType.TvSchedule);
                    }
                    Context.SaveChanges();
                }
            }

            return Json(new JsonResult { });
        }

        public ActionResult ReferenceEntity(ReferenceType referenceType)
        {
            var reference = new Reference
            {
                ReferenceType = referenceType
            };
            return PartialView("_ReferenceEntryEditor", reference);
        }

        private void SaveEachReference(Reference reference, ReferenceType referenceType)
        {
            var tvSchedule = new Reference
            {
                DateAppear = reference.DateAppear,
                ImageLink = reference.ImageLink,
                ReferenceType = referenceType,
                LastUpdateBy = Context.SecurityProvider.CurrentUserId,
                LastUpdateDate = DateTime.Now
            };

            Context.References.Add(tvSchedule);
        }

        private void RemoveOldReference(ReferenceType referenceType)
        {
            IList<Reference> oldReferences = Context.References.Where(x => x.ReferenceType == referenceType).ToList();

            if (oldReferences != null)
            {
                foreach (var reference in oldReferences)
                {
                    Context.References.Remove(reference);
                }
                Context.SaveChanges();
            }
        }

        public ActionResult NewsLetter()
        {
            var tvSchedulesViewModel = new ListReferencesViewModel
            {
                References = Context.References.Where(x => x.ReferenceType == ReferenceType.NewsLetter).ToList()
            };

            return View(tvSchedulesViewModel);
        }
    }
}