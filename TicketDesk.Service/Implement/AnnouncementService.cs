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

        public bool AddAnnouncement(Announcement announcement)
        {
            announcement.CreatedDateTime = DateTime.Now;
            announcement.LastModified = DateTime.Now;
            announcement.ModifiedBy = CurrentUsername;

            context.Announcements.Add(announcement);
            var result = context.SaveChanges();

            return result > 0;
        }

        public bool UpdateAnnouncement(Announcement announcement)
        {
            var result = false;
            var originalAnnouncement = GetAnnouncement(announcement.Id);

            if (originalAnnouncement != null && originalAnnouncement.State != AnnouncementState.Published)
            {
                originalAnnouncement.LastModified = DateTime.Now;
                originalAnnouncement.ModifiedBy = CurrentUsername;

                originalAnnouncement.PublishedDateTime = announcement.PublishedDateTime;
                originalAnnouncement.Message = announcement.Message;
                originalAnnouncement.Type = announcement.Type;

                result = context.SaveChanges() > 0;
            }

            return result;
        }

        public bool PublishAnnouncements(IList<int> ids)
        {
            var announcements = context.Announcements.Where(x => ids.Contains(x.Id));

            foreach (var announcement in announcements)
            {
                if (announcement != null && announcement.State != AnnouncementState.Published)
                {
                    announcement.PublishedDateTime = DateTime.Now;
                    announcement.LastModified = DateTime.Now;
                    announcement.ModifiedBy = CurrentUsername;
                }
            }

            var result = context.SaveChanges();
            return result > 0;
        }

        public bool DeleteAnnouncement(int id)
        {
            var result = false;

            var announcement = GetAnnouncement(id);

            if (announcement != null && announcement.State != AnnouncementState.Published)
            {
                context.Announcements.Remove(announcement);
                result = context.SaveChanges() > 0;
            }

            return result;
        }

        public int[] GetAllAnnouncementIds()
        {
            return context.Announcements.Where(a => a.PublishedDateTime < DateTime.Now).Select(a => a.Id).ToArray();
        }
    }
}