﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace OmniSharp.Extensions.JsonRpc.Pipelines
{
    public class RequestMustNotBeNullProcessor<T> : IRequestPreProcessor<T>
    {
        public Task Process(T request, CancellationToken cancellationToken)
        {
            if (typeof(T).IsClass && EqualityComparer<T>.Default.Equals(request, default))
                throw new ArgumentNullException(nameof(request), $"Pipeline request ({typeof(T).FullName}) must not be null");
            return Task.CompletedTask;
        }
    }
}
