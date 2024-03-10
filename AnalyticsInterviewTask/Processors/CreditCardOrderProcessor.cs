using AnalyticsInterviewTask;

namespace AnalyticsInterviewTask.Processors;

public class CreditCardOrderProcessor : OrderProcessorBase
{
    protected override User ProcessPayment(Order order)
    {
        var sum = order.Products.Sum(p => p.Price);
        var newCreditCardBalance = order.User.CreditCardBalance - sum;
        return new User(Name: order.User.Name, CashBalance: order.User.CashBalance, CreditCardBalance: newCreditCardBalance);
    }

    /// <summary>
    /// <exception cref="AnalyticsException"Error generating receipt</exception>
    /// </summary>

    protected override async Task<string> GenerateReceiptAsync(Order order)
    {
        await Task.Delay(1000);
        return $"Card Receipt for {order.User.Name}:\n{string.Join("\n", order.Products)}\n";
    }

    public override bool CanProcess(Order order)
    {
        return order.OrderOperation == OrderOperation.CreditCard;
    }
}
