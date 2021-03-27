using System;

namespace LegacyApp
{
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/november/net-framework-hidden-disposables

    public sealed class PotentialDisposable<T> : IDisposable where T : class
    {
        private readonly T _instance;
        public T Instance { get { return _instance; } }
        public PotentialDisposable(T instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        public void Dispose()
        {
            if (Instance is IDisposable disposableInstance)
            {
                disposableInstance.Dispose();
            }
        }
    }
}
