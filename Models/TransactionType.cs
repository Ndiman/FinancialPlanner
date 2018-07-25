using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public TransactionType()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }
}