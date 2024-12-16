using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ODNS.Request
{
    public class GetDnsEntriesRequest
    {
        public string rid { get; set; } = Guid.NewGuid().ToString();
        public Pagination? pagination {  get; set; } //= new Pagination();
        public DnsEntryFilter? filter { get; set; }
        public Sort? sort { get; set; }

        public void fixSortField()
        {
            if(this.sort == null)
                return;

            PropertyInfo[] fields = typeof(DnsEntryFilter).GetProperties(BindingFlags.Public|BindingFlags.Instance );
            foreach (var field in fields)
            {
                // Check if the field has the CustomAttribute
                var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attribute != null)
                {
                    if (this.sort.field.ToLower() == attribute.Name.ToLower())
                    {
                        this.sort.field = field.Name;
                        break;
                    }
                    Console.WriteLine($"Field: {field.Name}, Attribute Description: {attribute.Name}");
                }
            }
        }
    }
}
