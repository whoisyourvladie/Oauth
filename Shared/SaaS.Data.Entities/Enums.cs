using System;

namespace SaaS.Data.Entities
{
    public enum ApplicationTypes
    {
        Web = 1,
        Desktop
    };

    public enum Status : byte
    {
        None = 0,
        InProcess = 1,
        Complete = 2,
        Error = 3,
        Deleted = 4,
        NotStarted = 5
    }

    [Flags]
    public enum AccountStatus : ulong
    {
        None = 0,
        IsActivated = 1 << 0,
        IsAnonymous = 1 << 1,
        IsBusiness = 1 << 2,
        IsPreview = 1 << 3
    }

    [Flags]
    public enum ProductStatus : ulong
    {
        None = 0,
        IsDisabled = 1 << 0,
        IsExpired = 1 << 1,
        IsTrial = 1 << 2,
        IsFree = 1 << 3,
        IsMinor = 1 << 4,
        IsActive = 1 << 5,
        IsPPC = 1 << 6,
        IsUpgradable = 1 << 7,
        IsNew = 1 << 8,
        IsPaymentFailed = 1 << 9,
        IsRenewal = 1 << 10,
        IsOwner = 1 << 11,
        IsNotAbleToRenewCreditCartExpired = 1 << 12,
        IsRefund = 1 << 13,
        IsChargeback = 1 << 14,
        IsUpgraded = 1 << 15,
        IsManualCancelled = 1 << 16,
        IsNotSupportedClient = 1 << 17,
        IsBusiness = 1 << 18
    }

    public enum AssignStatus
    {
        Ok = 0,
        Failed = -50000,
        FailAlreadyWasAssigned = -50013
    }
    public enum UnassignStatus
    {
        Ok = 0,
        Failed = -50000,
        FailAlreadyWasUnassigned = -50013
    }

    public enum AllowedCountStatus
    {
        Ok = 0,
        Failed = -50000,
        FailCantChangeAllowedCount = -50016
    }

    public enum eSignTransactionInitiatorType
    {
        Free = 1,
        Product,
        Microtransaction
    }
}