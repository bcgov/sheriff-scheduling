<template>
    <b-card bg-variant="white" class="home" no-body>        
        <schedule-header v-on:change="loadScheduleInformation()" />           
            <b-overlay opacity="0.6" :show="!isManageScheduleDataMounted">
                <template #overlay>
                    <loading-spinner :inline="true"/>
                </template>    
                <b-table
                    :key="updateTable"
                    :items="shiftSchedules" 
                    :fields="fields"
                    small
                    head-row-variant="primary"   
                    bordered
                    fixed>
                        <template v-slot:table-colgroup>
                            <col style="width:8.5rem;">                            
                        </template>
                        <template v-slot:head() = "data" >
                            <span class="text">{{data.column}}</span> <span> {{data.label}}</span>
                        </template>
                        <template v-slot:head(myteam) = "data" >  
                            <span>{{data.label}}</span>
                        </template>
                        <template v-slot:cell()="data" >
                            <schedule-card :courtAdminId="data.item.myteam.courtAdminId" :scheduleInfo=" data.value"/>
                        </template>
                        <template v-slot:cell(myteam) = "data" > 
                            <team-member-card v-on:change="loadScheduleInformation()" :courtAdminInfo="data.item.myteam" />
                        </template>
                </b-table>
                <div v-if="!isManageScheduleDataMounted && this.shiftSchedules.length == 0" style="min-height:115.6px;">
                </div>
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

    import ScheduleCard from './components/ScheduleCard.vue';
    import TeamMemberCard from './components/TeamMemberCard.vue';
    import ScheduleHeader from './components/ScheduleHeader.vue';

    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import { locationInfoType, commonInfoType } from '@/types/common';
    import { courtAdminAvailabilityInfoType, shiftRangeInfoType, weekShiftInfoType,conflictsInfoType } from '@/types/ShiftSchedule/index'
    import { courtAdminsAvailabilityJsonType } from '@/types/ShiftSchedule/jsonTypes';
    

    @Component({
        components: {
            ScheduleCard,
            TeamMemberCard,
            ScheduleHeader
        }
    })
    export default class ManageSchedule extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;
        
        @commonState.State
        public location!: locationInfoType;

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        @shiftState.State
        public courtAdminsAvailabilityInfo!: courtAdminAvailabilityInfoType[];

        @shiftState.Action
        public UpdateCourtAdminsAvailabilityInfo!: (newCourtAdminsAvailabilityInfo: courtAdminAvailabilityInfoType[]) => void

        isManageScheduleDataMounted = false;
        headerDates: string[] = [];
        numberOfheaderDates = 7;
        updateTable=0;

        errorText =''
        openErrorModal=false
        maxRank = 1000;

        fields=[
            {key:'myteam', label:'My Team', tdClass:'px-0 mx-0', thClass:'text-center'},
            {key:'Sun', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Mon', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Tue', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Wed', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Thu', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Fri', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'},
            {key:'Sat', label:'', tdClass:'px-0 mx-0', thStyle:'text-align: center;'}
        ]

        shiftSchedules: weekShiftInfoType[] =[];

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isManageScheduleDataMounted) {
                this.loadScheduleInformation() 
                //console.log('watch')               
            }            
        } 

        // mounted()
        // {                       
        //     //this.loadScheduleInformation();
        //     console.log('mount manage')
        // }

        public loadScheduleInformation() {

            this.UpdateSelectedShifts([]);
            this.isManageScheduleDataMounted=false;

            this.headerDate();

            const endDate = moment.tz(this.shiftRangeInfo.endDate, this.location.timezone).endOf('day').utc().format();
            const startDate = moment.tz(this.shiftRangeInfo.startDate, this.location.timezone).startOf('day').utc().format();

            const url = 'api/shift/shiftavailability?locationId='+this.location.id+'&start='+startDate+'&end='+endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        //console.log(response.data)
                        this.extractTeamAvailabilityInfo(response.data);                        
                    }                                   
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }      
                    this.shiftSchedules = [];
                    this.isManageScheduleDataMounted=true;
                })            
        }

        public headerDate() {
            this.headerDates = [];
            for(let dayOffset=0; dayOffset<this.numberOfheaderDates; dayOffset++)
            {
                const date= moment(this.shiftRangeInfo.startDate).add(dayOffset,'days').format();
                this.headerDates.push(date);
                this.fields[dayOffset+1].label = ' ' + Vue.filter('beautify-date')(date);
            }
            //console.log(this.headerDates)
        }

        public extractTeamAvailabilityInfo(courtAdminsAvailabilityJson: courtAdminsAvailabilityJsonType[]) {

           // const courtAdminsAvailability: courtAdminAvailabilityInfoType[] = [];
            this.shiftSchedules = [];
            
            for(const courtAdminAvailabilityJson of courtAdminsAvailabilityJson) {
                const courtAdminAvailability = {} as courtAdminAvailabilityInfoType;
                courtAdminAvailability.courtAdminId = courtAdminAvailabilityJson.courtAdminId;
                courtAdminAvailability.firstName = courtAdminAvailabilityJson.courtAdmin.firstName;
                courtAdminAvailability.lastName = courtAdminAvailabilityJson.courtAdmin.lastName;
                courtAdminAvailability.badgeNumber = courtAdminAvailabilityJson.courtAdmin.badgeNumber;
                courtAdminAvailability.rank = (courtAdminAvailabilityJson.courtAdmin.actingRank?.length>0)?  (courtAdminAvailabilityJson.courtAdmin.actingRank[0].rank)+' (A)': courtAdminAvailabilityJson.courtAdmin.rank;
                courtAdminAvailability.rankOrder = this.getRankOrder(courtAdminAvailability.rank)[0]?this.getRankOrder(courtAdminAvailability.rank)[0].id:0;
                courtAdminAvailability.homeLocation= {id: courtAdminAvailabilityJson.courtAdmin.homeLocation.id, name: courtAdminAvailabilityJson.courtAdmin.homeLocation.name}
                const isInLoanLocation = (courtAdminAvailabilityJson.courtAdmin.homeLocation.id !=this.location.id)
                courtAdminAvailability.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(courtAdminAvailabilityJson.conflicts) :this.extractConflicts(courtAdminAvailabilityJson.conflicts, false);        
                //courtAdminsAvailability.push(courtAdminAvailability)
               // console.log(isInLoanLocation)
                
                this.shiftSchedules.push({
                    myteam: courtAdminAvailability,
                    Sun: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==0) return true}),
                    Mon: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==1) return true}),
                    Tue: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==2) return true}),
                    Wed: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==3) return true}),
                    Thu: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==4) return true}),
                    Fri: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==5) return true}),
                    Sat: courtAdminAvailability.conflicts.filter(conflict=>{if(conflict.dayOffset ==6) return true})
                })
            }
            //this.UpdateCourtAdminsAvailabilityInfo(courtAdminsAvailability);
            this.maxRank = this.commonInfo.courtAdminRankList.reduce((max, rank) => rank.id > max ? rank.id : max, this.commonInfo.courtAdminRankList[0].id);
            this.shiftSchedules = this.sortTeamMembers(this.shiftSchedules);
            this.isManageScheduleDataMounted = true;
            this.updateTable++;
        }

        public sortTeamMembers(teamList) {            
            return _.chain(teamList)
                    .sortBy(member =>{return (member.myteam['lastName']? member.myteam['lastName'].toUpperCase() : '')})
                    .sortBy(member =>{return (member.myteam['rankOrder']? member.myteam['rankOrder'] : this.maxRank + 1)})
                    .value()
        }

        public extractConflicts(conflictsJson, onlyShedules){

            const conflicts: conflictsInfoType[] = []

            for(const conflict of conflictsJson){
                if (conflict.conflict=="Scheduled" && conflict.locationId != this.location.id) continue;
                if (conflict.conflict!="Scheduled" && onlyShedules) continue;
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();                              
                if(Vue.filter('isDateFullday')(conflict.start,conflict.end))
                {
                    if (conflict.conflict=='AwayLocation' && this.location.timezone != conflict.location.timezone){
                        conflict.start = moment(conflict.start).tz(conflict.location.timezone).format();
                        conflict.end = moment(conflict.end).tz(conflict.location.timezone).format();
                    }               
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex]                        
                        if(date>=conflict.start && date<=conflict.end)
                        {
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:date, 
                                startTime:'', 
                                endTime:'',
                                startInMinutes:0, 
                                timeDuration:0, 
                                type:this.getConflictsType(conflict), 
                                subType: (this.getConflictsType(conflict)=='Leave' && conflict.courtAdminEventType)?conflict.courtAdminEventType:'',
                                fullday: true,
                                courtAdminEventType:conflict.courtAdminEventType?conflict.courtAdminEventType:'' ,
                                comment: conflict.comment? conflict.comment :''
                            })        
                        }                       
                    }
                }
                else{
                    
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex].substring(0,10);
                        const nextDate = moment(this.headerDates[dateIndex]).add(1,'days').format().substring(0,10);
                        if(date == conflict.start.substring(0,10) && date == conflict.end.substring(0,10))
                        {  
                            const start = moment(conflict.start)
                            const end = moment(conflict.end)

                            //console.log(conflict)
                            
                            if (conflict.conflict == "Scheduled" && conflict.overtimeHours !=0) {
                                
                                const conflictDuration = moment.duration(end.diff(start)).asHours();
                                const overtime = (conflictDuration <= conflict.overtimeHours)? conflictDuration : conflict.overtimeHours
                                const regularTimeEnd = moment(conflict.end).subtract(overtime, 'h').tz(this.location.timezone);
                                // console.log(overtime)
                                // console.log(conflictDuration)
                                // console.log(conflict.overtimeHours)
                                
                                let duration = moment.duration(regularTimeEnd.diff(start));
                                
                                const regularShift = {
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(conflict.start), 
                                    endTime:Vue.filter('beautify-time')(regularTimeEnd.format()), 
                                    startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                    timeDuration:duration.asMinutes(), 
                                    type:this.getConflictsType(conflict),
                                    subType: (this.getConflictsType(conflict)=='Leave' && conflict.courtAdminEventType)?conflict.courtAdminEventType:'',                                 
                                    fullday:false,
                                    comment: conflict.comment? conflict.comment :''
                                }

                                duration = moment.duration(end.diff(regularTimeEnd))

                                const overTimeShift = {
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:this.headerDates[dateIndex], 
                                    startTime:Vue.filter('beautify-time')(regularTimeEnd.format()), 
                                    endTime:Vue.filter('beautify-time')(conflict.end), 
                                    startInMinutes:moment.duration(regularTimeEnd.diff(moment(regularTimeEnd.format()).startOf('day'))).asMinutes(),
                                    timeDuration:duration.asMinutes(), 
                                    type:'overTimeShift', 
                                    fullday:false,
                                    comment: conflict.comment? conflict.comment :''
                                }
                                // console.log(regularShift)
                                if(conflictDuration > conflict.overtimeHours)conflicts.push(regularShift);
                                conflicts.push(overTimeShift);                                

                            } else {
                                const duration = moment.duration(end.diff(start));//duration.asMinutes()
                                conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:duration.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                subType: (this.getConflictsType(conflict)=='Leave' && conflict.courtAdminEventType)?conflict.courtAdminEventType:'',                                
                                fullday:false,
                                courtAdminEventType:conflict.courtAdminEventType?conflict.courtAdminEventType:'',
                                comment: conflict.comment? conflict.comment :''
                                }) 

                            }   
                        }else if(date == conflict.start.substring(0,10) && nextDate == conflict.end.substring(0,10))
                        {  
                            const start = moment(conflict.start)
                            const midnight = moment(conflict.start).endOf('day')
                            const end = moment(conflict.end)
                            const durationStart = moment.duration(midnight.diff(start));
                            const durationEnd = moment.duration(end.diff(midnight));
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(midnight.format()), 
                                startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                timeDuration:durationStart.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                subType: (this.getConflictsType(conflict)=='Leave' && conflict.courtAdminEventType)?conflict.courtAdminEventType:'',
                                fullday:false,
                                courtAdminEventType:conflict.courtAdminEventType?conflict.courtAdminEventType:'',
                                comment: conflict.comment? conflict.comment :'' 
                            })
                            conflicts.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex)+1, 
                                date:moment(this.headerDates[dateIndex]).add(1,'day').format(), 
                                startTime:'00:00', 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                startInMinutes:0,
                                timeDuration:durationEnd.asMinutes(), 
                                type:this.getConflictsType(conflict), 
                                subType: (this.getConflictsType(conflict)=='Leave' && conflict.courtAdminEventType)?conflict.courtAdminEventType:'',                                
                                fullday:false,
                                courtAdminEventType:conflict.courtAdminEventType?conflict.courtAdminEventType:'',
                                comment: conflict.comment? conflict.comment :'' 
                            })        
                        }                       
                    }
                } 
            }
            //console.log(conflicts)

            return conflicts
        } 

        public extractInLoanLocationConflicts(conflictsJson){
            let conflictsJsonAwayLocation: any[] = []
            const conflicts: conflictsInfoType[] = []
            for(const conflict of conflictsJson){  
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();              
                if(conflict.conflict !='AwayLocation' || conflict.locationId != this.location.id) continue;
                conflict['startDay']=conflict.start.substring(0,10);
                conflict['endDay']=conflict.end.substring(0,10);
                conflictsJsonAwayLocation.push(conflict);
            }
            conflictsJsonAwayLocation = _.sortBy(conflictsJsonAwayLocation,'start')

            for(const dateIndex in this.headerDates){
                const date = this.headerDates[dateIndex]
                const day = date.substring(0,10);
                let numberOfConflictsPerDay = 0;
                let previousConflictEndDate = moment(date).startOf('day').format();
                for(const conflict of conflictsJsonAwayLocation){

                    if(day>=conflict.startDay && day<=conflict.endDay)
                    { 
                        numberOfConflictsPerDay++;
                        if(Vue.filter('isDateFullday')(conflict.start,conflict.end)){                            
                            break;
                        } else {
                            //console.log(conflict)                            
                            numberOfConflictsPerDay++;
                            //console.log( numberOfConflictsPerDay)
                            const start = moment(previousConflictEndDate)
                            const end = moment(conflict.start)
                            const duration = moment.duration(end.diff(start)).asMinutes();
                            if(duration>5)
                                conflicts.push({
                                    id:'0',
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:date, 
                                    startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                                    endTime:Vue.filter('beautify-time')(conflict.start), 
                                    startInMinutes:moment.duration(start.diff(moment(conflict.start).startOf('day'))).asMinutes(),
                                    timeDuration:duration, 
                                    type:'Unavailable', 
                                    fullday:false
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
                        startInMinutes:0, 
                        timeDuration:0, 
                        type:'Unavailable', 
                        fullday: true
                    })
                } else if( numberOfConflictsPerDay > 1){
                    // console.warn('start_end_now')                    
                    // console.log(previousConflictEndDate)
                    // console.log(date)
                    const start = moment(previousConflictEndDate)
                    const end = moment(date).endOf('day')
                    const duration = moment.duration(end.diff(start)).asMinutes();
                    // console.log(start)
                    // console.log(end)
                    // console.log(duration)
                    if(duration>5)
                        conflicts.push({
                            id:'0',
                            location: '',
                            dayOffset: Number(dateIndex), 
                            date:date, 
                            startTime:Vue.filter('beautify-time')(previousConflictEndDate), 
                            endTime:Vue.filter('beautify-time')(end.format()), 
                            startInMinutes:moment.duration(start.diff(moment(previousConflictEndDate).startOf('day'))).asMinutes(),
                            timeDuration:duration, 
                            type:'Unavailable', 
                            fullday:false
                        })
                }
            }
            const shifts = this.extractConflicts(conflictsJson, true);

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
        
        public getRankOrder(rankName: string) {
            if(rankName?.includes(' (A)'))
                rankName = rankName.replace(' (A)','');
            return this.commonInfo.courtAdminRankList.filter(rank => {
                if (rank.name == rankName) {
                    return true;
                }
            })
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>