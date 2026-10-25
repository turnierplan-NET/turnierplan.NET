select * from turnierplan_temporary.default_project;
select * from turnierplan_temporary.user_mapping;
select * from turnierplan_temporary.role_assignment;

select * from turnierplan_v2."__EFMigrationHistory";
select * from turnierplan_v2."__TPVersionHistory";
select * from turnierplan_v2.api_key;
select * from turnierplan_v2.api_key_request;
select * from turnierplan_v2.api_key_role_assignment;
select * from turnierplan_v2.organization;
select * from turnierplan_v2.organization_role_assignment;
select id, name, organization_id from turnierplan_v2.project where name like 'Default project (%';
select * from turnierplan_v2.project where name not like 'Default project (%';
select * from turnierplan_v2.project_role_assignment;
select * from turnierplan_v2.tournament;
select * from turnierplan_v2.tournament_role_assignment;
select * from turnierplan_v2.user;
