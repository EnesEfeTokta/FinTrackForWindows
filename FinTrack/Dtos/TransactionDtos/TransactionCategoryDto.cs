using FinTrackForWindows.Enums;
using System.Text.Json.Serialization;

namespace FinTrackForWindows.Dtos.TransactionDtos
{
    public class TransactionCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
