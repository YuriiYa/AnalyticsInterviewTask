using AnalyticsInterviewTask;

namespace AnalyticsInterviewTask.Processors;

public interface IOrderProcessor
{
    bool CanProcess(Order order);
    Task<(string receipt, User newUserState)> ProcessAsync(Order order);
}
