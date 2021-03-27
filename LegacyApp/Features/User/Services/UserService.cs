using System;

namespace LegacyApp
{
    public sealed class UserService
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly IClientRepository _clientRepository;

        // avec du DI j'aurais injecté le service transient directement et il se serait fait disposer par le di engine
        // mais là j'utilise une factory pour pouvoir créer l'instance du service dans la méthode et pouvoir la disposer
        private readonly UserCreditServiceFactory _userCreditServiceFactory;

        public UserService(): this(new ClientRepository(), new RealUserCreditServiceFactory(), new UserDataRepository()) { }

        public UserService(IClientRepository clientRepository, UserCreditServiceFactory userCreditServiceFactory, IUserDataRepository userDataRepository)
        {
            _clientRepository = clientRepository;
            _userCreditServiceFactory = userCreditServiceFactory;
            _userDataRepository = userDataRepository;
        }
        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            return AddUser(new UserDto(firname, surname, email, dateOfBirth), clientId);
        }

        public bool AddUser(UserDto userDto, int clientId)
        {
            if(!userDto?.Validate() ?? false)
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);
            if(client == null)
            {
                return false;
            }

            var user = new User
            {
                Client = client,
                DateOfBirth = userDto.DateOfBirth,
                EmailAddress = userDto.EmailAddress,
                Firstname = userDto.Firstname,
                Surname = userDto.Surname,
                HasCreditLimit = client.HasCreditLimit
            };

            if (client.HasCreditLimit)
            {
                // J'ai trouvé cette classe sur le web, utilie parce qu'on ne sait pas si la factory va retourner une instance disposable ou pas.
                using var userCreditService = new PotentialDisposable<IUserCreditService>(_userCreditServiceFactory.GetCreditService());
                var creditLimit = userCreditService.Instance.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                user.CreditLimit = client.CreditLimitFactor * creditLimit;
            }

            if (!user.ValidateCredit())
            {
                return false;
            }

            _userDataRepository.AddUser(user);

            return true;
        }
    }
}