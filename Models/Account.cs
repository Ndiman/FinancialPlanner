using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int AccountTypeId { get; set; }
        public int BankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        
        public virtual AccountType AccountType { get; set; }
        public virtual Bank Bank { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public Account()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }
}