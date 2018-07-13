using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public ICollection<Account> Accounts { get; set; }

        public AccountType()
        {
            this.Accounts = new HashSet<Account>();
        }
    }
}