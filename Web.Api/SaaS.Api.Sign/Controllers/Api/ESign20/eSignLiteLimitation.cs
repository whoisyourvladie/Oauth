namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    internal static class eSignLiteLimitation
    {
        /// <summary>
        /// In bytes(10 mb)
        /// </summary>
        internal const ulong FileSize = 10 * 1024 * 1024;
        internal const string FileSizeError = "Users file exceeds the maximum size.";
        internal const string FileSizeMinorWarning= "Users file exceeds the maximum size. Users sign will be used.";

        internal const ulong Recipients = 2;
        internal const string RecipientsError = "User exceeded the maximum amount of recipients.";
        internal const string RecipientsMinorWarning = "User exceeded the maximum amount of recipients. Users sign will be used.";

        internal const uint SignsPerDay = 1;        
        internal const string SignsPerDayError = "User exceeded the maximum amount of signs.";
        internal const string SignsPerDayMinorWarning = "User exceeded the maximum amount of signs. Users sign will be used.";
    }
}