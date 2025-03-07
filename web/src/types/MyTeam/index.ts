import {leaveInfoType, locationInfoType, trainingInfoType} from '../common';
import {actingRankJsontype, awayLocationsJsontype, leaveJsontype, trainingJsontype, userRoleJsonType} from './jsonTypes';

export interface teamMemberInfoType {

    id?: string;
    idirUserName?: string;
    rank: string;
    rankOrder: number;
    firstName?: string;
    lastName?: string;
    email?: string;
    badgeNumber: string;
    gender: string;
    fullName?: string;
    image?: string | null;
    userRoles?: userRoleJsonType[];
    isEnabled?: boolean;
    homeLocationId?: number | null;
    homeLocationNm?: string | null;
    homeLocation?: locationInfoType;
    awayLocation?: awayLocationsJsontype[];
    actingRank: actingRankJsontype[]; 
    training?: trainingJsontype[];
    leave?: leaveJsontype[];
    loanedOut?: awayLocationsJsontype[];
}

export interface userRoleInfoType{
    value: string;
    text: string;
    desc: string;    
    effectiveDate: string;
    expiryDate: string;
  }

export interface roleOptionInfoType{
    text: string;
    desc: string;
    value: string;
}

export interface permissionInfoType {
    id?: string;    
    name?: string;
    description?: string;
}

export interface roleInfoType {
    id?: string;    
    name?: string;
    description?: string;
    expiryDate?: string;
    permissions?: string[];
}

export interface permissionOptionInfoType{
    text: string;
    desc: string;
    value: string;
    selected: boolean;
}

export interface loanedLocationInfoType{
    id: number|null;    
    locationId: number|null;
    locationName: string;
    isFullDay: boolean;
    startDate: string;
    endDate: string;
    sheriffId: string;
    comment?: string;      
}

export interface awayLocationInfoType{
    id: number|null;    
    locationId: number|null;
    startDate: string;
    endDate: string;
    sheriffId: string;
    concurrencyToken?: number;
    comment?: string;      
}

export interface userTrainingInfoType{ 
    id: number;   
    trainingType: trainingInfoType;
    trainingTypeId?:number|null;
    trainingName?: string;
    sheriffId?: string;
    expiryDate: string;
    isFullDay?: boolean;
    startDate: string;
    endDate: string;
    comment?: string;
    note?: string;    
}

export interface userLeaveInfoType{
    id: number;    
    leaveTypeId: number|null;
    leaveName?: string;
    leaveType?: leaveInfoType;
    comment: string;
    isFullDay: boolean;
    startDate: string;
    endDate: string;    
}

export interface trainingReportInfoType {
    name: string;
    trainingType: string;
    start?: string;
    end: string;
    expiryDate: string;
    excluded?: boolean;
    sheriffId?: string;
    location?: string;
    region?: string;
    status?: string;
}

export interface trainingStatusCardInfoType { 
    danger: trainingReportInfoType[]; 
    alert: trainingReportInfoType[];
    warning: trainingReportInfoType[];
    notify: trainingReportInfoType[]; 
};

export interface trainingStatusInfoType { 
    danger: string; 
    alert: string;
    warning: string;
    notify: string; 
};

export interface sheriffOption {
    id: string;
    fullName: string;
}