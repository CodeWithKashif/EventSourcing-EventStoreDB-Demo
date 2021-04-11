using System.Collections.Generic;
using EventStoreDB.Demo.DomainEvents;

namespace EventStoreDB.Demo.DomainModels
{
    public class BankAccount
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<Transaction> Transactions = new List<Transaction>();

        public BankAccount() { }

        public void Apply(AccountCreated @event)
        {
            AccountNumber = @event.AccountNumber;
            Name = @event.CustomerName;
            CurrentBalance = 0;
        }

        public void Apply(FundsDeposited @event)
        {
            var newTransaction = new Transaction { AccountNumber = @event.AccountNumber, Amount = @event.Amount };
            Transactions.Add(newTransaction);
            CurrentBalance = CurrentBalance + @event.Amount;
        }

        public void Apply(FundsWithdrawn @event)
        {
            var newTransaction = new Transaction { AccountNumber = @event.AccountNumber, Amount = @event.Amount };
            Transactions.Add(newTransaction);
            CurrentBalance = CurrentBalance - @event.Amount;
        }

    }
}