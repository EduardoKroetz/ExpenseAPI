using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.DTOs.Expenses;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ExpensesController(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
    }

    [Authorize]
    [HttpGet("{expenseId:guid}")]
    public async Task<IActionResult> GetExpenseByIdAsync([FromRoute] Guid expenseId)
    {
        var expense = await _expenseRepository.GetAsync(expenseId);
        if (expense == null)
        {
            return NotFound(new ResultDto("Despesa não encontrada"));
        }

        return Ok(new ResultDto(expense));
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateExpenseAsync([FromBody] EditorExpenseDto editorExpanseDto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out Guid userId))
        {
            return BadRequest("Id do usuário é inválido. Refaça o login e tente novamente");
        }

        var category = await _categoryRepository.GetAsync(editorExpanseDto.CategoryId);
        if (category == null)
        {
            return NotFound(new ResultDto("Categoria não encontrada"));
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow,
            Name = editorExpanseDto.Name,
            UserId = userId
        };

        await _expenseRepository.CreateAsync(expense);

        return Ok(new ResultDto(new { expense.Id }, "Despesa criada com sucesso!"));
    }

    [Authorize]
    [HttpPut("{expenseId:guid}")]
    public async Task<IActionResult> UpdateExpenseAsync([FromBody] EditorExpenseDto editorExpanseDto, [FromRoute] Guid expenseId)
    {
        var category = await _categoryRepository.GetAsync(editorExpanseDto.CategoryId);
        if (category == null)
        {
            return NotFound(new ResultDto("Categoria não encontrada"));
        }

        var expense = await _expenseRepository.GetAsync(expenseId);
        if (expense == null)
        {
            return NotFound(new ResultDto("Despesa não encontrada"));
        }

        expense.Name = editorExpanseDto.Name;
        expense.CategoryId = editorExpanseDto.CategoryId;

        await _expenseRepository.UpdateAsync(expense);

        return Ok(new ResultDto(new { expense.Id }, "Despesa atualizada com sucesso!"));
    }


    [Authorize]
    [HttpDelete("{expenseId:guid}")]
    public async Task<IActionResult> UpdateExpenseAsync([FromRoute] Guid expenseId)
    {

        var expense = await _expenseRepository.GetAsync(expenseId);
        if (expense == null)
        {
            return NotFound(new ResultDto("Despesa não encontrada"));
        }

        await _expenseRepository.DeleteAsync(expense);

        return Ok(new ResultDto(new { expense.Id }, "Despesa deletada com sucesso!"));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetExpensesAsync([FromQuery] string filter)
    {
        var expenses = await _expenseRepository.FilterAsync(filter);
        return Ok(new ResultDto(expenses));
    }

}
