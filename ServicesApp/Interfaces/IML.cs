namespace ServicesApp.Interfaces
{
    public interface IML
    {
        Task<bool> MatchJobAndService(int serviceId, string jobTitle);


    }
}
