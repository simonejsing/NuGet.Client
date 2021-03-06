﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Console = NuGet.Common.Console;

namespace NuGet.CommandLine.Test
{
    public class ConsoleCredentialProviderTest
    {
        private static readonly Uri Uri = new Uri("http://fake/");

        [Fact]
        public async Task ConsoleCredentialProvider_FailsWhenNonInteractive()
        {
            // Arrange
            var console = new Console();
            var provider = new ConsoleCredentialProvider(console);

            // Act
            var actual = await provider.Get(Uri, null, true, false, true, CancellationToken.None);

            // Assert
            Assert.Equal(Credentials.CredentialStatus.ProviderNotApplicable, actual.Status);
            Assert.Null(actual.Credentials);
        }

    }
}
