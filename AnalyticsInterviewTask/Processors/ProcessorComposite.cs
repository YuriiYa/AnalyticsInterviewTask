using AnalyticsInterviewTask.Processors;

namespace AnalyticsInterviewTask;

public class ProcessorComposite : IOrderProcessor
{
    private readonly List<IOrderProcessor> _orderProcessors;

    // In Case we have IoC container use Import all IOrderProcessor with condition
    public ProcessorComposite(IEnumerable<IOrderProcessor> orderProcessors)
    {
        _orderProcessors = new List<IOrderProcessor>(orderProcessors);

    }

    public bool CanProcess(Order order)
    {
        var canProcessProcessors = (from item in _orderProcessors
                                    where item.CanProcess(order)
                                    select item).ToList();

        if (canProcessProcessors.Count > 1)
        {
            // This is assumption
            throw new AnalyticsException($"Only one processor should handled order, choose only one {string.Join(',', canProcessProcessors)}");
        }

        if (canProcessProcessors.Any())
            return true;

        return false;
    }

    public async Task<(string receipt,User newUserState)> ProcessAsync(Order order)
    {
        foreach (var item in from item in _orderProcessors
                             where item.CanProcess(order)
                             select item)
        {
            return await item.ProcessAsync(order);
        }

        throw new AnalyticsException("Noone can process the request");
    }
}
