using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Enums
{
    /// <summary>
    /// Protocol used during the scan. Available protocolos are UDP and TCP.
    /// </summary>
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

    /// <summary>
    /// Open DNS classification (Resolver:= queried_ip == replying_ip == backend_resolver, Forwarder:= queried_ip == replying_ip != backend_resolver, Transparent Forwarder:= queried_ip != replying_ip)
    /// </summary>
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
