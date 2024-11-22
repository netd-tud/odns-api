using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityRepository.SecurityMethods.Authorization.ApiKey
{
    public interface IApiKeyRepositoryFactory
    {
        IApiKeyRepository GetInstance(string token);
    }
    public class ApiKeyRepositoryFactory
    {
        private readonly IEnumerable<IApiKeyRepository> _services;

        public ApiKeyRepositoryFactory(IEnumerable<IApiKeyRepository> services)
        {
            _services = services;
        }

        public IApiKeyRepository GetService(Type type)
        {
            return _services.FirstOrDefault(x => x.GetType() == type);
        }

        public IApiKeyRepository GetInstance(string token)
        {
            return token switch
            {
                "POSTSQL" => this.GetService(typeof(ApiKeyPostgresRepository)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
