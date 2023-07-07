using NotificationService.Exceptions;

namespace NotificationService.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware

    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException badRequestException)
            {
                _logger.LogError(badRequestException, badRequestException.Message);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequestException.Message);
            }
            catch (NotFoundException notFoundException)
            {
                _logger.LogError(notFoundException, notFoundException.Message);
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (UnprocessableContentException unprocessableContent)
            {
                _logger.LogError(unprocessableContent, unprocessableContent.Message);
                context.Response.StatusCode = 422;
                await context.Response.WriteAsync(unprocessableContent.Message);
            }
            catch (UnauthorizedAccessException unauthorizedAcces)
            {
                _logger.LogError(unauthorizedAcces, unauthorizedAcces.Message);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(unauthorizedAcces.Message);
            }
            catch (AccessForbiddenException forbiddenException)
            {
                _logger.LogError(forbiddenException, forbiddenException.Message);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(forbiddenException.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = 500;
                //await context.Response.WriteAsync(e.Message);
                await context.Response.WriteAsync("Unknown exception occured");
            }
        }
    }
}