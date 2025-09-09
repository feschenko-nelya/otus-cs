
using Core.Entity;
using LinqToDB.Mapping;

namespace HW2.Core.DataAccess.Models
{
    [Table("ToDoList")]
    public class ToDoListModel
    {
        [Column("id"), PrimaryKey, NotNull]
        public Guid Id { get; set; }
        [Column("userId"), NotNull]
        public Guid UserId { get; set; }
        [Column("name"), NotNull]
        public string Name { get; set; } = "";
        [Column("createdAt"), NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.UserId))]
        public required ToDoUser User { get; set; }
    }
}
