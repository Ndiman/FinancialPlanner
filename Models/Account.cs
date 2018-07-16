using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int BankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        
        public virtual AccountType Type { get; set; }
        public virtual Bank Bank { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public Account()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }
}