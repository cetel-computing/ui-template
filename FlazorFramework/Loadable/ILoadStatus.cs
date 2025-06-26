namespace Framework.Flazor.Loadable
{
    /// <summary>
    /// Marker interface to identify records related to loading of data. This permits us to convert them to <see cref="Loadable{T}"/> using
    /// the 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILoadStatus<T>
    {
    }
}
