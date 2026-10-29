using System.ComponentModel.DataAnnotations;

namespace OpenAI_DotNet_Integration.Domain.Dtos;

/// <summary>Payload for creating a new customer.</summary>
public sealed record CreateCustomerRequest
{
    [Required]
    [StringLength(100)]
    public required string FirstName { get; init; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public required string Email { get; init; }

    [Required]
    [Phone]
    [StringLength(30)]
    public required string PhoneNumber { get; init; }
}

/// <summary>Payload for updating an existing customer.</summary>
public sealed record UpdateCustomerRequest
{
    [Required]
    [StringLength(100)]
    public required string FirstName { get; init; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public required string Email { get; init; }

    [Required]
    [Phone]
    [StringLength(30)]
    public required string PhoneNumber { get; init; }
}

/// <summary>Represents a customer returned by the API.</summary>
public sealed record CustomerResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
