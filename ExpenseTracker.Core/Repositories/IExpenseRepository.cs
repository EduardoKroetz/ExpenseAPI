using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetAsync(Guid expenseId);
    Task CreateAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
    Task<IEnumerable<Expense>> FilterAsync(string filter);
}
