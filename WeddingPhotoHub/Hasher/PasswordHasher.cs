using System.Security.Cryptography;

namespace WeddingPhotoHub.Hasher
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            var pbkf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);

            byte[] hash = pbkf2.GetBytes(32);

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');

            if (parts.Length != 2)
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);

                var pbkf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hashToCompare = pbkf2.GetBytes(32);

                return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
            }
            catch
            {
                return false;
            }
        }
    }
}
