using ProtoBuf;
using System.Collections.Generic;

namespace ProtoBufferConsoleAppExample
{
    [ProtoContract]
    public class Address
    {
        [ProtoMember(1, IsRequired = true)]
        public string Line1 { get; set; }

        [ProtoMember(2)]
        public string Line2 { get; set; }
    }
    [ProtoContract]
    public class Person
    {
        [ProtoMember(1, IsRequired = true)]
        public int Id { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public IList<Address> Address { get; set; }
    }
}
