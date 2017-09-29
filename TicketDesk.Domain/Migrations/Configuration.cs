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
                });

            context.TicketTypes.AddOrUpdate(
                new TicketType
                {
                    TicketTypeId = 1,
                    TicketTypeName = "For repair"
                },
                new TicketType
                {
                    TicketTypeId = 2,
                    TicketTypeName = "For improvement"
                });

            base.Seed(context);
        }
    }
}