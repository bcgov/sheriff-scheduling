<template>
    <b-card bg-variant="white" class="home" no-body>        
        <manage-assignments-header v-on:change="loadScheduleInformation" />         
        <b-overlay opacity="0.6" :show="!isManageScheduleDataMounted">
            <template #overlay>
                <loading-spinner :inline="true"/>
            </template>    
            <b-table
                :key="updateTable"
                :items="courtAdminSchedules" 
                :fields="fields"
                small
                head-row-variant="primary"   
                bordered
                fixed>
                    <template v-slot:table-colgroup>
                        <col style="width:8.5rem;">                            
                    </template>
                    <template v-slot:head() = "data" >
                        <span>{{data.column}}</span> <span> {{data.label}}</span>
                    </template>
                    <template v-slot:head(myteam) = "data" >  
                        <span>{{data.label}}</span>
                    </template>
                    <template v-slot:cell()="data" > 
                        <assignment-card 
                            :courtAdminId="data.item.myteam.courtAdminId" 
                            :courtAdminName="data.item.myteam.name" 
                            :scheduleInfo="data.value"
                            :showAllDuties="showAllAssignments"
                            :cardDate="data.field.label"
                            v-on:change="loadScheduleInformation()"/>
                    </template>
                    <template v-slot:cell(myteam) = "data" > 
                        <team-member-card v-on:change="loadScheduleInformation()" :courtAdminInfo="data.item.myteam" />
                    </template>
            </b-table>
           
        </b-overlay>
        <b-card><br></b-card>

        <b-modal v-model="openErrorModal" header-class="bg-warning text-light">
            <b-card class="h4 mx-2 py-2">
				<span class="p-0">{{errorText}}</span>
            </b-card>                        
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="openErrorModal=false">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="openErrorModal=false"
                >&times;</b-button>
            </template>
        </b-modal>

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from "vuex-class";
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    import AssignmentCard from './components/AssignmentCard.vue';
    import TeamMemberCard from './components/TeamMemberCard.vue';
    import ManageAssignmentsHeader from './components/ManageAssignmentsHeader.vue';

	import "@store/modules/AssignmentScheduleInformation";
	const assignmentState = namespace("AssignmentScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");


    import { locationInfoType, commonInfoType } from '@/types/common';
    import { shiftRangeInfoType, selectShiftInfoType } from '@/types/ShiftSchedule/index';
    import { courtAdminsAvailabilityJsonType, conflictJsonType } from '@/types/ShiftSchedule/jsonTypes';
    import { assignmentCardWeekInfoType, attachedDutyInfoType, manageAssignmentDutyInfoType, manageScheduleInfoType, manageAssignmentsInfoType, manageAssignmentsScheduleInfoType, conflictsJsonAwayLocationInfoType, dutyRangeInfoType } from '@/types/DutyRoster';
    
    @Component({
        components: {
            AssignmentCard,
            TeamMemberCard,
            ManageAssignmentsHeader
        }
    })
    export default class ManageAssignments extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;
        
        @commonState.State
        public location!: locationInfoType;

        @commonState.State
		public locationList!: locationInfoType[];

        @assignmentState.State
		public assignmentRangeInfo!: shiftRangeInfoType;

        @assignmentState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: selectShiftInfoType[]) => void

        @assignmentState.State
        public dutyShiftAssignmentsWeek!: assignmentCardWeekInfoType[];

        @assignmentState.Action
        public UpdateDutyShiftAssignmentsWeek!: (newDutyRosterAssignmentsWeek: assignmentCardWeekInfoType[]) => void

        showAllAssignments = true;
        isManageScheduleDataMounted = false;
        headerDates: string[] = [];
        numberOfheaderDates = 7;
        updateTable = 0;

        errorText ='';
        openErrorModal=false;
        maxRank = 1000;

        dutyRostersJson: attachedDutyInfoType[] = [];

        fields: any[] = []
        originalFields = [
            {key:'myteam', label:'My Team', tdClass:'px-0 mx-0', thClass:'text-center'},
            {key:'Sun', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Mon', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Tue', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Wed', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Thu', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Fri', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Sat', label:'', tdClass:'align-middle px-0 mx-0', thStyle:'text-align: center;'}
        ];

        courtAdminSchedules: manageAssignmentsInfoType[] = [];

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isManageScheduleDataMounted) {
                this.loadScheduleInformation()               
            }            
        } 

        // mounted()
        // {                       
        //     //this.loadScheduleInformation();
        //     console.log('mount manage')
        // }

        public getDutyRosters(startDate, endDate){            
            const url = 'api/dutyroster?locationId='+this.location.id+'&start='+startDate+'&end='+endDate;
            return this.$http.get(url)
        }

        public getAssignments(startDate, endDate){
            const url = 'api/assignment?locationId='+this.location.id+'&start='+startDate+'&end='+endDate;
            return this.$http.get(url)
        }

        public getDistributeschedule(startDate, endDate){
            const url = 'api/distributeschedule/location?locationId='
                        +this.location.id+'&start='+startDate+'&end='+endDate + '&includeWorkSection=true';
            return this.$http.get(url)
        }        

        async loadScheduleInformation(allAssignments?) {
            
            this.extractTableFields(allAssignments);

            this.UpdateSelectedShifts([]);
            this.isManageScheduleDataMounted=false;           

            this.headerDate();

            const endDate = moment.tz(this.assignmentRangeInfo.endDate, this.location.timezone).endOf('day').utc().format();
            const startDate = moment.tz(this.assignmentRangeInfo.startDate, this.location.timezone).startOf('day').utc().format();
                        
            const response = await Promise.all([
                this.getDutyRosters(startDate, endDate),
                this.getAssignments(startDate, endDate),
                this.getDistributeschedule(startDate, endDate)
            ]).catch(err=>{
                this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                if (err.response.status != '401') {
                    this.openErrorModal=true;
                }   
                this.courtAdminSchedules = [];
                this.isManageScheduleDataMounted=true;
            });
            
            this.dutyRostersJson = response[0].data;
            this.extractAssignmentsInfo(response[1].data);
            this.extractTeamScheduleInfo(response[2].data);            
        }

        public extractTeamScheduleInfo(courtAdminsScheduleJson: courtAdminsAvailabilityJsonType[]) {
            
            this.courtAdminSchedules = [];
            
            for(const courtAdminScheduleJson of courtAdminsScheduleJson) {
                //console.log(courtAdminScheduleJson)
                const courtAdminSchedule = {} as manageScheduleInfoType;
                courtAdminSchedule.courtAdminId = courtAdminScheduleJson.courtAdminId;                
                courtAdminSchedule.name = Vue.filter('capitalizefirst')(courtAdminScheduleJson.courtAdmin.lastName) 
                                        + ', ' + Vue.filter('capitalizefirst')(courtAdminScheduleJson.courtAdmin.firstName);
                courtAdminSchedule.rank = courtAdminScheduleJson.courtAdmin.rank;
                courtAdminSchedule.actingRank = courtAdminScheduleJson.courtAdmin.actingRank;
                courtAdminSchedule.badgeNumber = courtAdminScheduleJson.courtAdmin.badgeNumber; 
                courtAdminSchedule.homeLocation = courtAdminScheduleJson.courtAdmin.homeLocation.name;                                        
                const isInLoanLocation = (courtAdminScheduleJson.courtAdmin.homeLocation.id !=this.location.id)
                courtAdminSchedule.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(courtAdminScheduleJson.conflicts) :this.extractSchedules(courtAdminScheduleJson.conflicts, false);    
                
                this.courtAdminSchedules.push({
                    myteam: courtAdminSchedule,
                    Sun: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==0) return true}),
                    Mon: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==1) return true}),
                    Tue: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==2) return true}),
                    Wed: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==3) return true}),
                    Thu: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==4) return true}),
                    Fri: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==5) return true}),
                    Sat: courtAdminSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==6) return true})
                })
                //break //TODO remove break
            } 
            //console.log(this.courtAdminSchedules)         
            this.isManageScheduleDataMounted = true;            
            this.updateTable++;
        }

        public headerDate() {
            this.headerDates = [];
            for(let dayOffset=0; dayOffset<this.numberOfheaderDates; dayOffset++)
            {
                const date= moment(this.assignmentRangeInfo.startDate).add(dayOffset,'days').format();
                this.headerDates.push(date);
                this.fields[dayOffset+1].label = ' ' + Vue.filter('beautify-date')(date);
            }
        }

        public extractSchedules(conflictsJson, onlyShedules){

            const schedules: manageAssignmentsScheduleInfoType[] = []

            for(const conflict of conflictsJson){                
                if (conflict.conflict=="Scheduled" && conflict.locationId != this.location.id) continue;
                if (conflict.conflict!="Scheduled" && onlyShedules) continue;
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();    
                         
                if(Vue.filter('isDateFullday')(conflict.start,conflict.end)) {                                  
                    for(const dateIndex in this.headerDates) {
                        const date = this.headerDates[dateIndex]
                        if(date>=conflict.start && date<=conflict.end) {
                            if (conflict.conflict =='Scheduled'){

                                if (conflict.dutySlots?.length > 0){

                                    const duties = this.extractDutyInfo(conflict.dutySlots);
                                    
                                    schedules.push({
                                            id:conflict.shiftId? conflict.shiftId:0,
                                            location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                            dayOffset: Number(dateIndex), 
                                            date:date, 
                                            startTime:'', 
                                            endTime:'',
                                            type:this.getConflictsType(conflict),
                                            subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'',
                                            duties: duties,
                                            workSection: '',
                                            workSectionColor: '',
                                            fullday: true,
                                            overtime: conflict.overtimeHours
                                        })

                                } else {

                                    schedules.push({
                                        id:conflict.shiftId? conflict.shiftId:0,
                                        location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                        dayOffset: Number(dateIndex), 
                                        date:date, 
                                        startTime:'', 
                                        endTime:'',
                                        duties: [],
                                        type:this.getConflictsType(conflict),
                                        subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'',
                                        workSection: '',
                                        workSectionColor: '',
                                        fullday: true,
                                        overtime: conflict.overtimeHours
                                    }) 

                                }

                                
                            } else {
                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:date, 
                                    startTime:'', 
                                    endTime:'',
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'',
                                    duties: [],
                                    workSection: '',
                                    workSectionColor: '',
                                    fullday: true,
                                    overtime: conflict.overtimeHours
                                })
                            }   
                        }                       
                    }
                } else {
                    
                    for(const dateIndex in this.headerDates) {
                        const date = this.headerDates[dateIndex].substring(0,10);
                        const nextDate = moment(this.headerDates[dateIndex]).add(1,'days').format().substring(0,10);
                        if(date == conflict.start.substring(0,10) && date == conflict.end.substring(0,10)) {  

                            if (conflict.dutySlots?.length > 0){

                                const duties = this.extractDutyInfo(conflict.dutySlots);
                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(conflict.start), 
                                    endTime:Vue.filter('beautify-time')(conflict.end), 
                                    type:this.getConflictsType(conflict),                                
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: duties,
                                    workSection:'',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                })  

                            } else {

                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(conflict.start), 
                                    endTime:Vue.filter('beautify-time')(conflict.end), 
                                    type:this.getConflictsType(conflict),                                
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: [],
                                    workSection: '',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                }) 
                            }

                        } else if(date == conflict.start.substring(0,10) && nextDate == conflict.end.substring(0,10)) {  
                            const midnight = moment(conflict.start).endOf('day');  
                            if (conflict.dutySlots?.length > 0){

                                const duties = this.extractDutyInfo(conflict.dutySlots);

                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(conflict.start), 
                                    endTime:Vue.filter('beautify-time')(midnight.format()),
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: duties,
                                    workSection: '',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                })

                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex)+1, 
                                    date:moment(this.headerDates[dateIndex]).add(1,'day').format(), 
                                    startTime:'00:00', 
                                    endTime:Vue.filter('beautify-time')(conflict.end),
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: duties,
                                    workSection: '',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                }) 

                            } else {
                                                                                
                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(conflict.start), 
                                    endTime:Vue.filter('beautify-time')(midnight.format()),
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: [],
                                    workSection:'',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                })
                                schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex)+1, 
                                    date:moment(this.headerDates[dateIndex]).add(1,'day').format(), 
                                    startTime:'00:00', 
                                    endTime:Vue.filter('beautify-time')(conflict.end),
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.courtAdminEventType)?conflict.courtAdminEventType:'', 
                                    duties: [],
                                    workSection:'',
                                    workSectionColor: '',
                                    fullday: false,
                                    overtime: conflict.overtimeHours
                                })   
                            }     
                        }                       
                    }
                } 
            }

            return schedules
        } 

        public extractDutyInfo(dutySlots){
            const WSColors = Vue.filter('WSColors')()
            const duties: manageAssignmentDutyInfoType[] = [];
            for (const duty of dutySlots){                
                const dutyData = {} as manageAssignmentDutyInfoType;
                dutyData.dutyId = duty.dutyId?duty.dutyId:'';
                dutyData.id = duty.id?duty.id:'';
                dutyData.startTime = Vue.filter('beautify-time')(moment(duty.startDate).tz(duty.timezone).format());
                dutyData.endTime = Vue.filter('beautify-time')(moment(duty.endDate).tz(duty.timezone).format());
                dutyData.dutyType = (duty.assignmentLookupCode?.type)?duty.assignmentLookupCode.type:'';
                dutyData.dutySubType = (duty.assignmentLookupCode?.code)?duty.assignmentLookupCode.code:'';
                dutyData.dutyNotes = (duty.dutyComment)?(duty.dutyComment):''// + ' (' +dutyData.startTime + '-' + dutyData.endTime + ')'):'';
                dutyData.assignmentNotes = (duty.assignmentComment)?(duty.assignmentComment + ' (' +dutyData.dutySubType+ ')'):'';
                dutyData.color = WSColors[duty.assignmentLookupCode.type]?WSColors[duty.assignmentLookupCode.type]:'';
                duties.push(dutyData);                                            
            }
            return duties;
        }

        public extractInLoanLocationConflicts(conflictsJson: conflictJsonType[]){
          
            let conflictsJsonAwayLocations: conflictsJsonAwayLocationInfoType[] = [];
            const conflicts: manageAssignmentsScheduleInfoType[] = [];
            for(const conflict of conflictsJson){
                const conflictsJsonAwayLocation = {} as conflictsJsonAwayLocationInfoType; 
                conflictsJsonAwayLocation.start = moment(conflict.start).tz(this.location.timezone).format();
                conflictsJsonAwayLocation.end = moment(conflict.end).tz(this.location.timezone).format();              
                if(conflict.conflict !='AwayLocation' || conflict.locationId != this.location.id) continue;
                conflictsJsonAwayLocation.courtAdminId = conflict.courtAdminId;
                conflictsJsonAwayLocation.conflict = conflict.conflict;
               
                conflictsJsonAwayLocation.locationId = conflict.locationId;
                conflictsJsonAwayLocation.startDay = conflict.start.substring(0,10);
                conflictsJsonAwayLocation.endDay = conflict.end.substring(0,10);
                conflictsJsonAwayLocations.push(conflictsJsonAwayLocation);
            }
            conflictsJsonAwayLocations = _.sortBy(conflictsJsonAwayLocations,'start')

            for(const dateIndex in this.headerDates) {
                const date = this.headerDates[dateIndex];
                const day = date.substring(0,10);
                let numberOfConflictsPerDay = 0;
                let previousConflictEndDate = moment(date).startOf('day').format();
                for(const conflict of conflictsJsonAwayLocations) {

                    if(day>=conflict.startDay && day<=conflict.endDay) { 
                        numberOfConflictsPerDay++;
                        if(Vue.filter('isDateFullday')(conflict.start,conflict.end)){                            
                            break;
                        } else {                            
                            numberOfConflictsPerDay++;
                            //console.log( numberOfConflictsPerDay)
                            const start = moment(previousConflictEndDate)
                            const end = moment(conflict.start)
                            const duration = moment.duration(end.diff(start)).asMinutes();
                            if(duration>5)                                
                                conflicts.push({
                                    id:'0',
                                    location:conflict.conflict=='AwayLocation'?this.locationList.filter(locationInfo => {
                                        if (locationInfo.id == conflict.locationId) {
                                                return true
                                        }
                                    })[0].name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:date, 
                                    startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                                    endTime:Vue.filter('beautify-time')(conflict.start),
                                    type:'Unavailable',
                                    workSection: '',
                                    workSectionColor:'',
                                    fullday: false
                                })
                            previousConflictEndDate = conflict.end;  
                        }
                    }   
                }

                if( numberOfConflictsPerDay == 0){
                    conflicts.push({
                        id:'0',
                        location:'',
                        dayOffset: Number(dateIndex), 
                        date:date, 
                        startTime:'', 
                        endTime:'',
                        type:'Unavailable', 
                        workSection: '',
                        workSectionColor:'',
                        fullday: true
                    })
                } else if( numberOfConflictsPerDay > 1) {
                    const start = moment(previousConflictEndDate)
                    const end = moment(date).endOf('day')
                    const duration = moment.duration(end.diff(start)).asMinutes();
                    if(duration>5)
                        conflicts.push({
                            id:'0',
                            location: '',
                            dayOffset: Number(dateIndex), 
                            date:date, 
                            startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                            endTime:Vue.filter('beautify-time')(end.format()),
                            type:'Unavailable', 
                            workSection:'',
                            workSectionColor:'',
                            fullday: false
                        })
                }
            }
            const shifts = this.extractSchedules(conflictsJson, true);
            for (const shift of shifts) {
                conflicts.push(shift);
            }
            return conflicts
        }

        public getConflictsType(conflict){
            if(conflict.conflict =='AwayLocation') return 'Loaned'
            else if(conflict.conflict =='Scheduled') return 'Shift'
            else return conflict.conflict
        } 


        // //__ASSIGNMENTS__
        public extractAssignmentsInfo(assignments){
            const dutyWeekDates: string[] = []
            for(let day=0; day<7; day++)
                dutyWeekDates.push(moment(this.assignmentRangeInfo.startDate).add(day,'days').format().substring(0,10))                

            //console.log(dutyWeekDates)
            const dutyRosterAssignments: assignmentCardWeekInfoType[] =[]
            let sortOrder = 0;
            for(const assignment of assignments){
                sortOrder++;
                const dutyRostersForThisAssignment: attachedDutyInfoType[] = this.dutyRostersJson.filter(dutyroster=>{if(dutyroster.assignmentId == assignment.id)return true}) 

               
                if(dutyRostersForThisAssignment.length>0){
                    let maximumRow = -2;
                    for(const dutydate of dutyWeekDates){
                        const dutyRostersInOneDay = dutyRostersForThisAssignment.filter(dutyRoster => moment(dutyRoster.startDate).tz(this.location.timezone).format().substring(0,10) == dutydate)
                        if(dutyRostersInOneDay.length > maximumRow) maximumRow = dutyRostersInOneDay.length;
                    }

                    const dutyRosterAssignment: assignmentCardWeekInfoType[] = [];

                    for(let row=0; row<maximumRow; row++){
                        dutyRosterAssignment.push({
                            assignment:('00' + sortOrder).slice(-3)+'FTE'+('0'+ row).slice(-2) ,
                            assignmentDetail: assignment,
                            name:assignment.name,
                            code:assignment.lookupCode.code,
                            type: this.getType(assignment.lookupCode.type),
                            0: null,
                            1: null,
                            2: null,
                            3: null,
                            4: null,
                            5: null,
                            6: null,
                            FTEnumber: row,
                            totalFTE: maximumRow
                        })
                    }

                    for(const dutydateInx in dutyWeekDates){
                        const dutyRostersInOneDay = dutyRostersForThisAssignment.filter(dutyRoster => moment(dutyRoster.startDate).tz(this.location.timezone).format().substring(0,10) == dutyWeekDates[dutydateInx])
                        for(const dutyRosterInOneDay of dutyRostersInOneDay){
                            for(let row=0; row<maximumRow; row++){

                                if(!dutyRosterAssignment[row][dutydateInx]){
                                    dutyRosterAssignment[row][dutydateInx] = dutyRosterInOneDay;
                                    break;
                                }
                            }
                        }
                    }

                    dutyRosterAssignments.push(...dutyRosterAssignment)
                }else{                
                    dutyRosterAssignments.push({
                        assignment:('00' + sortOrder).slice(-3)+'FTE00' ,
                        assignmentDetail: assignment,
                        name:assignment.name,
                        code:assignment.lookupCode.code,
                        type: this.getType(assignment.lookupCode.type),
                        0: null,
                        1: null,
                        2: null,
                        3: null,
                        4: null,
                        5: null,
                        6: null,
                        FTEnumber: 0,
                        totalFTE: 0
                    })
                }
            }

           this.UpdateDutyShiftAssignmentsWeek(dutyRosterAssignments)
        }

        public getType(type: string){
            return Vue.filter('getColorByType')(type)
        }

        public extractTableFields(allAssignments){
            if(allAssignments==true || allAssignments==false) 
                this.showAllAssignments = allAssignments;

            this.fields = JSON.parse(JSON.stringify(this.originalFields))  
            if(this.showAllAssignments==false){
                const day = Number(moment().weekday())+1                    
                this.fields[day]['thClass']="bg-current-day"
                this.fields[day]['tdClass']="bg-current-day align-middle px-0 mx-0"
            }            
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>