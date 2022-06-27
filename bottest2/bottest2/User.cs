using System;
using System.Collections.Generic;

#nullable disable

namespace bottest2
{
    public partial class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long? Balance { get; set; }
        public string Ip { get; set; }
        public long? ChatId { get; set; }
        public string Date { get; set; }

        public virtual UserTarif UserTarif { get; set; }
    }
}
