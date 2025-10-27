// GameRecordService.cs
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace snakegame
{
    public class GameRecordService
    {
        private readonly string _connectionString =
            "Server=localhost;Database=Master;Integrated Security=true;Trusted_Connection=true;TrustServerCertificate=true;";
        public async Task SaveScoreAsync(int score, string playerName = "Anonymous")
        {
            const string sql = "INSERT INTO GameScore (UserID, Score) VALUES (@PlayerName, @Score);";
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PlayerName", playerName ?? "Anonymous");
                command.Parameters.AddWithValue("@Score", score);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // 建议记录日志，这里简单输出到控制台
                Console.WriteLine($"保存得分失败: {ex.Message}");
            }
        }
    }
}