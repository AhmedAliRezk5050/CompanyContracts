namespace Infrastructure.Utility;

public class Permissions
{
    public static List<string> GeneratePermissions(string module)
    {
        return new List<string>
        {
            $"Permission.{module}.View",
            $"Permission.{module}.Add",
            $"Permission.{module}.Edit",
            $"Permission.{module}.Delete",
        };
    }
    
    public static List<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();

        foreach (var item in Enum.GetValues(typeof(Modules)))
        {
            allPermissions.AddRange(GeneratePermissions(item.ToString()));
        }

        return allPermissions;
    }
    
    public static class Funders
    {
        public const string View = "Permission.Funders.View";
        public const string Add = "Permission.Funders.Add";
        public const string Edit = "Permission.Funders.Edit";
        public const string Delete = "Permission.Funders.Delete";
    }
    
    public static class Contracts
    {
        public const string View = "Permission.Contracts.View";
        public const string Add = "Permission.Contracts.Add";
        public const string Edit = "Permission.Contracts.Edit";
        public const string Delete = "Permission.Contracts.Delete";
    }
    
    public static class InstallmentPayments
    {
        public const string View = "Permission.InstallmentPayments.View";
        public const string Add = "Permission.InstallmentPayments.Add";
        public const string Edit = "Permission.InstallmentPayments.Edit";
        public const string Delete = "Permission.InstallmentPayments.Delete";
    }
    
    public static class Destructions
    {
        public const string View = "Permission.Destructions.View";
        public const string Add = "Permission.Destructions.Add";
        public const string Edit = "Permission.Destructions.Edit";
        public const string Delete = "Permission.Destructions.Delete";
    }
    
    public static class Users
    {
        public const string View = "Permission.Users.View";
        public const string Add = "Permission.Users.Add";
        public const string Edit = "Permission.Users.Edit";
        public const string Delete = "Permission.Users.Delete";
    }
    
    public static class Claims
    {
        public const string View = "Permission.Claims.View";
        public const string Add = "Permission.Claims.Add";
        public const string Edit = "Permission.Claims.Edit";
        public const string Delete = "Permission.Claims.Delete";
    }
}