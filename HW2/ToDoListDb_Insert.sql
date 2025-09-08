-- ToDoUser

insert into "ToDoUser" ("telegramId", "telegramName")
values(328775192, 'nelya_yenko');

-- ToDoList

insert into "ToDoList"("userId", name)
values (5, 'first');

insert into "ToDoList"("userId", name)
values (5, 'second');

-- ToDoUser

insert into "ToDoItem"("userId", "listId", name)
values(5, null, 'common');

insert into "ToDoItem"("userId", "listId", name, deadline)
values(5, 0, 'first task', '01.10.2025 18:00:00');

insert into "ToDoItem"("userId", "listId", name, deadline)
values(5, 0, 'second task', '10.10.2025 18:00:00');

insert into "ToDoItem"("userId", "listId", name, deadline)
values(5, 1, 'task A', '01.10.2025 18:00:00');

insert into "ToDoItem"("userId", "listId", name, deadline)
values(5, 1, 'task B', '10.10.2025 18:00:00');
