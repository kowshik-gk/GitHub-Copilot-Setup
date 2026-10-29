using OpenAI_DotNet_Integration.Domain.Dtos;

namespace OpenAI_DotNet_Integration.Domain.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<CustomerResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);

    Task<CustomerResponse> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
