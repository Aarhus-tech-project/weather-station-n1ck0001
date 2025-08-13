using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace _2WeekCaseApi.Common.Interfaces.Factories;

public interface IResponseFactory
{
    IActionResult CreateResponse<T>(Result<T> result);
}