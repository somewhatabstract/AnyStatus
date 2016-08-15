using RestSharp.Serializers;

namespace AnyStatus.Models
{
    /// <summary>
    /// AnyStatus View
    /// </summary>
    public class View
    {
        [SerializeAs(Name = "name")]
        public string Name { get; set; }

        [SerializeAs(Name = "url")]
        public string Url { get; set; }
    }
}
