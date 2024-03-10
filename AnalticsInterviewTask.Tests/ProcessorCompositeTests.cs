using AnalyticsInterviewTask.Processors;
using AnalyticsInterviewTask;
using NSubstitute;

namespace AnalticsInterviewTask.Tests
{
    public class ProcessorCompositeTests
    {
        IOrderProcessor _objectOntest;


        [Test]
        public void CanProcessWhenThereIsNoProcessorThenCannotProcess()
        {
            _objectOntest = new ProcessorComposite(new IOrderProcessor[] { });
            var order = new Order(new User("", 0, 0), new Product[] { }, OrderOperation.Unknown);
            var result = _objectOntest.CanProcess(order);
            Assert.That(result, Is.False, "Should not be able to process");
        }

        [Test]
        public void ProcessAsyncWhenThereIsNoProcessorThrowAnalyticsException()
        {
            _objectOntest = new ProcessorComposite(new IOrderProcessor[] { });
            var order = new Order(new User("", 0, 0), new Product[] { }, OrderOperation.Unknown);

            var exp = Assert.ThrowsAsync<AnalyticsException>(async () =>
            {
                _ = await _objectOntest.ProcessAsync(order);
            }, "Exception is expected to be shown");

            Assert.That(exp.Message == "Noone can process the request");
        }

        [Test]
        public async Task ProcessAsyncWhenHaveProcessorCorrectlyProcessed()
        {
            string receipt = "receipt";
            User user = new User("Name", 2, 2);
            var processor = Substitute.For<IOrderProcessor>();
            processor.CanProcess(Arg.Any<Order>()).Returns(true);
            processor.ProcessAsync(Arg.Any<Order>()).Returns((receipt, user));
            _objectOntest = new ProcessorComposite(new IOrderProcessor[] { processor });
            var order = new Order(new User("", 0, 0), new Product[] { }, OrderOperation.Cash);

            var result = await _objectOntest.ProcessAsync(order);

            Assert.That(result.receipt, Is.EqualTo(receipt));
            Assert.That(result.newUserState, Is.EqualTo(user));
        }

        [Test]
        public void CanProcessAsyncWhenHaveTwoCorrectProcessorThrowAnalyticsException()
        {
            string receipt = "receipt";
            User user = new User("Name", 2, 2);
            var processor = Substitute.For<IOrderProcessor>();
            processor.CanProcess(Arg.Any<Order>()).Returns(true);
            processor.ProcessAsync(Arg.Any<Order>()).Returns((receipt, user));

            var processor2 = Substitute.For<IOrderProcessor>();
            processor2.CanProcess(Arg.Any<Order>()).Returns(true);
            processor2.ProcessAsync(Arg.Any<Order>()).Returns((receipt, user));

            _objectOntest = new ProcessorComposite(new IOrderProcessor[] { processor, processor2 });
            var order = new Order(new User("", 0, 0), new Product[] { }, OrderOperation.Cash);

            var exp = Assert.Throws<AnalyticsException>(() =>
            {
                _= _objectOntest.CanProcess(order);
            }, "Exception is expected to be shown");

            Assert.That(exp.Message.Contains("Only one processor should handled order, choose only one"));
        }
    }
}