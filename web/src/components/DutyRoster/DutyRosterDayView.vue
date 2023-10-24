<template>
    <div> 
        <loading-spinner v-if="!isDutyRosterDataMounted" />      
        <court-admin-day-view v-else-if="sheriffFullview" />       

        <b-table
            v-else
            :items="dutyRosterAssignments" 
            :fields="fields"
            sort-by="assignment"
            :style="{ overflowX: 'scroll', height: getHeight, maxHeight: '100%', marginBottom: '0px' }" 
            small   
            borderless
            :sticky-header="getHeight"                  
            fixed>
                <template v-slot:table-colgroup>
                    <col style="width:9rem">                         
                </template>
                
                <template v-slot:cell(assignment) ="data"  >
                    <duty-roster-assignment v-on:change="getData" :assignment="data.item" :weekview="false"/>
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
                    <div class="grid24">
                        <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                    </div>
                </template>

                <template v-slot:cell(h0)="data" >
                    <duty-card v-on:change="getData" :dutyRosterInfo="data.item"/>
                </template>
        </b-table>                
        <court-admin-fuel-gauge v-show="isDutyRosterDataMounted && displayFuelGauge" class="fixed-bottom bg-white"/>
        <div id="app-footer-assignment" v-if="!sheriffFullview">
            <b-row style="margin:0 0 .25rem .25rem; ">
                <div
                    style="width:5.2rem;"
                    class="m-0 p-0"
                    v-for="color in dutyColorsCode" 
                    :key="color.colorCode"> 
                    <div :style="{backgroundColor:color.colorCode, width:'.65rem', height:'.65rem', borderRadius:'15px', margin:'.2rem .2rem 0 0', float:'left'}"/>
                    <div style="font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0">{{color.name}}</div>
                </div>
            </b-row>            
        </div>

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

    import DutyCard from './components/DutyCard.vue'
    import SheriffFuelGauge from './components/SheriffFuelGauge.vue'
    import DutyRosterAssignment from './components/DutyRosterAssignment.vue'
    import SheriffDayView from './components/SheriffDayView.vue'

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import {locationInfoType, userInfoType, commonInfoType } from '@/types/common';
    import { assignmentCardInfoType, attachedDutyInfoType, dutyRangeInfoType, myTeamShiftInfoType, dutiesDetailInfoType, selectedDutyCardInfoType} from '@/types/DutyRoster';
    import { shiftInfoType } from '@/types/ShiftSchedule';

    @Component({
        components: {
            DutyCard,
            SheriffFuelGauge,
            DutyRosterAssignment,
            SheriffDayView
        }
    })
    export default class DutyRosterDayView extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;

        @commonState.State
        public location!: locationInfoType;

        @dutyState.State
        public displayFuelGauge!: boolean;

        @dutyState.State
        public sheriffFullview!: boolean;

        @dutyState.State
        public printSheriffFullview!: boolean;

        @commonState.State
        public userDetails!: userInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.Action
        public UpdateShiftAvailabilityInfo!: (newShiftAvailabilityInfo: myTeamShiftInfoType[]) => void

        @dutyState.State
        public dutyRosterAssignments!: assignmentCardInfoType[];

        @dutyState.Action
        public UpdateDutyRosterAssignments!: (newDutyRosterAssignments: assignmentCardInfoType[]) => void

        @dutyState.Action
        public UpdateSelectedDuties!: (newSelectedDuties: selectedDutyCardInfoType[]) => void

        @dutyState.State
        public zoomLevel!: number;

        @Prop({required: true})
        runMethod!: any;
        
        isDutyRosterDataMounted = false;
        hasPermissionToAddAssignments = false;

        dutyRostersJson: attachedDutyInfoType[] = [];
        dutyRosterAssignmentsJson;

        errorText=''
		openErrorModal=false;

        windowHeight = 0;
        tableHeight = 0;
        scrollPositions = {scrollDuty:0, scrollGauge:0, scrollTeamMember:0 };

        fields =[
            {key:'assignment', stickyColumn: true, label:'Assignments', thClass:'text-white m-0 p-0', tdClass:'p-0 m-0', thStyle:'background-color: #556077;'},
            {key:'h0', label:'', thClass:'', tdClass:'p-0 m-0', thStyle:'margin:0; padding:0;'}
        ]

        dutyColors = [
            {name:'courtroom',  colorCode:'#189fd4'},
            {name:'court',      colorCode:'#189fd4'},
            {name:'jail' ,      colorCode:'#A22BB9'},
            {name:'transport',  colorCode:'#ffb007'},
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

        @Watch('displayFuelGauge')
        footerChange() 
        {
            Vue.nextTick(() => this.calculateTableHeight() )
        }

        @Watch('sheriffFullview')
        SheriffFullViewChanged() 
        {  
            Vue.nextTick(() => this.scrollAdjustment() );
        }

        @Watch('printSheriffFullview')
        printSheriffFullviewChanged(){
            Vue.nextTick(() => this.scrollAdjustment() );
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
            const footerHeight = 0//document.getElementById("footer")?.offsetHeight || 0;
            const gaugeHeight = (document.getElementsByClassName("fixed-bottom")[0] as HTMLElement)?.offsetHeight || 0;
            const bottomHeight = !this.displayFuelGauge ? footerHeight : gaugeHeight;
            this.tableHeight = (topHeaderHeight + bottomHeight + secondHeader)
        }

        get dutyColorsCode(){
            return this.dutyColors.filter(color=>(color.name!='courtroom' && color.name!='free'))
        }

        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
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

            this.UpdateSelectedDuties([])

            Vue.nextTick(() => {
                this.extractTeamShiftInfo(shiftsData);                        
                this.extractAssignmentsInfo(this.dutyRosterAssignmentsJson);                 
            })
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

        public getShifts(){
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
                shiftInfo.sheriffId = shiftJson.sheriffId;
                shiftInfo.locationId = shiftJson.locationId;
                shiftInfo.overtimeHours = shiftJson.overtimeHours
                const rangeBin = this.getTimeRangeBins(shiftInfo.startDate, shiftInfo.endDate, this.location.timezone);

                const dutySlots = allDutySlots.filter(dutyslot=>{if(dutyslot.sheriffId==shiftInfo.sheriffId)return true})

                let duties = Array(96).fill(0)
                const dutiesDetail: dutiesDetailInfoType[] = [];
                const shiftArray = this.fillInArray(Array(96).fill(0), 1 , rangeBin.startBin,rangeBin.endBin)
                for(const duty of dutySlots){

                    const color = this.getType(duty.assignmentLookupCode.type)
                    const dutyRangeBin = this.getTimeRangeBins(duty.startDate, duty.endDate, this.location.timezone);
                    const dutyArray = this.fillInArray(Array(96).fill(0), 1 , dutyRangeBin.startBin,dutyRangeBin.endBin)


                    if( this.sumOfArrayElements(this.unionArrays(dutyArray,shiftArray))>0){

                        if(shiftInfo.overtimeHours>0){
                            const dutyRosterIndex = this.dutyRostersJson.findIndex(dutyroster=>{if(dutyroster.id == duty.dutyId)return true}) 
                            const dutySlotIndex = this.dutyRostersJson[dutyRosterIndex].dutySlots.findIndex(dutyslot=>{if(dutyslot.id == duty.id)return true})
                            
                            this.dutyRostersJson[dutyRosterIndex].dutySlots[dutySlotIndex].isOvertime = true
                        }                        

                        dutiesDetail.push({
                            id:duty.id , 
                            startBin:dutyRangeBin.startBin, 
                            endBin: dutyRangeBin.endBin,
                            name: color.name,
                            colorCode: color.colorCode,
                            color: (duty.isOvertime||shiftInfo.overtimeHours>0)? this.dutyColors[5].colorCode:color.colorCode,
                            type: duty.assignmentLookupCode.type,
                            code: duty.assignmentLookupCode.code
                        })

                        duties = this.fillInArray(duties, duty.id , dutyRangeBin.startBin,dutyRangeBin.endBin)
                    }
                }

                const index = this.shiftAvailabilityInfo.findIndex(shift => shift.sheriffId == shiftInfo.sheriffId)
                
                if (index != -1) {
                    let availability = this.shiftAvailabilityInfo[index].availability
                    if(shiftInfo.overtimeHours==0)
                        availability = this.fillInArray(this.shiftAvailabilityInfo[index].availability, shiftJson.id , rangeBin.startBin,rangeBin.endBin)
                    
                    const newavailability = this.subtractUnionOfArrays(availability, duties);
                    availability = this.unionArrays(availability, newavailability);
                    this.shiftAvailabilityInfo[index].duties = duties;
                    this.shiftAvailabilityInfo[index].availability = availability;
                    this.shiftAvailabilityInfo[index].shifts.push(shiftInfo);
                    this.shiftAvailabilityInfo[index].dutiesDetail.push(...dutiesDetail);
                } else {
                    let availability = Array(96).fill(0);
                    if(shiftInfo.overtimeHours==0)
                        availability = this.fillInArray(Array(96).fill(0), shiftJson.id , rangeBin.startBin,rangeBin.endBin)
                    
                    const newavailability = this.subtractUnionOfArrays(availability, duties);
                    availability = this.unionArrays(availability, newavailability);
                    availabilityInfo.shifts = [shiftInfo];
                    availabilityInfo.sheriffId = shiftJson.sheriff.id;
                    availabilityInfo.badgeNumber = shiftJson.sheriff.badgeNumber;
                    availabilityInfo.firstName = shiftJson.sheriff.firstName;
                    availabilityInfo.lastName = shiftJson.sheriff.lastName;
                    availabilityInfo.rank = ( shiftJson.sheriff.actingRank?.length>0)?  ( shiftJson.sheriff.actingRank[0].rank)+' (A)': shiftJson.sheriff.rank;
                    availabilityInfo.rankOrder = this.getRankOrder(availabilityInfo.rank)[0]?this.getRankOrder(availabilityInfo.rank)[0].id:0;
                    availabilityInfo.availability = availability;
                    availabilityInfo.duties = duties;
                    availabilityInfo.dutiesDetail = dutiesDetail;
                    this.shiftAvailabilityInfo.push(availabilityInfo);
                }
            }
            this.UpdateShiftAvailabilityInfo(this.shiftAvailabilityInfo);           
        }

        public extractAssignmentsInfo(assignments){

            const dutyRosterAssignments: assignmentCardInfoType[] =[]
            let sortOrder = 0;
            for(const assignment of assignments){
                sortOrder++;
                const dutyRostersForThisAssignment: attachedDutyInfoType[] = this.dutyRostersJson.filter(dutyroster=>{if(dutyroster.assignmentId == assignment.id)return true}) 
               
               if(dutyRostersForThisAssignment.length>0){
                    for(const rosterInx in dutyRostersForThisAssignment){
                        dutyRosterAssignments.push({
                            assignment:('00' + sortOrder).slice(-3)+'FTE'+('0'+ rosterInx).slice(-2) ,
                            assignmentDetail: assignment,
                            name:assignment.name,
                            code:assignment.lookupCode.code,
                            type: this.getType(assignment.lookupCode.type),
                            attachedDuty: dutyRostersForThisAssignment[rosterInx],
                            FTEnumber: Number(rosterInx),
                            totalFTE: dutyRostersForThisAssignment.length
                        })
                    }
                }else{                
                    dutyRosterAssignments.push({
                        assignment:('00' + sortOrder).slice(-3)+'FTE00' ,
                        assignmentDetail: assignment,
                        name:assignment.name,
                        code:assignment.lookupCode.code,
                        type: this.getType(assignment.lookupCode.type),
                        attachedDuty: null,
                        FTEnumber: 0,
                        totalFTE: 0
                    })
                }
            }

            this.UpdateDutyRosterAssignments(dutyRosterAssignments)

            this.isDutyRosterDataMounted = true;
            this.$emit('dataready')
            Vue.nextTick(()=>this.scrollAdjustment())
        }

        public scrollAdjustment(){
            this.getWindowHeight();
            const el = document.getElementsByClassName('b-table-sticky-header')                
            const scrollSize = window.innerWidth*0.9173-185

            if(el[0]) el[0].addEventListener("scroll",()=>{
                if(el[1]) el[1].scrollLeft = el[0].scrollLeft
            })

            if(el[0]){
                el[0].scrollLeft = (scrollSize*0.5425);
                el[0].scrollTop = this.scrollPositions.scrollDuty;
            }

            if(el[1]){
                el[1].scrollLeft = (scrollSize*0.5425);

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

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
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
            return this.commonInfo.sheriffRankList.filter(rank => {
                if (rank.name == rankName) {
                    return true;
                }
            })
        }
    }
</script>

<style scoped lang="scss">   

    .card {
        border: white;
    }

    .gauge {
        direction:rtl;
        position: sticky;
    }

    .grid24 {        
        display:grid;
        grid-template-columns: repeat(24, 8.333%);
        grid-template-rows: 1.65rem;
        inline-size: 100%;
        font-size: 10px;
        height: 1.58rem;         
    }

    .grid24 > * {      
        padding: 0.3rem 0;
        border: 1px dotted rgb(185, 143, 143);
        color: white;
        background-color:   #003366;
        font-size: 12px;
    }
    #app-footer-assignment {
        padding: 2px 17px;
        position: absolute;
        position: fixed;
        left: 0;
        bottom: 0;
        z-index: 100;    
    }
</style>

<style lang="scss">
    .modal-backdrop{
        width: 1000vw !important;
        height: 1000vh !important;
    }
</style>