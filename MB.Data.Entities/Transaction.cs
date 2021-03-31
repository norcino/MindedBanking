using System;

namespace MB.Data.Entities
{
    public class Transaction
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }

        public decimal OriginalAmount { get; set; }
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
