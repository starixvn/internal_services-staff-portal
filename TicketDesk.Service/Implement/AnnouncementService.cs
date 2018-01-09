namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Criteria.Announcement;
    using DelegateDecompiler;
    using Domain.Model;
    using Interface;

    public class AnnouncementService : EntityFrameworkBaseService, IAnnouncementService
    {
        public Announcement GetAnnouncement(int id)
        {
            return context.Announcements.Find(id);
        }

        public IQueryable<Announcement> GetAnnouncements(GetAnnouncementsCriteria criteria)
        {
            IQueryable<Announcement> announcements = context.Announcements;

            if (criteria.PublishedFrom != null)
            {
                announcements = announcements.Where(x => criteria.PublishedFrom <= DbFunctions.TruncateTime(x.PublishedDateTime));
            }

            if (criteria.PublishedTo != null)
            {
                announcements = announcements.Where(x => DbFunctions.TruncateTime(x.PublishedDateTime) <= criteria.PublishedTo);
            }

            if (criteria.Status != null)
            {
                announcements = announcements.Where(x => x.Status == criteria.Status);
            }

            if (criteria.State != null)
            {
                announcements = announcements.Where(x => x.State == criteria.State).Decompile();
            }

            return announcements;
        }

        private static NewAnnouncementCriteria CommitAnnouncement(NewAnnouncementCriteria criteria)
        {
            // Set date-time
            criteria.Announcement.CreatedDateTime = DateTime.Now;
            criteria.Announcement.LastModified = DateTime.Now;

            // Set published date-time
            if (criteria.Date != null && criteria.Time != null)
            {
                var publishedDateTime = criteria.Date.Value;
                publishedDateTime = publishedDateTime.AddSeconds(criteria.Time.Value.TotalSeconds);

                criteria.Announcement.PublishedDateTime = publishedDateTime;
            }

            criteria.Announcement.ModifiedBy = criteria.ModifiedBy;

            criteria.Announcement.Status = criteria.IsImportant ? AnnouncementStatus.Important : AnnouncementStatus.Normal;

            return criteria;
        }

        public bool AddAnnouncement(NewAnnouncementCriteria criteria)
        {
            var committedCriteria = CommitAnnouncement(criteria);

            context.Announcements.Add(committedCriteria.Announcement);
            var result = context.SaveChanges();

            return result > 0;
        }

        public bool UpdateAnnouncement(NewAnnouncementCriteria criteria)
        {
            var committedCriteria = CommitAnnouncement(criteria);

            var originalAnnouncement = GetAnnouncement(criteria.Announcement.Id);

            context.Entry(originalAnnouncement).CurrentValues.SetValues(committedCriteria.Announcement);

            var result = context.SaveChanges();

            return result > 0;
        }

        public bool DeleteAnnouncement(int id)
        {
            var announcement = GetAnnouncement(id);

            if (announcement == null)
            {
                return false;
            }

            context.Announcements.Remove(announcement);
            var result = context.SaveChanges();
            return result > 0;
        }

        public IQueryable<Announcement> Pagination(IQueryable<Announcement> originalList, int skipped, int took)
        {
            return originalList.OrderByDescending(x => x.Id).Skip(skipped).Take(took);
        }
    }
}
