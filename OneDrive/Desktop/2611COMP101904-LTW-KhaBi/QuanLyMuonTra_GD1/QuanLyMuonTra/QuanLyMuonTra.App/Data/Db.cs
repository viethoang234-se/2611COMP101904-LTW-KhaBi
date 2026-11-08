using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyMuonTra.App.Data;

/// <summary>Lỗi nghiệp vụ có thông báo hiển thị cho người dùng. Ném bên trong transaction để rollback toàn bộ.</summary>
public sealed class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

/// <summary>Kết nối SQL Server. Chuỗi kết nối đọc từ file connection.txt cạnh file .exe.</summary>
public static class Db
{
    const string DefaultConnectionString =
        "Server=localhost;Database=QuanLyMuonTra;Trusted_Connection=True;TrustServerCertificate=True;";

    public static string ConnectionString { get; } = Load();

    static string Load()
    {
        var file = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        if (File.Exists(file))
        {
            var text = File.ReadAllText(file).Trim();
            if (text.Length > 0) return text;
        }
        return DefaultConnectionString;
    }

    public static SqlConnection Open()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Chạy một nghiệp vụ trong SQL transaction: lỗi bất kỳ -> rollback toàn bộ.
    /// Tự thử lại tối đa 3 lần khi SQL Server chọn giao dịch này làm nạn nhân deadlock (mã lỗi 1205).
    /// </summary>
    public static T InTransaction<T>(Func<SqlConnection, SqlTransaction, T> work)
    {
        const int maxAttempts = 3;
        for (int attempt = 1; ; attempt++)
        {
            using var conn = Open();
            using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                var result = work(conn, tx);
                tx.Commit();
                return result;
            }
            catch (SqlException ex) when (ex.Number == 1205 && attempt < maxAttempts)
            {
                TryRollback(tx);
                Thread.Sleep(100 * attempt);
            }
            catch
            {
                TryRollback(tx);
                throw;
            }
        }
    }

    static void TryRollback(SqlTransaction tx)
    {
        try { tx.Rollback(); } catch { /* giao dịch đã bị SQL Server hủy */ }
    }

    /// <summary>Chuyển ngoại lệ thành thông báo tiếng Việt dễ hiểu cho người dùng.</summary>
    public static string Describe(Exception ex)
    {
        if (ex is BusinessException) return ex.Message;
        if (ex is SqlException sql)
        {
            return sql.Number switch
            {
                -2 => "Hết thời gian chờ SQL Server. Vui lòng thử lại.",
                -1 or 2 or 53 or 233 or 258 or 1231 or 10053 or 10054 or 10060 or 40 =>
                    "Không kết nối được tới SQL Server. Kiểm tra SQL Server đang chạy và chuỗi kết nối trong connection.txt.",
                4060 => "Không mở được cơ sở dữ liệu QuanLyMuonTra. Hãy chạy các script trong thư mục Database.",
                18456 => "SQL Server từ chối đăng nhập. Kiểm tra tài khoản trong connection.txt.",
                1205 => "Hệ thống đang bận xử lý đồng thời. Vui lòng thử lại.",
                2601 or 2627 => "Dữ liệu bị trùng (mã đã tồn tại).",
                547 => "Dữ liệu vi phạm ràng buộc của cơ sở dữ liệu (số lượng không hợp lệ hoặc đang được sử dụng).",
                _ => "Lỗi cơ sở dữ liệu: " + sql.Message,
            };
        }
        if (ex is InvalidOperationException && ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase))
            return "Không kết nối được tới cơ sở dữ liệu.";
        return "Đã xảy ra lỗi: " + ex.Message;
    }
}
