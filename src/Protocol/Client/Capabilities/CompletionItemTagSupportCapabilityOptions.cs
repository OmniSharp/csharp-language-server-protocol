﻿using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionItemTagSupportCapabilityOptions
    {
        /// <summary>
        /// The tags supported by the client.
        /// </summary>
        public Container<CompletionItemTag> ValueSet { get; set; } = null!;
    }
}
