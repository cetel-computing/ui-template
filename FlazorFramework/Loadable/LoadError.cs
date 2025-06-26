using System;

namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// Represents a failed attempt to load data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="Exception">The exception that caused the load failure.</param>
    public record LoadError<T>(Exception Exception) : ILoadStatus<T>
    {

    }
}
