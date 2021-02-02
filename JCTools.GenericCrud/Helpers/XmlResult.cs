using System;
using System.Net;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// An action result which formats the given object as XML.
    /// </summary>
    public static class XmlResult
    {
        /// <summary>
        /// Creates a <see cref="JsonResult"/> object that serializes the specified value to JSON.
        /// </summary>
        /// <param name="controller">The controller that will send the generated response</param>
        /// <param name="value">The value to serialize as xml</param>
        /// <param name="rootElementName">The name of the root element to append when deserializing.</param>
        /// <returns>The created <see cref="JsonResult"/> that serializes the specified value 
        /// to JSON format for a HTTP response.</returns>
        public static ContentResult Xml(
            this GenericController controller,
            object value,
            string rootElementName = null)
        {
            var serializedData = JsonConvert.DeserializeXNode(
                    JsonConvert.SerializeObject(value),
                    string.IsNullOrWhiteSpace(rootElementName) ? value.GetType().Name : rootElementName
                );

            return new ContentResult()
            {
                Content = serializedData?.ToString(),
                ContentType = Constants.XmlMimeType,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}