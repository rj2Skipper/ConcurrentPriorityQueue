using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConcurrentPriorityQueue.Core
{
    public class ConcurrentPriorityQueue<T, TP> : IConcurrentPriorityQueue<T, TP>
        where T : IHavePriority<TP>
        where TP : IEquatable<TP>, IComparable<TP>
    {
        private readonly Dictionary<TP, Queue<T>> _internalQueues;
        private readonly object _syncRoot = new object();
        private readonly int _capacity;

        public int Capacity
        {
            get { return _capacity; }
        }
        public int Count => _internalQueues.Count == 0 ? _internalQueues.Count :
            _internalQueues.Values.Select(q => q.Count).Aggregate((a, b) => a + b);

        public bool IsSynchronized => true;

        public object SyncRoot => _syncRoot;

        public ConcurrentPriorityQueue(int capacity = 0)
        {
            _internalQueues = new Dictionary<TP, Queue<T>>();
            _capacity = capacity;
        }

        public void CopyTo(T[] array, int index)
        {
            var itemsArray = ToArray();
            lock (_syncRoot)
                itemsArray.CopyTo(array, index);
        }

        public void CopyTo(Array array, int index) => CopyTo((T[])array, index);

        public T[] ToArray()
        {
            lock (_syncRoot)
            {
                var itemsList = new List<T>();
                foreach (var queue in _internalQueues.OrderBy(q => q.Key).Select(q => q.Value))
                {
                    itemsList.AddRange(queue.ToArray());
                }

                return itemsList.ToArray();
            }
        }

        public bool TryAdd(T item)
        {
            return Enqueue(item);
        }

        public bool TryTake(out T item)
        {
            var qi = Dequeue();
            lock (_syncRoot)
            {
                item = default(T);
                if (qi == null)
                    return false;

                item = qi;
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Enqueue(T item)
        {
            lock (_syncRoot) return AddOrUpdate(item);
        }

        public T Dequeue()
        {
            lock (_syncRoot)
            {
                var q = GetNextQueue();
                if (q != null)
                    return q.Dequeue();
                return default(T);
            }
        }

        public T Peek()
        {
            lock (_syncRoot)
            {
                var q = GetNextQueue();
                if (q != null)
                    return q.Peek();
                return default(T);
            }
        }

        private Queue<T> GetNextQueue()
        {
            foreach (var queue in _internalQueues.OrderBy(q => q.Key).Select(q => q.Value))
            {
                try
                {
                    queue.Peek();
                    return queue;
                }
                catch (Exception) { }
            }

            return null;
        }

        private bool AddOrUpdate(T item)
        {
            if (!_internalQueues.ContainsKey(item.Priority))
            {
                if (IsAtMaxCapacity())
                    return false;
                _internalQueues.Add(item.Priority, new Queue<T>());
            }

            _internalQueues[item.Priority].Enqueue(item);
            return true;
        }

        private bool IsAtMaxCapacity() => _capacity != 0 && Count == _capacity;
    }
}
