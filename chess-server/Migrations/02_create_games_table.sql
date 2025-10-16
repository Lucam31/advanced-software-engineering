CREATE TABLE IF NOT EXISTS games (
   id uuid PRIMARY KEY,
   white_player_id uuid NOT NULL REFERENCES users(id),
   black_player_id uuid NOT NULL REFERENCES users(id),
   moves text[] NOT NULL
);