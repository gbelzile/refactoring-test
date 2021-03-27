namespace LegacyApp
{
    public sealed class UserDataRepository : IUserDataRepository
    {
        public void AddUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}