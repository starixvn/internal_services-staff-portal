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

using System.Collections.Generic;
using System.Linq;

namespace TicketDesk.Domain.Model
{
    public static class UserTicketListFilterColumnExtensions
    {
        /// <summary>
        /// Changes the preferences to filter by the specified assigned user.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="assigned">The assigned user to filter by.</param>
        internal static void ChangeAssignedFilter(this ICollection<UserTicketListFilterColumn> filters, string assigned)
        {
            if (!string.IsNullOrEmpty(assigned))
            {
                var fColumn = filters.SingleOrDefault(fc => fc.ColumnName == "AssignedTo");

                // TODO: need an invariant value with Localized display text
                if (assigned == "anyone")
                {
                    if (fColumn != null)
                    {
                        filters.Remove(fColumn);
                    }
                }
                else
                {
                    if (fColumn == null)
                    {
                        fColumn = new UserTicketListFilterColumn("AssignedTo")
                        {
                            ColumnValueType = typeof(string)
                        };
                        filters.Add(fColumn);
                    }

                    // TODO: need an invariant value with Localized display text
                    if (assigned == "unassigned")
                    {
                        fColumn.UseEqualityComparison = null;
                        fColumn.ColumnValue = null;
                    }
                    else
                    {
                        fColumn.UseEqualityComparison = true;
                        fColumn.ColumnValue = assigned;
                    }
                }
            }
        }

        /// <summary>
        /// Changes the preferences to filter by the specified owner.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="ownerValue">The owner name to filter by.</param>
        internal static void ChangeOwnerFilter(this ICollection<UserTicketListFilterColumn> filters, string ownerValue)
        {
            if (!string.IsNullOrEmpty(ownerValue))
            {
                var fColumn = filters.SingleOrDefault(fc => fc.ColumnName == "Owner");

                // TODO: need an invariant value with Localized display text
                if (ownerValue == "anyone")
                {
                    if (fColumn != null)
                    {
                        filters.Remove(fColumn);
                    }
                }
                else
                {
                    if (fColumn == null)
                    {
                        fColumn = new UserTicketListFilterColumn("Owner")
                        {
                            ColumnValueType = typeof(string)
                        };
                        filters.Add(fColumn);
                    }

                    fColumn.UseEqualityComparison = true;
                    fColumn.ColumnValue = ownerValue;
                }
            }
        }

        /// <summary>
        /// Changes the preferences to filter by the specified current status.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="statusValue">The status value to filter by.</param>
        internal static void ChangeTicketStatusFilter(this ICollection<UserTicketListFilterColumn> filters, string statusValue)
        {
            if (!string.IsNullOrEmpty(statusValue))
            {
                var fColumn = filters.SingleOrDefault(fc => fc.ColumnName == "TicketStatus");

                // TODO: need an invariant value with Localized display text
                if (statusValue == "Any")
                {
                    if (fColumn != null)
                    {
                        filters.Remove(fColumn);
                    }
                }
                else
                {
                    // TODO: need an invariant value with Localized display text
                    bool equality = statusValue != "Open";
                    if (fColumn == null)
                    {
                        fColumn = new UserTicketListFilterColumn("TicketStatus")
                        {
                            // enum, doesn't matter which value we call gettype from
                            ColumnValueType = TicketStatus.Closed.GetType()
                        };
                        filters.Add(fColumn);
                    }

                    fColumn.UseEqualityComparison = equality;

                    // TODO: need an invariant value with Localized display text
                    fColumn.ColumnValue = (statusValue == "Open") ? "Closed" : statusValue;
                }
            }
        }
    }
}