
using AnalyticsInterviewTask.Processors;

namespace AnalyticsInterviewTask;


// TODO: some considerations
// 1. Move all messages in separate class to be able to access them from  Tests as well + to be able to localize them
// 2. Think about possibility of splitting Process and Report. Right now they are both using together but can be other needs
// 3. It is just few examples of the tests can be few times more
// 4. Main ca be moved to a separate class and can be tested as well
public enum OrderOperation
{
    Unknown,
    Cash,
    CreditCard
}

public record User(string Name, double CashBalance, double CreditCardBalance)
{
    public override string ToString() =>
        $"Name: {Name}; CashBalance: {CashBalance:N2}; CreditCardBalance: {CreditCardBalance:N2}";
}

public record Product(string Name, double Price);

public record Order(User User, IEnumerable<Product> Products, OrderOperation OrderOperation);


public static partial class Program
{
    /// <summary>
    /// <exception cref="AnalyticsException"Product is out of stock</exception>
    /// </summary>
    public static IEnumerable<Product> GetCartProducts()
    {
        yield return new("Laptop", 800);
        yield return new("Smartphone", 700);
    }

    public static async Task Main()
    {
        Console.WriteLine("Starting Application");

        Console.WriteLine("Choose the operation type: C - paying by cache; Cd - paying by card");
        var isPaingByCard = Console.ReadLine();
        OrderOperation orderOperation;
        User user = new("John Smith", 1000, 1000);
        IEnumerable<Product> products;

        try
        {
            products = GetCartProducts().ToList();
        }
        catch (AnalyticsException ex)
        {
            Console.WriteLine($"Order can't be processed: {ex.Message}. Check the correctness of the data");
            return;
        }

        orderOperation = isPaingByCard switch
        {
            "C" => OrderOperation.Cash,
            "Cd" => OrderOperation.CreditCard,
            _ => OrderOperation.Unknown
        };

        if (orderOperation == OrderOperation.Unknown)
        {
            Console.WriteLine("Payment type is not recognized, try one more time");
            return;
        }

        Order order = new Order(user, products, orderOperation);

        IOrderProcessor orderProcessor = new ProcessorComposite(new IOrderProcessor[] { new CashOrderProcessor(), new CreditCardOrderProcessor() });

        if (orderProcessor.CanProcess(order))
        {
            try
            {
                var result = await orderProcessor.ProcessAsync(order);
                user = result.newUserState;
                Console.WriteLine(result.receipt);
            }
            catch (AnalyticsException ex)
            {
                Console.WriteLine($"Order can't be processed: {ex.Message}.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Payment type is not recognized, try one more time");
        }

        PrintUserInformation(user);
    }

    private static void PrintUserInformation(User user)
    {
        Console.WriteLine($"New User state: {user.Name}; CashBalance: {user.CashBalance} CreditCardBalance: {user.CreditCardBalance} ");
    }
}