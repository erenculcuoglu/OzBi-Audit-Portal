# Workspace Rules (OzBI Portal CRM)

## MariaDB Read-Only Constraint
- **MariaDB (`ozbiappc_app` / `elegance.odeaweb.com`) is strictly READ-ONLY.**
- We do **NOT** have write access, migration permissions, or DDL/DML permissions on the MariaDB database.
- Never propose or attempt `ALTER TABLE`, `UPDATE`, `INSERT`, `DELETE`, or schema modifications on MariaDB.
- All writable portal CRM features (favorites, audit logs, local configurations, snapshots) must use the local **SQLite database (`AppDbContext` / `ozbi_audit.db`)**.
