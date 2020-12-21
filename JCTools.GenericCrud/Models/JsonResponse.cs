namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Allows send a response in JSON to the client
    /// </summary>
    public class JsonResponse
    {
        /// <summary>
        /// True if the operation was successful; False another case
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// The redirection url to be used for refresh the displayed user data
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}