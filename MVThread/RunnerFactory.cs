namespace MVThread
{
    public static class RunnerFactory
    {

        #region Without using parallelization

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StopEventArgs> onStopped)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StopEventArgs> onStopped, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StopEventArgs> onStopped, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StopEventArgs> onStopped, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, ConfigStatus> onConfig, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(false),
                RunnerType.Thread => new ThreadRunner(false),
                RunnerType.Parallel => new ParallelRunner(false),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfig += (s, e) => { return onConfig(e); };
            return result;
        }

        #endregion

        #region Using parallelization

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StopEventArgs> onStopped)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StopEventArgs> onStopped, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StopEventArgs> onStopped, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StopEventArgs> onStopped, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action onCompleted)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        public static IRunner Create(RunnerType type, Func<DataEventArgs, Task<ConfigStatus>> onConfigAsync, Action<StartEventArgs> onStarted, Action<StopEventArgs> onStopped, Action onCompleted, Action<ExceptionEventArgs> onException)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(true),
                RunnerType.Thread => new ThreadRunner(true),
                RunnerType.Parallel => new ParallelRunner(true),
                _ => throw new NotImplementedException()
            };
            result.OnException += (s, e) => { onException(e); };
            result.OnStarted += (s, e) => { onStarted(e); };
            result.OnStopped += (s, e) => { onStopped(e); };
            result.OnCompleted += (s, e) => { onCompleted(); };
            result.OnConfigAsync += async (s, e) => { return await onConfigAsync(e); };
            return result;
        }

        #endregion
    }
}
