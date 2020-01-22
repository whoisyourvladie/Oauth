using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.Data.Entities.View.Accounts;
using SaaS.Data.Entities.View.Oauth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public interface IAuthRepository : IDisposable
    {
        void SetDataProtectorProvider(IDataProtectionProvider dataProtectorProvider);

        /*******************************************Account*****************************************************/
        string HashPassword(string password);
        bool PasswordIsEqual(string source, string password);

        Task<Account> AccountGetAsync(Guid accountId);
        Task<Account> AccountGetAsync(string email, bool isIncludeSubEmails = false);
        Task<Account> AccountGetAsync(string email, string password);
        Task<Account> AccountGetByTransactionOrderUidAsync(string transactionOrderUid);
        Task<List<Account>> AccountsGetAsync(string filter, string globalOrderId);
        Task AccountDeleteAsync(Guid accountId);
        Task AccountDeleteAsync(Account user);
        Task AccountActivateAsync(Account user);
        Task AccountMaskAsBusinessAsync(Account user);
        Task AccountOptinSetAsync(Account account, bool? optin);
        Task AccountVisitorIdSetAsync(Account account, Guid? visitorId);
        Task<IdentityResult> AccountCreateAsync(Account user, string password = "");
        Task<IdentityResult> AccountChangePasswordAsync(Guid accountId, string oldPassword, string newPassword);
        Task<IdentityResult> AccountConfirmEmailAsync(Guid accountId);
        Task<IdentityResult> AccountConfirmEmailAsync(Guid accountId, string token);
        Task<IdentityResult> AccountResetPasswordAsync(Guid accountId, string token, string password);
        Task<IdentityResult> AccountUpdateAsync(Account user);
        Task<IdentityResult> AccountPasswordSetAsync(Account user, string password);

        List<ViewAccountEmail> AccountEmailsGet(Guid accountId);

        Task<List<AccountSystem>> AccountSystemsGetAsync(AccountProductPair pair);

        Task<Account> AccountAnonymousCreateAsync(string password);

        /*******************************************AccountDetails*****************************************************/
        Task<ViewAccountDetails> AccountDetailsGetAsync(Guid accountId);
        Task AccountDetailsSetAsync(ViewAccountDetails accountDetail);
        Task<int?> AccountUidGetAsync(Guid accountId);


        /*******************************************AccountPreference*****************************************************/
        Task<AccountPreference> AccountPreferenceGetAsync(Guid accountId);
        Task AccountPreferenceSetAsync(Guid accountId, string json);
        Task AccountPreferenceDeleteAsync(Guid accountId);

        /*******************************************AccountSubEmailPending*****************************************************/
        Task AccountEmailSetAsync(AccountSubEmailPending pending);
        Task<List<AccountSubEmail>> AccountSubEmailsGetAsync(Guid accountId);
        Task AccountSubEmailDeleteAsync(int id);
        Task<AccountSubEmailPending> AccountSubEmailPendingSetAsync(Guid accountId, string email);
        Task<AccountSubEmailPending> AccountSubEmailPendingGetAsync(Guid id);
        Task<List<AccountSubEmailPending>> AccountSubEmailPendingsGetAsync(Guid accountId);
        Task AccountSubEmailPendingDeleteAsync(Guid accountId);

        /*******************************************Client*****************************************************/
        List<Client> ClientsGet();

        /*******************************************AccountMergePending*****************************************************/
        Task AccountMergeAsync(ViewAccountMergePending pending);
        Task<ViewAccountMergePending> AccountMergePendingMergeAsync(Guid accountIdTo, Guid accountIdFrom, Guid accountIdPrimaryEmail);
        Task<ViewAccountMergePending> AccountMergePendingGetAsync(Guid id);
        Task<List<ViewAccountMergePending>> AccountMergePendingsGetAsync(Guid accountId);
        Task AccountMergePendingDeleteAsync(Guid accountId);

        /*******************************************Link*************************************************************/
        Task<Uri> GeneratePasswordResetTokenLinkAsync(Account user, Uri uri);
        Task<NameValueCollection> GenerateEmailConfirmationToken(Account user);
        Task<Uri> GenerateEmailConfirmationTokenLinkAsync(Account user, Uri uri);
        Uri GenerateEmailChangeConfirmationTokenLinkAsync(Uri uri, AccountSubEmailPending pending);
        Uri GenerateMergeConfirmationTokenLinkAsync(Uri uri, ViewAccountMergePending pending);

        /*******************************************OauthSystem*****************************************************/
        Task<OauthSystem> SystemInsertAsync(OauthSystem system);

        /*******************************************SessionToken*****************************************************/
        Task<SessionToken> SessionTokenGetAsync(Guid id);
        Task<List<SessionToken>> SessionTokensGetAsync(Guid accountId);
        Task SessionTokenDeleteAsync(Guid id);
        Task SessionTokenInsertAsync(SessionToken token, Guid? oldId, bool isRemoveOldSessions, bool isInsertAccountSystem);

        /*******************************************SessionTokenExternalHistory*****************************************************/
        Task<List<ViewSessionTokenExternalHistory>> SessionTokenExternalHistoriesAsync(Guid accountId);
        Task SessionTokenExternalHistorySetAsync(Guid accountId, string externalClientName, string externalAccountId, string email);
        Task SessionTokenExternalHistoryConnectAccountAsync(Guid accountId, string externalClientName, string externalAccountId, string email);
        Task SessionTokenExternalHistorySetStateAsync(Guid id, bool isUnlinked);
        /*******************************************Survey*****************************************************/
        Task SurveyAsync(Guid accountId, byte[] data, string lang);
        Task<List<AccountSurvey>> GetAllSurveyAsync();
    }
}