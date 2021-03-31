using System.Collections.Generic;

namespace MB.Data.Entities
{
    public class Account
    {
        public int ID { get; set; }
        public int Number { get; set; }

        public int DefaultCurrencyId { get; set; }
        public virtual Currency DefaultCurrency { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
