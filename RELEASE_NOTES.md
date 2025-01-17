#### 1.2.1 January 25 2022 ####

* [Bump Akka.NET to 1.4.32](https://github.com/akkadotnet/akka.net/releases/tag/1.4.32)
* [Fix Kafka polling method](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/262)
* [Fix excessively high CPU consumption](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/234)

This is a minor version release that addresses a bug in the underlying `Confluent.Kafka` library. The `Confluent.Kafka.AdminClient` class have a very tight internal update loop that will consume a lot of CPU resource while it is instantiated.  

The update fixed the high idle CPU consumption as shown in these CPU usage benchmark numbers: 

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19041.1415 (2004/May2020Update/20H1)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.100
  [Host]     : .NET Core 3.1.20 (CoreCLR 4.700.21.47003, CoreFX 4.700.21.47101), X64 RyuJIT

Sample duration: 5000 ms  
```

__Before:__

| CPU Usage Mode |         Mean |    StdErr |    StdDev |       Median |      Maximum |
|----------------|-------------:|----------:|----------:|-------------:|-------------:|
| User           | 4,954.688 ms | 12.780 ms | 40.414 ms | 4,968.750 ms | 5,000.000 ms |
| Kernel         |     9.375 ms |  2.421 ms |  7.655 ms |    15.625 ms |    15.625 ms |
| Total          | 4,964.062 ms | 13.082 ms | 41.369 ms | 4,984.375 ms | 5,015.625 ms |

__After:__

| CPU Usage Mode |     Mean |   StdErr |   StdDev |   Median |  Maximum |
|----------------|---------:|---------:|---------:|---------:|---------:|
| User           | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms |
| Kernel         | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms |
| Total          | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms | 0.000 ms |

These changes have no impact on throughput performance of the plugin as seen in these benchmark numbers: 

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19041.1415 (2004/May2020Update/20H1)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.100
  [Host]     : .NET Core 3.1.20 (CoreCLR 4.700.21.47003, CoreFX 4.700.21.47101), X64 RyuJIT

InvocationCount=2000  IterationCount=80  MinWarmupIterationCount=5  
UnrollFactor=1  
```

__Before:__

|                Method |     Mean |    Error |   StdDev |   Median |
|---------------------- |---------:|---------:|---------:|---------:|
|   PlainSinkThroughput | 128.9 μs | 12.04 μs | 31.30 μs | 112.5 μs |
| KafkaClientThroughput | 120.7 μs | 17.21 μs | 45.03 μs | 111.0 μs |

__After:__

|                Method |     Mean |    Error |   StdDev |   Median |
|---------------------- |---------:|---------:|---------:|---------:|
|   PlainSinkThroughput | 123.3 μs |  6.30 μs | 14.36 μs | 129.4 μs |
| KafkaClientThroughput | 117.5 μs | 18.61 μs | 48.70 μs | 110.0 μs |

#### 1.2.0 November 17 2021 ####
* [Bump Akka.NET to 1.4.28](https://github.com/akkadotnet/akka.net/releases/tag/1.4.28)
* [Bump Confluent.Kafka to 1.8.2](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/234)
* [Log consumer errors more explicitly](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/244)
* [Improve consumer polling efficiency](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/241)
* [Fix supervision strategy bug](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/246)
* [Add default supervision strategy](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/254)

#### 1.1.4 September 08 2021 ####

* [Upgrade to Akka.NET 1.4.25](https://github.com/akkadotnet/akka.net/releases/tag/1.4.25)
* [Optimize Consumer polling](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/217)
* [Fix topic partition assignment and revocation bug](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/221)
* [Fix failing Kafka seek causing plain partitioned source to fail](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/225)

__Kafka consumer client polling optimization__

These are the before and after benchmark comparison of the consumer stream throughput.
`KafkaClientThroughput` uses the native client as the baseline number for comparison.

__Before:__
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19041.1165 (2004/May2020Update/20H1)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=5.0.201
  [Host]     : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT
  Job-WTNALI : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT

InvocationCount=2000  IterationCount=100  MinWarmupIterationCount=10  
UnrollFactor=1  
```

|                Method |     Mean |     Error |    StdDev |
|---------------------- |---------:|----------:|----------:|
|   PlainSinkThroughput | 892.8 μs | 225.73 μs | 590.69 μs |
| KafkaClientThroughput | 120.4 μs |  17.29 μs |  45.24 μs |


__After:__
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19041.1165 (2004/May2020Update/20H1)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=5.0.201
  [Host]     : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT
  Job-WTNALI : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT

InvocationCount=20000  IterationCount=20  MinWarmupIterationCount=5  
UnrollFactor=1  
```

|                Method |     Mean |   Error |  StdDev |
|---------------------- |---------:|--------:|--------:|
|   PlainSinkThroughput | 134.0 μs | 0.97 μs | 1.12 μs |
| KafkaClientThroughput | 135.2 μs | 0.21 μs | 0.24 μs |


#### 1.1.3 May 24 2021 ####

* [Add ConnectionChecker feature](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/190)
* Upgrade to Confluent.Kafka 1.7.0
* Upgrade to Akka.NET 1.4.20
* [Added support for custom statistics handler](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/204)

#### 1.1.2 March 09 2021 ####

* Upgraded to [Akka.NET v1.4.16](https://github.com/akkadotnet/akka.net/releases/tag/1.4.16)
* Upgrade to Confluent.Kafka 1.6.2
* [Use Decider for choosing producer background error handling](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/185)

#### 1.1.1 December 16 2020 ####

* Upgraded to [Akka.NET v1.4.13](https://github.com/akkadotnet/akka.net/releases/tag/1.4.13)
* Upgrade to Confluent.Kafka 1.5.3
* [Make Akka.Streams.Kafka client compatible with Azure Event Hub Kafka API](https://github.com/akkadotnet/Akka.Streams.Kafka/pull/167)
