using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using SaaS.Mailer.Models;
using System;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Notification
{
    public class NotificationManager
    {
        private readonly IAuthRepository _auth;
        private readonly SaaS.Mailer.EmailRepository _repository;
        private readonly NotificationSettings _settings;

        private async Task<SaaS.Mailer.Models.Notification> CreateNotification(Account account)
        {
            var uriBuilder = new UriBuilder(_settings.DownloadLink);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("email", account.Email);

            if (account.IsBusiness)
            {
                query.Add("build", "b2b");
                query.Add(await _auth.GenerateEmailConfirmationToken(account));
            }
            uriBuilder.Query = query.ToString();

            return new SaaS.Mailer.Models.Notification
            {
                AccountId = account.Id,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                DownloadLink = uriBuilder.Uri.ToString()
            };
        }
        private async Task<CreatePasswordNotification> CreatePasswordNotification(Account targetAccount)
        {
            var targetUserNotification = await CreateNotification(targetAccount);
            var notification = new CreatePasswordNotification(targetUserNotification);

            var uri = await _auth.GeneratePasswordResetTokenLinkAsync(targetAccount, _settings.CreatePassword);
            notification.CreatePasswordLink = uri.ToString();

            return notification;
        }

        public NotificationManager(IAuthRepository auth, NotificationSettings settings)
        {
            _auth = auth;

            _repository = new SaaS.Mailer.EmailRepository();
            _settings = settings;
        }

        public async Task AccountCreationComplete(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);

            var uri = await _auth.GeneratePasswordResetTokenLinkAsync(targetUser, _settings.ResetPassword);
            var notification = new RecoverPasswordNotification(targetUserNotification, uri.ToString());

            await _repository.SendAccountCreationCompleteNotification(notification);
        }
        public async Task BusinessDownloadNewAccount(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);

            await _repository.SendBusinessDownloadNewAccountNotification(targetUserNotification);
        }
        public async Task BusinessDownload(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);

            await _repository.SendBusinessDownloadNotification(targetUserNotification);
        }
        public async Task PasswordChanged(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);

            await _repository.SendPasswordChangedNotification(targetUserNotification);
        }
        public async Task ProductSuspend(Guid accountId, ViewOwnerProduct product, DateTime? nextRebillDate)
        {
            if (nextRebillDate.HasValue)
                return;

            if (product.EndDate.HasValue &&
                product.EndDate.Value > DateTime.UtcNow && (product.EndDate.Value - DateTime.UtcNow).TotalDays <= 10)
                return;

            var targetUser = await _auth.AccountGetAsync(accountId);
            var targetUserNotification = await CreateNotification(targetUser);
            var productSuspendNotification = new ProductSuspendNotification(targetUserNotification, product.ProductName, product.EndDate);

            await _repository.SendProductSuspendNotification(productSuspendNotification);
        }
        public async Task ProductAssigned(Guid accountId, ViewOwnerProduct product, Account targetUser)
        {
            if (object.Equals(targetUser, null) || targetUser.Id == accountId)
                return;

            var ownerUser = await _auth.AccountGetAsync(accountId);

            var targetUserNotification = await CreateNotification(targetUser);
            var ownerUserNotification = await CreateNotification(ownerUser);

            var notification = new ProductAssignedNotification(targetUserNotification, product.ProductName, ownerUserNotification);

            if (targetUser.IsEmptyPassword())
            {
                var uri = await _auth.GeneratePasswordResetTokenLinkAsync(targetUser, _settings.CreatePassword);
                notification.CreatePasswordLink = uri.ToString();
            }

            if (product.IsPPC)
                await _repository.SendProductEditionAssignedNotification(notification);
            else
                await _repository.SendProductAssignedNotification(notification);
        }
        public async Task ProductUnassigned(Guid accountId, ViewOwnerProduct product, Account targetUser)
        {
            if (object.Equals(targetUser, null) || targetUser.Id == accountId)
                return;

            var ownerUser = await _auth.AccountGetAsync(accountId);

            var targetUserNotification = await CreateNotification(targetUser);
            var ownerUserNotification = await CreateNotification(ownerUser);

            var notification = new ProductAssignedNotification(targetUserNotification, product.ProductName, ownerUserNotification);

            await _repository.SendProductUnassignedNotification(notification);
        }
        public async Task RecoverPassword(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);

            var uri = await _auth.GeneratePasswordResetTokenLinkAsync(targetUser, _settings.ResetPassword);
            var notification = new RecoverPasswordNotification(targetUserNotification, uri.ToString());

            await _repository.SendRecoverPasswordNotification(notification);
        }
        public async Task EmailConfirmationCovermount(Account targetUser)
        {
            var uri = await _auth.GenerateEmailConfirmationTokenLinkAsync(targetUser, _settings.EmailConfirmation);

            var targetUserNotification = await CreateNotification(targetUser);
            var activationNotification = new EmailConfirmationNotification(targetUserNotification, uri.ToString());

            await _repository.SendEmailConfirmationCovermountNotification(activationNotification);
        }
        public async Task EmailConfirmation(Account targetUser)
        {
            var uri = await _auth.GenerateEmailConfirmationTokenLinkAsync(targetUser, _settings.EmailConfirmation);

            var targetUserNotification = await CreateNotification(targetUser);
            var activationNotification = new EmailConfirmationNotification(targetUserNotification, uri.ToString());

            await _repository.SendEmailConfirmationNotification(activationNotification);
        }
        public async Task EmailChangeConfirmationNotification(Account targetUser, string newEmail)
        {
            var pending = await _auth.AccountSubEmailPendingSetAsync(targetUser.Id, newEmail);
            var uri = _auth.GenerateEmailChangeConfirmationTokenLinkAsync(_settings.EmailChangeConfirmation, pending);

            var targetUserNotification = await CreateNotification(targetUser);
            var activationNotification = new EmailChangeConfirmationNotification(targetUserNotification, newEmail, uri.ToString());

            await _repository.SendEmailChangeConfirmationNotification(activationNotification);
        }
        public async Task EmailChange(Account targetUser, string newEmail)
        {
            var targetUserNotification = await CreateNotification(targetUser);
            var activationNotification = new EmailChangeNotification(targetUserNotification, newEmail);

            await _repository.SendEmailChangeNotification(activationNotification);
        }

        public async Task MergeConfirmationNotification(Account targetUser, Account targetUserFrom, Account targetUserPrimaryEmail)
        {
            var pending = await _auth.AccountMergePendingMergeAsync(targetUser.Id, targetUserFrom.Id, targetUserPrimaryEmail.Id);
            var uri = _auth.GenerateMergeConfirmationTokenLinkAsync(_settings.MergeConfirmation, pending);

            var targetUserNotification = await CreateNotification(targetUserFrom);
            var activationNotification = new MergeConfirmationNotification(targetUserNotification, uri.ToString());

            await _repository.SendMergeConfirmationNotification(activationNotification);
        }

        public async Task LegacyActivationCreatePassword(Account targetUser)
        {
            var notification = await CreatePasswordNotification(targetUser);
            await _repository.SendLegacyActivationCreatePasswordNotification(notification);
        }

        public async Task LegacyActivationSignInNotification(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);
            await _repository.SendLegacyActivationSignInNotification(targetUserNotification);
        }

        public async Task LegacyCreatePasswordReminder(Account targetUser)
        {
            var notification = await CreatePasswordNotification(targetUser);
            await _repository.SendLegacyCreatePasswordReminder(notification);
        }
        public async Task Welcome(Account targetUser)
        {
            var targetUserNotification = await CreateNotification(targetUser);
            await _repository.SendWelcomeNotification(targetUserNotification);
        }
        public async Task MicrotransactionCreatePassword(Account targetUser)
        {
            var notification = await CreatePasswordNotification(targetUser);
            await _repository.MicrotransactionCreatePassword(notification);
        }
    }
    public class NotificationSettings
    {
        public Uri DownloadLink { get; set; }
        public Uri ResetPassword { get; set; }
        public Uri CreatePassword { get; set; }
        public Uri EmailConfirmation { get; set; }
        public Uri MergeConfirmation { get; set; }
        public Uri EmailChangeConfirmation { get; set; }
    }
}