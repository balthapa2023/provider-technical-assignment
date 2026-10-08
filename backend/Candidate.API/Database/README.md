# SQLite database workspace

This directory is for SQLite-related deliverables for the take-home assignment.

## Layout

| Folder | What to put here |
| --- | --- |
| `Scripts/` | SQL scripts |
| `Data/` | The SQLite database file |

`.gitkeep` keeps each folder in source control while it is empty. You can remove a `.gitkeep` file after that folder contains your files.

## Your design

You are responsible for the database schema, relationships, soft-delete implementation, and database queries. Choose the SQLite database file name and the schema yourself.

SQL scripts and database artifacts should support the requirements in the official assignment: [Take-Home Technical Assignment](../../../assignment/Take-Home_Technical_Assignment_Formatted.pdf).

## Submission

The final submission must include the SQLite `.db` file and any applicable SQL scripts.

Commit the database file after database connections are closed, or commit a consistent backup or snapshot. Git ignores transient SQLite files (`*.db-wal`, `*.db-shm`, `*.db-journal`, and the matching `*.sqlite-*` files). Those files are not submission artifacts.

## Documentation

Document database setup and design decisions in the root `README.md`.
