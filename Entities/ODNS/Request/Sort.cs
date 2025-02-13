using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.ODNS.Request
{
    public class Sort
    {
        public string field {  get; set; }
        public ESortOrder order { get; set; } //"asc" | "desc"
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ESortOrder 
    { 
        asc,
        desc
    }

    public static class ESortOrderValues
    {
        private static Dictionary<ESortOrder, string> mapping = new Dictionary<ESortOrder, string>()
        {
            {ESortOrder.asc,"asc" },
            {ESortOrder.desc,"desc" }
        };

        public static string? Get(EResponseType? choice)
        {
            return choice != null ? mapping[(ESortOrder)choice] : null;
        }
    }

}
