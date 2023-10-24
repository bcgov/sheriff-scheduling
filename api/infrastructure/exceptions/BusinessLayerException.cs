using System;

namespace CAS.API.infrastructure.exceptions
{
    /// <summary>
    /// BusinessLayerException class, provides a way to handle bad request exceptions so that they are returned by the middleware in a standardized way.
    /// </summary>
    public class BusinessLayerException : Exception
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of a BusinessLayerException object, initializes it with the specified arguments.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public BusinessLayerException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of a BusinessLayerException object, initializes it with the specified arguments.
        /// /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        public BusinessLayerException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Constructors
    }
}
