import {} from '../common';
import {} from '../DutyRoster/jsonTypes';

export interface assignmentTypeInfoType {
    code: string;
    concurrencyToken?: number;
    id: number;
    locationId: number;
    type: string;
    sortOrder: number;
}

export interface leaveTrainingTypeInfoType {
    code: string;
    concurrencyToken?: number;
    id: number;
    // locationId: number;
    type: string;
    sortOrder: number;
}

