using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JCTools.GenericCrud.Models.Rest
{
    /// <summary>
    /// Contains the data to return to the API rest requests when is requested all entity data
    /// </summary>
    public class IndexModel
    {
        /// <summary>
        /// The collection of the found entity data
        /// </summary>
        public object[] Data { get; }
        /// <summary>
        /// The info message to show to the user
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// The desired entity to highlight of all found data; null is is not found.
        /// </summary>
        public object Selected { get; }

        /// <summary>
        /// Init an empty instance
        /// </summary>
        public IndexModel() { }

        /// <summary>
        /// Inits the current instance
        /// </summary>
        /// <param name="model">The model with the access to the desired data</param>
        public IndexModel(IIndexModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var all = model.GetCollectionData();
            Data = all.Select(d => d.GetEntity()).ToArray();
            Message = model.Message?.Text;
            Selected = all
                .Where(d => d.GetKeyValue().Equals(model.GetId()))
                .FirstOrDefault()
                ?.GetEntity() ?? null;
        }

        /// <summary>
        /// Creates a <see cref="JsonResult"/> object that serializes the current instance to JSON.
        /// </summary>
        /// <returns>The created <see cref="JsonResult"/> that serializes the current instance 
        /// to JSON format for a HTTP response.</returns>
        public JsonResult ToJson() => new JsonResult(this);

            /// <summary>
        /// Creates a <see cref="JsonResult"/> object that serializes the current instance to JSON.
        /// </summary>
        /// <returns>The created <see cref="JsonResult"/> that serializes the current instance 
        /// to JSON format for a HTTP response.</returns>
        public ContentResult ToXml() => Helpers.XmlResult.Xml(null, this, "All");
    }
}