import { sortOrderForLocationJsonType } from '@/types/common/jsonTypes';
import { actingRankJsontype, courtAdminRoleJsonType, userLocationJsonType } from '@/types/MyTeam/jsonTypes';

export interface courtAdminsAvailabilityJsonType {    
    start: string;
    end: string;
    conflicts: conflictJsonType[];
    courtAdmin: courtAdminsAvailabilityDataJsonType;
    courtAdminId: string;
    timezone: string;
}

export interface courtAdminsAvailabilityDataJsonType {
    gender: string;
    badgeNumber: string;
    rank: string;
    awayLocation: availabilityAwayLocationJsonType[];
    actingRank: actingRankJsontype[];
    leave: availabilityLeaveJsonType[];
    training: availabilityTrainingJsonType[];
    photoUrl: string;
    lastPhotoUpdate: string;
    id: string;
    isEnabled: true;
    firstName: string;
    lastName: string;
    email: string;
    homeLocationId: number;
    homeLocation: userLocationJsonType;
    activeRoles: courtAdminRoleJsonType[];
    roles: courtAdminRoleJsonType[];
    concurrencyToken: number;
} 

export interface conflictJsonType {
    courtAdminId: string;
    conflict: string;
    start: string;
    end: string;
    locationId: number;
}

export interface availabilityTrainingJsonType  {
    trainingType: availabilityTrainingDataJsonType;
    trainingTypeId: number;
    trainingCertificationExpiry: string;
    id: string;
    startDate:  string;
    endDate: string;
    expiryDate:  string;
    expiryReason: string;
    courtAdminId:  string;
    comment: string;
    timezone: string;
    concurrencyToken: number;
}

export interface availabilityTrainingDataJsonType  {
    id: string;
    type: string;
    code: string;
    subCode: string;
    description: string;
    effectiveDate: string;
    expiryDate: string;
    location: userLocationJsonType;
    locationId: number;
    concurrencyToken: number;
    sortOrderForLocation: sortOrderForLocationJsonType;
}

export interface availabilityAwayLocationJsonType {
    location: userLocationJsonType;
    locationId: number;
    id: number;
    startDate: string;
    endDate: string;
    expiryDate: string;
    expiryReason: string;
    courtAdminId: string;
    comment: string;
    timezone: string;
    concurrencyToken: number;
}

export interface availabilityLeaveJsonType {
    leaveType: availabilityLeaveDataJsonType;
    leaveTypeId: number;
    id: number;
    startDate: string;
    endDate: string;
    expiryDate: string;
    expiryReason: string;
    courtAdminId:   string;
    comment: string;
    timezone: string;
    concurrencyToken: number;
}

export interface availabilityLeaveDataJsonType {
    id: number;
    type:  string;
    code: string;
    subCode: string;
    description: string;
    effectiveDate: string;
    expiryDate: string;
    location: userLocationJsonType;
    locationId: number;
    concurrencyToken: number;
    sortOrderForLocation: sortOrderForLocationJsonType;
}
