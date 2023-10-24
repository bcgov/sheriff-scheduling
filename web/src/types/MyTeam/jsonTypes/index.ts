
export interface teamMemberJsonType {
     loanedIn : awayLocationsJsontype[];
     loanedOut : awayLocationsJsontype[];
     gender : string;
     badgeNumber : string;
     rank : string;
     awayLocation : awayLocationsJsontype[];
     actingRank : actingRankJsontype[];
     leave : leaveJsontype[];
     training : trainingJsontype[];
     photoUrl : string;
     id : string;
     idirName : string;
     isEnabled : boolean;
     firstName : string;
     lastName : string;
     email : string;
     homeLocationId : number;
     homeLocation : userLocationJsonType;
     activeRoles : courtAdminRoleJsonType[];
     roles : courtAdminRoleJsonType[];
     concurrencyToken : number;      
}

export interface courtAdminRoleJsonType {
  role : roleJsonType;
  effectiveDate : string;
  expiryDate : string;
}

export interface permissionJsonType {
     id : string;
     name : string;
     description : string;
     concurrencyToken : number;
}

export interface roleJsonType {  
     id : string;
     name : string;
     description : string;
     rolePermissions : rolePermissionsJsonType[];      
     expiryDate? : string;
     concurrencyToken : number;  
}

export interface userRoleJsonType {  
    role: roleDataJsonType;
    effectiveDate: string;
    expiryDate: string;  
}

export interface roleDataJsonType { 
  id: number;
  name: string;
  description: string;
}

export interface userRoleHistoryJsonType {
  id: number;
  tableName: string;
  concurrencyToken: number;
  createdById: string;
  createdBy: teamMemberJsonType;
  createdOn: string;
  keyValuesJson: userRoleHistoryKeyValuesJsonType;
  newValuesJson: any;
  oldValuesJson: any;
}

export interface userRoleHistoryKeyValuesJsonType {
  Id: number;
  RoleId: number;
  UserId: string;
}

export interface rolePermissionsJsonType {
    id : string;
    roleId : string;
    permission : permissionJsonType;
    permissionId : string;
    concurrencyToken : number;
}

export interface awayLocationsJsontype {  
    id : number;
    location : userLocationJsonType;
    locationId : number;
    startDate : string;
    endDate : string;
    expiryDate : string;
    isFullDay : boolean;
    courtAdminId : string;
    concurrencyToken : number ;
    comment?: string; 
}

export interface actingRankJsontype {  
  id : number;
  rank : string;
  startDate : string;
  endDate : string;
  expiryDate? : string;
  isFullDay? : boolean;
  courtAdminId : string;
  timezone: string;
  concurrencyToken? : number ;
  comment?: string; 
}

export interface trainingJsontype {  
    id : number;
    trainingType : userTrainingJsontype;
    trainingTypeId : number;
    startDate : string;
    endDate : string;
    timezone: string;
    expiryDate : string;
    expiryReason: string;    
    trainingCertificationExpiry :string;
    courtAdminId : string;
    concurrencyToken : number;
    comment?: string;
    note?: string;
}

export interface userTrainingJsontype { 
  id : number;
  type : string;
  code : string;
  description : string;
  concurrencyToken : number;
}


export interface leaveJsontype {
  id : number;
  leaveType : userLeaveJsonType;
  leaveTypeId : number;
  startDate : string;
  endDate : string;
  expiryDate : string;
  comment : string;
  courtAdminId : string;
  concurrencyToken : number; 
}

export interface userLeaveJsonType {
  id : number;
  type : string;
  code : string;
  subCode : string;
  description : string;
  effectiveDate : string;
  expiryDate : string;
  sortOrder : number;
  location : userLocationJsonType;
  locationId : number;
  concurrencyToken : number;
}

export interface userLocationJsonType {
  id : number;
  agencyId : string;
  name : string;
  justinCode : string;
  parentLocationId : number;
  expiryDate : string;
  regionId : number;
  timezone : string;
  concurrencyToken : number;
}

export interface userEventsInfoType {
  name: string;
  type: string;
  start: string;
  end?: string;
  comment?: string;
}