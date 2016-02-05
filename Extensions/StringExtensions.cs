using System.Text;

namespace ipm_quickstart_csharp_mac.Extensions
{
    public static class StringExtensions
    {
        public readonly static string MyNumber = "[YOUR_TWILIO_NUMBER]";
        public readonly static string MyName = "YourName";

        public static string RemoveSpecialCharacters(string str) {
            var sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}