-- Table: public.ToDoUser

-- DROP TABLE IF EXISTS public."ToDoUser";

CREATE TABLE IF NOT EXISTS public."ToDoUser"
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    registered_at time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    telegram_id bigint NOT NULL,
    telegram_name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT todouser_primary_id PRIMARY KEY (id),
    CONSTRAINT todouser_unique_id UNIQUE (id, telegram_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoUser"
    OWNER to postgres;

-- Table: public.ToDoList

-- DROP TABLE IF EXISTS public."ToDoList";

CREATE TABLE IF NOT EXISTS public."ToDoList"
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    user_id uuid NOT NULL,
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    created_at time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT todolist_primary_id PRIMARY KEY (id),
    CONSTRAINT todolist_unique_name UNIQUE (name),
    CONSTRAINT todolist_foreign_userid FOREIGN KEY (user_id)
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoList"
    OWNER to postgres;

-- Table: public.ToDoItem

-- DROP TABLE IF EXISTS public."ToDoItem";

CREATE TABLE IF NOT EXISTS public."ToDoItem"
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    user_id uuid NOT NULL,
    list_id uuid,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    state "ToDoItemState" NOT NULL DEFAULT 0,
    created_at time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    state_changed_at time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deadline time with time zone,
    CONSTRAINT todoitem_primary_id PRIMARY KEY (id),
    CONSTRAINT todoitem_unique_name UNIQUE (name),
    CONSTRAINT todoitem_foreign_list_id FOREIGN KEY (list_id)
        REFERENCES public."ToDoList" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT todoitem_foreign_user_id FOREIGN KEY (user_id)
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoItem"
    OWNER to postgres;
-- Index: index_todoitem_userid

-- DROP INDEX IF EXISTS public.index_todoitem_userid;

CREATE INDEX IF NOT EXISTS index_todoitem_userid
    ON public."ToDoItem" USING btree
    (user_id ASC NULLS LAST, list_id ASC NULLS LAST)
    WITH (deduplicate_items=True)
    TABLESPACE pg_default;

