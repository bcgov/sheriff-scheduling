export interface locationInfoType {
    name: string;
    id: number;
    regionId: number| string| null;
    agencyId?: string;
    concurrencyToken?: number;
    justinCode?: string;
    timezone: string;
}

export interface regionInfoType {
    name: string;
    id: string;
    concurrencyToken?: number;
    justinId?: number;
}

export interface leaveInfoType {
    code: string;
    id: number;
    description?: string;
}

export interface trainingInfoType {
    code: string;
    id: number;
    validityPeriod?: number;
    description?: string;
}

export interface userInfoType {
    firstName: string;
    lastName: string;
    roles: string[];
    homeLocationId: number;
    permissions: string[];
    userId: string;
}

export interface commonInfoType {
    courtAdminRankList: courtAdminRankInfoType[]    
}

export interface courtAdminRankInfoType {
    id: number;
    name: string;
}

export interface localTimeInfoType {
    timeString: string;
    timeSlot: number;
    dayOfWeek: number;
    isTodayInView: boolean;
}

export interface reportInfoType {
    startDate: string;
    endDate: string;
    reportType: string;
    reportSubtype: string;
    region: string;
    location: string;
    courtAdminName: string;
}

export interface dateRangeInfoType {
    startDate: string;
    endDate: string;
    valid: boolean;
}