using SaaS.Mailer.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SaaS.WinService.Mailer.Qa
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            cbxLanguage.SelectedIndex = 0;

            var emailTemplates = Enum.GetNames(typeof(EmailTemplate)).OrderBy(e => e);
            foreach (var emailTemplate in emailTemplates)
                cbxEmailTemplate.Items.Add(emailTemplate);

            cbxEmailTemplate.SelectedIndex = 0;
            cbxProducts.SelectedIndex = 0;
        }

        private string FirstName { get { return txtFirstName.Text.Trim(); } }
        private string LastName { get { return txtLastName.Text.Trim(); } }
        private string Email { get { return txtEmail.Text.Trim(); } }
        private string Language { get { return cbxLanguage.Text.Trim(); } }
        private string Product { get { return cbxProducts.Text.Trim(); } }
        private EmailTemplate emailTemplate { get { return (EmailTemplate)Enum.Parse(typeof(EmailTemplate), cbxEmailTemplate.SelectedItem.ToString()); } }

        private enum EmailTemplate
        {
            //SupportIntro, TipsTricks
            AccountCreationCompleteNotification,
            BusinessToPremiumNotification,
            eSignEmailConfirmationNotification,
            eSignSignPackageNotification,
            MicrotransactionCreatePassword,
            MergeConfirmation,
            PasswordChangedNotification,
            ProductAssignedNotificationCreatePassword,
            Welcome,
            WelcomePurchase,
            WelcomeFreeProductCovermountNotification,
            EmailConfirmationCovermount,
            EmailConfirmation,
            EmailConfirmationLate,
            EmailChange,
            EmailChangeConfirmation,
            OsSunset,
            PolicyUpdate,
            ProductSuspend,
            ProductRenewalOff,
            ProductAssignedCreatePassword,
            ProductUnassigned,
            WelcomePurchaseMigrationFromSuiteNotification
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var notification = new Notification { FirstName = FirstName, LastName = LastName, Email = Email };

            switch (emailTemplate)
            {
                case EmailTemplate.AccountCreationCompleteNotification: Sender.AccountCreationCompleteNotification(Language, notification); break;
                case EmailTemplate.BusinessToPremiumNotification: Sender.BusinessToPremiumNotification(Language, notification); break;
                case EmailTemplate.eSignEmailConfirmationNotification: Sender.eSignEmailConfirmationNotification(Language, notification); break;
                case EmailTemplate.eSignSignPackageNotification: Sender.eSignSignPackageNotification(Language, notification); break;
                case EmailTemplate.MicrotransactionCreatePassword: Sender.MicrotransactionCreatePassword(Language, notification); break;
                case EmailTemplate.MergeConfirmation: Sender.MergeConfirmation(Language, notification); break;
                case EmailTemplate.PasswordChangedNotification: Sender.PasswordChangedNotification(Language, notification); break;
                case EmailTemplate.ProductAssignedNotificationCreatePassword: Sender.ProductAssignedNotificationCreatePassword(Language, notification); break;
                case EmailTemplate.Welcome: Sender.Welcome(Language, notification); break;
                case EmailTemplate.WelcomePurchase: Sender.WelcomePurchase(Language, notification, Product); break;
                case EmailTemplate.WelcomeFreeProductCovermountNotification: Sender.WelcomeFreeProductCovermountNotification(Language, notification, Product); break;
                case EmailTemplate.WelcomePurchaseMigrationFromSuiteNotification: Sender.WelcomePurchaseMigrationFromSuiteNotification(Language, notification, Product); break;

                case EmailTemplate.EmailConfirmationCovermount: Sender.EmailConfirmationCovermount(Language, notification); break;
                case EmailTemplate.EmailConfirmation: Sender.EmailConfirmation(Language, notification); break;
                case EmailTemplate.EmailConfirmationLate: Sender.EmailConfirmationLate(Language, notification); break;
                case EmailTemplate.EmailChange: Sender.EmailChange(Language, notification); break;
                case EmailTemplate.EmailChangeConfirmation: Sender.EmailChangeConfirmation(Language, notification); break;

                case EmailTemplate.OsSunset: Sender.OsSunset(Language, notification); break;
                case EmailTemplate.PolicyUpdate: Sender.PolicyUpdate(Language, notification); break;
                case EmailTemplate.ProductSuspend: Sender.ProductSuspend(Language, notification, Product); break;
                case EmailTemplate.ProductRenewalOff: Sender.ProductRenewalOff(Language, notification, Product); break;


                case EmailTemplate.ProductAssignedCreatePassword: Sender.ProductAssignedCreatePassword(Language, notification, Product); break;
                case EmailTemplate.ProductUnassigned: Sender.ProductUnassigned(Language, notification, Product); break;

                default:
                    MessageBox.Show("Email template is not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            MessageBox.Show("Done");
        }

        private void cbxEmailTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblProducts.Visible = emailTemplate == EmailTemplate.ProductSuspend ||
                                  emailTemplate == EmailTemplate.ProductRenewalOff ||
                                  emailTemplate == EmailTemplate.ProductRenewalOff;
            cbxProducts.Visible = lblProducts.Visible;
        }
    }
}