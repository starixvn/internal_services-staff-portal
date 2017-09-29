// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

using System.Configuration;
using System.Linq;
using TicketDesk.Domain.Model;

namespace TicketDesk.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

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
                context.Projects.Add(new Project() { ProjectName = "Default", ProjectDescription = "Default project" });
            }

            if (!string.IsNullOrEmpty(demoMode) && demoMode.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                DemoDataManager.SetupDemoData(context);
            }

            //context.TicketCategories.AddOrUpdate(
            //    new TicketCategory
            //    {
            //        CategoryId = 1,
            //        Name = "Facility"
            //    },
            //    new TicketCategory
            //    {
            //        CategoryId = 2,
            //        Name = "Hardware"
            //    },
            //    new TicketCategory
            //    {
            //        CategoryId = 3,
            //        Name = "Software"
            //    },
            //    new TicketCategory
            //    {
            //        CategoryId = 4,
            //        Name = "Network"
            //    },
            //    new TicketCategory
            //    {
            //        CategoryId = 5,
            //        Name = "Customer Support"
            //    },
            //    new TicketCategory
            //    {
            //        CategoryId = 6,
            //        Name = "Incident"
            //    });

            base.Seed(context);
        }
    }
}