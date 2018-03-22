using System.Threading;
using System.Threading.Tasks;
using CodingMilitia.Grpc.Client;
using CodingMilitia.Grpc.Client.Internal;
using CodingMilitia.Grpc.HelloWorld.Service;

namespace CodingMilitia.Grpc.HelloWorld.Client
{
    class HammeredSampleServiceClient : GrpcClientBase, ISampleService
    {
        protected HammeredSampleServiceClient(GrpcClientOptions options) : base(options)
        {
        }

        public Task<SampleResponse> SendAsync(SampleRequest request, CancellationToken ct)
        {
            return CallUnaryMethodAsync<SampleRequest, SampleResponse>(request, ct);
        }
    }
}