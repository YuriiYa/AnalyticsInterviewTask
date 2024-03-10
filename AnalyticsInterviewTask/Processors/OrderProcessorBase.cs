using AnalyticsInterviewTask;

namespace AnalyticsInterviewTask.Processors;

// Consider that ProcessPayment and GenerateReceiptAsync 2 operation that always goes together
// Consider that can be more processors
public abstract class OrderProcessorBase : IOrderProcessor
{
    public abstract bool CanProcess(Order order);
    protected abstract User ProcessPayment(Order order);

    protected abstract Task<string> GenerateReceiptAsync(Order order);

    public async Task<(string receipt, User newUserState)> ProcessAsync(Order order) // Why crashing here is not good
    {
        Console.WriteLine($"{GetType().Name}: Processing order for [{order.User}]");

        try
        {
            Console.WriteLine($"{GetType().Name}: Processing payment");
            var newUserState = ProcessPayment(order);

            Console.WriteLine($"{GetType().Name}: Generating Receipt");
            return (await GenerateReceiptAsync(order), newUserState);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{GetType().Name}: Error processing order: {e.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine($"{GetType().Name}: Order Processed");
        }
    }
}
