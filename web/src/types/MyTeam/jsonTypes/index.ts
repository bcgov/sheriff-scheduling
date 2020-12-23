import { any } from 'underscore';
import {} from '../../common';

export interface teamMemberJsonType {
     loanedIn : awayLocationsJsontype[],
     loanedOut : awayLocationsJsontype[],
     gender : string,
     badgeNumber : string,
     rank : string,
     awayLocation : awayLocationsJsontype[],
     leave : leaveJsontype[],
     training : trainingJsontype[],
     photoUrl : string,
     id : string,
     idirName : string,
     isEnabled : boolean,
     firstName : string,
     lastName : string,
     email : string,
     homeLocationId : number,
     homeLocation : userLocationJsonType,
     activeRoles : sheriffRoleJsonType[],
     roles : sheriffRoleJsonType[],
     concurrencyToken : number      
}

export interface sheriffRoleJsonType {
  role : roleJsonType,
  effectiveDate : string,
  expiryDate : string
}

export interface permissionJsonType {
     id : string,
     name : string,
     description : string,
     concurrencyToken : number
}

export interface roleJsonType {  
     id : string,
     name : string,
     description : string,
     rolePermissions : rolePermissionsJsonType[],      
     expiryDate? : string,
     concurrencyToken : number  
}

export interface userRoleJsonType {  
    role: {
        id: number;
        name: string;
        description: string;
    };
    effectiveDate: string;
    expiryDate: string;  
}

export interface userRoleHistoryJsonType {
  id: number,
  tableName: string,
  concurrencyToken: number,
  createdById: string,
  createdBy: teamMemberJsonType,
  createdOn: string,
  keyValuesJson: {
    Id: number
    RoleId: number
    UserId: string
  }
  newValuesJson: any,
  oldValuesJson: any

}

export interface rolePermissionsJsonType {
    id : string,
    roleId : string,
    permission : permissionJsonType,
    permissionId : string,
    concurrencyToken : number
}

export interface awayLocationsJsontype {  
    id : number,
    location : userLocationJsonType,
    locationId : number,
    startDate : string,
    endDate : string,
    expiryDate : string,
    isFullDay : boolean,
    sheriffId : string,
    concurrencyToken : number ,
    comment?: string 
}

export interface trainingJsontype {  
    id : number,
    trainingType : {
      id : number,
      type : string,
      code : string,
      description : string,
      concurrencyToken : number
    },
    trainingTypeId : number,
    startDate : string,
    endDate : string,
    timezone: string,
    expiryDate : string,
    expiryReason: string,    
    trainingCertificationExpiry :string,
    sheriffId : string,
    concurrencyToken : number,
    comment?: string,
    note?: string,
}


export interface leaveJsontype {
  id : number,
  leaveType : {
    id : number,
    type : string,
    code : string,
    subCode : string,
    description : string,
    effectiveDate : string,
    expiryDate : string,
    sortOrder : number,
    location : userLocationJsonType,
    locationId : number,
    concurrencyToken : number
  },
  leaveTypeId : number,
  startDate : string,
  endDate : string,
  expiryDate : string,
  comment : string,
  sheriffId : string,
  concurrencyToken : number 
}

export interface userLocationJsonType {
  id : number,
  agencyId : string,
  name : string,
  justinCode : string,
  parentLocationId : number,
  expiryDate : string,
  regionId : number,
  timezone : string,
  concurrencyToken : number
}