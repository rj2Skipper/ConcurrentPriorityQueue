namespace ConcurrentPriorityQueue.Core
{
    public interface IPriorityQueue<T>
    {
        int Capacity { get; }

        bool Enqueue(T item);

        T Dequeue();

        T Peek();
    }
}
