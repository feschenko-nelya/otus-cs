using LinqToDB.Mapping;
using Core.Entity;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoItem")]
    public class ToDoItemModel
    {
        [Column("id"), PrimaryKey, Identity]
        public Guid Id { get; set; }

        [Column("user_id"), NotNull]
        public Guid UserId { get; set; }

        [Column("list_id")]
        public Guid? ListId { get; set; }

        [Column("name"), NotNull]
        public string Name { get; set; } = "";

        [Column("created_at"), NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("deadline")]
        public DateTime? Deadline { get; set; }

        [Column("state_changed_at"), NotNull]
        public DateTime StateChangedAt { get; set; } = DateTime.Now;

        [Column("state"), NotNull]
        public ToDoItemState State { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
        public required ToDoUserModel User { get; set; }

        [Association(ThisKey = nameof(ListId), OtherKey = nameof(List.Id))]
        public required ToDoListModel? List { get; set; }
    }
}
