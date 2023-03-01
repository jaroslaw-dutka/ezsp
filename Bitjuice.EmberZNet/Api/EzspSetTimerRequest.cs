using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api
{
    public class EzspSetTimerRequest
    {
        [FieldOrder(0)]
        public byte TimerId { get; set; }

        [FieldOrder(1)]
        public ushort Time { get; set; }

        [FieldOrder(2)]
        public EmberEventUnits Units { get; set; }

        [FieldOrder(3)]
        public bool Repeat { get; set; }
    }
}
