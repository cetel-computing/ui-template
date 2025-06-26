namespace Framework.Flazor
{
    /// <summary>
    /// Action to notify the user an error has occurred.
    /// </summary>
    public record Error
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; init; } = string.Empty;
    }
}
