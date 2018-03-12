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
            var references = context.References.Where(x => x.ReferenceType == referenceType).ToList();
            if (referenceType != ReferenceType.Newsletters)
            {
                references = references.Where(r => r.DateAppear.Date >= DateTime.Now.Date).ToList();
            }
            return references;
        }

        public void UploadReferences(ReferenceType referenceType, IEnumerable<ReferenceCriteria> references, string modifiedBy)
        {
            if (references == null || !references.Any())
            {
                RemoveAllReferences(referenceType);
            }
            else
            {
                var oldReferences = references.Where(r => r.Id != 0).ToList();

                if (oldReferences != null && oldReferences.Count > 0)
                {
                    RemoveOldReferences(referenceType, oldReferences);
                }

                foreach (ReferenceCriteria reference in references)
                {
                    if (reference.Id == null)
                    {
                        SaveReference(reference, referenceType, modifiedBy);
                    }
                    else
                    {
                        UpdateReference(reference, modifiedBy);
                    }
                }
            }
        }

        private void SaveReference(ReferenceCriteria reference, ReferenceType referenceType, string modifiedBy)
        {
            var data = new Reference
            {
                DateAppear = referenceType != ReferenceType.Newsletters ? reference.DateAppear : DateTime.Now,
                ImageLink = reference.ImageLink,
                ReferenceType = referenceType,
                LastUpdateBy = modifiedBy,
                LastUpdateDate = DateTime.Now
            };

            context.References.Add(data);
            context.SaveChanges();
        }

        private void UpdateReference(ReferenceCriteria reference, string modifiedBy)
        {
            var oldReference = context.References.Find(reference.Id);
            if (IsChangedReference(oldReference, reference))
            {
                oldReference.ImageLink = reference.ImageLink;
                oldReference.DateAppear = oldReference.ReferenceType != ReferenceType.Newsletters ? reference.DateAppear : DateTime.Now;
                oldReference.LastUpdateDate = DateTime.Now;
                oldReference.LastUpdateBy = modifiedBy;
            }
            context.SaveChanges();
        }

        private static bool IsChangedReference(Reference oldReference, ReferenceCriteria newReference)
        {
            var isHaveChanged = false;
            if (oldReference.ReferenceType != ReferenceType.Newsletters
                && (DateTime.Compare(oldReference.DateAppear.Date, newReference.DateAppear.Date) != 0
                || !string.Equals(oldReference.ImageLink, newReference.ImageLink)))
            {
                isHaveChanged = true;
            }

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
            foreach (var reference in references)
            {
                DeleteReference(reference.Id);
            }
        }

        private void RemoveOldReferences(ReferenceType referenceType, List<ReferenceCriteria> references)
        {
            var originReferences = GetListReference(referenceType);
            var excludedIDs = new HashSet<int?>(references.Select(p => p.Id));
            var listDeletedReferenceIds = originReferences.Where(p => !excludedIDs.Contains(p.Id)).Select(p => p.Id).ToList();
            foreach (var referenceId in listDeletedReferenceIds)
            {
                DeleteReference(referenceId);
            }
        }

        private void DeleteReference(int id)
        {
            var reference = context.References.Find(id);

            if (reference != null)
            {
                context.References.Remove(reference);
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