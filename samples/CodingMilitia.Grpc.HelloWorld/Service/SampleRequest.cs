namespace CodingMilitia.Grpc.HelloWorld.Service
{
    [ProtoBuf.ProtoContract]
    public class SampleRequest
    {
        [ProtoBuf.ProtoMember(1)]
        public int Value { get; set; }
    }
}