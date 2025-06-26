namespace Framework.Flazor
{
    /// <summary>
    /// Represents data that can be loaded asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    public record Loadable<T>
    {
        private bool _errored;

        private T? _data;

        public Loadable()
        {
            Loaded = false;
        }

        public Loadable(T data)
        {
            Loaded = true;
            Data = data;
        }

        /// <summary>
        /// Gets if <see cref="Data"/> has been loaded.
        /// </summary>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Gets if the data failed to be loaded.
        /// </summary>
        public bool Failed
        {
            get => _errored;
            init
            {
                _errored = value;
                if (_errored)
                {
                    Loaded = false;
                    _data = default;
                }
            }
        }

        /// <summary>
        /// The data.
        /// </summary>
        public T? Data
        {
            get => _data;
            init
            {
                _data = value;
                Loaded = true;
                _errored = false;
            }
        }

        /// <summary>
        /// Converts a value of type <typeparamref name="T"/> into the equivalent <see cref="Loadable{T}"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Loadable<T>(T value)
        {
            return new(value);
        }
    }
}
