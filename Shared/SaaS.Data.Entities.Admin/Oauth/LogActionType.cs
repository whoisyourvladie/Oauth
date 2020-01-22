namespace SaaS.Data.Entities.Admin.Oauth
{
    public class LogActionType : Entity<int>
    {
        public string Name { get; set; }
    }

    public enum LogActionTypeEnum
    {
        UserActivate = 1,
        UserDeactivate = 2,
        UserEdit = 3,
        UserCreate = 4,
        UserEditPassword = 5,

        AccountCreate = 6,
        AccountEdit = 7,
        AccountDelete = 8,
        AccountMaskBusiness = 9,
        AccountActivate = 10,
        AccountChangePassword = 11,
        AccountProductAdd = 12,
        AccountProductAssign = 13,
        AccountProductUnassign = 14,
        AccountProductDeactivate = 15,
        AccountMerge = 16,
        AccountProductNextRebillDateEdit = 17,
        AccountProductEndDateEdit = 18
    }
}