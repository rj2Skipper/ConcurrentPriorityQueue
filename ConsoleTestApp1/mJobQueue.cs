using System;
using System.Collections.Concurrent;
using ConcurrentPriorityQueue.Core;

namespace OfficeMaster.FMSRV.DirectSIP.Model
{
    internal enum MessageType
    {
        Reject,
        Mwi,
        Fax,
        Sms,
        Voip
    }

    internal class JobQueueItemPriority : IEquatable<JobQueueItemPriority>, IComparable<JobQueueItemPriority>
    {
        public short Priority { get; set; }
        public MessageType MsgType{get;set;}

        internal static JobQueueItemPriority FromJob(JobQueueItem job)
        {
            if (job == null)
                return null;
            return new JobQueueItemPriority() { Priority = job.Prio, MsgType = job.MsgType };
        }
        public JobQueueItemPriority()
        {

        }

        int IComparable<JobQueueItemPriority>.CompareTo(JobQueueItemPriority other)
        {
            if (other == null)
                return 1;
            if(other.Priority==this.Priority)
            {
                return other.MsgType.CompareTo(this.MsgType);
            }
            return other.Priority.CompareTo(this.Priority);

        }

        bool IEquatable<JobQueueItemPriority>.Equals(JobQueueItemPriority other)
        {

            if (other == null)
                return false;
            if (other.Priority == this.Priority)
            {
                return other.MsgType.Equals(this.MsgType);
            }
            return other.Priority.Equals(this.Priority);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class JobQueueItem:IDisposable, IHavePriority<JobQueueItemPriority>
    {
		internal MessageType MsgType
		{
			get
            {
                if (this.Priority != null)
                {
                    return this.Priority.MsgType;
                }
                return MessageType.Fax;
            }
            set
            {
                if (this.Priority != null)
                {
                    this.Priority.MsgType = value;
                }
            }
		}
		internal UInt32 JobId
		{
			get;
			set;
		}
		internal string IpMediaPrcId
		{
			get;
			set;
		}
		internal bool IsCancelled
		{
			get;
			set;
		}
		internal bool IsSubmitting
		{
			get;
			set;
		}
		internal short Prio
		{
            get
            {
                if (this.Priority != null)
                {
                    return this.Priority.Priority;
                }
                return 0;
            }
            set
            {
                if (this.Priority != null)
                {
                    this.Priority.Priority = value;
                }
            }
        }
		internal string Subject
		{
			get;
			set;
		}


		internal System.Threading.ManualResetEventSlim ProcessingStartedEvent
		{
			get;
			private set;
		}
		internal bool HasProcessingStarted
		{
			get
			{
				return this.ProcessingStartedEvent.IsSet;
			}
			private set
			{
				this.ProcessingStartedEvent.Reset();
			}
		}

		internal System.Collections.Generic.Queue<JobQueueItem> ChildJobs
		{
			get;
			set;
		}
		internal JobQueueItem ParentJob
		{
			get;
			set;
		}
        public JobQueueItemPriority Priority
        {
            get;
            set;
        }

        public JobQueueItem(uint jobId, MessageType msgType )
		{
			this.ProcessingStartedEvent = new System.Threading.ManualResetEventSlim(false);
			this.JobId = jobId;
            this.Priority = new JobQueueItemPriority() { MsgType = msgType };
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				disposedValue = true;
				if(disposing)
				{
					// dispose managed state (managed objects).
					this.ProcessingStartedEvent?.Set();
					this.ProcessingStartedEvent?.Dispose();
					this.ProcessingStartedEvent = null;
					this.ParentJob = null;
					this.ChildJobs?.Clear();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~JobQueueItem() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", this.JobId, this.Priority?.MsgType);
        }

    }

    //internal class JobQueue : BlockingCollection<JobQueueItem>
    //internal class JobQueue : ConcurrentPriorityQueue<IHavePriority<JobQueueItemPriority>, JobQueueItemPriority>
    internal class JobQueue: ConcurrentPriorityQueue<JobQueueItem, JobQueueItemPriority>
    {
		public JobQueue() : base()
		{

		}
		public JobQueue(int boundedCapacity) : base(boundedCapacity)
		{

		}
	}
}
