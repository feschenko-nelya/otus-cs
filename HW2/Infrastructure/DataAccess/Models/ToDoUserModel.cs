using LinqToDB.Mapping;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoUser")]
    public class ToDoUserModel
    {
        [Column("id"), PrimaryKey, NotNull]
        public Guid Id { get; set; }
        [Column("telegram_id"), NotNull]
        public long TelegramId { get; set; }
        [Column("telegram_name"), NotNull]
        public string TelegramName { get; set; } = "";
        [Column("registered_at"), NotNull]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
