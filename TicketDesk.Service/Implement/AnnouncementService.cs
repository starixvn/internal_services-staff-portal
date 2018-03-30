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

            GetAnnouncementReadStatus(announcements);

            return announcements;
        }

        public bool SetAnnouncementReadStatus(int announcementId, int userId, bool isRead = true)
        {
            var userReadAnnouncement =
                context.UserReadAnnouncements.FirstOrDefault(x => x.UserId == userId && x.AnnouncementId == announcementId);

            if (userReadAnnouncement != null)
            {
                userReadAnnouncement.IsRead = isRead;
            }
            else
            {
                userReadAnnouncement = new UserReadAnnouncement
                {
                    UserId = userId,
                    AnnouncementId = announcementId,
                    IsRead = isRead
                };

                context.UserReadAnnouncements.Add(userReadAnnouncement);
            }

            return context.SaveChanges() > 0;
        }

        public IEnumerable<Announcement> GetHomeAnnouncements()
        {
            var announcements = context.Announcements
                .Where(x => x.State == AnnouncementState.Published)
                .OrderByDescending(x => x.PublishedDateTime)
                .Decompile();

            GetAnnouncementReadStatus(announcements);

            return announcements;
        }

        private void GetAnnouncementReadStatus(IQueryable<Announcement> announcements)
        {
            var readAnnouncementIds = context.UserReadAnnouncements
                            .Where(x => x.UserId == context.SecurityProvider.NapCurrentUserId && x.IsRead)
                            .Select(x => x.AnnouncementId);

            if (readAnnouncementIds.Any())
            {
                foreach (var announcement in announcements)
                {
                    announcement.IsRead = readAnnouncementIds.Contains(announcement.Id);
                }
            }
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
    }
}