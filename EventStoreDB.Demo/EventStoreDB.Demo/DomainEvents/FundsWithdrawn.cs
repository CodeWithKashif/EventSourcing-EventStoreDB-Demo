using System;

namespace EventStoreDB.Demo.DomainEvents
{
    public class FundsWithdrawn : IEvent
    {
        public string AccountNumber { get; private set; }
        public string EventType { get; } = "FundsWithdrawn";
        public Decimal Amount { get; private set; }

        public FundsWithdrawn(string accountNumber, decimal amount)
        {
            AccountNumber = accountNumber;
            Amount = amount;
        }
    }
}