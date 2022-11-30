namespace SilvermineNordic.Api.Services
{
    public class CacheResponseMetadata
    {

    }

    public class AddCacheHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public AddCacheHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<CacheResponseMetadata>() is { } mutateResponseMetadata) 
            {
                if (context.Response.HasStarted)
                {
                    throw new InvalidOperationException();
                }
                context.Response.Headers.CacheControl = new[] { "public", "max-age=3600" };
            }
            await _next(context);
        }
    }
}
