using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Funder : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ContactEmployee { get; set; } = null!;
        public string? MainNumber { get; set; }
        public string? SubNumber { get; set; }

        public IList<Contract> Contracts { get; set; } = null!;
    }
}