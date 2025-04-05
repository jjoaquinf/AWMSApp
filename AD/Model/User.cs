using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AD.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; } // Será almacenado con hash
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string EmployeeNumber { get; set; }
        public string? ContactPhone { get; set; }

        // Relación con Perfil
        public Guid ProfileId { get; set; }
        public Profile? Profile { get; set; }
    }
}
