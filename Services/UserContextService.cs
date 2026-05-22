namespace ProviderAssignmentStarter.Services
{
    public interface IUserContextService
    {
        string GetCurrentUserName();
    }
    public class UserContextService : IUserContextService
    {
        public string GetCurrentUserName()
        {
            return "system.user";
        }
    }
}
