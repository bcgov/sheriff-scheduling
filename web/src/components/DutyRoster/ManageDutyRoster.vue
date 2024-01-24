<template>
    <b-card bg-variant="white" class="home" no-body>
        <b-row  class="mx-0 mt-0 mb-0 p-0" cols="2" >
            <b-col  class="m-0 p-0" :cols="(courtAdminFullview && !weekView)? 12: 11" >
                <duty-roster-header v-on:change="reloadDutyRosters" :runMethod="headerAddAssignment" />
                <duty-roster-week-view :runMethod="dutyRosterWeekViewMethods" v-if="weekView" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()" />
                <duty-roster-day-view id="duty-pdf" :runMethod="dutyRosterDayViewMethods" v-if="!weekView&&headerReady" :key="updateDutyRoster" v-on:addAssignmentClicked="addAssignment" v-on:dataready="reloadMyTeam()"/>
                
            </b-col>
            <b-col v-if="!courtAdminFullview || weekView" class="p-0 " cols="1"  style="overflow: auto;">
                <b-card
                    v-if="isDutyRosterDataMounted"
                    :key="updateMyTeam"                     
                    body-class="mx-2 p-0"
                    style="overflow-x: hidden;"
                    class="bg-dark m-0 p-0 no-top-rounding">
                    <div id="myTeamHeader" class="mb-2">
                        <b-card-header header-class="m-0 text-white py-2 px-0"> 
                            My Team
                            <b-button
                                v-if="!weekView"
                                @click="toggleDisplayMyteam()"
                                v-b-tooltip.hover                            
                                title="Display Graphical Availability of MyTeam "                            
                                style="font-size:10px; width:1.1rem; margin:0 0 0 .2rem; padding:0; background-color:white; color:#189fd4;" 
                                size="sm">
                                    <b-icon-bar-chart-steps /> 
                            </b-button>                           
                        </b-card-header>
                        <duty-roster-team-member-card :courtAdminInfo="memberNotRequired" :weekView="weekView"/>
                        <duty-roster-team-member-card :courtAdminInfo="memberNotAvailable" :weekView="weekView"/>
                        <duty-roster-team-member-card :courtAdminInfo="memberIsClosed" :weekView="weekView"/>  
                    </div>                   
                    <div id="dutyrosterteammember" :style="{overflowX: 'hidden', overflowY: 'auto', height: getHeight}">
                        <duty-roster-team-member-card v-on:change="updateDutyRosterPage()" v-for="member in sortedShiftAvailabilityInfo" :key="member.courtAdminId" :courtAdminInfo="member" :weekView="weekView"/>
                    </div>
                </b-card>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import DutyRosterHeader from './components/DutyRosterHeader.vue'
    import DutyRosterTeamMemberCard from './components/DutyRosterTeamMemberCard.vue'
    import * as _ from 'underscore';
    
    import DutyRosterDayView from './DutyRosterDayView.vue';
    import DutyRosterWeekView from './DutyRosterWeekView.vue'

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import { localTimeInfoType, commonInfoType } from '@/types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType} from '@/types/DutyRoster';
    
    @Component({
        components: {
            DutyRosterHeader,
            DutyRosterTeamMemberCard,
            DutyRosterDayView,
            DutyRosterWeekView
        }
    })
    export default class ManageDutyRoster extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;

        @commonState.State
        public localTime!: localTimeInfoType;

        @commonState.Action
        public UpdateLocalTime!: (newLocalTime: localTimeInfoType) => void

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @dutyState.State
        public displayFuelGauge!: boolean;

        @dutyState.Action
        public UpdateDisplayFuelGauge!: (newDisplayFuelGauge: boolean) => void

        @dutyState.State
        public courtAdminFullview!: boolean;
        

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.State
        public zoomLevel!: number;

        @dutyState.Action
		public UpdateZoomLevel!: (newZoomLevel: number) => void;

        memberNotRequired = { courtAdminId: '00000-00000-11111' } as myTeamShiftInfoType;
        memberNotAvailable = { courtAdminId: '00000-00000-22222' } as myTeamShiftInfoType;
        memberIsClosed = { courtAdminId: '00000-00000-33333' } as myTeamShiftInfoType;
        
        isDutyRosterDataMounted = false;
        updateDutyRoster = 0;
        updateMyTeam = 0;

        weekView = false;
        headerReady = false;
        windowHeight = 0;
        bottomHeight = 0;
        gaugeHeight = 0;
        tableHeight = 0;

        maxRank = 1000;

        timeHandle1
        timeHandle2

        headerAddAssignment = new Vue();         
        dutyRosterDayViewMethods = new Vue();
        dutyRosterWeekViewMethods = new Vue();

        @Watch('zoomLevel')
        zoomLevelChange() 
        {   
            Vue.nextTick(() =>this.getWindowHeight())
        }

        @Watch('displayFuelGauge')
        footerChange() 
        {
            Vue.nextTick(() => this.calculateTableHeight())
        }

        mounted()
        {
            this.maxRank = this.commonInfo.courtAdminRankList.reduce((max, rank) => rank.id > max ? rank.id : max, this.commonInfo.courtAdminRankList[0].id);
            this.isDutyRosterDataMounted = false;
            this.timeHandle1 = window.setTimeout(this.updateCurrentTimeCallBack, 1000);
            window.addEventListener('resize', this.getWindowHeight);
            this.getWindowHeight()
        }


        beforeDestroy() {
            window.removeEventListener('resize', this.getWindowHeight);
            clearTimeout(this.timeHandle1);
            clearTimeout(this.timeHandle2);
            document.body.style.zoom = "100%"
            this.UpdateZoomLevel(100)
        }
        
        public reloadDutyRosters(type){
            this.isDutyRosterDataMounted = false;
            // console.log(type)
            // console.log('reload dutyroster')                
            this.updateCurrentTime();
            if(type=='Day' && this.courtAdminFullview){
                this.weekView = false

            }else if(type=='Day'){
                this.weekView = false
            } else{
                this.weekView = true
                this.UpdateDisplayFuelGauge(false)
            }

            this.headerReady = true;
            this.updateDutyRoster++;
        }

        public reloadMyTeam(){            
            this.isDutyRosterDataMounted=true
            Vue.nextTick(()=>this.calculateTableHeight())
            this.updateMyTeam++;
        }

        public getWindowHeight() {
            this.windowHeight = Math.ceil(100*document.documentElement.clientHeight/this.zoomLevel);
            this.calculateTableHeight();  
        }

        get getHeight() {
            return this.windowHeight - this.tableHeight + 'px';
        }

        public calculateTableHeight() {
            const topHeaderHeight = (document.getElementsByClassName("app-header")[0] as HTMLElement)?.offsetHeight || 0;
            const myTeamHeader =  document.getElementById("myTeamHeader")?.offsetHeight || 0;
            const footerHeight = 0//document.getElementById("footer")?.offsetHeight || 0;
            this.gaugeHeight = (document.getElementsByClassName("fixed-bottom")[0] as HTMLElement)?.offsetHeight || 0;
            this.bottomHeight = !this.displayFuelGauge ? footerHeight : this.gaugeHeight;
            // console.log('My Team - Window: ' + this.windowHeight)
            // console.log('My Team - Top: ' + topHeaderHeight)
            // console.log('My Team - TeamHeader: ' + myTeamHeader)
            // console.log('My Team - BottomHeight: ' + this.bottomHeight)
            // console.log('My Team - New height: ' + (this.windowHeight - topHeaderHeight - myTeamHeader - this.bottomHeight))
            this.tableHeight = (topHeaderHeight + myTeamHeader + this.bottomHeight+8)
        }

        public toggleDisplayMyteam(){
            if(!this.displayFuelGauge){
                this.UpdateDisplayFuelGauge(true)
                const el = document.getElementsByClassName('b-table-sticky-header') 
                Vue.nextTick(()=>{            
                    if(el[1]) el[1].scrollLeft = el[0].scrollLeft
                })
            }
            else this.UpdateDisplayFuelGauge(false)
        } 
        
        public addAssignment(){ 
            this.headerAddAssignment.$emit('addassign');
        }

        public updateCurrentTime() {
            const currentTime = moment();
            const startOfDay = moment(currentTime).startOf('day')
            const timeBin = (moment.duration(currentTime.diff(startOfDay)).asMinutes()/15 +0.5)            
            const currentTimeObject = { 
                timeString: currentTime.format(), 
                timeSlot: Math.floor(timeBin),                
                dayOfWeek: currentTime.weekday(),
                isTodayInView: currentTime.isBetween(this.dutyRangeInfo.startDate, this.dutyRangeInfo.endDate),
            }
            //console.log(currentTimeObject)
            this.UpdateLocalTime(currentTimeObject);
        }

        public updateCurrentTimeCallBack() {
            this.updateCurrentTime();
            this.timeHandle2 = window.setTimeout(this.updateCurrentTimeCallBack, 60000);
        }

        public updateDutyRosterPage() {
            if (!this.weekView) {
                this.dutyRosterDayViewMethods.$emit('getData');
            } else {
                this.dutyRosterWeekViewMethods.$emit('getData');
            }
        }

        get sortedShiftAvailabilityInfo() {
            const teamList = this.shiftAvailabilityInfo        
            return _.chain(teamList)
                    .sortBy(member =>{return (member['lastName']? member['lastName'].toUpperCase() : '')})
                    .sortBy(member =>{return (member['rankOrder']? member['rankOrder'] : this.maxRank + 1)})
                    .value()
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

    .grid24 {        
        display:grid;
        grid-template-columns: repeat(24, 4.1666%);
        grid-template-rows: 1.65rem;
        inline-size: 100%;
        font-size: 10px;
        height: 1.58rem;         
    }

    .grid24 > * {      
        padding: 0.3rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>