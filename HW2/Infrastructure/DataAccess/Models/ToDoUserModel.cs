using LinqToDB.Mapping;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoUser")]
    public class ToDoUserModel
    {
        [Column("id"), PrimaryKey, NotNull]
        public Guid Id { get; set; }
        [Column("telegramId"), NotNull]
        public long TelegramId { get; set; }
        [Column("telegramName"), NotNull]
        public string TelegramName { get; set; } = "";
        [Column("registeredAt"), NotNull]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
