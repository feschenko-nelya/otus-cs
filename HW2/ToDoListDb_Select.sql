-- GetAllByUserId

select * from "ToDoItem" where "userId"=:userId order by id;

-- GetActiveByUserId

select * from "ToDoItem" where "userId"=:userId and state=1 order by id;

-- Get

select * from "ToDoItem" where id=:id;

-- ExistsByName

select id from "ToDoItem" where "userId"=:userId and name=:name;

-- CountActive

select count(id) from "ToDoItem" where "userId"=:userId and state=1;
