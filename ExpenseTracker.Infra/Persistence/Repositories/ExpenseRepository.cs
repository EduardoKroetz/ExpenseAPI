﻿using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infra.Persistence.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    public readonly ExpenseTrackerDbContext _context;

    public ExpenseRepository(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expense expense)
    {
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<Expense?> GetAsync(Guid expenseId)
    {
        return await _context.Expenses
            .Include(x => x.User)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id.Equals(expenseId));
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Expense>> FilterAsync(string filter)
    {
        DateTime date = DateTime.UtcNow;
        switch (filter)
        {
            case "week":
                date = DateTime.UtcNow.AddDays(-7);
                break;
            case "month":
                date = DateTime.UtcNow.AddMonths(-1);
                break;
            case "three-months":
                date = DateTime.UtcNow.AddMonths(-3);
                break;
        }

        return await _context.Expenses
            .Where(x => x.CreatedAt > date)
            .Include(x => x.User)
            .Include(x => x.Category)
            .ToListAsync();
    }
}
