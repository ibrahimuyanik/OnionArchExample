using FluentValidation;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;

namespace Onion.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
			try
			{
				await next(httpContext);
			}
			catch (Exception ex)
			{
				await HadleExceptionAsync(httpContext, ex);

            }
        }

		private static Task HadleExceptionAsync(HttpContext httpContext, Exception exception)
		{
			int statusCode = GetStatusCode(exception);
			httpContext.Response.ContentType = "application/json";
			httpContext.Response.StatusCode = statusCode;

			if (exception.GetType() == typeof(ValidationException))  // eğer hata fluent validation hatası ise burası çalışacak
			{
				return httpContext.Response.WriteAsync(new ExceptionModel
				{
					Errors = ((ValidationException)exception).Errors.Select(x => x.ErrorMessage),
					StatusCode = StatusCodes.Status400BadRequest // doğrulama hatası alındığında hata kodu hep 400'dür değişmez
				}.ToString());
			}


			List<string> errors = new()
			{
				$"Hata Mesajı : {exception.Message}"
			};

			return httpContext.Response.WriteAsync(new ExceptionModel() { Errors = errors, StatusCode = statusCode}.ToString());
			// new ExceptionModel'de değerleri verdikten sonra ToString() metodunu çağırdık bu metodu modelin içinde override etmiştik
			// gelecek olan hata mesajlarını serialize işlemi yapacak.

		}

		private static int GetStatusCode(Exception exception) =>
			exception switch
			{
				BadRequestException => StatusCodes.Status400BadRequest,
				NotFoundException => StatusCodes.Status400BadRequest,
				ValidationException => StatusCodes.Status422UnprocessableEntity,
				_ => StatusCodes.Status500InternalServerError
			};
    }
}
