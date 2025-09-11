
using LinqToDB.Mapping;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoList")]
    public class ToDoListModel
    {
        [Column("id"), PrimaryKey, NotNull]
        public Guid Id { get; set; }
        [Column("user_id"), NotNull]
        public Guid UserId { get; set; }
        [Column("name"), NotNull]
        public string Name { get; set; } = "";
        [Column("created_at"), NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
        public required ToDoUserModel User { get; set; }
    }
}
