using OpenAI_DotNet_Integration.Data;
using OpenAI_DotNet_Integration.Domain.Dtos;
using OpenAI_DotNet_Integration.Domain.Exceptions;
using OpenAI_DotNet_Integration.Domain.Interfaces;
using OpenAI_DotNet_Integration.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace OpenAI_DotNet_Integration.Infrastructure.Services;

public sealed class CustomerService(CustomerDbContext dbContext) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Id)
            .ToListAsync(cancellationToken);

        return customers.Select(MapToResponse).ToList();
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        if (await dbContext.Customers.AnyAsync(customer => customer.Email == email, cancellationToken))
        {
            throw new ConflictException($"A customer with email '{email}' already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        var email = request.Email.Trim();

        if (await dbContext.Customers.AnyAsync(entity => entity.Id != id && entity.Email == email, cancellationToken))
        {
            throw new ConflictException($"A customer with email '{email}' already exists.");
        }

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.Email = email;
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CustomerResponse MapToResponse(Customer customer)
        => new(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.CreatedAt,
            customer.UpdatedAt);
}
