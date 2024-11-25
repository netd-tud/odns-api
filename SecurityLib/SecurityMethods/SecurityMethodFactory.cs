using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityLib.SecurityMethods.Authorization;
using SecurityRepository.SecurityMethods.Authorization.ApiKey;

namespace SecurityLib.SecurityMethods
{
    public interface ISecurityMethodFactory
    {
        ISecurityMethod GetInstance(string token);
    }
    public class SecurityMethodFactory
    {
        private readonly IEnumerable<ISecurityMethod> _services;

        public SecurityMethodFactory(IEnumerable<ISecurityMethod> services)
        {
            _services = services;
        }

        public ISecurityMethod GetService(Type type)
        {
            return _services.FirstOrDefault(x => x.GetType() == type);
        }

        public ISecurityMethod GetInstance(string token)
        {
            return token switch
            {
                "apikey" => this.GetService(typeof(ApiKeyManager)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
