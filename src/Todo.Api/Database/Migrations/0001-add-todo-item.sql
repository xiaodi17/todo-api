CREATE TABLE IF NOT EXISTS todo_items (
    todo_id UUID NOT NULL PRIMARY KEY,
    name TEXT NOT NULL,
    is_complete BOOLEAN NOT NULL
);