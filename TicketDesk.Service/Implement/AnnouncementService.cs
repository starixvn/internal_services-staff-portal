namespace TicketDesk.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Core.Enumerations;
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

        public IEnumerable<Announcement> GetAnnouncements(GetAnnouncementsCriteria criteria)
        {
            IQueryable<Announcement> announcements = context.Announcements;

            if (criteria.PublishedFrom != null)
            {
                announcements = announcements.Where(x => criteria.PublishedFrom <= DbFunctions.TruncateTime(x.PublishedDateTime)
                                                         || x.PublishedDateTime == null);
            }

            if (criteria.PublishedTo != null)
            {
                announcements = announcements.Where(x => DbFunctions.TruncateTime(x.PublishedDateTime) <= criteria.PublishedTo
                                                         || x.PublishedDateTime == null);
            }

            if (criteria.Type != null)
            {
                announcements = announcements.Where(x => x.Type == criteria.Type);
            }

            if (criteria.State != null)
            {
                announcements = announcements.Where(x => x.State == criteria.State).Decompile();
            }

            return announcements;
        }

        public IEnumerable<Announcement> GetHomeAnnouncements()
        {
            return context.Announcements
                .Where(x => x.State == AnnouncementState.Published)
                .OrderByDescending(x => x.PublishedDateTime)
                .Decompile();
        }

        public IEnumerable<Announcement> CalculateAnnouncementsReadStatus(
            IEnumerable<Announcement> announcements,
            string readAnnouncements)
        {
            if (!string.IsNullOrEmpty(readAnnouncements))
            {
                var readAnnouncementIds = readAnnouncements
                .Split(',')
                .Distinct()
                .Select(int.Parse).ToList();

                foreach (var announcement in announcements)
                {
                    announcement.IsRead = readAnnouncementIds.Contains(announcement.Id);
                }
            }
            else
            {
                foreach (var announcement in announcements)
                {
                    announcement.IsRead = false;
                }
            }

            return announcements;
        }

        public int CountUnreadAnnouncement(string readAnnouncements)
        {
            var publishedAnnouncementIds = context.Announcements
                .Where(x => x.State == AnnouncementState.Published)
                .Decompile()
                .Select(x => x.Id.ToString());

            int result = publishedAnnouncementIds.Count();

            if (!string.IsNullOrEmpty(readAnnouncements))
            {
                var readAnnouncementIds = readAnnouncements.Split(',');
                result = result - publishedAnnouncementIds.Intersect(readAnnouncementIds).Count();
            }

            return result;
        }

        private NewAnnouncementCriteria CommitAnnouncement(NewAnnouncementCriteria criteria)
        {
            // Set date-time
            criteria.Announcement.CreatedDateTime = DateTime.Now;
            criteria.Announcement.LastModified = DateTime.Now;
            criteria.Announcement.ModifiedBy = CurrentUsername;

            criteria.Announcement.Type = criteria.IsImportant ? AnnouncementType.Important : AnnouncementType.Normal;

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

        public bool PublishAnnouncements(PublishAnnouncementsCriteria criteria)
        {
            foreach (var id in criteria.Ids)
            {
                var originalAnnouncement = GetAnnouncement(id);

                if (originalAnnouncement.State == AnnouncementState.Published)
                {
                    continue;
                }

                originalAnnouncement.PublishedDateTime = DateTime.Now;
                originalAnnouncement.LastModified = DateTime.Now;

                originalAnnouncement.ModifiedBy = CurrentUsername;

                context.Entry(originalAnnouncement).CurrentValues.SetValues(originalAnnouncement);
            }

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

        public int[] GetAllAnnouncementIds()
        {
            return context.Announcements.Where(a => a.PublishedDateTime < DateTime.Now).Select(a => a.Id).ToArray();
        }
    }
}