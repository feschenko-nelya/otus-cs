
using System.Reflection;
using Core.Entity;
using HW2.Core.DataAccess.Models;
using HW2.Core.Entities;

namespace HW2.Infrastructure.DataAccess
{
    internal static class ModelMapper
    {
        public static ToDoUser MapFromModel(ToDoUserModel model)
        {
            return new ToDoUser
            {
                UserId = model.Id,
                TelegramUserId = model.TelegramId,
                TelegramUserName = model.TelegramName,
                RegisteredAt = model.RegisteredAt
            };
        }
        public static ToDoUserModel MapToModel(ToDoUser entity)
        {
            return new ToDoUserModel
            { 
                Id = entity.UserId,
                TelegramId = entity.TelegramUserId,
                TelegramName = entity.TelegramUserName,
                RegisteredAt = entity.RegisteredAt
            };

        }
        public static ToDoItem MapFromModel(ToDoItemModel model)
        {
            return new ToDoItem
            {
                Id = model.Id,
                UserId = model.UserId,
                ListId = model.ListId,
                Name = model.Name,
                State = model.State,
                CreatedAt = model.CreatedAt,
                Deadline = model.Deadline,
                StateChangedAt = model.StateChangedAt
            };
        }
        public static ToDoItemModel MapToModel(ToDoItem entity)
        {
            return new ToDoItemModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ListId = entity.ListId,
                Name = entity.Name,
                State = entity.State,
                CreatedAt = entity.CreatedAt,
                Deadline = entity.Deadline,
                StateChangedAt = entity.StateChangedAt,

                User = new ToDoUser { },
                List = new ToDoList { }
            };
        }
        public static ToDoList MapFromModel(ToDoListModel model)
        {
            return new ToDoList
            {
                Id = model.Id,
                UserId = model.UserId,
                Name = model.Name,
                CreatedAt = model.CreatedAt
            };
        }
        public static ToDoListModel MapToModel(ToDoList entity)
        {
            return new ToDoListModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name ?? "",
                CreatedAt = entity.CreatedAt,

                User = new ToDoUser
                { }
            };
        }
    }
}
