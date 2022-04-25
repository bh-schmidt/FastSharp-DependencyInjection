namespace FastSharp.DependencyInjection.Extensions
{
    /// <summary>
    /// Informs how should the duplicate values be handled
    /// </summary>
    public enum DuplicationOptions
    {
        /// <summary>
        /// Throws an error when there is an interface with multiple implementations. (Default)
        /// </summary>
        ThrowOnDuplicate = 1,
        /// <summary>
        /// Ignore interfaces with multiple implementations.
        /// </summary>
        Ignore = 2,
        /// <summary>
        /// Register the first implementation of interface.
        /// </summary>
        TakeFirst = 3
    }
}
