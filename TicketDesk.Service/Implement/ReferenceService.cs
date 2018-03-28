namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Criteria.Reference;
    using Domain.Model;
    using Interface;

    public class ReferenceService : EntityFrameworkBaseService, IReferenceService
    {
        public IEnumerable<Reference> GetListReference(ReferenceType referenceType)
        {
            var references = context.References.Where(x => x.ReferenceType == referenceType);
            if (referenceType != ReferenceType.Newsletters)
            {
                references = references.Where(r => r.DateAppear.Date >= DateTime.Now.Date).OrderBy(x => x.DateAppear.Date);
            }
            return references;
        }

        public void UploadReferences(ReferenceType referenceType, IEnumerable<ReferenceCriteria> references)
        {
            if (references == null || !references.Any())
            {
                RemoveAllReferences(referenceType);
            }
            else
            {
                RemoveOldReferences(referenceType, references);

                UpdateAndSaveReferences(references);
            }
        }

        private void UpdateAndSaveReferences(IEnumerable<ReferenceCriteria> references)
        {
            foreach (ReferenceCriteria reference in references)
            {
                if (reference.Id == 0 || IsNotExistReference(reference.Id))
                {
                    SaveReference(reference);
                }
                else
                {
                    UpdateReference(reference);
                }
            }
        }

        private bool IsNotExistReference(int id)
        {
            return context.References.Find(id) == null;
        }

        private void SaveReference(ReferenceCriteria reference)
        {
            var data = new Reference
            {
                DateAppear = reference.ReferenceType != ReferenceType.Newsletters ? reference.DateAppear : DateTime.Now,
                ImageLink = reference.ImageLink,
                ReferenceType = reference.ReferenceType,
                LastUpdateBy = CurrentUsername,
                LastUpdateDate = DateTime.Now
            };

            context.References.Add(data);
            context.SaveChanges();
        }

        private void UpdateReference(ReferenceCriteria reference)
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

            context.SaveChanges();
        }

        private static bool IsChangedReference(Reference oldReference, ReferenceCriteria newReference)
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
            context.SaveChanges();
        }

        private void RemoveOldReferences(ReferenceType referenceType, IEnumerable<ReferenceCriteria> references)
        {
            if (references.Any(r => r.Id > 0))
            {
                var originReferences = GetListReference(referenceType);
                var removedReferences = originReferences.Where(r => !references.Any(x => x.Id == r.Id));
                context.References.RemoveRange(removedReferences);
                context.SaveChanges();
            }
        }

        public Reference GetRelevantReference(ReferenceType referenceType)
        {
            return context.References.Where(t => t.ReferenceType == referenceType)
                                               .OrderByDescending(r => r.DateAppear)
                                               .FirstOrDefault(r => DateTime.Compare(r.DateAppear, DateTime.Now) <= 0);
        }
    }
}