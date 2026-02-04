using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessApp.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createUserDto);
        }

        try
        {
            await _userService.CreateAsync(createUserDto);
            _logger.LogInformation("Usuario creado: {Email}", createUserDto.Email);
            TempData["Success"] = "Usuario creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario: {Email}", createUserDto.Email);
            ModelState.AddModelError("", ex.Message);
            return View(createUserDto);
        }
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateUserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Roles = user.Roles
        };

        ViewBag.UserId = id;
        return View(updateDto);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            return View(updateUserDto);
        }

        try
        {
            await _userService.UpdateAsync(id, updateUserDto);
            _logger.LogInformation("Usuario actualizado: {Id}", id);
            TempData["Success"] = "Usuario actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario: {Id}", id);
            ModelState.AddModelError("", ex.Message);
            ViewBag.UserId = id;
            return View(updateUserDto);
        }
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var result = await _userService.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Usuario eliminado: {Id}", id);
                TempData["Success"] = "Usuario eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el usuario";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario: {Id}", id);
            TempData["Error"] = "Error al eliminar el usuario";
        }

        return RedirectToAction(nameof(Index));
    }
}
