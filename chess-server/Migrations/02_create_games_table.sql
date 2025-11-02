CREATE TABLE IF NOT EXISTS games (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    white_player_id uuid NOT NULL REFERENCES users(id),
    black_player_id uuid NOT NULL REFERENCES users(id),
    moves text[] NOT NULL,
    created_at timestamp NOT NULL DEFAULT NOW()
);