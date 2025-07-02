using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.Core
{
    public static class TokenValidator
    {
        public static bool IsTokenValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokenHanler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHanler.ReadJwtToken(token);

                var expiration = jwtToken.ValidTo;

                return expiration > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
