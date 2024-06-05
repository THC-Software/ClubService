using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/admins")]
[ApiController]
[ApiVersion("1.0")]
public class AdminController(
    IRegisterAdminService registerAdminService,
    IDeleteAdminService deleteAdminService,
    IUpdateAdminService updateAdminService,
    IAdminReadModelRepository adminReadModelRepository)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> RegisterAdmin(
        [FromBody] AdminRegisterCommand adminRegisterCommand)
    {
        var registeredAdminId = await registerAdminService.RegisterAdmin(adminRegisterCommand);
        return CreatedAtAction(nameof(RegisterAdmin), new { id = registeredAdminId }, registeredAdminId);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> DeleteAdmin(Guid id)
    {
        var deletedAdminId = await deleteAdminService.DeleteAdmin(id);
        return Ok(deletedAdminId);
    }
    
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> UpdateAdmin(Guid id, [FromBody] AdminUpdateCommand adminUpdateCommand)
    {
        var updatedAdminId = await updateAdminService.ChangeFullName(
            id,
            adminUpdateCommand.FirstName,
            adminUpdateCommand.LastName
        );
        return Ok(updatedAdminId);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdminReadModel>> GetAdminById(Guid id)
    {
        var adminReadModel = await adminReadModelRepository.GetAdminById(id);
        
        if (adminReadModel == null)
        {
            return NotFound($"Admin with id {id} not found!");
        }
        
        return Ok(adminReadModel);
    }
}