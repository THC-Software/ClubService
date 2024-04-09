using Asp.Versioning;
using ClubService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/admins")]
[ApiController]
[ApiVersion("1.0")]
public class AdminController
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateAdmin(AdminCreateDto adminCreateDto)
    {
        return await Task.FromResult("");
    }
    
    [HttpDelete("{adminId}")]
    public async Task<ActionResult<string>> DeleteAdmin(string adminId)
    {
        return await Task.FromResult("");
    }
}