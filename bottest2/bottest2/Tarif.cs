using System;
using System.Collections.Generic;

#nullable disable

namespace bottest2
{
    public partial class Tarif
    {
        public Tarif()
        {
            UserTarifs = new HashSet<UserTarif>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public long Speed { get; set; }

        public virtual ICollection<UserTarif> UserTarifs { get; set; }
    }
}
