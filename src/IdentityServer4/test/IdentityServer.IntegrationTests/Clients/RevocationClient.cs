// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Duende.IdentityModel.Client;
using IdentityServer.IntegrationTests.Clients.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class RevocationClient: IAsyncLifetime
    {
        private const string TokenEndpoint = "https://server/connect/token";
        private const string RevocationEndpoint = "https://server/connect/revocation";
        private const string IntrospectionEndpoint = "https://server/connect/introspect";

        private HttpClient _client;
        private IHost _host;

        public async ValueTask DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        public async ValueTask InitializeAsync()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer() // If using TestServer
                        //.UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>();
                    //.UseKestrel();
                })
                .Build();
            await _host.StartAsync();

            var server = _host.GetTestServer();

            _client = server.CreateClient();
        }

        [Fact]
        public async Task Revoking_reference_token_should_invalidate_token()
        {
            // request acccess token
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "secret",

                Scope = "api1",
                UserName = "bob",
                Password = "bob"
            });

            response.IsError.Should().BeFalse();

            // introspect - should be active
            var introspectionResponse = await _client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = IntrospectionEndpoint,
                ClientId = "api",
                ClientSecret = "secret",

                Token = response.AccessToken
            });

            introspectionResponse.IsActive.Should().Be(true);

            // revoke access token
            var revocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = RevocationEndpoint,
                ClientId = "roclient.reference",
                ClientSecret = "secret",

                Token = response.AccessToken
            });

            // introspect - should be inactive
            introspectionResponse = await _client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = IntrospectionEndpoint,
                ClientId = "api",
                ClientSecret = "secret",

                Token = response.AccessToken
            });

            introspectionResponse.IsActive.Should().Be(false);
        }
    }
}