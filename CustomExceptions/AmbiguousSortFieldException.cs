namespace CustomExceptions
{
    public class AmbiguousSortFieldException : ArgumentException
    {
        /// <summary>
        /// The original, incorrect field name provided by the user.
        /// </summary>
        public string OriginalField { get; }

        /// <summary>
        /// The corrected field name that the system suggests.
        /// </summary>
        public string CorrectedField { get; }

        /// <summary>
        /// Initializes a new instance of the AmbiguousSortFieldException class.
        /// </summary>
        /// <param name="originalField">The original value that was not found.</param>
        /// <param name="correctedField">The suggested, corrected value.</param>
        public AmbiguousSortFieldException(string originalField, string correctedField)
            : base($"The provided sort field '{originalField}' was not found. Did you mean '{correctedField}' ?")
        {
            OriginalField = originalField;
            CorrectedField = correctedField;
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousSortFieldException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public AmbiguousSortFieldException(string originalField) 
            : base($"The provided sort field '{originalField}' was not found.")
        {
            // Set default values if not provided through the primary constructor
            OriginalField = originalField;
            CorrectedField = string.Empty;
        }

    }
}
