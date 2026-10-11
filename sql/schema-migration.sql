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
-- new schema with the EXCEPTION of the following:
--
--  * API keys and existing API key request logs are not migrated due to a change of API key ownership.
--
-- Run this script if and ONLY if ALL of the following conditions are met!
--
--  1) All migrations up to turnierplan.NET 2026.3.0 have been applied. This can be verified using the following command:
--         select "MigrationId" from turnierplan."__EFMigrationHistory" order by "MigrationId" desc limit 1;
--     It should return '20260717182006_Add_ResourcePlanner'. If not, install and run turnierplan.NET 2026.3.0
--     once to apply the missing migrations.
--
--  2) The 'turnierplan_v2' schema does not exist yet. In other words, you must not run turnierplan.NET 2027.1 or newer
--     before executing this migration script. This script will drop the 'turnierplan_v2' if it already exists.
--
-- Note that this script does not delete or change any existing data in the v1 schema. Also, this script is designed
-- to run idempotently such that running the migration multiple times will always result in the exact same result.
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
[ ]  Folders
[ ]  GroupParticipants
[ ]  Groups
[ ]  IAM_ApiKey
[ ]  IAM_Folder
[ ]  IAM_Image
[ ]  IAM_Organization
[ ]  IAM_PlanningRealm
[ ]  IAM_ResourcePlanner
[ ]  IAM_Tournament
[ ]  IAM_Venue
[ ]  Images
[ ]  InvitationLinkEntries
[ ]  InvitationLinks
[ ]  Labels
[ ]  Matches
[X]  Organizations
[ ]  PlanningRealms
[ ]  RankingOverwrites
[ ]  ResourceAssignments
[ ]  ResourceGroupResourcePlannerView
[ ]  ResourceGroups
[ ]  ResourcePlanners
[ ]  ResourcePlannerViews
[ ]  Resources
[ ]  TeamLinks
[ ]  Teams
[ ]  TournamentClasses
[ ]  Tournaments
[X]  Users
[ ]  Venues

[ ] Add commands to delete temporary tables
[ ] Wrap in transaction
[ ] Remove delete commands at the top and insert DDL for the entire schema v2 (once finished)
*/
-- Delete all existing data from the v2 tables
delete
from turnierplan_v2."__TPVersionHistory";
delete
from turnierplan_v2.api_key;
delete
from turnierplan_v2.api_key_request;
delete
from turnierplan_v2.api_key_role_assignment;
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

create table turnierplan_temporary.user_mappings
(
    new_identifier       bigint primary key,
    old_identifier       uuid not null unique,
    normalized_user_name text not null unique
);

insert into turnierplan_temporary.user_mappings (new_identifier, old_identifier, normalized_user_name)
select turnierplan_v2.user.id, turnierplan."Users"."Id", turnierplan_v2.user.normalized_user_name
from turnierplan_v2.user
         inner join turnierplan."Users" on normalized_user_name = "NormalizedUserName";

-- Migrate organizations (incl. role assignments) and update the primary key sequence accordingly

insert into turnierplan_v2.organization (id, public_id, created_at, name)
select "Id", "PublicId", "CreatedAt", "Name"
from turnierplan."Organizations";

select setval(pg_get_serial_sequence('turnierplan_v2.organization', 'id'),
              coalesce((select max(id) from turnierplan_v2.organization), 1),
              exists(select 1 FROM turnierplan_v2.organization));

