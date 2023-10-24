<template>
    <div>
        <div 
            v-if="isDataMounted"
            :id="'member-'+sheriffId"
            :draggable="hasPermissionToAddAssignDuty" 
            v-on:dragstart="DragStart" 
            style="border-radius:5px"          
            :class="bgcolor+' p-1'">
                <b-col v-if="!specialMember" @click="openMemberDetails(sheriffId)" class="team-member-view">
                    <div style="font-size:11px; line-height: 16px;"># {{sheriffInfo.badgeNumber}}</div>
                    <div style="font-size:9px; line-height: 14px;">{{sheriffInfo.rank}}</div>
                    <div 
                        style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                        v-b-tooltip.hover.noninteractive                               
                        :title="fullName.length>13?fullName:''">
                            {{fullName|truncate(11)}}
                    </div>

                    <b-row v-if="!weekView">
                        <b-badge v-b-tooltip.hover.noninteractive.v-warning.html="allShifts" class="mx-auto mt-1">{{shifts[0]}}<span v-if="shifts.length>1"> ...</span></b-badge>
                    </b-row>

                    <b-row v-else style="margin:0; padding:0; font-size:10px;">
                        <b-badge 
                            class="week-view-badge"
                            v-for="sch in sheriffSchedules"                            
                            :key="sch.weekday"
                            :id="(sheriffModal?'court-admin-modal-sch':'sch')+sheriffId+'-'+sch.weekday"
                            :variant="sch.variant" 
                            :style="sch.style" >
                                {{sch.text}}                            
                        

                            <b-tooltip v-if="sch.shifts.length>0" noninteractive :target="(sheriffModal?'court-admin-modal-sch':'sch')+sheriffId+'-'+sch.weekday" variant="warning" show.sync ="true" triggers="hover" placement="topright">
                                <h2 class="text-danger  mb-1 mx-0 p-0">{{sch.name}}</h2>
                                <b-card bg-variant="dark" header-class="text-warning m-0 p-0" body-class="m-0 p-0" header="Sheriff Shifts:">             
                                    <b-table  
                                        :items="sch.shifts"
                                        :fields="shiftFields"
                                        sort-by="startDate"                
                                        borderless
                                        striped
                                        small 
                                        responsive="sm"
                                        class="my-0 py-0 text-white"
                                        >
                                        <template v-slot:cell(startDate)="data" >
                                            <span :class="data.item.overtimeHours?'text-danger':''"  >{{data.value | beautify-time}}</span> 
                                        </template>
                                        <template v-slot:cell(endDate)="data" >
                                            <span :class="data.item.overtimeHours?'text-danger':''" >{{data.value | beautify-time}}</span> 
                                        </template>
                                    </b-table>
                                </b-card> 
                                
                                <b-card v-if="sch.duties.length>0" class="mt-1" bg-variant="dark" header-class="text-warning m-0 p-0" body-class="m-0 p-0" header="Assigned Duties:">
                               
                                    <b-table  
                                        :items="sch.duties"
                                        :fields="dutyFields"
                                        sort-by="startTime"                
                                        borderless
                                        striped
                                        small 
                                        responsive="sm"
                                        class="my-0 py-0"
                                        >
                                        <template v-slot:cell(startTime)="data" >
                                            <span :class="data.item.color=='#e85a0e'?'text-danger':''" >{{data.value | beautify-time}}</span> 
                                        </template>
                                        <template v-slot:cell(endTime)="data" >
                                            <span :class="data.item.color=='#e85a0e'?'text-danger':''" >{{data.value | beautify-time}}</span> 
                                        </template>
                                        <template v-slot:cell()="data" >
                                            <span :class="data.item.color=='#e85a0e'?'text-danger':''" >{{data.value}}</span> 
                                        </template>
                                    </b-table>     
                                </b-card>                                   
                            </b-tooltip>
                        </b-badge>                                 
                    </b-row>

                </b-col>
                <b-col class="m-0 p-0" v-else>
                    <div class="text-center" style="font-size:13px;">{{fullName}}</div>
                </b-col>
        </div>

        <b-modal size="xl" v-model="showMemberDetails" id="bv-modal-team-member-details" header-class="bg-primary text-light">            
            <template v-slot:modal-title>                
                 <h2 class="mb-0 text-light"> Updating User Profile </h2>
            </template>
            <b-card v-if="isUserDataMounted" no-body style="user-select: none;">
                <b-row>
                    <b-col cols="3">
                        <user-summary-template v-on:photoChange="photoChanged" :user="userToEdit" :editMode="true"/>
                    </b-col>
                    <b-col cols="9">
                        <b-card no-body >
                            <b-tabs  card v-model="tabIndex" @activate-tab="onTabChanged">
                                <b-tab title="Identification">
                                    <identification-tab 
                                        :runMethod="identificationTabMethods"                                         
                                        v-on:closeMemberDetails="closeMemberDetailWindow()" 
                                        v-on:profileUpdated="getSheriffs()"
                                        v-on:enableSave="enableSave()"   
                                        v-on:changeTab="changeTab"                                     
                                        :createMode="false" 
                                        :editMode="true" />
                                </b-tab>

                                <b-tab title="Locations"> 
                                    <location-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                                <b-tab title="Leaves">
                                    <leave-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                    
                                </b-tab>

                                <b-tab title="Training"> 
                                    <training-tab
                                        v-on:refresh="refreshProfile"
                                        v-on:change="getSheriffs()"/>
                                </b-tab>

                                <b-tab v-if="hasPermissionToAssignRoles" title="Roles" class="p-0">
                                    <role-assignment-tab  v-on:change="getSheriffs()"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>
                                </b-tab>

                                <b-tab title="Acting Rank"> 
                                    <rank-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                            </b-tabs>
                        </b-card>
                    </b-col>
                </b-row>        
            </b-card>
            <template v-slot:modal-footer>
                <b-button
                    variant="secondary" 
                    @click="closeProfileWindow()"                  
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>{{getCancelLabel}}</b-button>
                <b-button     
                    v-if="tabIndex<1"
                    variant="success"
                    :disabled="!hasPermissionToEditUsers || saving" 
                    @click="saveMemberProfile()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button
                  variant="outline-primary"
                  class="text-light closeButton"
                  @click="closeProfileWindow()"                  
                  >
                  &times;</b-button>
            </template>           
        </b-modal>

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
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");
    
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    import LocationTab from '@/components/MyTeam/Tabs/LocationTab.vue';
    import LeaveTab from '@/components/MyTeam/Tabs/LeaveTab.vue';
    import TrainingTab from '@/components/MyTeam/Tabs/TrainingTab.vue';
    import RoleAssignmentTab from '@/components/MyTeam/Tabs/RoleAssignmentTab.vue';
    import IdentificationTab from '@/components/MyTeam/Tabs/IdentificationTab.vue';
    import RankTab from '@/components/MyTeam/Tabs/RankTab.vue'
    import UserSummaryTemplate from "@/components/MyTeam/Tabs/UserSummaryTemplate.vue";
    
    import { locationInfoType, userInfoType } from '@/types/common';
    import { dutyRangeInfoType, myTeamShiftInfoType } from '@/types/DutyRoster';
    import { teamMemberInfoType } from '@/types/MyTeam';

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            RoleAssignmentTab,
            UserSummaryTemplate,
            IdentificationTab,
            LocationTab,
            LeaveTab,
            TrainingTab,
            RankTab
        }
    })
    export default class DutyRosterTeamMemberCard extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @dutyState.State
        public dutyRangeInfo!: dutyRangeInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        @Prop({required: true})
        public sheriffInfo!: myTeamShiftInfoType;

        @Prop({required: true})
        public weekView!: boolean;

        @Prop({required: false, default:false})
        public sheriffModal!: boolean;

        identificationTabMethods = new Vue();

        sheriffId = '';
        isDataMounted = false;
        hasPermissionToAddAssignDuty = false;
        hasPermissionToEditUsers = false;
        hasPermissionToAssignRoles = false;
        fullName = '';
        shifts: string[] = [];
        allShifts = {title:''};

        showMemberDetails = false;

        errorText='';
		openErrorModal=false;

        tabIndex = 0;
        isUserDataMounted = false;        
        saving = false;
        sectionHeader = '';
        photokey = 0;
        
        newTabIndex = 0;
        firstNavigation = true;

        specialMember = false;
        bgcolor='bg-white mb-2';

        shiftFields = [
            {key:'startDate', label:'Start', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'endDate', label:'End', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''}
        ]

        dutyFields = [
            {key:'name', label:'Type', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'code', label:'Name', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'startTime', label:'Start', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''},
            {key:'endTime', label:'End', thClass:'text-info m-0 p-0', tdClass:'text-white p-0 m-0', thStyle:''}
        ]

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];
        weekDayNames = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];

        sheriffSchedules: any[] = [];

        mounted()
        {
            this.isDataMounted = false;
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");
            this.hasPermissionToAssignRoles = this.userDetails.permissions.includes("CreateAndAssignRoles");
            this.sheriffId = this.sheriffInfo.sheriffId; 
            if(this.sheriffId== '00000-00000-11111'){
                this.fullName = 'Not Required'
                this.bgcolor='bg-success text-white mb-2'
                this.specialMember = true
            } else if(this.sheriffId== '00000-00000-22222'){
                this.fullName = 'Not Available'
                this.bgcolor='bg-danger text-white mb-2'
                this.specialMember = true
            }else if(this.sheriffId== '00000-00000-33333'){
                this.fullName = 'Closed'
                this.bgcolor='bg-bright text-dark h2 my-2 py-2'
                this.specialMember = true
            }else{      
                this.fullName = this.sheriffInfo.lastName +', '+this.sheriffInfo.firstName;
                this.bgcolor='bg-white mb-2'
                for(let dayOffset=0; dayOffset<7; dayOffset++){
                    const date= moment(this.dutyRangeInfo.startDate).add(dayOffset,'days').format()
                    this.sheriffSchedules.push({
                        date: date, 
                        weekday: dayOffset, 
                        text:this.WeekDay[dayOffset],
                        name:this. weekDayNames[dayOffset], 
                        variant:'white',  
                        style:'', 
                        shifts:[],
                        duties:[]
                    })
                }
                if (this.weekView) {
                    this.extractSchedules();
                } else {
                    this.extractShifts();                    
                }                
            }
            this.isDataMounted = true;
        }

        public extractSchedules() {
            // if(this.sheriffInfo.firstName=='Alex'){
                //console.warn(this.sheriffInfo)
                //console.log(this.sheriffSchedules)
            // }
            for(const scheduleIndex in this.sheriffSchedules) {

                const schedule = this.sheriffSchedules[scheduleIndex];            
                
                const filteredShifts = this.sheriffInfo.shifts.filter(shift=>{if(shift.startDate.substring(0,10)==schedule.date.substring(0,10))return true;});
                const filteredDuties = this.sheriffInfo.dutiesDetail.filter(duty=>{if(duty.startTime && duty.startTime.substring(0,10)==schedule.date.substring(0,10))return true;});
                if (filteredShifts.length == 0) {
                    this.sheriffSchedules[scheduleIndex].variant = 'danger';
                } else {
                    this.sheriffSchedules[scheduleIndex].shifts = filteredShifts 
                    this.sheriffSchedules[scheduleIndex].duties = filteredDuties
                    if(filteredDuties.length>0) {
                         
                        let availability = this.getAvailability(filteredShifts, schedule.date)
                        const duties = this.getSheriffDuties(filteredDuties) 
                        availability = this.subtractUnionOfArrays(availability,duties) 
                        //console.log(availability)
                        //console.log(duties)
                        this.sheriffSchedules[scheduleIndex].variant = 'info';
                        if(this.sumOfArrayElements(availability)==0)
                            this.sheriffSchedules[scheduleIndex].style =''
                        else 
                            this.sheriffSchedules[scheduleIndex].style = this.halfSchStyle;
                    }
                }
                
            }
            // console.log(this.sheriffSchedules)
        }

        public extractShifts() {
            //console.log(this.sheriffInfo)
            this.shifts = [];
            let tooltipTitle = '<div>';
            const sortedShifts = _.sortBy(this.sheriffInfo.shifts,'startDate');
            for(const shift of sortedShifts){

                this.shifts.push(shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16))
                tooltipTitle += (shift.overtimeHours>0)? ' <div style=\'color:red;\'> ':'<div>'
                tooltipTitle += shift.startDate.substring(11,16) +' - '+shift.endDate.substring(11,16)
                tooltipTitle +='<br/>'
                tooltipTitle +='</div>'
            }

            tooltipTitle += '</div>'
            this.allShifts.title = tooltipTitle ;
        }

        public DragStart(event: any) 
        { 
            event.dataTransfer.setData('text', event.target.id);
        }

        public getAvailability(shifts, startOfDay){
            
            //console.log(shifts)
            let availability = Array(96).fill(0)
            for(const shift of shifts){
                if(shift.overtimeHours==0){
                    const rangeBin = this.getTimeRangeBins(shift.startDate, shift.endDate, startOfDay, this.location.timezone);
                    //console.log(rangeBin)
                    availability = this.fillInArray(availability, shift.id , rangeBin.startBin,rangeBin.endBin);
                }
            }
            return availability            
        }

        public getSheriffDuties(dutiesDetail){
    
                //console.log(dutiesDetail)
                let duties = Array(96).fill(0)
                for(const duty of dutiesDetail){                    
                    //console.log(duty)
                    duties = this.fillInArray(duties, duty.id , duty.startBin, duty.endBin)
                }
                return duties           
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getTimeRangeBins(startDate, endDate, startOfDay, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*(arrayB[index]>0?0:1)});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public getSheriffs() {
            this.$emit('change');            
        }
        
        public refreshProfile(userId){
            this.closeProfileWindow();
            this.openMemberDetails(userId);
        }      

        public openMemberDetails(userId){
            this.loadUserDetails(userId);
        }

        public loadUserDetails(userId): void {
            this.resetProfileWindowState();       
            const url = 'api/sheriff/' + userId;
            this.$http.get(url)
                .then(response => {
                    if(response.data){                                              
                        this.extractUserInfo(response.data);
                        this.isUserDataMounted = true;
                        this.showMemberDetails=true;                                              
                    }                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }   
                });
        }

        public extractUserInfo(userJson): void {            
            const user = {} as teamMemberInfoType;            
            user.idirUserName =  userJson.idirName;
            user.firstName = userJson.firstName;
            user.lastName = userJson.lastName;
            user.fullName = Vue.filter('capitalizefirst')(userJson.firstName) + ' ' + Vue.filter('capitalizefirst')(userJson.lastName);
            user.gender = gender[userJson.gender];
            user.rank = userJson.rank;
            user.email = userJson.email;
            user.badgeNumber = userJson.badgeNumber;
            user.id = userJson.id;
            user.homeLocationId = userJson.homeLocationId;
            user.homeLocationNm = userJson.homeLocation? userJson.homeLocation.name: '';
            user.image = userJson['photoUrl']?userJson['photoUrl']:'';
            if(userJson.homeLocation)
                user.homeLocation  = {id: userJson.homeLocation.id, name: userJson.homeLocation.name, regionId: userJson.homeLocation.regionId, timezone: userJson.homeLocation.timezone};
          
            if(userJson.awayLocation && userJson.awayLocation.length>0)
                user.awayLocation = userJson.awayLocation;

            user.actingRank = userJson.actingRank;
            user.leave = userJson.leave;
            user.training = userJson.training;
            user.userRoles = userJson.roles
            this.UpdateUserToEdit(user);  
        }

        public saveMemberProfile() {
            this.saving = true; 
            this.identificationTabMethods.$emit('saveMemberProfile');
        }
        
        public enableSave() {
            this.saving = false;
        }

        public closeProfileWindow(){            
            if(this.tabIndex ==0)
            {  
                this.identificationTabMethods.$emit('closeProfileWindow');
            }
            else 
                this.closeMemberDetailWindow()
        }

        public closeMemberDetailWindow(){            
            this.showMemberDetails = false;           
        }

        public resetProfileWindowState(){
            this.tabIndex = 0;
            const user = {} as teamMemberInfoType;
            this.UpdateUserToEdit(user);  
        }

        get getCancelLabel(){
            if(this.tabIndex<1) return 'Cancel'; else return 'Close'
        }

        public photoChanged(id: string, image: string){   
            console.log('photo changed')        
        }

        public onTabChanged(newTabIndex , prevTabIndex, bvEvt) {
            this.newTabIndex = newTabIndex;            
            if(prevTabIndex == 0 && this.firstNavigation) {
                this.firstNavigation = false;
                bvEvt.preventDefault();
                this.identificationTabMethods.$emit('switchTab');   
            }            
        }

        public changeTab(changingTab) {
            if(changingTab) this.tabIndex = this.newTabIndex;
            Vue.nextTick().then(()=>{this.firstNavigation = true;});
        }



    }
</script>

<style scoped>   

    .card {
       border: white;
    }

    .custom-control-input{
        background-color: darkorange;
    }

    .tooltip >>> .tooltip-inner{
        max-width: 500px !important;
        width: 400px !important;
    } 

    .week-view-badge {
        width: 0.75rem;
        height: 0.9rem;
        margin:0.5rem .04rem 0 .04rem;
        padding: 0.15rem 0 0 0;
        border: solid 1px rgb(53, 56, 53); 
        border-radius: 5px;
        /* background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);*/        
    }
    .week-view-badge:hover{
        border: 2px solid rgb(255, 208, 0);
       
    }

    .team-member-view {
        padding: 0 0 0 0;
        border: 0 0 0 0;
    }
    
    .team-member-view:hover{
        cursor: pointer;
        background-image: linear-gradient(to bottom right, rgb(200, 207, 91),rgb(243, 232, 232), white);
    }

</style>