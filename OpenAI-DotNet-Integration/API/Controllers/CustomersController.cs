using OpenAI_DotNet_Integration.Domain.Dtos;
using OpenAI_DotNet_Integration.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace OpenAI_DotNet_Integration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetCustomers(CancellationToken cancellationToken)
    {
        var customers = await customerService.GetAllAsync(cancellationToken);
        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetCustomerById(int id, CancellationToken cancellationToken)
    {
        var customer = await customerService.GetByIdAsync(id, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var createdCustomer = await customerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> UpdateCustomer(
        int id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updatedCustomer = await customerService.UpdateAsync(id, request, cancellationToken);
        return Ok(updatedCustomer);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
    {
        await customerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
