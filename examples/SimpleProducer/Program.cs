﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Kafka.Messages;
using Akka.Streams.Kafka.Settings;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Producer = Akka.Streams.Kafka.Dsl.Producer;

namespace SimpleProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fallbackConfig = ConfigurationFactory.FromResource<ProducerSettings<object, object>>("Akka.Streams.Kafka.reference.conf");

            var system = ActorSystem.Create("TestKafka", fallbackConfig);
            var materializer = system.Materializer();

            var producerSettings = ProducerSettings<Null, string>.Create(system, null, new StringSerializer(Encoding.UTF8))
                .WithBootstrapServers("localhost:9092");

            // producer as a Sink
            Source
                .From(Enumerable.Range(1, 200))
                .Select(c => c.ToString())
                .Select(elem => new ProduceRecord<Null, string>("akka2", null, elem))
                .RunWith(Producer.PlainSink(producerSettings), materializer);

            // producer as a Flow
            Source
                .From(Enumerable.Range(1, 100))
                .Select(c => c.ToString())
                .Select(elem => new ProduceRecord<Null, string>("akka", null, elem))
                .Via(Producer.CreateFlow(producerSettings))
                .Select(result =>
                {
                    var record = result.Result.Metadata;
                    Console.WriteLine($"{record.Topic}/{record.Partition} {result.Result.Offset}: {record.Value}");
                    return result;
                })
                .RunWith(Sink.Ignore<Task<Result<Null, string>>>(), materializer);

            // TODO: producer as a Commitable Sink

            // TODO: Sharing KafkaProducer

            Console.ReadLine();
        }
    }
}
