
using HW2.Core.DataAccess.Models;
using LinqToDB.Mapping;

namespace HW2.Infrastructure.DataAccess.Models
{
    [Table("Notification")]
    public class NotificationModel
    {
        [Column("id"), PrimaryKey, Identity]
        public Guid Id;

        [Column("user_id"), NotNull]
        public Guid UserId;

        [Column("notify_type")]
        public required string Type; //Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}
        
        [Column("text")]
        public required string Text; //Текст, который будет отправлен
        
        [Column("scheduled_at")]
        public DateTime ScheduledAt; //Запланированная дата отправки
        
        [Column("is_notified")]
        public bool IsNotified; //Флаг отправки
        
        [Column("notified_at")]
        public DateTime? NotifiedAt; //Фактическая дата отправки

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
        public required ToDoUserModel User { get; set; }
    }
}
