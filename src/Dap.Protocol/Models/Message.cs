﻿using System.Collections.Generic;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A structured message object. Used to return errors from requests.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Unique identifier for the message.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// A format string for the message. Embedded variables have the form '{name}'.
        /// If variable name starts with an underscore character, the variable does not contain user data (PII) and can be safely used for telemetry purposes.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// An object used as a dictionary for looking up the variables in the format string.
        /// </summary>
        [Optional] public IDictionary<string, string> Variables { get; set; }

        /// <summary>
        /// If true send to telemetry.
        /// </summary>
        [Optional] public bool? SendTelemetry { get; set; }

        /// <summary>
        /// If true show user.
        /// </summary>
        [Optional] public bool? ShowUser { get; set; }

        /// <summary>
        /// An optional url where additional information about this message can be found.
        /// </summary>
        [Optional] public string Url { get; set; }

        /// <summary>
        /// An optional label that is presented to the user as the UI for opening the url.
        /// </summary>
        [Optional] public string UrlLabel { get; set; }
    }
}