using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var categories = await _categoryRepository.GetAsync();
        return Ok(new ResultDto(categories));
    }
    
}
