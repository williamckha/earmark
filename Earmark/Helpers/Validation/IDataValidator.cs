namespace Earmark.Helpers.Validation
{
    public interface IDataValidator<TData>
    {
        /// <summary>
        /// Validates the provided data.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <param name="errorMessage">The error message if the data fails validation.</param>
        /// <returns>True if the data passes validation.</returns>
        bool Validate(TData data, out string errorMessage);
    }
}
