using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Model
{
    public class Permission
    {
        public Guid Id { get; set; }
        public required string RoleName { get; set; }
        public string? Description { get; set; }
        public Guid? ShiftId { get; set; } // Para uso futuro

        // Relación con Perfiles
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    }
}
