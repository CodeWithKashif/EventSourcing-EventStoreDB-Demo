using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStoreDB.Demo.DomainEvents;
using EventStoreDB.Demo.DomainModels;
using Newtonsoft.Json;

namespace EventStoreDB.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            IEventStoreConnection connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync();

            //Here input(Account Number) is the Aggregate Id
            Console.WriteLine("Please enter Account Number:");
            string aggregateId = Console.ReadLine();

            var eventsToRun = new List<IEvent>
            {
                new AccountCreated(accountNumber: aggregateId, "Kashif"),
                new FundsDeposited(accountNumber: aggregateId, 150),
                new FundsDeposited(accountNumber: aggregateId, 100),
                new FundsWithdrawn(accountNumber: aggregateId, 60),
                new FundsWithdrawn(accountNumber: aggregateId, 94),
                new FundsDeposited(accountNumber: aggregateId, 4)
            };

            PostEventsToEventStore(eventsToRun, connection, aggregateId);

            GetEventStoreData(connection, aggregateId);

            Console.ReadLine();
        }

        private static void PostEventsToEventStore(List<IEvent> eventsToRun, IEventStoreConnection connection, string aggregateId)
        {
            foreach (IEvent item in eventsToRun)
            {
                string jsonString = JsonConvert.SerializeObject(item,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.None});

                byte[] jsonPayload = Encoding.UTF8.GetBytes(jsonString);
                var eventStoreDataType = new EventData(eventId: Guid.NewGuid(), type: item.EventType,
                    isJson: true, data: jsonPayload, null);
                WriteResult writeResult =connection.AppendToStreamAsync(GetStreamName(aggregateId), ExpectedVersion.Any, eventStoreDataType).Result;
            }
        }

        private static void GetEventStoreData(IEventStoreConnection connection, string aggregateId)
        {
            Task<StreamEventsSlice> results = Task.Run(() =>
                connection.ReadStreamEventsForwardAsync(GetStreamName(aggregateId), StreamPosition.Start, 999,
                    false));
            Task.WaitAll();

            StreamEventsSlice resultsData = results.Result;
            var bankState = new BankAccount();

            foreach (ResolvedEvent evnt in resultsData.Events)
            {
                string esJsonData = Encoding.UTF8.GetString(evnt.Event.Data);
                Console.WriteLine("\n" + esJsonData);

                switch (evnt.Event.EventType)
                {
                    case "AccountCreated":
                    {
                        var objState = JsonConvert.DeserializeObject<AccountCreated>(esJsonData);
                        bankState.Apply(objState);
                        break;
                    }
                    case "FundsDeposited":
                    {
                        var objState = JsonConvert.DeserializeObject<FundsDeposited>(esJsonData);
                        bankState.Apply(objState);
                        break;
                    }
                    default:
                    {
                        var objState = JsonConvert.DeserializeObject<FundsWithdrawn>(esJsonData);
                        bankState.Apply(objState);
                        break;
                    }
                }

                Console.WriteLine("Current Balance: " + bankState.CurrentBalance);
            }
        }

        private static string GetStreamName(string id)
        {
            return $"BankAccount-{id}";
        }

    }
}
