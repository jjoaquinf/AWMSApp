using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Model
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
