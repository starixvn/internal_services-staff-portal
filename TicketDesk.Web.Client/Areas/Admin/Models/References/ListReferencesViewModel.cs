namespace TicketDesk.Web.Client.Areas.Admin.Models.References
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Criteria.Reference;

    public class ListReferencesViewModel
    {
        public bool IsHostPermission { get; set; }

        public ReferenceType Type { get; set; }

        public IList<ReferenceViewModel> References { get; set; }

        public IList<ReferenceCriteria> GetQueryCriteria()
        {
            var criteriaList = new List<ReferenceCriteria>();

            if (References != null)
            {
                foreach (var reference in References)
                {
                    var criteria = new ReferenceCriteria
                    {
                        Id = reference.Id,
                        ImageLink = reference.ImageLink,
                        DateAppear = reference.DateAppear,
                        ReferenceType = reference.ReferenceType
                    };
                    criteriaList.Add(criteria);
                }
            }

            return criteriaList;
        }
    }
}