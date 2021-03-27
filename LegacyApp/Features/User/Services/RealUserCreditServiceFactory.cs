namespace LegacyApp
{
    public sealed class RealUserCreditServiceFactory : UserCreditServiceFactory
    {
        public override IUserCreditService GetCreditService()
        {
            return new UserCreditServiceClient();
        }
    }
}
