namespace Framework.Flazor.Autoregister
{
    /// <summary>
    /// A dynamic feature is a way of avoiding have to create a boilerplate Feature class. 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <remarks>
    /// Dynamic features are loaded by the <see cref="AutoregisterFeature"/> middleware.
    /// </remarks>
    public class Feature<TState> : Fluxor.Feature<TState>
        where TState : new()
    {
        private readonly string _name;

        public Feature()
        {
            _name = typeof(TState).Name;
        }

        public Feature(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            _name = name;
        }

        public override string GetName() => _name;

        protected override TState GetInitialState() => new();
    }
}
