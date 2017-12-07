using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DashboardCode.Routines
{
    public static class AsyncManager
    {
        public static void Handle(Func<Task> func)
        {
            using (var asyncTaskScheduler = new AsyncTaskScheduler())
            {
                var asyncTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.HideScheduler, TaskContinuationOptions.HideScheduler, asyncTaskScheduler);

                var asyncSynchronizationContext = new AsyncSynchronizationContext(asyncTaskScheduler, 
                    (SendOrPostCallback callback, object state) => {
                            var actionTask = asyncTaskFactory.StartNew(() => callback(state), asyncTaskFactory.CancellationToken, TaskCreationOptions.HideScheduler | TaskCreationOptions.DenyChildAttach, asyncTaskScheduler);
                               return actionTask;
                    });
                {
                    SynchronizationContext oldContext = SynchronizationContext.Current;
                    SynchronizationContext.SetSynchronizationContext(asyncSynchronizationContext);

                    asyncTaskScheduler.Increment();
                    var funcTask = asyncTaskFactory.StartNew(func, asyncTaskFactory.CancellationToken, TaskCreationOptions.HideScheduler | TaskCreationOptions.DenyChildAttach, asyncTaskScheduler).Unwrap();
                    var continuationTask = funcTask.ContinueWith(t =>
                    {
                        asyncTaskScheduler.Decrement();
                        t.GetAwaiter().GetResult();
                    }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, asyncTaskScheduler);
                    
                    var tasks = asyncTaskScheduler.GetConsumingEnumerable();
                    foreach (var (hasAwait, t) in tasks)
                    {
                        asyncTaskScheduler.PublicTryExecuteTask(t);
                        if (hasAwait)
                            t.GetAwaiter().GetResult();
                    }
                    continuationTask.GetAwaiter().GetResult();

                    SynchronizationContext.SetSynchronizationContext(oldContext);
                }
            }
        }
    }

    public class AsyncTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Tuple<bool, Task>> blockingCollection;
        private int blockingCollectionFlag;

        public AsyncTaskScheduler()
        {
            blockingCollection = new BlockingCollection<Tuple<bool, Task>>();
            blockingCollectionFlag = 0;
        }

        internal void Increment()
        {
            Interlocked.Increment(ref blockingCollectionFlag);
        }

        internal void Decrement()
        {
            if (Interlocked.Decrement(ref blockingCollectionFlag) == 0)
                blockingCollection.CompleteAdding();
        }

        public void Dispose()
        {
            blockingCollection.Dispose();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            foreach (var (hasAwait, task) in blockingCollection)
                yield return task;
        }

        internal void TryAdd(Task task)
        {
            blockingCollection.TryAdd(new Tuple<bool, Task>(true, task));
        }

        protected override void QueueTask(Task task)
        {
            Increment();
            task.ContinueWith(t => Decrement(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, this);
            blockingCollection.TryAdd(new Tuple<bool, Task>(false, task));

        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return (SynchronizationContext.Current is AsyncSynchronizationContext asyncSynchronizationContext && asyncSynchronizationContext.IsInlineTo(this)) && TryExecuteTask(task);
        }

        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        public void PublicTryExecuteTask(Task task)
        {
            TryExecuteTask(task);
        }

        internal IEnumerable<Tuple<bool, Task>> GetConsumingEnumerable()
        {
            return blockingCollection.GetConsumingEnumerable();
        }
    }

    public class AsyncSynchronizationContext : SynchronizationContext
    {
        private readonly AsyncTaskScheduler asyncTaskScheduler;
        private readonly Func<SendOrPostCallback, object, Task> startNew;

        public AsyncSynchronizationContext(AsyncTaskScheduler asyncTaskScheduler, Func<SendOrPostCallback, object, Task> startNew)
        {
            this.asyncTaskScheduler = asyncTaskScheduler;
            this.startNew= startNew;
        }

        public bool IsInlineTo(AsyncTaskScheduler asyncTaskScheduler)
        {
            return this.asyncTaskScheduler == asyncTaskScheduler;
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            var actionTask = startNew(callback, state);
            asyncTaskScheduler.Increment();
            actionTask.ContinueWith(t => asyncTaskScheduler.Decrement(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, asyncTaskScheduler);
            asyncTaskScheduler.TryAdd(actionTask);
        }


        public override void Send(SendOrPostCallback callback, object state)
        {
            throw new NotImplementedException(nameof(Send) +" not used in this concreate use case");
            // NOTE: sample realization
            // if (Current as AsyncSynchronizationContext == this)
            // {
            //    callback(state);
            // }
            // else
            // {
            //    var actionTask = startNew(callback, state);
            //    actionTask.GetAwaiter().GetResult();
            // }
        }

        public override SynchronizationContext CreateCopy()
        {
            throw new NotImplementedException(nameof(CreateCopy) + " not used in this concreate use case");
            // NOTE: sample realization
            // return this;
        }

        public override void OperationStarted()
        {
            throw new NotImplementedException(nameof(OperationStarted) + " not used in this concreate use case");
            // NOTE: sample realization
            // asyncTaskScheduler.Increment();
        }

        public override void OperationCompleted()
        {
            throw new NotImplementedException(nameof(OperationCompleted) + " not used in this concreate use case");
            // NOTE: sample realization
            // asyncTaskScheduler.Decrement();
        }
    }
}