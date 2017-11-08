namespace TicketDesk.Web.Client.Models.Extensions
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.Core.Domain;

    public class MenuItem : IMenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IMenuItem Parent { get; set; }

        public IFunction Function { get; set; }

        public ISite Site { get; set; }

        public IUser Creator { get; set; }

        public IUser Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public short DisplayOrder { get; set; }

        public string Environment { get; set; }

        public string Url { get; set; }

        public int Policy { get; set; }

        public IEnumerable<MenuItem> Children { get; set; }

        public string IconClass { get; set; }

        public static implicit operator MenuItem(Fanex.NAP.Core.Model.MenuItem napMenuItem)
        {
            MenuItem newMenuItem = new MenuItem
            {
                Name = napMenuItem.Name,
                CreatedDate = napMenuItem.CreatedDate,
                DisplayOrder = napMenuItem.DisplayOrder,
                Parent = napMenuItem.Parent,
                Description = napMenuItem.Description,
                Url = napMenuItem.Url,
                Creator = napMenuItem.Creator,
                Environment = napMenuItem.Environment,
                Function = napMenuItem.Function,
                Id = napMenuItem.Id,
                IsActive = napMenuItem.IsActive,
                ModifiedDate = napMenuItem.ModifiedDate,
                Modifier = napMenuItem.Modifier,
                Policy = napMenuItem.Policy,
                Site = napMenuItem.Site,
                IconClass = MenuIconMapper.MenuIconDictionary[napMenuItem.Url]
            };


            return newMenuItem;
        }
    }
}