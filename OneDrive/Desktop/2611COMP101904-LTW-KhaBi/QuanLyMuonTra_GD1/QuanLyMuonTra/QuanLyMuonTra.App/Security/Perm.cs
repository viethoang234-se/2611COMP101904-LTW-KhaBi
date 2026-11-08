namespace QuanLyMuonTra.App.Security;

/// <summary>Mã quyền (khớp bảng Permissions trong SQL).</summary>
public static class Perm
{
    public const string DashboardView = "DASHBOARD_VIEW";
    public const string UserManage = "USER_MANAGE";
    public const string RoleManage = "ROLE_MANAGE";
    public const string CategoryView = "CATEGORY_VIEW";
    public const string CategoryManage = "CATEGORY_MANAGE";
    public const string DeviceManage = "DEVICE_MANAGE";
    public const string BorrowerManage = "BORROWER_MANAGE";
    public const string LoanCreate = "LOAN_CREATE";
    public const string LoanView = "LOAN_VIEW";
    public const string ReturnCreate = "RETURN_CREATE";
    public const string RecordView = "RECORD_VIEW";
    public const string RecordCancel = "RECORD_CANCEL";
    public const string OverdueView = "OVERDUE_VIEW";
    public const string ReportPrint = "REPORT_PRINT";
    public const string ReportView = "REPORT_VIEW";
    public const string CodeConfig = "CODE_CONFIG";
    public const string StatsView = "STATS_VIEW";
}
