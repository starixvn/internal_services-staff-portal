namespace TicketDesk.Domain.Migrations
{
    using Model;
    using System;
    using System.Configuration;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<TdDomainContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TicketDeskCore";
        }

        private void AddDefaultData(TdDomainContext context)
        {
            AddTicketCategories(context);
            AddCustomFieldTypes(context);
        }

        private void AddTicketCategories(TdDomainContext context)
        {
            context.TicketCategories.AddOrUpdate(
                new TicketCategory
                {
                    CategoryId = 1,
                    Name = "Facility"
                },
                new TicketCategory
                {
                    CategoryId = 2,
                    Name = "Hardware"
                },
                new TicketCategory
                {
                    CategoryId = 3,
                    Name = "Software"
                },
                new TicketCategory
                {
                    CategoryId = 4,
                    Name = "Network"
                },
                new TicketCategory
                {
                    CategoryId = 5,
                    Name = "Customer Support"
                },
                new TicketCategory
                {
                    CategoryId = 6,
                    Name = "Incident"
                },
                new TicketCategory
                {
                    CategoryId = 7,
                    Name = "Feedback"
                });
        }

        private void AddCustomFieldTypes(TdDomainContext context)
        {
            context.CustomFieldTypes.AddOrUpdate(
                new CustomFieldType
                {
                    Id = 1,
                    Name = "string"
                },
                new CustomFieldType
                {
                    Id = 2,
                    Name = "int"
                },
                new CustomFieldType
                {
                    Id = 3,
                    Name = "bool"
                },
                new CustomFieldType
                {
                    Id = 4,
                    Name = "list"
                },
                new CustomFieldType
                {
                    Id = 5,
                    Name = "date/time"
                });
        }

        protected override void Seed(TdDomainContext context)
        {
            var demoMode = ConfigurationManager.AppSettings["ticketdesk:DemoModeEnabled"];
            if (!context.Projects.Any())
            {
                context.Projects.Add(new Project { ProjectName = "Default", ProjectDescription = "Default project" });
            }

            if (!string.IsNullOrEmpty(demoMode) && demoMode.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                DemoDataManager.SetupDemoData(context);
            }

            AddDefaultData(context);

            base.Seed(context);
        }
    }
}