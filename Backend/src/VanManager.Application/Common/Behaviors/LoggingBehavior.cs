using MediatR;
using Microsoft.Extensions.Logging;

namespace VanManager.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("[START] {RequestName} {@Request}", 
            requestName, request);

        try
        {
            var result = await next();
            
            _logger.LogInformation("[END] {RequestName} completed successfully", 
                requestName);
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ERROR] {RequestName} failed with error: {Error}", 
                requestName, ex.Message);
                
            throw;
        }
    }
}