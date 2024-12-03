using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EProtocol
    {
        UDP=0, 
        TCP=1
    }

    public static class EProtocolValues
    {
        private static Dictionary<EProtocol, string> mapping = new Dictionary<EProtocol, string>()
        {
            {EProtocol.TCP,"tcp" },
            {EProtocol.UDP,"udp" }
        };

        public static string? Get(EProtocol? choice)
        {
            return choice!= null ? mapping[(EProtocol)choice] : null;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EResponseType
    {
        Resolver = 0,
        Forwarder = 1,
        TransparentForwarder = 2
    }

    public static class EResponseTypeValues
    {
        private static Dictionary<EResponseType, string> mapping = new Dictionary<EResponseType, string>()
        {
            {EResponseType.Resolver,"Resolver" },
            {EResponseType.Forwarder,"Forwarder" },
            {EResponseType.TransparentForwarder,"Transparent Forwarder" }
        };

        public static string? Get(EResponseType? choice)
        {
            return choice != null ? mapping[(EResponseType)choice] : null;
        }
    }
}
