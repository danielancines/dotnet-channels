using System.Threading.Channels;

Console.WriteLine("Hello, World!");
const int producerDelay = 100;
const int consumerDelay = 1000;


void RunBoundedChannel()
{
    var boundedChannel = Channel.CreateBounded<int>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropNewest
    }, (dropped) =>
    {
        Console.WriteLine($"Dropped Item: {dropped}");
    });

    var count = 0;

    _ = Task.Run(async () =>
    {
        while (true)
        {
            await Task.Delay(producerDelay);
            await boundedChannel.Writer.WriteAsync(count);
            count++;
        }
    });

    _ = Task.Run(async () =>
    {
        await foreach (var number in boundedChannel.Reader.ReadAllAsync())
        {
            await Task.Delay(consumerDelay);
            Console.WriteLine($"First consumer: {number}");
        }
    });

    _ = Task.Run(async () =>
    {
        await foreach (var number in boundedChannel.Reader.ReadAllAsync())
        {
            await Task.Delay(consumerDelay);
            Console.WriteLine($"Second consumer: {number}");
        }
    });
}

RunBoundedChannel();

Console.ReadKey();


