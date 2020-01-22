using eSign20.Api.Client;
using NLog;
using SaaS.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.WinService.eSign
{
    public sealed class EmailChangeWorker : IDisposable
    {
        private const string DASHES = "---------------------------------------------------------------------------------------------------------";
        private IAuthRepository _auth;
        private IeSignRepository _eSign;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public EmailChangeWorker()
        {
            _auth = new AuthRepository();
            _eSign = new eSignRepository();
        }

        public int Do(int top)
        {
            //select * from accounts.account where email like '%ozvieriev%' --
            //exec[eSign].[peSignApiKeySetByAccountId] 'B80E654B-A2EB-4B15-933D-0AB74C14D48F', 1, 'fasdfa'

            var apiKeys = _eSign.eSignApiKeysNeedRefreshGet(top);

            foreach (var apiKey in apiKeys)
            {
                var accountEmails = _auth.AccountEmailsGet(apiKey.AccountId);
                var responseTask = Task.Run(async () =>
                {
                    return await eSign20Client.AccountSendersChangeEmailAsync(accountEmails.Select(e => e.Email));
                });

                responseTask.Wait();

                if (responseTask.Result.IsSuccessStatusCode)
                    _logger.Info("AccountEmailChange - HttpStatus: {0} Emails: {1}",
                        responseTask.Result.StatusCode, string.Join(",", accountEmails));
                else
                    _logger.Error("AccountEmailChange - HttpStatus: {0} Emails: {1}",
                        responseTask.Result.StatusCode, string.Join(",", accountEmails));

                _eSign.eSignApiKeyDelete(apiKey.Id);
            }

            return apiKeys.Count;
        }

        public void Dispose()
        {
            _auth.Dispose();
            _eSign.Dispose();
        }
    }
}
