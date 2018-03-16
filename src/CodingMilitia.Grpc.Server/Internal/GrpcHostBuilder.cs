using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CodingMilitia.Grpc.Shared.Internal;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CodingMilitia.Grpc.Server.Internal
{
    public class GrpcHostBuilder<TService> : IGrpcHostBuilder<TService> where TService : class, IGrpcService
    {
        private readonly IServiceProvider _appServices;
        private readonly ServerServiceDefinition.Builder _builder;
        private Uri _uri = new Uri("http://127.0.0.1", UriKind.Absolute);
        private int _port = 5000;

        public GrpcHostBuilder(IServiceProvider appServices)
        {
            _appServices = appServices;
            _builder = ServerServiceDefinition.CreateBuilder();
        }

        public IGrpcHostBuilder<TService> SetUri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public IGrpcHostBuilder<TService> SetPort(int port)
        {
            _port = port;
            return this;
        }

        public IGrpcHostBuilder<TService> AddUnaryMethod<TRequest, TResponse>(
            Func<TService, TRequest, CancellationToken, Task<TResponse>> handler,
            string serviceName,
            string methodName
        )
            where TRequest : class
            where TResponse : class
        {
            _builder.AddMethod(
                CreateMethodDefinition<TRequest, TResponse>(MethodType.Unary, serviceName, methodName),
                async (request, context) =>
                {
                    using (var scope = _appServices.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<TService>();
                        var baseService = service as GrpcServiceBase;
                        if (baseService != null)
                        {
                            baseService.Context = context;
                        }
                        return await handler(service, request, CancellationToken.None);
                    }
                }
            );
            return this;
        }

        private static Method<TRequest, TResponse> CreateMethodDefinition<TRequest, TResponse>(
            MethodType methodType,
            string serviceName,
            string methodName
        )
            where TRequest : class
            where TResponse : class
        {
            return new Method<TRequest, TResponse>(
                type: methodType,
                serviceName: serviceName,
                name: methodName,
                requestMarshaller: Marshallers.Create(
                    serializer: Serializer<TRequest>.ToBytes,
                    deserializer: Serializer<TRequest>.FromBytes
                ),
                responseMarshaller: Marshallers.Create(
                    serializer: Serializer<TResponse>.ToBytes,
                    deserializer: Serializer<TResponse>.FromBytes
                )
            );
        }

        internal GrpcHost<TService> Build()
        {
            return new GrpcHost<TService>(_builder.Build());
        }
    }
}