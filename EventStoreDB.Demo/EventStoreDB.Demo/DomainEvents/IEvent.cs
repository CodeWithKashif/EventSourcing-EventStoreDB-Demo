namespace EventStoreDB.Demo.DomainEvents
{
    public interface IEvent
    {
        string EventType { get; }
    }
}
