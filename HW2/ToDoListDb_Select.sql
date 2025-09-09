-- GetAllByUserId

select * from "ToDoItem" where user_id=:user_id order by id;

-- GetActiveByUserId

select * from "ToDoItem" where user_id=:user_id and state=1 order by id;

-- Get

select * from "ToDoItem" where id=:id;

-- ExistsByName

select id from "ToDoItem" where user_id=:user_id and name=:name;

-- CountActive

select count(id) from "ToDoItem" where user_id=:user_id and state=1;
