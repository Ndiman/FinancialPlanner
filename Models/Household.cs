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

        public virtual List<Account> Accounts { get; set; }
        public virtual List<Budget> Budgets { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual List<Invitation> Invitations { get; set; }

        public Household()
        {
            this.Accounts = new List<Account>();
            this.Budgets = new List<Budget>();
            this.Users = new List<ApplicationUser>();
            this.Invitations = new List<Invitation>();
        }
    }
}