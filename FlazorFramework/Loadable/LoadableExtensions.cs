using System;

namespace Framework.Flazor
{
    public static class LoadableExtensions
    {
        /// <summary>
        /// Gets the data in a <see cref="Loadable{T}"/> if it has been loaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadable"></param>
        /// <param name="data">Returns the data, if it has been loaded.</param>
        /// <returns><c>true</c> if the data was loaded, else <c>false</c></returns>
        public static bool TryGetLoaded<T>(this Loadable<T> loadable, out T? data)
        {
            if (loadable.Loaded)
            {
                data = loadable.Data ?? throw new Exception("Data is loaded but is null.");
                return true;
            }
            else
            {
                data = default;
                return false;
            }
        }
    }
}
