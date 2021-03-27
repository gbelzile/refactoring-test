using System;

namespace LegacyApp
{
    public sealed class User
    {
        // Perso je mettrais seulement le clientId ici mais je ne veux pas changer le DataAccess
        public Client Client { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public bool HasCreditLimit { get; set; }
        public int CreditLimit { get; set; }
        public bool ValidateCredit()
        {
            return !(HasCreditLimit && CreditLimit < 500);
        }
    }
}