﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Lsp.Tests.Integration.Fixtures
{
    public sealed class DefaultClient : IConfigureLanguageClientOptions
    {
        public void Configure(LanguageClientOptions options)
        {
        }
    }


    public partial record Data : IHandlerIdentity
    {
        public string Name { get; init; }
        public Guid Id { get; init; }
        public Nested Child { get; init; }
    }

    public partial record Nested : IHandlerIdentity
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTimeOffset Date { get; init; }
    }
}
