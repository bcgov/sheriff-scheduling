<template>
    <b-card bg-variant="white" class="home" no-body>

        <distribute-header v-on:change="loadScheduleInformation" @includeTime="includeTime=$event" />

        <b-card id="pdf" v-if="isDistributeDataMounted" class="container" no-body>
            
            <div class="ss-header">
                <div class="row m-0">
                    <div style="width:45%" >
                        <div class="row m-0">                           
                            <div style="width:20%;"  class="mr-0">
                                <img style="width:80%;" 
                                    :src="src"                                                         
                                    alt="B.C. Gov"/>                                                                   
                            </div>                                
                            <div style="width:70%; margin-top:1.5rem;">
                                <div style="font-size:13pt;"><b>B.C. Sheriff Service</b></div>
                                <div style="font-size:12pt;" class="text-secondary font-italic"><b>Honour - Integrity - Commitment</b></div>
                            </div>
                        </div>
                    </div>
                
                    <div style="width:52%; margin-top: 0.2rem;" >
                        <div class="card m-0 border border-dark text-center">
                            <div class="mt-1">{{location.name}} Schedule</div>
                            <div v-if="weekView" style="font-size:12pt;"><b>{{shiftRangeInfo.startDate | beautify-full-date}} - {{shiftRangeInfo.endDate | beautify-full-date}}</b></div>
                            <div v-else style="font-size:12pt;"><b>{{dailyShiftRangeInfo.startDate | beautify-full-date}}</b></div>                            
                            <div class="text-secondary" style="margin-bottom:0.5rem;">Summary as of: <b><i>{{today | beautify-date-time-weekday}}</i></b></div>                                
                        </div>
                    </div>            

                </div>
            </div>
                
            <b-overlay opacity="0.6" :show="!isDistributeDataMounted">
                <template #overlay>
                    <loading-spinner :inline="true"/>
                </template> 
                <div v-for="page,inx in sheriffPages" :key="'pdf-'+inx" class="ss-body">   
                    <weekly-schedule :key="updateTable" :fields="fields"  :sheriffSchedules="sheriffSchedules.slice(page.start,page.end)" :includeTime="includeTime" v-if="weekView" />
                    <daily-schedule :key="updateDailyTable" :dailySheriffSchedules="dailySheriffSchedules.slice(page.start,page.end)" v-else/>

                    <div v-if="!isDistributeDataMounted && sheriffSchedules.length == 0" style="min-height:115.6px;">
                    </div>
                
                    <div v-if="(inx+1)<sheriffPages.length" class="new-page" />
                </div>
            </b-overlay>

        </b-card>

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
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    import DistributeHeader from './components/DistributeHeader.vue';
    import DailySchedule from './components/DailySchedule.vue';
    import WeeklySchedule from './components/WeeklySchedule.vue';

    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");    

    import { locationInfoType } from '@/types/common';
    import { shiftRangeInfoType, scheduleInfoType, weekScheduleInfoType, distributeScheduleInfoType, distributeTeamMemberInfoType, sheriffPagesInfoType, distributeScheduleDutyInfoType } from '@/types/ShiftSchedule/index'
    import { sheriffsAvailabilityJsonType, conflictJsonType } from '@/types/ShiftSchedule/jsonTypes';

    import {srcFile} from "./components/Logo";

    @Component({
        components: {
            DistributeHeader,
            WeeklySchedule,
            DailySchedule
        }
    })
    export default class DistributeSchedule extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @shiftState.State
        public dailyShiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateTeamMemberList!: (newTeamMemberList: distributeTeamMemberInfoType[]) => void

        isDistributeDataMounted = false;
        weekView = false;
        headerDates: string[] = [];
        today = '';
        numberOfheaderDates = 7;
        updateTable=0;
        updateDailyTable=0;        
        includeTime=true;

        errorText='';
		openErrorModal=false;

        teamMembers: distributeTeamMemberInfoType[] = [];

        fields = [
            {key:'myteam',  label:'Name', tdClass:'px-1 mx-0 align-middle my-team', thStyle:'text-align: center; font-size: 8pt; width: 7.5rem;'},
            {key:'Sun',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Mon',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Tue',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Wed',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Thu',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Fri',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'},
            {key:'Sat',     label:'',     tdClass:'px-1 mx-0 py-0', thStyle:'text-align: center; font-size: 7pt; width: 6.5rem;'}
        ]

        WSColors = {
            'CourtRole':'#189fd4',
            'CourtRoom':'#189fd4',
            'JailRole':'#A22BB9',
            'EscortRun':'#ffb007',
            'OtherAssignment':'#7a4528'
        }

        src="";

        sheriffSchedules: weekScheduleInfoType[] =[];
        dailySheriffSchedules: distributeScheduleInfoType[] = [];
        sheriffSchedulesLength = 0;

        sheriffPages: sheriffPagesInfoType[]=[];
        marginToLastPageRows = '0rem';

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isDistributeDataMounted) {
                this.loadScheduleInformation(true, '');
            }            
        }

        mounted() {
            this.src = srcFile;
            this.isDistributeDataMounted=false;			
			//this.loadScheduleInformation(false, '');
            this.today = moment().tz(this.location.timezone).format();
        }

        public loadScheduleInformation(weekView: boolean, sheriffId: string) {                       
            
            this.isDistributeDataMounted=false;
            this.weekView = weekView;
            this.headerDate();
            
            let url = '';

            if (weekView){
                const endDate = moment(this.shiftRangeInfo.endDate).endOf('day').format();
                url = 'api/distributeschedule/location?locationId='
                        +this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+endDate + '&includeWorkSection=true';
            } else { 
                const endDate = moment(this.dailyShiftRangeInfo.endDate).endOf('day').format();               
                url = 'api/distributeschedule/location?locationId='
                        +this.location.id+'&start='+this.dailyShiftRangeInfo.startDate+'&end='+endDate + '&includeWorkSection=true';
            }            
            
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractTeamInfo(response.data);

                        let info = [];
                        if (sheriffId.length == 0) {
                            info = response.data;
                        } else {
                            info = response.data.filter(data =>{if(data.sheriffId == sheriffId ) return true})
                        }
                        this.sheriffSchedulesLength = info.length
                        //console.log(info)
                        if (this.weekView){
                            this.extractTeamScheduleInfo(info);
                        } else {
                            this.extractTeamDailyScheduleInfo(info); 
                        }                      
                                                 
                    }                                   
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }      
                    this.teamMembers = [];
                    this.sheriffSchedules = [];
                    this.isDistributeDataMounted=true;
                })            
        }

        public extractTeamInfo (teamJson) {
            this.teamMembers = [];
            
            for(const sheriffJson of teamJson) {
                const sheriff = {} as distributeTeamMemberInfoType;
                //console.log(sheriffJson)
                sheriff.sheriffId = sheriffJson.sheriffId;           
                sheriff.email = sheriffJson.sheriff.email;     
                sheriff.name = Vue.filter('capitalize')(sheriffJson.sheriff.lastName) 
                                        + ', ' + Vue.filter('capitalize')(sheriffJson.sheriff.firstName);
                this.teamMembers.push(sheriff);
            }
            this.UpdateTeamMemberList(this.teamMembers);
        }

        public headerDate() {

            this.headerDates = [];
            if (this.weekView){
                for(let dayOffset=0; dayOffset<this.numberOfheaderDates; dayOffset++) {
                    const date= moment(this.shiftRangeInfo.startDate).add(dayOffset,'days').format();
                    this.headerDates.push(date);
                    this.fields[dayOffset+1].label = ' ' + Vue.filter('beautify-date')(date);
                }

            } else {

                const date= moment(this.dailyShiftRangeInfo.startDate).format();
                this.headerDates.push(date);
            }
            
        }

        public extractTeamScheduleInfo(sheriffsScheduleJson: sheriffsAvailabilityJsonType[]) {
            
            this.sheriffSchedules = [];
            
            for(const sheriffScheduleJson of sheriffsScheduleJson) {
                //console.log(sheriffScheduleJson)
                const sheriffSchedule = {} as distributeScheduleInfoType;
                sheriffSchedule.sheriffId = sheriffScheduleJson.sheriffId;                
                sheriffSchedule.name = Vue.filter('capitalizefirst')(sheriffScheduleJson.sheriff.lastName) 
                                        + ', ' + Vue.filter('capitalizefirst')(sheriffScheduleJson.sheriff.firstName);
                sheriffSchedule.rank = sheriffScheduleJson.sheriff.rank;
                sheriffSchedule.actingRank = sheriffScheduleJson.sheriff.actingRank;
                sheriffSchedule.badgeNumber = sheriffScheduleJson.sheriff.badgeNumber; 
                sheriffSchedule.homeLocation = sheriffScheduleJson.sheriff.homeLocation.name;                                        
                const isInLoanLocation = (sheriffScheduleJson.sheriff.homeLocation.id !=this.location.id)
                sheriffSchedule.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(sheriffScheduleJson.conflicts) :this.extractSchedules(sheriffScheduleJson.conflicts, false);        
                
                
                this.sheriffSchedules.push({
                    myteam: sheriffSchedule,
                    Sun: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==0) return true}),
                    Mon: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==1) return true}),
                    Tue: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==2) return true}),
                    Wed: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==3) return true}),
                    Thu: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==4) return true}),
                    Fri: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==5) return true}),
                    Sat: sheriffSchedule.conflicts.filter(conflict=>{if(conflict.dayOffset ==6) return true})
                })
            }          
            this.splitSheriffPages()
            this.isDistributeDataMounted = true;            
            this.updateTable++;
        }
   
        public extractTeamDailyScheduleInfo(sheriffsScheduleJson: sheriffsAvailabilityJsonType[]) {
            
            this.dailySheriffSchedules = [];
            
            for(const sheriffScheduleJson of sheriffsScheduleJson) {                
                const sheriffSchedule = {} as distributeScheduleInfoType;
                sheriffSchedule.sheriffId = sheriffScheduleJson.sheriffId;                
                sheriffSchedule.name = Vue.filter('capitalizefirst')(sheriffScheduleJson.sheriff.lastName) 
                                        + ', ' + Vue.filter('capitalizefirst')(sheriffScheduleJson.sheriff.firstName);
                sheriffSchedule.homeLocation = sheriffScheduleJson.sheriff.homeLocation.name;   
                sheriffSchedule.rank = sheriffScheduleJson.sheriff.rank;
                sheriffSchedule.actingRank = sheriffScheduleJson.sheriff.actingRank;
                sheriffSchedule.badgeNumber = sheriffScheduleJson.sheriff.badgeNumber;                                                     
                const isInLoanLocation = (sheriffScheduleJson.sheriff.homeLocation.id !=this.location.id)
                sheriffSchedule.conflicts =isInLoanLocation? this.extractInLoanLocationConflicts(sheriffScheduleJson.conflicts) :this.extractSchedules(sheriffScheduleJson.conflicts, false);
                this.dailySheriffSchedules.push(sheriffSchedule)
            }          
            this.splitSheriffPages();
            this.isDistributeDataMounted = true;            
            this.updateDailyTable++;
        }

        public extractInLoanLocationConflicts(conflictsJson: conflictJsonType[]){
            let conflictsJsonAwayLocation: any[] = []
            const conflicts: scheduleInfoType[] = []
            for(const conflict of conflictsJson){ 
                //console.log(conflict) 
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
                                    type:'Unavailable',
                                    workSection: '',
                                    workSectionColor:''
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
                        workSectionColor:''
                    })
                } else if( numberOfConflictsPerDay > 1){
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
                            workSectionColor:''
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
        
        public extractSchedules(conflictsJson, onlyShedules){

            const schedules: scheduleInfoType[] = []

            for(const conflict of conflictsJson){                
                if (conflict.conflict=="Scheduled" && conflict.locationId != this.location.id) continue;
                if (conflict.conflict!="Scheduled" && onlyShedules) continue;
                conflict.start = moment(conflict.start).tz(this.location.timezone).format();
                conflict.end = moment(conflict.end).tz(this.location.timezone).format();    
                         
                if(Vue.filter('isDateFullday')(conflict.start,conflict.end))
                {                                  
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex]
                        if(date>=conflict.start && date<=conflict.end)
                        {
                            const duties = ((conflict.conflict =='Scheduled')&&(conflict.dutySlots?.length > 0)) ? this.extractDutyInfo(conflict.dutySlots): [];
                            
                            schedules.push({
                                    id:conflict.shiftId? conflict.shiftId:0,
                                    location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                    dayOffset: Number(dateIndex), 
                                    date:date, 
                                    startTime:'', 
                                    endTime:'',
                                    type:this.getConflictsType(conflict),
                                    subType: (conflict.sheriffEventType)?conflict.sheriffEventType:'',
                                    duties: duties,
                                    workSection: '',
                                    workSectionColor: '',
                                    fullday: true,
                                    overtime: conflict.overtimeHours
                                }) 
                        }                       
                    }
                } else {
                    
                    for(const dateIndex in this.headerDates){
                        const date = this.headerDates[dateIndex].substring(0,10);
                        const nextDate = moment(this.headerDates[dateIndex]).add(1,'days').format().substring(0,10);
                        if(date == conflict.start.substring(0,10) && date == conflict.end.substring(0,10)) {  

                            const duties = (conflict.dutySlots?.length > 0) ? this.extractDutyInfo(conflict.dutySlots) :[];
                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(conflict.end), 
                                type:this.getConflictsType(conflict),                                
                                subType: (conflict.sheriffEventType)?conflict.sheriffEventType:'', 
                                duties: duties,
                                workSection:'',
                                workSectionColor: '',
                                fullday: false,
                                overtime: conflict.overtimeHours
                            })                            

                        } else if(date == conflict.start.substring(0,10) && nextDate == conflict.end.substring(0,10)) {  
                            const midnight = moment(conflict.start).endOf('day');  
                            
                            const duties = (conflict.dutySlots?.length > 0) ? this.extractDutyInfo(conflict.dutySlots): [];

                            schedules.push({
                                id:conflict.shiftId? conflict.shiftId:0,
                                location:conflict.conflict=='AwayLocation'?conflict.location.name:'',
                                dayOffset: Number(dateIndex), 
                                date:this.headerDates[dateIndex], 
                                startTime:Vue.filter('beautify-time')(conflict.start), 
                                endTime:Vue.filter('beautify-time')(midnight.format()),
                                type:this.getConflictsType(conflict),
                                subType: (conflict.sheriffEventType)?conflict.sheriffEventType:'', 
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
                                subType: (conflict.sheriffEventType)?conflict.sheriffEventType:'', 
                                duties: duties,
                                workSection: '',
                                workSectionColor: '',
                                fullday: false,
                                overtime: conflict.overtimeHours
                            }) 
                           
                        }                       
                    }
                } 
            }

            return schedules
        } 
        
        public splitSheriffPages(){
            this.sheriffPages =[]
            const PAGE_ITEMS=10
            const len = this.sheriffSchedulesLength
            for(let page=1; page<=(Math.ceil(len/PAGE_ITEMS)); page++){
                this.sheriffPages.push({
                    start:(page-1)*PAGE_ITEMS,
                    end:Math.min(len, page*PAGE_ITEMS)
                })
            }  
            const mod = this.sheriffSchedulesLength % PAGE_ITEMS
            this.marginToLastPageRows = (mod==0)? '0rem' :((PAGE_ITEMS-mod)*2.1)+'rem'
        }

        public extractDutyInfo(dutySlots){

            const duties: distributeScheduleDutyInfoType[] = [];

            for (const duty of dutySlots){
                const dutyData = {} as distributeScheduleDutyInfoType;
                dutyData.startTime = Vue.filter('beautify-time')(moment(duty.startDate).tz(duty.timezone).format());
                dutyData.endTime = Vue.filter('beautify-time')(moment(duty.endDate).tz(duty.timezone).format());
                dutyData.dutyType = (duty.assignmentLookupCode?.type)?duty.assignmentLookupCode.type:'';
                dutyData.dutySubType = (duty.assignmentLookupCode?.code)?duty.assignmentLookupCode.code:'';
                dutyData.dutyNotes = (duty.dutyComment)?(duty.dutyComment + ' (' +dutyData.startTime + '-' + dutyData.endTime + ')'):'';
                dutyData.assignmentNotes = (duty.assignmentComment)?(duty.assignmentComment + ' (' +dutyData.dutySubType+ ')'):'';
                dutyData.color = this.WSColors[duty.assignmentLookupCode.type]?this.WSColors[duty.assignmentLookupCode.type]:'';
                duties.push(dutyData);                                            
            }

            return duties;

        }

    }
</script>

<style scoped lang="scss" src="@/styles/_pdf.scss">

</style>