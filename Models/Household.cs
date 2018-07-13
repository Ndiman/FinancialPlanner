using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

        public Household()
        {
            this.Accounts = new HashSet<Account>();
            this.Budgets = new HashSet<Budget>();
            this.Users = new HashSet<ApplicationUser>();
            this.Invitations = new HashSet<Invitation>();
        }
    }
}