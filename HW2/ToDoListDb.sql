-- Table: public.ToDoUser

-- DROP TABLE IF EXISTS public."ToDoUser";

CREATE TABLE IF NOT EXISTS public."ToDoUser"
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    registered_at time without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    telegram_id bigint NOT NULL,
    telegram_name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT ix_todouser_pk_id PRIMARY KEY (id),
    CONSTRAINT uq_todouser_telegram_id UNIQUE (telegram_id)
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
    created_at time without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT ix_todolist_pk_id PRIMARY KEY (id),
    CONSTRAINT uq_todolist_name UNIQUE (name),
    CONSTRAINT ix_todolist_user_id FOREIGN KEY (user_id)
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ToDoList"
    OWNER to postgres;

-- DOMAIN: public.ToDoItemState

-- DROP DOMAIN IF EXISTS public."ToDoItemState";

CREATE DOMAIN public."ToDoItemState"
    AS smallint
    DEFAULT 0
    NOT NULL;

ALTER DOMAIN public."ToDoItemState" OWNER TO postgres;

ALTER DOMAIN public."ToDoItemState"
    ADD CONSTRAINT enum CHECK (VALUE = ANY (ARRAY[0, 1, 2]));
	
-- Table: public.ToDoItem

-- DROP TABLE IF EXISTS public."ToDoItem";

CREATE TABLE IF NOT EXISTS public."ToDoItem"
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    user_id uuid NOT NULL,
    list_id uuid,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    state "ToDoItemState" NOT NULL DEFAULT 0,
    created_at time without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    state_changed_at time without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deadline time without time zone,
    CONSTRAINT ix_todoitem_pk_id PRIMARY KEY (id),
    CONSTRAINT uq_todoitem_name UNIQUE (name),
    CONSTRAINT ix_todoitem_fk_user_id FOREIGN KEY (user_id)
        REFERENCES public."ToDoUser" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT ix_todoitem_list_id FOREIGN KEY (list_id)
        REFERENCES public."ToDoList" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
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
