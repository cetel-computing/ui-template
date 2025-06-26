namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// A successful attempt to load data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="Value">The data.</param>
    public record Loaded<T>(T Value) : ILoadStatus<T>
    {
    }
}
