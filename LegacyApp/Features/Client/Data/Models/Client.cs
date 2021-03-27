namespace LegacyApp
{
    public sealed class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ClientStatus ClientStatus { get; set; }

        public bool HasCreditLimit
        {
            get { return Name != "VeryImportantClient"; }
        }

        public int CreditLimitFactor
        {
            get { return Name == "ImportantClient" ? 2 : 1; }
        }
    }
}