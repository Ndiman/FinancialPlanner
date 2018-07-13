using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal SpendingTarget { get; set; }
        public decimal CurrentBalance { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }

    }
}