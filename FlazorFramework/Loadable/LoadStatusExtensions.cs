using System;

namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// Extension methods on <see cref="ILoadStatus{T}" />
    /// </summary>
    public static class LoadStatusExtensions
    {
        /// <summary>
        /// Converts any <see cref="ILoadStatus{T}"/> to the equivalent <see cref="Loadable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <returns>A loadable appropriately representing the </returns>
        public static Loadable<T> ToLoadable<T>(this ILoadStatus<T> status)
        {
            return status switch
            {
                Loading<T> => new(),
                LoadError<T> => new() { Failed = true },
                Loaded<T> loaded => new(loaded.Value),
                _ => throw new NotSupportedException($"Type '{status?.GetType()}' is not supported")
            };
        }
    }
}
