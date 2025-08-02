DROP TABLE IF EXISTS public.service_instance;
DROP TABLE IF EXISTS public.role_permission_map;
DROP TABLE IF EXISTS public.role_user_map;
DROP TABLE IF EXISTS public.token;
DROP TABLE IF EXISTS public.queue;
DROP TABLE IF EXISTS public.service_info;
DROP TABLE IF EXISTS public.permission;
DROP TABLE IF EXISTS public.role;
DROP TABLE IF EXISTS public."user";

CREATE TABLE IF NOT EXISTS public."user"
(
    user_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY NOT NULL ,
    name text COLLATE pg_catalog."default" NOT NULL,
    email character varying(100) COLLATE pg_catalog."default" NOT NULL,
    password_hash text COLLATE pg_catalog."default" NOT NULL
) TABLESPACE pg_default ;

ALTER TABLE IF EXISTS public."user"
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS public.role
(
    role_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY NOT NULL ,
    role_name text COLLATE pg_catalog."default" NOT NULL
)

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.role
    OWNER to postgres;


CREATE TABLE IF NOT EXISTS public.permission
(
    permission_id BIGINT PRIMARY KEY NOT NULL ,
    permission_name text COLLATE pg_catalog."default" NOT NULL
    )

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.permission
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS public.service_info
(
    service_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY NOT NULL ,
    service_name text COLLATE pg_catalog."default" NOT NULL,
    service_description text COLLATE pg_catalog."default",
    usual_service_days integer NOT NULL,
    service_fee double precision NOT NULL
)

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.service_info
    OWNER to postgres;
 

CREATE TABLE IF NOT EXISTS public.queue
(
    queue_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY NOT NULL ,
    queue_name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    user_id integer NOT NULL,
    queue_status integer NOT NULL
    )

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.queue
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS public.sequence
(
    -- The prefix for the sequence (e.g., 'INV', 'ORD')
    sequence_prefix text COLLATE pg_catalog."default" NOT NULL,

    -- The generated 3-character date token (e.g., '57T')
    date_part text COLLATE pg_catalog."default" NOT NULL,

    -- The sequence number for that prefix and date, from 1 to 999
    sequence_no BIGINT NOT NULL,

    -- The timestamp when the record was created
    generated_at timestamp without time zone NOT NULL DEFAULT now(),

    -- Defines the composite primary key
    CONSTRAINT sequence_pkey PRIMARY KEY (sequence_prefix, date_part, sequence_no)
);

-- Optional: Set the owner of the table
ALTER TABLE IF EXISTS public.sequence
    OWNER to postgres;

-- Optional: Add a comment to describe the table's purpose
COMMENT ON TABLE public.sequence
    IS 'Stores sequence numbers that reset daily for different prefixes.';

------------- FUNC -------------

-- Drop the function if it already exists to allow for easy updates
DROP FUNCTION IF EXISTS public.get_next_sequence_no(text, text);

-- Create the function that will get the next sequence number
CREATE OR REPLACE FUNCTION public.get_next_sequence_no(
    p_sequence_prefix text,
    p_date_part text
)
    RETURNS BIGINT AS $$
DECLARE
    next_seq_no BIGINT;
BEGIN
    -- Lock the table to prevent race conditions where two transactions
    -- might try to get the same sequence number simultaneously.
    LOCK TABLE public.sequence IN EXCLUSIVE MODE;

    -- Find the maximum sequence number for the given prefix and date
    SELECT MAX(sequence_no) + 1
    INTO next_seq_no
    FROM public.sequence
    WHERE sequence_prefix = p_sequence_prefix AND date_part = p_date_part;

    -- If no record exists for this combination, start the sequence at 1
    RETURN COALESCE(next_seq_no, 1);
END;
$$ LANGUAGE plpgsql;

-- Grant execution rights to your database user
    ALTER FUNCTION public.get_next_sequence_no(text, text)
    OWNER TO postgres;

-- Optional: Add a comment to describe what the function does
COMMENT ON FUNCTION public.get_next_sequence_no(text, text)
    IS 'Calculates the next sequence number for a given prefix and date part, resetting to 1 for new combinations. Handles concurrency with a table lock.';


--------------END ------------


CREATE TABLE IF NOT EXISTS public.token
(
    token_id text COLLATE pg_catalog."default" NOT NULL,
    generated_at timestamp without time zone NOT NULL,
    appointment_at timestamp without time zone NULL,
    completed_at timestamp without time zone NULL,
    description text COLLATE pg_catalog."default",

    mobile_no character varying(30) COLLATE pg_catalog."default",
    email character varying(100) COLLATE pg_catalog."default",
    service_type BIGINT[] NOT NULL,
    passport_no text,
    nid_no text,
    CONSTRAINT token_pkey PRIMARY KEY (token_id)
)

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.token
    OWNER to postgres;


CREATE TABLE IF NOT EXISTS public.role_permission_map
(
    role_id integer NOT NULL,
    permission_id integer NOT NULL,
    CONSTRAINT role_permission_map_pkey PRIMARY KEY (role_id, permission_id),
    CONSTRAINT role_permission_map_permission_id_fkey FOREIGN KEY (permission_id)
    REFERENCES public.permission (permission_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION,
    CONSTRAINT role_permission_map_role_id_fkey FOREIGN KEY (role_id)
    REFERENCES public.role (role_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    )

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.role_permission_map
    OWNER to postgres;


CREATE TABLE IF NOT EXISTS public.role_user_map
(
    user_id integer NOT NULL,
    role_id integer NOT NULL,
    CONSTRAINT role_user_map_pkey PRIMARY KEY (user_id, role_id),
    CONSTRAINT role_user_map_role_id_fkey FOREIGN KEY (role_id)
    REFERENCES public.role (role_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION,
    CONSTRAINT role_user_map_user_id_fkey FOREIGN KEY (user_id)
    REFERENCES public."user" (user_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    )

    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.role_user_map
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS public.service_instance (
      service_instance_id SERIAL PRIMARY KEY,
      service_info_id INT NOT NULL,
      payment_made_at TIMESTAMP NOT NULL,
      delivery_date TIMESTAMP NOT NULL,
      delivered_at TIMESTAMP NOT NULL,
      note TEXT,
      attachments_received TEXT[],
      token_id TEXT NOT NULL
) TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.service_instance
    OWNER to postgres;

---------- SEED SCRIPT -------

INSERT INTO permission (permission_id, permission_name)
values 
(02, 'UPDATE_USER'),
(03, 'DELETE_USER'),
(04, 'DISABLE_USER'),
(05, 'CHANGE_PASSWORD'),

(12, 'UPDATE_ROLE'),
(23, 'MODIFY_ROLE'),
(14, 'DELETE_ROLE'),

(22, 'UPDATE_SERVICE_INFO'),

(32, 'CREATE_TOKEN'),
(33, 'UPDATE_TOKEN'),

(42, 'UPDATE_SERVICE_INSTANCE')











