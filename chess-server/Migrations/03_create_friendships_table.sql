CREATE TABLE IF NOT EXISTS friendships (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id_1 uuid NOT NULL REFERENCES users(id),
    user_id_2 uuid NOT NULL REFERENCES users(id),
    status varchar(20) NOT NULL DEFAULT 'pending',
    initiated_by uuid NOT NULL REFERENCES users(id),
    created_at timestamp NOT NULL DEFAULT NOW(),
    CONSTRAINT friendship_unique UNIQUE(user_id_1, user_id_2),
    CONSTRAINT different_users CHECK (user_id_1 != user_id_2),
    CONSTRAINT ordered_ids CHECK (user_id_1 < user_id_2)
);

