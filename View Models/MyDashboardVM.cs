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
        public List<Bank> MyBanks { get; set; }

        public List<Account> MyBankAccounts { get; set; }
        public int BankAccountCnt { get; set; }

        public int TransactionCnt { get; set; }
        public List<Transaction> MyTransactions { get; set; }
        public List<Transaction> MyBudgetTransactions { get; set; }

        public int BudgetCnt { get; set; }
        public List<Budget> MyBudgets { get; set; }
    }
}