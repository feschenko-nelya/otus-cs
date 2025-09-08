-- Table: public.ToDoUser

-- DROP TABLE IF EXISTS public."ToDoUser";

CREATE TABLE IF NOT EXISTS public."ToDoUser"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "telegramId" integer NOT NULL,
    "telegramName" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "registeredAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT todouser_primary_id PRIMARY KEY (id),
    CONSTRAINT todouser_unique_id UNIQUE (id)
        INCLUDE(id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoUser"
    OWNER to postgres;
-- Index: index_todouser_telegramid

-- DROP INDEX IF EXISTS public.index_todouser_telegramid;

CREATE UNIQUE INDEX IF NOT EXISTS index_todouser_telegramid
    ON public."ToDoUser" USING btree
    ("telegramId" ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;
	
-- Table: public.ToDoList

-- DROP TABLE IF EXISTS public."ToDoList";

CREATE TABLE IF NOT EXISTS public."ToDoList"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 0 MINVALUE 0 MAXVALUE 2147483647 CACHE 1 ),
    "userId" integer NOT NULL,
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "createdAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT todolist_primary_id PRIMARY KEY (id),
    CONSTRAINT todolist_unique_name UNIQUE (name),
    CONSTRAINT todolist_foreign_userid_todouser_id FOREIGN KEY ("userId")
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoList"
    OWNER to postgres;
-- Index: index_todolist_userid

-- DROP INDEX IF EXISTS public.index_todolist_userid;

CREATE INDEX IF NOT EXISTS index_todolist_userid
    ON public."ToDoList" USING btree
    ("userId" ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;
	
-- Table: public.ToDoItem

-- DROP TABLE IF EXISTS public."ToDoItem";

CREATE TABLE IF NOT EXISTS public."ToDoItem"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 0 MINVALUE 0 MAXVALUE 2147483647 CACHE 1 ),
    "userId" integer NOT NULL,
    "listId" integer,
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "createdAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deadline timestamp with time zone,
    "stateChangedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    state "ToDoItemState" NOT NULL DEFAULT 1,
    CONSTRAINT todoitem_primary_id PRIMARY KEY (id),
    CONSTRAINT todouser_unique_name UNIQUE (name),
    CONSTRAINT todoitem_foreign_listid_todolist_id FOREIGN KEY ("listId")
        REFERENCES public."ToDoList" (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE SET NULL,
    CONSTRAINT todoitem_foreign_userid_todouser_id FOREIGN KEY ("userId")
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoItem"
    OWNER to postgres;
-- Index: index_todoitem_userid

-- DROP INDEX IF EXISTS public.index_todoitem_userid;

CREATE INDEX IF NOT EXISTS index_todoitem_userid
    ON public."ToDoItem" USING btree
    ("userId" ASC NULLS LAST, "listId" ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;
	
