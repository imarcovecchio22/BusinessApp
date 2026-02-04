using BusinessApp.Domain.Common;

namespace BusinessApp.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; } // DNI, CUIT, etc.
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}
