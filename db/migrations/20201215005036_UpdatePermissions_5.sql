do $$
declare
	SystemUserId uuid; 
	AdministratorId integer;
	ManagerId integer;
	CourtAdminId integer;
BEGIN
	SystemUserId := '00000000-0000-0000-0000-000000000001';

	SELECT "Id" INTO AdministratorId FROM "Role" WHERE "Name" = 'Administrator';
	SELECT "Id" INTO ManagerId FROM "Role" WHERE "Name" = 'Manager';
	SELECT "Id" INTO CourtAdminId FROM "Role" WHERE "Name" = 'CourtAdmin';

	DELETE FROM "RolePermission" WHERE "RoleId" = AdministratorId;
	DELETE FROM "RolePermission" WHERE "RoleId" = ManagerId;
	DELETE FROM "RolePermission" WHERE "RoleId" = CourtAdminId;

	-- CourtAdmin
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES (SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewShifts')),
	(SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAssignedLocation')),
	(SystemUserId,now(),NULL,NULL, CourtAdminId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocation'));

	-- Manager
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES (SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInOwnLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditUsers')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAssignedLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ImportShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion'));

	-- Administrator
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES (SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInOwnLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInAllLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAssignedLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ImportShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProvince')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAssignments')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAssignments')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditAssignments')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireAssignments')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDuties')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignDuties')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditDuties')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireDuties')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditIdir')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditPastTraining')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'RemovePastTraining'));
end;
$$;