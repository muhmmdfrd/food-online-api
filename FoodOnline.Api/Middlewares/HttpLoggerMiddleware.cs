using Microsoft.IO;
using MimeKit;

namespace FoodOnline.Api.Middlewares;

public class HttpLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpLoggerMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly IConfiguration _config;

    public HttpLoggerMiddleware(
        RequestDelegate next,
        ILogger<HttpLoggerMiddleware> logger,
        IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        _config = config;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = Guid.NewGuid().ToString().ToUpper();
        var notAllowedEndpoint = new[] {"/auth"};
        var notAllowed = notAllowedEndpoint.Any(q => httpContext.Request.Path.Value?.Contains(q) == true);

        await LogRequest(httpContext, correlationId, notAllowed);
        await LogResponse(httpContext, correlationId, notAllowed);   
    }

    private static string GetExtension(string contentType)
    {
        MimeTypes.TryGetExtension(contentType, out string ext);
        return ext;
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
         
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0); 
        
        return textWriter.ToString();    
    }

    private async Task LogRequest(HttpContext context, string correlationId, bool notAllowed = false)
    {
        if (context.Request.Headers.ContainsKey("X-Correlation-ID"))
        {
            context.Request.Headers["X-Correlation-ID"] = correlationId;
        }
        else
        {
            context.Request.Headers.Append("X-Correlation-ID", correlationId);
        }

        context.Request.EnableBuffering();
        
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);
        
        _logger.LogInformation($"\nHTTP Request Information:\n" +
                $"Method:{context.Request.Method}\n" +
                $"Path:{context.Request.Path}\n" +
                $"QueryString:{context.Request.QueryString}\n" +
                $"Headers:{FormatHeaders(context.Request.Headers)}\n" +
                $"Schema:{context.Request.Scheme}\n" +
                $"RemoteIpAddress:{context.Connection.RemoteIpAddress}\n" +
                (notAllowed ? "" : $"Body:{ReadStreamInChunks(requestStream)}\n"));

        context.Request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context, string correlationId, bool notAllowed = false)
    {
        var originalBodyStream = context.Response.Body;
        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        {
            context.Response.Body = responseBody;
            await _next(context);

            if (context.Response.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Response.Headers["X-Correlation-ID"] = correlationId;
            }
            else
            {
                context.Response.Headers.Append("X-Correlation-ID", correlationId);
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"\nHttp Response Information:\n" +
                $"StatusCode:{context.Response.StatusCode}\n" +
                $"Request Path:{context.Request.Path}\n" +
                $"ContentType:{context.Response.ContentType}\n" +
                $"Headers:{FormatHeaders(context.Response.Headers)}\n" +
                (notAllowed ? "" : $"Body:{text}\n"));

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private static string FormatHeaders(IHeaderDictionary headers) => string.Join(", ", headers.Select(kvp => $"{{{kvp.Key}: {string.Join(", ", kvp.Value)}}}"));
}