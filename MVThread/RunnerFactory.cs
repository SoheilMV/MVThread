namespace MVThread
{
    public static class RunnerFactory
    {
        public static IRunner Create(RunnerType type, bool useAsync = true)
        {
            IRunner result = type switch
            {
                RunnerType.Task => new TaskRunner(useAsync),
                RunnerType.Thread => new ThreadRunner(useAsync),
                _ => throw new NotImplementedException()
            };
            return result;
        }
    }
}
