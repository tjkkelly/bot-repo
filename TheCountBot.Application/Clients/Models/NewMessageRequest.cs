// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace TheCountBot.Application.Clients.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class NewMessageRequest
    {
        /// <summary>
        /// Initializes a new instance of the NewMessageRequest class.
        /// </summary>
        public NewMessageRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the NewMessageRequest class.
        /// </summary>
        public NewMessageRequest(string username = default(string), string number = default(string), System.DateTime? timestamp = default(System.DateTime?))
        {
            Username = username;
            Number = number;
            Timestamp = timestamp;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public System.DateTime? Timestamp { get; set; }

    }
}