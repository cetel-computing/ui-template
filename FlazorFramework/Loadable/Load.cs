using System;
using System.Threading.Tasks;

namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// Convenience methods for creating <see cref="Loaded{T}"/>.
    /// </summary>
    public static class Load
    {
        /// <summary>
        /// Creates a new <see cref="Loaded{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="value">The data that was loaded.</param>
        /// <returns>A Loaded containing the supplied data.</returns>
        /// <remarks>
        /// Convenience method to avoid having to explicitly declare the type when loading -
        /// type inference does not work on constructors.
        /// </remarks>
        public static Loaded<T> Success<T>(T value) => new(value);

        /// <summary>
        /// Creates a new <see cref="LoadError{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="exception">The exception that caused this error.</param>
        /// <returns>A LoadError representing this exception.</returns>
        public static LoadError<T> Error<T>(Exception exception) => new(exception);

        /// <summary>
        /// Creates a new <see cref="Loading{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <returns>A Loading record.</returns>
        public static Loading<T> Loading<T>() => new();
    }
}
