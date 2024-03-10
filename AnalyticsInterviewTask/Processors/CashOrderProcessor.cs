using AnalyticsInterviewTask;

namespace AnalyticsInterviewTask.Processors;

public class CashOrderProcessor : OrderProcessorBase
{
    protected override User ProcessPayment(Order order)
    {
        var sum = order.Products.Sum(p => p.Price);
        if (order.User.CashBalance < sum)
            throw new AnalyticsException("Insufficient Cash funds");

        var newCacheBalance = order.User.CashBalance - sum;
        return new User(Name: order.User.Name, CashBalance: newCacheBalance, CreditCardBalance: order.User.CreditCardBalance);
    }

    protected override Task<string> GenerateReceiptAsync(Order order)
    {
        return Task.FromResult($"Receipt for {order.User.Name}:\n{string.Join("\n", order.Products)}\n");
    }

    public override bool CanProcess(Order order)
    {
        return order.OrderOperation == OrderOperation.Cash;
    }
}
