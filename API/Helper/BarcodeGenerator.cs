namespace API.Helpers
{
    public class BarcodeGenerator
    {
        public static string GenerateBarcode(string componentType)
        {
            string guid_start = Guid.NewGuid().ToString("N")[..3].ToUpper();
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^4..].ToUpper();
            string guid_end = Guid.NewGuid().ToString("N")[^3..].ToUpper();
            return $"{componentType}_{guid_start}{timestamp}{guid_end}";
        }
    }
}