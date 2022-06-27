using System;
using System.Collections.Generic;

#nullable disable

namespace bottest2
{
    public partial class UserTarif
    {
        public long UId { get; set; }
        public long TId { get; set; }

        public virtual Tarif TIdNavigation { get; set; }
        public virtual User UIdNavigation { get; set; }
    }
}
