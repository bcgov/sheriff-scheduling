export interface courtAdminRankJsonType {
    id: string;
    type: string;
    description: string; 
}

export interface locationJsonType {
    id: number;
    agencyId: string;
    name: string;
    regionId: number|null;
    concurrencyToken: number;
    timezone: string;
    justinCode: string;
}

export interface leaveTypeJson {
    id: number;
    type: string;
    code: string;
    description: string;
    concurrencyToken: number;
}

export interface trainingTypeJson {
    id: number;
    type: string;
    code: string;
    validityPeriod?: number;
    description: string;
    concurrencyToken: number;
}

export interface sortOrderForLocationJsonType {
    id : number;
    lookupCodeId : number;
    locationId : number;
    sortOrder : number;
    concurrencyToken : number;
}