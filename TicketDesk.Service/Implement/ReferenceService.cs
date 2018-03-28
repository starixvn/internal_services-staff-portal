namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Domain.Model;
    using Interface;

    public class ReferenceService : EntityFrameworkBaseService, IReferenceService
    {
        public IList<Reference> GetListReference(ReferenceType referenceType)
        {
            var references = context.References.Where(x => x.ReferenceType == referenceType).ToList();
            if (referenceType != ReferenceType.Newsletters)
            {
                references = references.Where(r => r.DateAppear.Date >= DateTime.Now.Date).OrderBy(x => x.DateAppear.Date).ToList();
            }
            return references;
        }

        public void UploadReferences(ReferenceType referenceType, IList<Reference> references)
        {
            if (references == null || !references.Any())
            {
                RemoveAllReferences(referenceType);
            }
            else
            {
                var originReferences = GetListReference(referenceType);
                var removedReferences = originReferences.Where(r => !references.Any(x => x.Id == r.Id) && r.Id > 0);
                var editReferences = references.Where(r => originReferences.Any(x => x.Id == r.Id));
                var newReferences = references.Where(r => !originReferences.Any(x => x.Id == r.Id));

                RemoveReferences(removedReferences);
                UpdateReference(editReferences);
                AddNewReferences(newReferences);
            }
            context.SaveChanges();
        }

        private void RemoveReferences(IEnumerable<Reference> removedReferences)
        {
            context.References.RemoveRange(removedReferences);
        }

        private void AddNewReferences(IEnumerable<Reference> newReferences)
        {
            foreach (Reference reference in newReferences)
            {
                reference.LastUpdateBy = CurrentUsername;
                reference.LastUpdateDate = DateTime.Now;
            }
            context.References.AddRange(newReferences);
        }

        private void UpdateReference(IEnumerable<Reference> references)
        {
            foreach (var reference in references)
            {
                var oldReference = context.References.Find(reference.Id);

                if (oldReference != null && IsChangedReference(oldReference, reference))
                {
                    oldReference.ImageLink = reference.ImageLink;
                    oldReference.DateAppear = oldReference.ReferenceType != ReferenceType.Newsletters
                                                ? reference.DateAppear
                                                : DateTime.Now;
                    oldReference.LastUpdateDate = DateTime.Now;
                    oldReference.LastUpdateBy = CurrentUsername;
                }
            }
        }

        private static bool IsChangedReference(Reference oldReference, Reference newReference)
        {
            bool isHaveChanged = oldReference.ReferenceType != ReferenceType.Newsletters
                && (DateTime.Compare(oldReference.DateAppear.Date, newReference.DateAppear.Date) != 0
                || !string.Equals(oldReference.ImageLink, newReference.ImageLink));

            if (oldReference.ReferenceType == ReferenceType.Newsletters &&
                !string.Equals(oldReference.ImageLink, newReference.ImageLink))
            {
                isHaveChanged = true;
            }

            return isHaveChanged;
        }

        private void RemoveAllReferences(ReferenceType referenceType)
        {
            var references = GetListReference(referenceType);
            context.References.RemoveRange(references);
        }

        public Reference GetRelevantReference(ReferenceType referenceType)
        {
            return context.References.Where(t => t.ReferenceType == referenceType)
                                               .OrderByDescending(r => r.DateAppear)
                                               .FirstOrDefault(r => DateTime.Compare(r.DateAppear, DateTime.Now) <= 0);
        }
    }
}