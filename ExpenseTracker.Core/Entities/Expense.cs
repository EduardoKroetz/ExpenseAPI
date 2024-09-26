namespace ExpenseTracker.Core.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
}
