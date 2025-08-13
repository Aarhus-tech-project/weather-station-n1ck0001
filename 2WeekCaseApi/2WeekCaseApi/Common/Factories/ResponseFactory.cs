using _2WeekCaseApi.Common.Interfaces.Factories;
using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace _2WeekCaseApi.Common.Factories;

public class ResponseFactory : IResponseFactory
{
    public IActionResult CreateResponse<T>(Result<T> result)
    {
        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Value),
            404 => new NotFoundResult(),
            204 => new NoContentResult(),
            201 => new CreatedResult("Created", result.Value),
            400 => new BadRequestResult()
        };
    }
}