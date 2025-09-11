
using Core.Entity;
using HW2.Core.Entities;
using LinqToDB;
using LinqToDB.Data;

namespace HW2.Core.DataAccess
{
    public class ToDoDataContext : DataConnection
    {
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString)
        { }
        public ITable<ToDoUser> User => this.GetTable<ToDoUser>();
        public ITable<ToDoList> List => this.GetTable<ToDoList>();
        public ITable<ToDoItem> Item => this.GetTable<ToDoItem>();
    }
}
