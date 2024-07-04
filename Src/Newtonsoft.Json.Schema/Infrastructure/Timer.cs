#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion


#if PORTABLE
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal sealed class Timer : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Timer(Action<object> callback, object state, int dueTime, int period)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Delay(dueTime, _cancellationTokenSource.Token).ContinueWith(async (t, s) =>
                {
                    var tuple = (Tuple<Action<object>, object>)s;

                    while (true)
                    {
                        if (_cancellationTokenSource.IsCancellationRequested)
                            break;

                        await Task.Delay(period).ConfigureAwait(false);

#pragma warning disable 4014
                    Task.Run(() => tuple.Item1(tuple.Item2));
#pragma warning restore 4014
                    }
                }, Tuple.Create(callback, state), CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
#endif