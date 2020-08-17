using ConcurrentPriorityQueue.Core;
using OfficeMaster.FMSRV.DirectSIP.Model;
using System;
using System.Collections.Concurrent;

namespace ConsoleTestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var q = new JobQueue();
            var b = q.ToBlockingCollection();
            for(int i=0;i<4;i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                Console.WriteLine($"add job: '{item.ToString()}' - '{b.TryAdd(item)}'");
            }
            
            /*
            var q2 = new JobQueue(2);
            var b2 = q2.ToBlockingCollection();
            for (int i = 0; i < 4; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                b2.Add(item);
                Console.WriteLine($"add job: '{item.ToString()}' ");
            }*/
            /*
            var q3 = new JobQueue(3);
            for (int i = 0; i < 2; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                Console.WriteLine($"add job: '{item.ToString()}' - '{q3.TryAdd(item)}'");
            }

            var b3 = q3.ToBlockingCollection();
            for (int i = 2; i < 4; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                b3.Add(item);
                Console.WriteLine($"add job: '{item.ToString()}' ");
            }
            */

            //DoPriorityCollection();
            //DoPriorityQ();


        }
        private static void DoPriorityCollection()
        {
            var qq = new JobQueue(2);
            BlockingCollection<JobQueueItem> q = qq.ToBlockingCollection();
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                q.Add(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i + 5, (MessageType)i);
                q.Add(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i + 10, (MessageType)i);
                item.Prio = (short)i;
                q.Add(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }

            q.CompleteAdding();

            while (q.TryTake(out JobQueueItem item))
            {
                Console.WriteLine($"got job: '{item.ToString()}'");
            }
        }
        private static void DoPriorityQ()
        {
            JobQueue q = new JobQueue(2);
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i, (MessageType)i);
                q.TryAdd(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i + 5, (MessageType)i);
                q.TryAdd(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }
            for (int i = 0; i < 5; i++)
            {
                JobQueueItem item = new JobQueueItem((uint)i + 10, (MessageType)i);
                item.Prio = (short)i;
                q.TryAdd(item);
                Console.WriteLine($"add job: '{item.ToString()}'");
            }


            while (q.TryTake(out JobQueueItem item))
            {
                Console.WriteLine($"got job: '{item.ToString()}'");
            }
        }
    }
}
