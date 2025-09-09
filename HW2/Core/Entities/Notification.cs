
using Core.Entity;

namespace HW2.Core.Entities
{
    public class Notification
    {
        public Guid Id;
        public required ToDoUser User;
        public required string Type; //Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}
        public required string Text; //Текст, который будет отправлен
        public DateTime ScheduledAt; //Запланированная дата отправки
        public bool IsNotified; //Флаг отправки
        public DateTime? NotifiedAt; //Фактическая дата отправки
    }
}
