namespace EventStoreDB.Demo.DomainEvents
{
    public class AccountCreated : IEvent
    {
        /// <summary>
        /// AccountNumber is Aggregate Id
        /// </summary>
        public string AccountNumber { get; private set; }
        public string EventType { get; } = "AccountCreated";
        public string CustomerName { get; private set; }

        public AccountCreated(string accountNumber, string customerName)
        {
            AccountNumber = accountNumber;
            CustomerName = customerName;
        }
    }
}