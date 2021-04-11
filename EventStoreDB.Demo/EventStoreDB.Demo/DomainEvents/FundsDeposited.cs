using System;

namespace EventStoreDB.Demo.DomainEvents
{
    public class FundsDeposited : IEvent
    {
        public string AccountNumber { get; private set; }
        public string EventType { get; } = "FundsDeposited";
        public Decimal Amount { get; private set; }

        public FundsDeposited(string accountNumber, decimal amount)
        {
            AccountNumber = accountNumber;
            Amount = amount;
        }
    }
}