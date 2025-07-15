namespace FinTrackForWindows.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        void Logout();
        Task<bool> InitiateRegistrationAsnc(string firstName, string lastName, string email, string password);
        Task<bool> VerifyOtpAndRegisterCodeAsync(string email, string code);
    }
}
