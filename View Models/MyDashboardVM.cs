using FinancialPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.View_Models
{
    public class MyDashboardVM
    {
        public int BankCnt { get; set; }
        public List<Bank> banks { get; set; }

        public List<Account> BankAccounts { get; set; }
        public int BankAccountCnt { get; set; }

        public int TransactionCnt { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}