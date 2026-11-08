using Microsoft.Data.SqlClient;

namespace QuanLyMuonTra.App.Data;

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
}
