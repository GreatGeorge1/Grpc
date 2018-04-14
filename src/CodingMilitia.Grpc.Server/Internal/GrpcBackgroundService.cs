using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CodingMilitia.Grpc.Shared;

namespace CodingMilitia.Grpc.Server.Internal
{
    internal class GrpcBackgroundService<TService> : IHostedService where TService : class, IGrpcService
    {
        private readonly GrpcHost<TService> _host;
        
        public GrpcBackgroundService(GrpcHost<TService> host)
        {
            _host = host;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _host.StartAsync().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _host.StopAsync().ConfigureAwait(false);
        }
    }
}