using System;

namespace LegacyApp
{
    public record UserDto
    {
        public UserDto(string firstName, string lastName, string email, DateTime dateOfBirth) => 
            (Firstname, Surname, EmailAddress, DateOfBirth, Now) = (firstName, lastName, email, dateOfBirth, () => DateTime.Now);

        public UserDto(string firstName, string lastName, string email, DateTime dateOfBirth, Func<DateTime> now) =>
            (Firstname, Surname, EmailAddress, DateOfBirth, Now) = (firstName, lastName, email, dateOfBirth, now);

        public DateTime DateOfBirth { get; }

        public string EmailAddress { get; }

        public string Firstname { get; }

        public string Surname { get; }

        // Permet unit testing, par défaut DateTime.Now est utilisé.
        private Func<DateTime> Now { get; }

        private int Age
        {
            get
            {
                var now = Now();
                int age = now.Year - DateOfBirth.Year;

                if (now.Month < DateOfBirth.Month || (now.Month == DateOfBirth.Month && now.Day < DateOfBirth.Day))
                {
                    age--;
                }
                return age;
            }
        }

        public bool Validate ()
        {
            return Firstname.ValidateName() 
                && Surname.ValidateName() 
                && EmailAddress.ValidateEmail()
                && Age > 21;
        }
    }
}
