namespace Droplet.Helpers
{
    public static class PESELHelper
    {
        public static bool IsValidPESEL(string pesel)
        {
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i] * (pesel[i] - '0');
            }

            int checksum = 10 - (sum % 10);
            if (checksum == 10)
            {
                checksum = 0;
            }

            return checksum == (pesel[10] - '0');
        }
    }
}
