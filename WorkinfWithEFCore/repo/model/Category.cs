using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkinfWithEFCore.repo.model
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
        }
    }
}
