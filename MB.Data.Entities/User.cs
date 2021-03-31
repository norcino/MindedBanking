namespace MB.Data.Entities
{
    public class User
    {
        public int ID { get; set; }
        public int Name { get; set; }
        public int Surname { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
