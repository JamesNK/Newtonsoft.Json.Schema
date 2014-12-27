using System;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    public sealed class Timer : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Timer(Action<object> callback, object state, int dueTime, int period)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Delay(dueTime, _cancellationTokenSource.Token).ContinueWith(async (t, s) =>
            {
                Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>)s;

                while (true)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        break;
                    Task.Run(() => tuple.Item1(tuple.Item2));
                    await Task.Delay(period);
                }

            }, Tuple.Create(callback, state), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}