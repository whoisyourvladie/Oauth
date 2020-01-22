namespace SaaS.Mailer
{
    public enum EmailTemplate
    {
        None, //когда нет mapping с базой, то прилетает от базы нулевой templateId или вообще ничего не прилетает

        AccountCreationCompleteNotification,

        BusinessDownloadNewAccountNotification,
        BusinessDownloadNotification,
        BusinessToPremiumNotification,

        EmailChangeConfirmationNotification,
        EmailChangeNotification,
        EmailConfirmationLateNotification,
        EmailConfirmationCovermountNotification,
        EmailConfirmationNotification,

        eSignEmailConfirmationNotification,
        eSignSignPackageNotification,

        PasswordChangedNotification,
        RecoverPasswordNotification,
        ProductSuspendNotification,

        OsSunsetNotification,
        PolicyUpdateNotification,

        ProductAssignedNotification,
        ProductAssignedNotificationCreatePassword,

        ProductEditionAssignedNotification,
        ProductEditionAssignedNotificationCreatePassword,

        WelcomeNotification,
        WelcomeFreeProductCovermountNotification,
        WelcomeFreeProductNotification,

        WelcomePurchaseNotification,
        WelcomePurchaseHomePlanNotification,
        WelcomePurchasePremiumPlanNotification,
        WelcomePurchaseBasicPlanNotification,
        WelcomePurchaseBusinessPlanNotification,

        //PdfSam products
        WelcomePurchaseStandardPlanNotification,
        WelcomePurchaseConvertPlanNotification,
        WelcomePurchaseProOcrEditionNotification,
        WelcomePurchaseStandardEditionNotification,
        WelcomePurchaseProPlanNotification,
        WelcomePurchaseProOcrPlanNotification,
        WelcomePurchaseOcrPlanNotification,
        WelcomePurchaseEditPlanNotification,
        // =============


        WelcomePurchaseHomeEditionNotification,
        WelcomePurchaseProEditionNotification,
        WelcomePurchasePremiumEditionNotification,

        WelcomePurchaseEnterpriseNotification,

        ProductUnassignedNotification,
        ProductRenewalOffNotification,

        LegacyActivationCreatePasswordNotification,
        LegacyActivationSignInNotification,
        LegacyCreatePasswordReminder,

        MergeConfirmationNotification,
        MicrotransactionCreatePasswordNotification,

        WelcomePurchaseMigrationFromSuiteNotification
    }
}