CREATE TABLE IF NOT EXISTS users (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    username text NOT NULL UNIQUE,
    password_hash bytea NOT NULL,
    password_salt bytea NOT NULL,
    rating integer NOT NULL DEFAULT 400
);