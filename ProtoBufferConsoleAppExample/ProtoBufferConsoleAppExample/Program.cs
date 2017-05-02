using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ProtoBuf;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProtoBufferConsoleAppExample
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                Rabbit rabbitObject = new Rabbit();
                var person = new Person() { Id = 7, Name = "Mr. Bond...James Bond!"};

                var address1 = new Address() { Line1 = "Test Address 2 - Line 1", Line2 = "Test Address 2 - Line 2" };
                var address2 = new Address() { Line1 = "Teste Address 4 - Line 1", Line2 = "Test Address 4 - Line 2" };

                var adresses = new List<Address>();
                adresses.Add(address1);
                adresses.Add(address2);

                person.Address = adresses;

                Stopwatch cronometro = new Stopwatch();
                MemoryStream msTestString = new MemoryStream();

                cronometro.Start();
                Serializer.Serialize(msTestString, person);
                cronometro.Stop();
                Console.WriteLine($"Time to Serialize with ProtoBuffer object: {cronometro.ElapsedMilliseconds} ms.");
                cronometro.Reset();

                string stringBase64 = Convert.ToBase64String(msTestString.ToArray());
                byte[] byteAfter64 = Convert.FromBase64String(stringBase64);

                MemoryStream afterStream = new MemoryStream(byteAfter64);
                rabbitObject.Publish(stringBase64);
                
                cronometro.Start();
                Person person2 = Serializer.Deserialize<Person>(afterStream); // Just to check correct serialization.
                cronometro.Stop();
                Console.WriteLine($"Time to Deserialize with ProtoBuffer object: {cronometro.ElapsedMilliseconds} ms.");
                cronometro.Reset();

                cronometro.Start();
                var objectSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(person);
                cronometro.Stop();
                Console.WriteLine($"Time to Serialize with Json Convert object: {cronometro.ElapsedMilliseconds} ms.");
                cronometro.Reset();

                cronometro.Start();
                var personAfterSerializes = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(objectSerialized);
                cronometro.Stop();
                Console.WriteLine($"Time to Serialize with Json Convert object: {cronometro.ElapsedMilliseconds} ms.");
                cronometro.Reset();

                //rabbitObject.Subscribe();

            } catch(Exception exception) {
                Console.WriteLine(exception.Message);
            }
        }
    }
}