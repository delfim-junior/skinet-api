using System;

namespace API.Helpers
{
    public static class RandomStringGenerator
    {
        public static string GetString()
        {
            var guid = Guid.NewGuid();
            var guidString = Convert.ToBase64String(guid.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");

            return guidString;
        }
    }
}