using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Household
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Bank> Banks { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

        public Household()
        {
            this.Banks = new HashSet<Bank>();
            this.Budgets = new HashSet<Budget>();
            this.Users = new HashSet<ApplicationUser>();
            this.Invitations = new HashSet<Invitation>();
        }
    }
}