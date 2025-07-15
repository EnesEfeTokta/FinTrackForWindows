namespace FinTrackForWindows.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        void Logout();
        Task<bool> InitiateRegistrationAsnc(string userName, string email, string password);
        Task<bool> VerifyOtpAndRegisterCodeAsync(string email, string code);
    }
}
