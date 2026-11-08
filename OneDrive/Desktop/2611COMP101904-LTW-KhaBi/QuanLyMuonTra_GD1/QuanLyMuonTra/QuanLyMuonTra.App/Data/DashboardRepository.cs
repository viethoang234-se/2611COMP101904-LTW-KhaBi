using Dapper;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class DashboardRepository
{
    public static DashboardStats GetStats()
    {
        using var conn = Db.Open();
        return conn.QuerySingle<DashboardStats>(
            @"SELECT
                (SELECT COUNT(*) FROM Devices WHERE IsActive = 1) AS TotalDevices,
                (SELECT ISNULL(SUM(AvailableQuantity), 0) FROM Devices WHERE IsActive = 1) AS AvailableUnits,
                (SELECT COUNT(*) FROM LoanSlips WHERE Status = 0) AS ActiveLoans,
                (SELECT COUNT(*) FROM LoanSlips WHERE Status = 0 AND DueDate < CAST(SYSDATETIME() AS date)) AS OverdueLoans");
    }

    public static List<RecentSlip> GetRecentSlips(int top = 8)
    {
        using var conn = Db.Open();
        return conn.Query<RecentSlip>(
            @"SELECT TOP (@top)
                s.SlipCode,
                b.FullName AS BorrowerName,
                s.BorrowDate,
                s.DueDate,
                CASE WHEN s.Status = 1 THEN N'Đã trả'
                     WHEN s.DueDate < CAST(SYSDATETIME() AS date) THEN N'Quá hạn'
                     ELSE N'Đang mượn' END AS Status
              FROM LoanSlips s
              JOIN Borrowers b ON b.BorrowerId = s.BorrowerId
              ORDER BY s.LoanSlipId DESC",
            new { top }).ToList();
    }
}
