-- ToDoUser

insert into "ToDoUser" ("telegramId", "telegramName")
values(328775192, 'nelya_yenko');

-- ToDoList

insert into "ToDoList"(user_id, name)
values((select id from "ToDoUser" limit 1), 'first');

insert into "ToDoList"(user_id, name)
values((select id from "ToDoUser" limit 1), 'second');

-- ToDoUser

insert into "ToDoItem"(user_id, list_id, name)
values((select id from "ToDoUser" limit 1), null, 'without list');

insert into "ToDoItem"(user_id, list_id, name, deadline)
values((select id from "ToDoUser" limit 1), 
		(select id from "ToDoList" where name = 'first'), 
		'first task', 
		'01.10.2025 18:00:00');

insert into "ToDoItem"(user_id, list_id, name, deadline)
values((select id from "ToDoUser" limit 1), 
	   (select id from "ToDoList" where name = 'first'), 
	   'second task', 
	   '10.10.2025 18:00:00');

insert into "ToDoItem"(user_id, list_id, name, deadline)
values((select id from "ToDoUser" limit 1), 
	   (select id from "ToDoList" where name = 'second'), 
	   'task A', 
	   '01.10.2025 18:00:00');

insert into "ToDoItem"(user_id, list_id, name, deadline)
values((select id from "ToDoUser" limit 1), 
	   (select id from "ToDoList" where name = 'second'), 
	   'task B', 
	   '10.10.2025 18:00:00');
