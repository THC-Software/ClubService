using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/admins")]
[ApiController]
[ApiVersion("1.0")]
public class AdminController(IRegisterAdminService registerAdminService)
{
    [HttpPost]
    public async Task<ActionResult<string>> RegisterAdmin(
        [FromBody] AdminRegisterCommand adminRegisterCommand)
    {
        return await registerAdminService.RegisterAdmin(adminRegisterCommand);
    }
    
    [HttpDelete("{adminId}")]
    public async Task<ActionResult<string>> DeleteAdmin(string adminId)
    {
        return await Task.FromResult("");
    }
}