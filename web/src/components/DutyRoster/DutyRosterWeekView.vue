<template>
    <div>
        <loading-spinner v-if="!isDutyRosterDataMounted" />
        <b-table 
            v-else              
            :items="dutyRosterAssignmentsWeek" 
            :fields="fields"
            sort-by="assignment"
            small
            head-row-variant="primary"   
            borderless
            :style="{ height: getHeight, maxHeight: '100%', marginBottom: '0px' }"             
            :sticky-header="getHeight"                  
            fixed>
                <template v-slot:table-colgroup>
                    <col style="width:9rem">                            
                </template>
                
                <template v-slot:cell(assignment) ="data"  >
                    <duty-roster-assignment v-on:change="getData" :assignment="data.item" :weekview="true"/>
                </template>

                <template v-slot:head(assignment)="data" >
                    <div style="float: left; margin:0 1rem; padding:0;">
                        <div style="float: left; margin:.15rem .25rem 0  0; font-size:14px">{{data.label}}</div>
                        <b-button
                            v-if="hasPermissionToAddAssignments"
                            variant="success"
                            style="padding:0; height:1rem; width:1rem; margin:auto 0" 
                            @click="addAssignment();"
                            size="sm"> <div style="transform:translate(0,-3px)" >+</div>
                        </b-button>
                    </div>
                </template>

                <template v-slot:head(h0) >
                    <div class="grid796h">
                        <div v-for="i in 7" :key="i" :style="{gridColumnStart: ((i-1)*96)+1,gridColumnEnd:(i*96+2), gridRow:'1/1'}">
                            <div class="h6 text-center">{{getBeautifyTime(i-1)}}</div>
                        </div>
                    </div>
                </template>

                <template v-slot:cell(h0)="data" >
                    <duty-card-week-view v-on:change="getData" :dutyRosterInfo="data.item"/>
                </template>
        </b-table>                
        

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
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch, Prop} from 'vue-property-decorator';

    import DutyCardWeekView from './components/DutyCardWeekView.vue'
    import DutyRosterAssignment from './components/DutyRosterAssignment.vue'

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import {locationInfoType, userInfoType, commonInfoType } from '@/types/common';
    import { assignmentCardWeekInfoType, attachedDutyInfoType, dutyRangeInfoType, myTeamShiftInfoType, dutiesDetailInfoType, selectedDutyCardInfoType} from '@/types/DutyRoster';
    import { shiftInfoType } from '@/types/ShiftSchedule';

    @Component({
        components: {
            DutyCardWeekView,            
            DutyRosterAssignment
        }
    })
    export default class DutyRosterDayView extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;
        
        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.Action
        public UpdateShiftAvailabilityInfo!: (newShiftAvailabilityInfo: myTeamShiftInfoType[]) => void

        @dutyState.Action
        public UpdateSelectedDuties!: (newSelectedDuties: selectedDutyCardInfoType[]) => void

        @dutyState.State
        public dutyRosterAssignmentsWeek!: assignmentCardWeekInfoType[];

        @dutyState.Action
        public UpdateDutyRosterAssignmentsWeek!: (newDutyRosterAssignmentsWeek: assignmentCardWeekInfoType[]) => void

        @dutyState.State
        public zoomLevel!: number;

        @Prop({required: true})
        runMethod!: any;
        
        isDutyRosterDataMounted = false;
        hasPermissionToAddAssignments = false;

       

        dutyRostersJson: attachedDutyInfoType[] = [];
        dutyRosterAssignmentsJson;

        scrollPositions = {scrollDuty:0, scrollGauge:0, scrollTeamMember:0 };
        windowHeight = 0;
        tableHeight = 0;

        errorText=''
		openErrorModal=false;

        fields =[
            {key:'assignment', label:'Assignments', thClass:' m-0 p-0', tdClass:'p-0 m-0', thStyle:''},
            {key:'h0', label:'', thClass:'', tdClass:'p-0 m-0', thStyle:'margin:0; padding:0;'}
        ]

        dutyColors = [
            {name:'courtroom',  colorCode:'#189fd4'},
            {name:'court',      colorCode:'#189fd4'},
            {name:'jail' ,      colorCode:'#A22BB9'},
            {name:'escort',     colorCode:'#ffb007'},
            {name:'other',      colorCode:'#7a4528'}, 
            {name:'overtime',   colorCode:'#e85a0e'},
            {name:'free',       colorCode:'#e6d9e2'}                        
        ]

        @Watch('location.id', { immediate: true })
        async locationChange()
        {
            if (this.isDutyRosterDataMounted) {
                this.getData(this.scrollPositions);
            }            
        } 

        @Watch('zoomLevel')
        zoomLevelChange() 
        {   
            Vue.nextTick(() => this.scrollAdjustment() );
        }

        async mounted()
        {
            this.runMethod.$on('getData', this.getData)
            this.isDutyRosterDataMounted = false;
            this.getData(this.scrollPositions);
            window.addEventListener('resize', this.getWindowHeight);
            this.getWindowHeight()
        }

        beforeDestroy() {
            window.removeEventListener('resize', this.getWindowHeight);
        }
        public getWindowHeight() {
            this.windowHeight = Math.ceil(100*document.documentElement.clientHeight/this.zoomLevel);
            this.calculateTableHeight()
        }
        get getHeight() {
            return this.windowHeight - this.tableHeight + 'px'
        }
        public calculateTableHeight() {
            const topHeaderHeight = (document.getElementsByClassName("app-header")[0] as HTMLElement)?.offsetHeight || 0;
            const secondHeader =  document.getElementById("dutyRosterNav")?.offsetHeight || 0;
            const gageHeight = (document.getElementsByClassName("fixed-bottom")[0] as HTMLElement)?.offsetHeight || 0;
            const bottomHeight = gageHeight;
            this.tableHeight = (topHeaderHeight + bottomHeight + secondHeader+1)
        }

        public async getData(dutyScroll) {
            this.scrollPositions = dutyScroll? dutyScroll : {scrollDuty:0, scrollGauge:0, scrollTeamMember:0 }
            const response = await Promise.all([
                this.getDutyRosters(),
                this.getAssignments(),
                this.getShifts()
            ]).catch(err=>{
                this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                if (err.response.status != '401') {
                    this.openErrorModal=true;
                }   
                this.isDutyRosterDataMounted=true;
            });

            this.dutyRostersJson = response[0].data;
            this.dutyRosterAssignmentsJson = response[1].data;

            const shiftsData = response[2].data

            this.UpdateSelectedDuties([]);

            Vue.nextTick(() => {
                this.extractTeamShiftInfo(shiftsData);                        
                this.extractAssignmentsInfo(this.dutyRosterAssignmentsJson);  
            })
        }

        public getBeautifyTime(day: number){
            return moment(this.dutyRangeInfo.startDate).add(day, 'days').format('ddd DD MMM YYYY');
        }

        public getDutyRosters(){
            this.hasPermissionToAddAssignments = this.userDetails.permissions.includes("CreateAssignments");
            const url = 'api/dutyroster?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            return this.$http.get(url)
        }

        public getAssignments(){
            const url = 'api/assignment?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate;
            return this.$http.get(url)
        }

        public async getShifts(){
            this.isDutyRosterDataMounted = false;
            const url = 'api/shift?locationId='+this.location.id+'&start='+this.dutyRangeInfo.startDate+'&end='+this.dutyRangeInfo.endDate +'&includeDuties=true';
            return this.$http.get(url)
        }        

        public extractTeamShiftInfo(shiftsJson){
            this.UpdateShiftAvailabilityInfo([]);
            const allDutySlots: any[] = []
            for(const dutyRoster of this.dutyRostersJson)                
                    allDutySlots.push(...dutyRoster.dutySlots)
           
            for(const shiftJson of shiftsJson)
            {
               

                const availabilityInfo = {} as myTeamShiftInfoType;
                const shiftInfo = {} as shiftInfoType;
                shiftInfo.id = shiftJson.id;
                shiftInfo.startDate =  moment(shiftJson.startDate).tz(this.location.timezone).format();
                shiftInfo.endDate = moment(shiftJson.endDate).tz(this.location.timezone).format();
                shiftInfo.timezone = shiftJson.timezone;
                shiftInfo.courtAdminId = shiftJson.courtAdminId;
                shiftInfo.locationId = shiftJson.locationId;
                shiftInfo.overtimeHours = shiftJson.overtimeHours;

                const index = this.shiftAvailabilityInfo.findIndex(shift => shift.courtAdminId == shiftInfo.courtAdminId);

                const dutySlots = allDutySlots.filter(dutyslot=>{if(dutyslot.courtAdminId==shiftInfo.courtAdminId && dutyslot.startDate.substring(0,10)==shiftInfo.startDate.substring(0,10))return true})
                const dutiesDetail: dutiesDetailInfoType[] = [];
                const rangeBin = this.getTimeRangeBins(shiftInfo.startDate, shiftInfo.endDate, shiftInfo.timezone);
                const shiftArray = this.fillInArray(Array(96).fill(0), 1 , rangeBin.startBin,rangeBin.endBin)


                for(const duty of dutySlots){
                    const color = this.getType(duty.assignmentLookupCode.type)
                    const dutyRangeBin = this.getTimeRangeBins(duty.startDate, duty.endDate, this.location.timezone);
                    const dutyArray = this.fillInArray(Array(96).fill(0), 1 , dutyRangeBin.startBin,dutyRangeBin.endBin);
                    
                    if( this.sumOfArrayElements(this.unionArrays(dutyArray,shiftArray))>0){
                    
                        if(shiftInfo.overtimeHours>0){
                            const dutyRosterIndex = this.dutyRostersJson.findIndex(dutyroster=>{if(dutyroster.id == duty.dutyId)return true}) 
                            const dutySlotIndex = this.dutyRostersJson[dutyRosterIndex].dutySlots.findIndex(dutyslot=>{if(dutyslot.id == duty.id)return true})

                            this.dutyRostersJson[dutyRosterIndex].dutySlots[dutySlotIndex].isOvertime = true
                        } 

                        if(index!= -1){
                            const duplicateIndex =this.shiftAvailabilityInfo[index].dutiesDetail.findIndex(dupDuty=>{if(dupDuty.startBin==dutyRangeBin.startBin && dupDuty.endBin==dutyRangeBin.endBin && dupDuty.id==duty.id)return true})

                            if(duplicateIndex != -1) continue;
                        }  
                        
                        dutiesDetail.push({
                            id:duty.id ,
                            startBin:dutyRangeBin.startBin, 
                            endBin: dutyRangeBin.endBin,
                            startTime:moment(duty.startDate).tz(this.location.timezone).format(),
                            endTime:moment(duty.endDate).tz(this.location.timezone).format(),
                            name: color.name,
                            colorCode: color.colorCode,
                            color: duty.isOvertime? this.dutyColors[5].colorCode:color.colorCode,
                            type: duty.assignmentLookupCode.type,
                            code: duty.assignmentLookupCode.code
                        })
                    }

                }
                
                if (index != -1) {
                   
                    this.shiftAvailabilityInfo[index].duties = [];
                    this.shiftAvailabilityInfo[index].availability = [];
                    this.shiftAvailabilityInfo[index].shifts.push(shiftInfo);
                    this.shiftAvailabilityInfo[index].dutiesDetail.push(...dutiesDetail);
                } else {
                    availabilityInfo.shifts = [shiftInfo];
                    availabilityInfo.courtAdminId = shiftJson.courtAdmin.id;
                    availabilityInfo.badgeNumber = shiftJson.courtAdmin.badgeNumber;
                    availabilityInfo.firstName = shiftJson.courtAdmin.firstName;
                    availabilityInfo.lastName = shiftJson.courtAdmin.lastName;
                    availabilityInfo.rank = ( shiftJson.courtAdmin.actingRank?.length>0)?  ( shiftJson.courtAdmin.actingRank[0].rank)+' (A)': shiftJson.courtAdmin.rank;
                    availabilityInfo.rankOrder = this.getRankOrder(availabilityInfo.rank)[0]?this.getRankOrder(availabilityInfo.rank)[0].id:0;
                    availabilityInfo.availability = [];
                    availabilityInfo.duties = [];
                    availabilityInfo.dutiesDetail = dutiesDetail;
                    this.shiftAvailabilityInfo.push(availabilityInfo);
                }
            }

            this.UpdateShiftAvailabilityInfo(this.shiftAvailabilityInfo);            
        }

        public extractAssignmentsInfo(assignments){
            const dutyWeekDates: string[] = []
            for(let day=0; day<7; day++)
                dutyWeekDates.push(moment(this.dutyRangeInfo.startDate).add(day,'days').format().substring(0,10))                


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

           this.UpdateDutyRosterAssignmentsWeek(dutyRosterAssignments) 
           
           this.isDutyRosterDataMounted = true;
           this.$emit('dataready');

           Vue.nextTick(()=>this.scrollAdjustment())

        }
        
        public scrollAdjustment(){
                this.getWindowHeight();
                const el = document.getElementsByClassName('b-table-sticky-header') 
                if(el[0]){                    
                    el[0].scrollTop = this.scrollPositions.scrollDuty;
                }                

                const eltm = document.getElementById('dutyrosterteammember');
                if(eltm){
                    eltm.scrollTop = this.scrollPositions.scrollTeamMember;
                }
        }
        
        public getType(type: string){
            for(const color of this.dutyColors){
                if(type.toLowerCase().includes(color.name))return color
            }
            return this.dutyColors[3]
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        }

        public getTimeRangeBins(startDate, endDate, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startOfDay = moment(startTime).startOf("day");
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public addAssignment(){ 
            this.$emit('addAssignmentClicked');
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

    .gauge {
        direction:rtl;
        position: sticky;
    }

    .grid796h {        
        display:grid;
        grid-template-columns: repeat(672, 0.14881%);
        grid-template-rows: 1.65rem;
        inline-size: 100%;
        font-size: 10px;
        height: 1.58rem;         
    }

    .grid796h > div {      
        padding: 0.3rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>