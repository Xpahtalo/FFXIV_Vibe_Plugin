using System;
using System.Threading;

#nullable enable
public static class Debounce
{
    public static Action<T> Create<T>(Action<T> action, int milliseconds)
    {
        CancellationTokenSource cancelToken = (CancellationTokenSource)null;
        T lastArg = default(T);
        object lockObj = new object();
        return (Action<T>)(arg =>
        {
            lock (lockObj)
            {
                lastArg = arg;
                cancelToken?.Cancel();
                cancelToken = new CancellationTokenSource();
            }
            CancellationToken token = cancelToken.Token;
            ThreadPool.QueueUserWorkItem((WaitCallback)(_ =>
            {
                Thread.Sleep(milliseconds);
                if (token.IsCancellationRequested)
                    return;
                lock (lockObj)
                {
                    if (token.IsCancellationRequested)
                        return;
                    action(lastArg);
                }
            }));
        });
    }

    public static Action Create(Action action, int milliseconds)
    {
        CancellationTokenSource cancelToken = (CancellationTokenSource)null;
        object lockObj = new object();
        return (Action)(() =>
        {
            lock (lockObj)
            {
                cancelToken?.Cancel();
                cancelToken = new CancellationTokenSource();
            }
            CancellationToken token = cancelToken.Token;
            ThreadPool.QueueUserWorkItem((WaitCallback)(_ =>
            {
                Thread.Sleep(milliseconds);
                if (token.IsCancellationRequested)
                    return;
                lock (lockObj)
                {
                    if (token.IsCancellationRequested)
                        return;
                    action();
                }
            }));
        });
    }
}
