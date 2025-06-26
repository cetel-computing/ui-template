namespace Framework.Flazor
{
    /// <summary>
    /// Action to notify the user of success.
    /// </summary>
    public record Success
    {
        /// <summary>
        /// The success message.
        /// </summary>
        public string Message { get; init; } = string.Empty;
    }
}
