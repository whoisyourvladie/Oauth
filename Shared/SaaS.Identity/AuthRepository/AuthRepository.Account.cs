using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public string HashPassword(string password)
        {
            return _userManager.PasswordHasher.HashPassword(password);
        }
        public bool PasswordIsEqual(string source, string password)
        {
            return _context.PasswordIsEqual(source, password);
        }

        public async Task<Account> AccountGetAsync(Guid accountId)
        {
            return await _context.AccountGetAsync(accountId);
        }
        public async Task<Account> AccountGetAsync(string userEmail, bool isIncludeSubEmails = false)
        {
            return await _context.AccountGetAsync(userEmail, isIncludeSubEmails: isIncludeSubEmails);
        }
        public async Task<Account> AccountGetAsync(string primaryEmail, string password)
        {
            return await _context.AccountGetAsync(primaryEmail, password);
        }
        public async Task<List<Account>> AccountsGetAsync(string filter, string globalOrderId)
        {
            return await _context.AccountsGetAsync(filter, globalOrderId);
        }
        public async Task<Account> AccountGetByTransactionOrderUidAsync(string transactionOrderUid)
        {
            return await _context.AccountGetByTransactionOrderUidAsync(transactionOrderUid);
        }
        public async Task AccountDeleteAsync(Guid accountId)
        {
            await _context.AccountDeleteAsync(accountId);
        }
        public async Task AccountDeleteAsync(Account user)
        {
            await _context.AccountDeleteAsync(user.Id);
        }

        public async Task AccountActivateAsync(Account account)
        {
            if (!object.Equals(account, null) && (!account.IsActivated || account.IsBusiness) && !account.IsAnonymous)
            {
                account.IsActivated = true;
                await _context.AccountActivateAsync(account.Id);
            }
        }
        public async Task AccountMaskAsBusinessAsync(Account account)
        {
            if (object.Equals(account, null) || account.IsBusiness)
                return;

            account.IsBusiness = true;
            account.RefreshModifyDate();

            await _userManager.UpdateAsync(account);
        }
        public async Task AccountOptinSetAsync(Account account, bool? optin)
        {
            if (object.Equals(account, null) || !optin.HasValue)
                return;

            if (account.Optin.GetValueOrDefault() == optin.Value)
                return;

            account.Optin = optin.Value;
            account.RefreshModifyDate();

            await _userManager.UpdateAsync(account);
        }
        public async Task AccountNeverLoginSetAsync(Account account)
        {
            if (object.Equals(account, null) || !account.NeverLogged)
                return;

            account.NeverLogged = false;

            await _userManager.UpdateAsync(account);
        }

        public async Task AccountVisitorIdSetAsync(Account account, Guid? visitorId)
        {
            if (object.Equals(account, null) || !visitorId.HasValue)
                return;

            await _context.AccountVisitorIdSetAsync(account.Id, visitorId.Value);
        }
        public async Task<IdentityResult> AccountCreateAsync(Account user, string password)
        {
            if (string.IsNullOrEmpty(password))
                return await _userManager.CreateAsync(user);

            return await _userManager.CreateAsync(user, password);
        }
        public async Task<IdentityResult> AccountChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }
        public async Task<IdentityResult> AccountConfirmEmailAsync(Guid userId)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
            token = ToBase64String(token);
            return await AccountConfirmEmailAsync(userId, token);
        }
        public async Task<IdentityResult> AccountConfirmEmailAsync(Guid userId, string token)
        {
            token = FromBase64String(token);
            return await _userManager.ConfirmEmailAsync(userId, token);
        }
        public async Task<IdentityResult> AccountResetPasswordAsync(Guid userId, string token, string password)
        {
            token = FromBase64String(token);
            return await _userManager.ResetPasswordAsync(userId, token, password);
        }
        public async Task<IdentityResult> AccountUpdateAsync(Account user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> AccountPasswordSetAsync(Account user, string password)
        {
            user.Password = HashPassword(password);
            return await _userManager.UpdateAsync(user);
        }

        public List<ViewAccountEmail> AccountEmailsGet(Guid accountId)
        {
            return _context.AccountEmailsGet(accountId);
        }

        public async Task<List<AccountSystem>> AccountSystemsGetAsync(AccountProductPair pair)
        {
            return await _context.AccountSystemsGetAsync(pair);
        }

        public async Task<Account> AccountAnonymousCreateAsync(string password)
        {
            var account = new Account
            {
                Email = string.Format("{0}@anonymous.saas.com", Guid.NewGuid().ToString("N")),
                IsAnonymous = true
            };

            var result = await _userManager.CreateAsync(account, password);

            if (!result.Succeeded)
                return null;

            account.Password = password;

            return account;
        }
    }
}