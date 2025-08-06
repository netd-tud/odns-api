using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Auth
{
    public class ApiKeyRecord : ApiKeyRecordIn
    {
        public int? id {  get; set; }
        public string? api_key { get; set; }
        public string? created_at { get; set; }
        public bool? is_active { get; set; }
        public bool? is_processed { get; set; }
    }


    public class ApiKeyRecordIn
    {
        /// <summary>
        /// Your full name, First Name Last Name
        /// </summary>
        public string full_name { get; set; }
        /// <summary>
        /// Organiyation you are affiliated with
        /// </summary>
        public string affiliation { get; set; }
        /// <summary>
        /// Your organization email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Your purpose for using th api
        /// </summary>
        public string purpose { get; set; }
    }
}
