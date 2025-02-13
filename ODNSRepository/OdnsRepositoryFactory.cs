using ODNSRepository.Repository;

namespace ODNSRepository
{
    public interface IOdnsRepositoryFactory
    {
        IOdnsRepository GetInstance(string token);
    }
    public class OdnsRepositoryFactory : IOdnsRepositoryFactory
    {
        private readonly IEnumerable<IOdnsRepository> _services;

        public OdnsRepositoryFactory(IEnumerable<IOdnsRepository> services)
        {
            _services = services;
        }

        public IOdnsRepository GetService(Type type)
        {
            return _services.FirstOrDefault(x => x.GetType() == type);
        }

        public IOdnsRepository GetInstance(string token)
        {
            return token switch
            {
                "POSTSQL" => this.GetService(typeof(OdnsPostgresqlRepository)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
