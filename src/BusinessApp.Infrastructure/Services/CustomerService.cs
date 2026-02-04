using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using BusinessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return null;

        return MapToDto(customer);
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _context.Customers
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return customers.Select(MapToDto).ToList();
    }

    public async Task<List<CustomerDto>> SearchAsync(CustomerSearchDto searchDto)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            var term = searchDto.SearchTerm.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term) ||
                (c.Phone != null && c.Phone.Contains(term)) ||
                (c.TaxId != null && c.TaxId.Contains(term))
            );
        }

        if (searchDto.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == searchDto.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchDto.City))
        {
            query = query.Where(c => c.City == searchDto.City);
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Country))
        {
            query = query.Where(c => c.Country == searchDto.Country);
        }

        var customers = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return customers.Select(MapToDto).ToList();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto createDto)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            Phone = createDto.Phone,
            Address = createDto.Address,
            City = createDto.City,
            Country = createDto.Country,
            PostalCode = createDto.PostalCode,
            TaxId = createDto.TaxId,
            Notes = createDto.Notes,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateDto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            throw new Exception("Cliente no encontrado");
        }

        customer.FirstName = updateDto.FirstName;
        customer.LastName = updateDto.LastName;
        customer.Email = updateDto.Email;
        customer.Phone = updateDto.Phone;
        customer.Address = updateDto.Address;
        customer.City = updateDto.City;
        customer.Country = updateDto.Country;
        customer.PostalCode = updateDto.PostalCode;
        customer.TaxId = updateDto.TaxId;
        customer.IsActive = updateDto.IsActive;
        customer.Notes = updateDto.Notes;

        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            PostalCode = customer.PostalCode,
            TaxId = customer.TaxId,
            IsActive = customer.IsActive,
            Notes = customer.Notes,
            CreatedAt = customer.CreatedAt
        };
    }
}
