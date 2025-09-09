using LinqToDB.Mapping;
using Core.Entity;
using HW2.Core.Entities;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoItem")]
    public class ToDoItemModel
    {
        [Column("id"), PrimaryKey, Identity]
        public Guid Id { get; set; }
        [Column("userId"), NotNull]
        public Guid UserId { get; set; }
        [Column("listId")]
        public Guid? ListId { get; set; }
        [Column("name"), NotNull]
        public string Name { get; set; } = "";
        [Column("createdAt"), NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("deadline")]
        public DateTime? Deadline { get; set; }
        [Column("stateChangedAt"), NotNull]
        public DateTime StateChangedAt { get; set; } = DateTime.Now;
        [Column("state"), NotNull]
        public ToDoItemState State { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
        public required ToDoUserModel User { get; set; }

        [Association(ThisKey = nameof(ListId), OtherKey = nameof(List.Id))]
        public required ToDoListModel? List { get; set; }
    }
}
