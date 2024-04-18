namespace ServicesApp.Interfaces
{
    public interface IMLRepository
    {
        Task<bool> MatchJobAndService(int serviceId, string jobTitle);
    }
}
