-- noinspection SqlWithoutWhereForFile
-- 
--  __                                                     ___                                        __
-- /\ \__                        __                       /\_ \                                      /\ \__
-- \ \ ,_\  __  __  _ __    ___ /\_\     __   _ __   _____\//\ \      __      ___         ___      __\ \ ,_\
--  \ \ \/ /\ \/\ \/\`'__\/' _ `\/\ \  /'__`\/\`'__\/\ '__`\\ \ \   /'__`\  /' _ `\     /' _ `\  /'__`\ \ \/
--   \ \ \_\ \ \_\ \ \ \/ /\ \/\ \ \ \/\  __/\ \ \/ \ \ \L\ \\_\ \_/\ \L\.\_/\ \/\ \  __/\ \/\ \/\  __/\ \ \_
--    \ \__\\ \____/\ \_\ \ \_\ \_\ \_\ \____\\ \_\  \ \ ,__//\____\ \__/.\_\ \_\ \_\/\_\ \_\ \_\ \____\\ \__\
--     \/__/ \/___/  \/_/  \/_/\/_/\/_/\/____/ \/_/   \ \ \/ \/____/\/__/\/_/\/_/\/_/\/_/\/_/\/_/\/____/ \/__/
--                                                     \ \_\
--                                                      \/_/   database migration script V1 -> V2
--
-- This SQL script will migrate existing data from a turnierplan.NET <= 2026.3 database to the new database
-- schema required by versions 2027.1 and later. This script will migrate ALL data from the old schema to the
-- new schema except for the following:
--
--  * API keys and existing API key request logs are not migrated due to a change of API key ownership.
--
-- Run this script if and only if all the following conditions are met!
--
--  1) You are running PostgreSQL 17 or later. This is required for the random() function used to generate pseudo-random
--     identifiers based on 8-byte integers (bigint).
--
--  2) All migrations up to turnierplan.NET 2026.3.0 have been applied. This can be verified using the following command:
--         select "MigrationId" from turnierplan."__EFMigrationHistory" order by "MigrationId" desc limit 1;
--     It should return '20260307185525_Add_UserAllowCreateOrganization'. If not, install and run turnierplan.NET 2026.3.0
--     once to apply the missing migrations.
--
--  3) The 'turnierplan_v2' schema does not exist yet. In other words, you must not run turnierplan.NET 2027.1 or newer
--     before executing this migration script. This script will drop the 'turnierplan_v2' if it already exists.
--
-- Note that this script does not delete or change any existing data in the v1 schema. Also, this script is designed
-- to run idempotently such that running the migration multiple times will always result in the exact same result.
--
--
--                  **** ALWAYS CREATE A BACKUP OF THE DATABASE BEFORE RUNNING THIS SCRIPT! ****
--
--
-- This script is licensed under AGPL-3.0
-- (C) 2026 Elias Hörner
--

/* 
[-]  __EFMigrationHistory
[X]  __TPVersionHistory
[-]  ApiKeyRequests
[-]  ApiKeys
[ ]  ApplicationChangeLogs
[ ]  Applications
[ ]  ApplicationTeamLabel
[ ]  ApplicationTeams
[ ]  Documents
[X]  Folders
[ ]  GroupParticipants
[ ]  Groups
[X]  IAM_ApiKey
[X]  IAM_Folder
[X]  IAM_Image
[X]  IAM_Organization
[X]  IAM_PlanningRealm
[X]  IAM_Tournament
[X]  IAM_Venue
[ ]  Images
[ ]  InvitationLinkEntries
[ ]  InvitationLinks
[ ]  Labels
[ ]  Matches
[X]  Organizations
[ ]  PlanningRealms
[ ]  RankingOverwrites
[ ]  TeamLinks
[ ]  Teams
[ ]  TournamentClasses
[ ]  Tournaments
[X]  Users
[ ]  Venues

[ ] Add commands to delete temporary tables
[ ] Wrap in transaction
[ ] Ensure the numeric role ids are consistent
[ ] Remove delete commands at the top and insert DDL for the entire schema v2 (once finished)
[ ] Check if the 'turnierplan_temporary.user_mapping' is necessary
[ ] Integrate DDL for schema and update test bash script accordingly
[ ] Add all tables to assertions.sql in tests
*/
-- Delete all existing data from the v2 tables | THIS PART WILL BE REPLACED WITH EF CORE DDL!!!
-- TODO: REmove the "if not exists part"
create table if not exists turnierplan_v2."__TPVersionHistory"
(
    version   text                     not null
        unique,
    major     integer                  not null,
    minor     integer                  not null,
    patch     integer                  not null,
    timestamp timestamp with time zone not null
);

delete
from turnierplan_v2."__TPVersionHistory";
delete
from turnierplan_v2.api_key;
delete
from turnierplan_v2.api_key_request;
delete
from turnierplan_v2.api_key_role_assignment;

delete
from turnierplan_v2.project;
delete
from turnierplan_v2.project_role_assignment;

delete
from turnierplan_v2.organization;
delete
from turnierplan_v2.organization_role_assignment;
delete
from turnierplan_v2.user;
-- END OF PART THAT WILL BE REMOVED


-- Create a temporary schema which contains some temporary tables required during the migration

drop schema if exists turnierplan_temporary cascade;
create schema turnierplan_temporary;

-- Define a function to create a pseudo-random public ID

create or replace function turnierplan_temporary.random_public_id() returns bigint
    language plpgsql as
$$
declare
    result bigint;
begin
    select * into result from random(-9223372036854775808, 9223372036854775807);
    return result;
end;
$$;

-- Copy entries from the version history table

insert into turnierplan_v2."__TPVersionHistory" (version, major, minor, patch, timestamp)
select "Version", "Major", "Minor", "Patch", "Timestamp"
from turnierplan."__TPVersionHistory";

-- Schema v1 uses a guid as the primary key while schema v2 uses a numeric id. Because of this, a temporary
-- table is created which holds both the old guid and the new numeric id. This relation is required for the
-- migration of role assignments further below.

insert into turnierplan_v2.user (principal_id, created_at, user_name, normalized_user_name, full_name, e_mail,
                                 normalized_e_mail, password_hash, is_administrator, allow_create_organization,
                                 last_password_change, last_login, security_stamp)
select "PrincipalId",
       "CreatedAt",
       "UserName",
       "NormalizedUserName",
       "FullName",
       "EMail",
       "NormalizedEMail",
       "PasswordHash",
       "IsAdministrator",
       "AllowCreateOrganization",
       "LastPasswordChange",
       "LastLogin",
       "SecurityStamp"
from turnierplan."Users";

create table turnierplan_temporary.user_mapping
(
    new_identifier       bigint primary key,
    old_identifier       uuid not null unique,
    normalized_user_name text not null unique
);

insert into turnierplan_temporary.user_mapping (new_identifier, old_identifier, normalized_user_name)
select turnierplan_v2.user.id, turnierplan."Users"."Id", turnierplan_v2.user.normalized_user_name
from turnierplan_v2.user
         inner join turnierplan."Users" on normalized_user_name = "NormalizedUserName";

-- Dump all role assignments from the schema v1 into a single temporary table. This reduces the number of duplicated
-- commands required for the mapping of the API key and user principal IDs. The old schema stores the principal as a
-- formatted string whereas the new schema uses separate columns for the numeric principal kind and the principal uuid.

create table turnierplan_temporary.role_assignment
(
    id               serial primary key,
    source_table     text        not null,
    source_principal text        not null,
    entity_id        bigint      not null,
    created_at       timestamptz not null,
    role             integer     not null,
    principal_kind   text        not null,
    principal_kind_n integer     not null,
    principal_id     uuid        not null
);

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_ApiKey',
       "Principal",
       "ApiKeyId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_ApiKey";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_Folder',
       "Principal",
       "FolderId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_Folder";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_Image',
       "Principal",
       "ImageId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_Image";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_Organization',
       "Principal",
       "OrganizationId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_Organization";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_PlanningRealm',
       "Principal",
       "PlanningRealmId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_PlanningRealm";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_Tournament',
       "Principal",
       "TournamentId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_Tournament";

insert into turnierplan_temporary.role_assignment (source_table, source_principal, entity_id, created_at, role,
                                                    principal_kind, principal_kind_n, principal_id)
select 'IAM_Venue',
       "Principal",
       "VenueId",
       "CreatedAt",
       "Role",
       split_part("Principal", ':', 1),
       0,
       split_part("Principal", ':', 2)::uuid
from turnierplan."IAM_Venue";

update turnierplan_temporary.role_assignment
set principal_kind_n = 1
where role_assignment.principal_kind = 'ApiKey';
update turnierplan_temporary.role_assignment
set principal_kind_n = 2
where role_assignment.principal_kind = 'User';

-- Migrate organizations (incl. role assignments) and update the primary key sequence accordingly

insert into turnierplan_v2.organization (id, public_id, created_at, name)
select "Id", "PublicId", "CreatedAt", "Name"
from turnierplan."Organizations";

insert into turnierplan_v2.organization_role_assignment (created_at, organization_id, role, principal_kind, principal_id)
select created_at, entity_id, role, principal_kind_n, principal_id
from turnierplan_temporary.role_assignment
where source_table = 'IAM_Organization';

select setval(pg_get_serial_sequence('turnierplan_v2.organization', 'id'),
              coalesce((select max(id) from turnierplan_v2.organization), 1),
              exists(select 1 FROM turnierplan_v2.organization));

-- Create a "default" project inside each organization

insert into turnierplan_v2.project (public_id, created_at, name, organization_id)
select turnierplan_temporary.random_public_id(),
       NOW(),
       ('Default project (' || organization.name || ')'),
       organization.id
from turnierplan_v2.organization;

create table turnierplan_temporary.default_project
(
    id                 serial primary key,
    organization_id    bigint not null,
    default_project_id bigint not null
);

insert into turnierplan_temporary.default_project (organization_id, default_project_id)
select project.organization_id, project.id
from turnierplan_v2.project;

-- Migrate existing folders (incl. role assignments) as projects

insert into turnierplan_v2.project (public_id, created_at, name, organization_id)
select "PublicId", "CreatedAt", "Name", "OrganizationId"
from turnierplan."Folders";

insert into turnierplan_v2.project_role_assignment (created_at, project_id, role, principal_kind, principal_id)
select created_at, entity_id, role, principal_kind_n, principal_id
from turnierplan_temporary.role_assignment
where source_table = 'IAM_Folder';
