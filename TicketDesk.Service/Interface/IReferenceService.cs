namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Criteria.Reference;
    using Domain.Model;

    public interface IReferenceService
    {
        IList<Reference> GetListReference(ReferenceType referenceType);

        void UploadReferences(ReferenceType referenceType, IEnumerable<ReferenceCriteria> references, string modifiedBy);

        Reference GetReferenceRelevant(ReferenceType referenceType);
    }
}