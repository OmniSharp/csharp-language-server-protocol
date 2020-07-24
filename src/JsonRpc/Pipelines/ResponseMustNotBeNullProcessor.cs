using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Pipelines
{
    public class ResponseMustNotBeNullProcessor<T, R> : MediatR.Pipeline.IRequestPostProcessor<T, R>
    {
        public Task Process(T request, R response, CancellationToken cancellationToken)
        {
            if (typeof(R).IsClass && EqualityComparer<R>.Default.Equals(response, default))
                throw new ArgumentNullException(nameof(request), $"Pipeline response ({typeof(R).FullName}) must not be null");
            return Task.CompletedTask;
        }
    }
}
