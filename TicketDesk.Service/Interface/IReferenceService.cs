namespace TicketDesk.Service.Interface
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Domain.Model;

    public interface IReferenceService
    {
        IList<Reference> GetListReference(ReferenceType referenceType);

        void UploadReferences(ReferenceType referenceType, IList<Reference> references);

        Reference GetRelevantReference(ReferenceType referenceType);
    }
}