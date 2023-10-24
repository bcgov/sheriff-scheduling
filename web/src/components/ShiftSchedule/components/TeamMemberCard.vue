<template>
    <div v-if="isDataMounted">    
        <b-row 
            style="width:100%; height:3rem;" 
            bg-variant="white"            
            class="ml-2 my-0 p-0">
                <b-col cols="9" @click="openMemberDetails(courtAdminInfo.courtAdminId)" class="team-member-view">
                    <b-row style="font-size:11px; line-height: 16px;"># {{courtAdminInfo.badgeNumber}}</b-row>
                    <b-row style="font-size:9px; line-height: 14px;">{{courtAdminInfo.rank}}</b-row>
                    <b-row 
                        style="font-size:12px; line-height: 16px; font-weight: bold; text-transform: Capitalize;" 
                        v-b-tooltip.hover.topleft                                
                        :title="fullName.length>13?fullName:''">
                            {{fullName|truncate(11)}}
                    </b-row>
                </b-col>
                <b-col cols="3" class="m-0 p-0">                    
                    <b-button
                        v-if="hasPermissionToCreateShifts" 
                        style="margin:.7rem 0 0 0; padding:0;"                   
                        size="sm" 
                        variant="success" 
                        @click="AddShift()">
                            <b-icon-plus/>
                    </b-button>                     
                    <b-row
                        style="margin:0.1rem 0 0 0 ; padding:0;">
                        <div
                            style="height:1rem;"                       
                            v-if="LoanedInDesc"
                            v-b-tooltip.hover.topleft                                
                            :title="LoanedInDesc"> 
                                <b-icon-box-arrow-in-right style="transform:translate(0,-5px)" variant="jail" /> 
                        </div>
                    </b-row>                  
                </b-col>
        </b-row>

        <b-modal  v-model="showShiftDetails" id="bv-modal-shift-details" centered header-class="bg-primary text-light">
            <template v-slot:modal-title>                
                <span class="m-0 p-0" > 
                    <h3 class="m-0 p-0" >Creating Shift for </h3>
                    <h4  class="m-0 pt-2 pb-0 text-warning" style="text-align: center"> {{courtAdminInfo.firstName}} {{courtAdminInfo.lastName}}</h4>
                </span>
            </template>

            <b-card v-if="isShiftDataMounted" no-body style="font-size: 14px; user-select: none;" >
                
                <b-row v-if="shiftError" id="ShiftError" class="h3 mx-2">
                    <b-badge class="mx-1 mt-2"
                        v-b-tooltip.hover
                        :title="shiftErrorMsgDesc"
                        variant="danger"> {{shiftErrorMsg}}
                        <b-icon class="ml-3"
                            icon = x-square-fill
                            @click="shiftError = false"
                    /></b-badge>                    
                </b-row> 

                <b-row class="mx-0 my-3">
                    <b-form-group class="bg-light">
                        <b-row> 
                            <label class="h6" style="margin:0.2rem 0 0 1.1rem" >Days<span class="text-danger">*</span></label>  
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-jail"									
                                v-model="weekDaysSelected"
                                :disabled="selectWeekDisabled"
                                @change="toggleWeekDays">
                                    Select Week Days
                            </b-form-checkbox>                       
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-court"									
                                v-model="allDaysSelected"
                                :disabled="selectAllDisabled"
                                @change="toggleAllDays">
                                    Select All
                            </b-form-checkbox>
                        </b-row>
                        <b-form-checkbox-group
                            size="sm"			
                            v-model="selectedDays"								
                            :state = "selectedDayState?null:false">
                            <b-td
                                v-for="day in dayOptions"
                                :key="day.diff">
                                <b-tr style="float:left">
                                    <b-td class="px-0"><conflicts-icon  :conflictsInfo="day.conflicts.Unavailable" type="Unavailable" :index="day.diff" /></b-td>
                                    <b-td class="pr-0"><conflicts-icon  :conflictsInfo="day.conflicts.AllShifts" type="Shift" :index="day.diff" /></b-td>
                                    <b-td class="px-0"><conflicts-icon  :conflictsInfo="day.conflicts.Loaned" type="Loaned" :index="day.diff" /></b-td>
                                    <b-td class="px-0"><conflicts-icon  :conflictsInfo="day.conflicts.Leave" type="Leave" :index="day.diff" /></b-td>                    
                                    <b-td class="px-0"><conflicts-icon  :conflictsInfo="day.conflicts.Training" type="Training" :index="day.diff" /></b-td>                                
                                </b-tr>
                                <b-tr>
                                    <b-td colspan="3" >
                                        <b-form-checkbox style="width:3.0rem;"
                                            @change="weekdaysChanged"                                            					
                                            :disabled="day.fullday"                                            
                                            :value="day.diff">
                                            <span class="h6 font-weight-normal m-0 p-0">{{day.name}}</span>
                                        </b-form-checkbox>
                                    </b-td>
                                </b-tr>
                            </b-td>
                        </b-form-checkbox-group>
                    </b-form-group>
                </b-row>

                <b-row class="mx-auto my-0 p-0">
                    <b-form-group class="mr-3" style="width: 7rem">
                        <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedStartTime"
                            @click="startTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "startTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>

                    <b-form-group class="m-0" style="width: 7rem;">
                        <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedEndTime"
                            @click="endTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "endTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>                
                </b-row>
                <b-row class="mx-auto my-0 p-0">
                    <b-form-group class="m-0" style="width: 28.5rem">
                        <label class="h6 m-0 p-0">Comment</label>
                        <b-form-input
                            v-model="comment"
                            size="sm"
                            type="text"
                            :formatter="commentFormat"                            
                        ></b-form-input>
                    </b-form-group>                                    
                </b-row>
            </b-card>

            <template v-slot:modal-footer>
                <b-button
                        variant="secondary"
                        size="sm"
                        @click="closeShiftWindow()"
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button
                        variant="success"
                        size="sm"
                        @click="saveShift()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>
            <template v-slot:modal-header-close>
                <b-button
                    variant="outline-primary"
                    class="text-light closeButton"
                    @click="closeShiftWindow()"
                    >
                    &times;</b-button>
            </template>
		</b-modal>

        <b-modal v-model="showCancelWarning" id="bv-modal-shift-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light"> Unsaved New Shifts </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-shift-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseShiftWindow()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-shift-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>

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
                                        v-on:profileUpdated="getCourtAdmins()"
                                        v-on:enableSave="enableSave()"   
                                        v-on:changeTab="changeTab"                                     
                                        :createMode="false" 
                                        :editMode="true" />
                                </b-tab>

                                <b-tab title="Locations"> 
                                    <location-tab 
                                        v-on:change="getCourtAdmins()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                                <b-tab title="Leaves">
                                    <leave-tab 
                                        v-on:change="getCourtAdmins()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                    
                                </b-tab>

                                <b-tab title="Training"> 
                                    <training-tab
                                        v-on:refresh="refreshProfile"
                                        v-on:change="getCourtAdmins()"/>
                                </b-tab>

                                <b-tab v-if="hasPermissionToAssignRoles" title="Roles" class="p-0">
                                    <role-assignment-tab  v-on:change="getCourtAdmins()"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>
                                </b-tab>

                                <b-tab title="Acting Rank"> 
                                    <rank-tab 
                                        v-on:change="getCourtAdmins()"
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
    import moment from 'moment-timezone';

    import "@store/modules/ShiftScheduleInformation";
    const shiftState = namespace("ShiftScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");

    import ConflictsIcon from './ConflictsIcon.vue'
    import LocationTab from '@/components/MyTeam/Tabs/LocationTab.vue';
    import LeaveTab from '@/components/MyTeam/Tabs/LeaveTab.vue';
    import TrainingTab from '@/components/MyTeam/Tabs/TrainingTab.vue';
    import RoleAssignmentTab from '@/components/MyTeam/Tabs/RoleAssignmentTab.vue';
    import IdentificationTab from '@/components/MyTeam/Tabs/IdentificationTab.vue';
    import RankTab from '@/components/MyTeam/Tabs/RankTab.vue'
    import UserSummaryTemplate from "@/components/MyTeam/Tabs/UserSummaryTemplate.vue";

    import { dayOptionsInfoType, editedShiftInfoType, courtAdminAvailabilityInfoType,shiftInfoType,shiftRangeInfoType } from '@/types/ShiftSchedule';
    import { locationInfoType, userInfoType } from '@/types/common';
    import { teamMemberInfoType } from '@/types/MyTeam';

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            ConflictsIcon,
            RoleAssignmentTab,
            UserSummaryTemplate,
            IdentificationTab,
            LocationTab,
            LeaveTab,
            TrainingTab,
            RankTab
        }
    })
    export default class TeamMemberCard extends Vue {

        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @Prop({required: true})
        public courtAdminInfo!: courtAdminAvailabilityInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        identificationTabMethods = new Vue();

        courtAdminId = '';

        isDataMounted = false;
        hasPermissionToCreateShifts = false; 
        hasPermissionToEditUsers = false;
        hasPermissionToAssignRoles = false;
        fullName = '';

        halfUnavailStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), white);"
        halfUnavailHalfSchStyle="background-image: linear-gradient(to bottom right, rgb(194, 39, 28),rgb(243, 232, 232), rgb(12, 120, 170));"
        halfSchStyle="background-image: linear-gradient(to bottom right, rgb(12, 120, 170),rgb(243, 232, 232), white);"
        WeekDay = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

		selectedStartTime = '';
        selectedEndTime = '';
        comment = '';
		showShiftDetails = false;
		isShiftDataMounted = false;
		shift = {} as shiftInfoType;
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;

		selectedDays: number[] = [];
        allDaysSelected = false;
        weekDaysSelected = false;

        dayOptions: dayOptionsInfoType[] = [];

        showMemberDetails = false;

        tabIndex = 0;
        isUserDataMounted = false;        
        saving = false;
        sectionHeader = '';
        photokey = 0;
        
        newTabIndex = 0;
        firstNavigation = true;

		shiftError = false;
		shiftErrorMsg = '';
        shiftErrorMsgDesc = '';
        
        showCancelWarning = false;
        LoanedInDesc = '';

        errorText='';
		openErrorModal=false;

        
        mounted()
        {  
            this.isDataMounted = false;
            this.hasPermissionToCreateShifts = this.userDetails.permissions.includes("CreateAndAssignShifts");        
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");
            this.hasPermissionToAssignRoles = this.userDetails.permissions.includes("CreateAndAssignRoles");
            this.courtAdminId = this.courtAdminInfo.courtAdminId;          
            this.fullName = this.courtAdminInfo.lastName +', '+this.courtAdminInfo.firstName;

            this.dayOptions = [
                {name:'Sun', diff:0, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Mon', diff:1, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Tue', diff:2, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Wed', diff:3, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Thu', diff:4, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Fri', diff:5, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}},
                {name:'Sat', diff:6, fullday:false, conflicts:{Training: [], Leave: [], Loaned:[], AllShifts:[], Shift:[], overTimeShift:[], Unavailable:[]}}
            ];        
            this.extractConflicts();
        }

        public extractConflicts() {
            this.LoanedInDesc = '';
            if(this.courtAdminInfo.homeLocation.id != this.location.id) this.LoanedInDesc =  "Loaned In from " + this.courtAdminInfo.homeLocation.name
                      
            for(const conflict of this.courtAdminInfo.conflicts){
                this.dayOptions[conflict.dayOffset].conflicts[conflict.type].push(conflict); 
                this.dayOptions[conflict.dayOffset].fullday = this.dayOptions[conflict.dayOffset].fullday || conflict.fullday               
            }

            for(const dayOptionInx in this.dayOptions){
                this.dayOptions[dayOptionInx].conflicts.AllShifts.push(...this.dayOptions[dayOptionInx].conflicts.Shift);
                this.dayOptions[dayOptionInx].conflicts.AllShifts.push(...this.dayOptions[dayOptionInx].conflicts.overTimeShift);
            }
            
            Vue.nextTick(()=>{this.isDataMounted = true;})                     
        }

        get selectAllDisabled(){
            for(const day of this.dayOptions)
                if(day.fullday) return true;
            return false;
        }

        get selectWeekDisabled(){
            for(const day in this.dayOptions)
                if(this.dayOptions[day].fullday && this.dayOptions[day].diff !=0 && this.dayOptions[day].diff !=6) return true;
            return false;
        }

        public weekdaysChanged(){
			Vue.nextTick(()=>{
                this.allDaysSelected = this.selectedDays.length==7? true: false
                this.weekDaysSelected = this.selectedDays.includes(1) && this.selectedDays.includes(2) && this.selectedDays.includes(3) && this.selectedDays.includes(4) && this.selectedDays.includes(5) 
			})
		}

		public toggleAllDays(checked) {
            this.weekDaysSelected = checked ? true: false;
			this.selectedDays = checked ? [0,1,2,3,4,5,6] : [];
        }
        
        public toggleWeekDays(checked) {
            this.allDaysSelected = false;
			this.selectedDays = checked ? [1,2,3,4,5] : [];
		}

		public AddShift() {
			//console.log('adding shift')
			this.isShiftDataMounted = true;
            this.showShiftDetails = true;
        }
        
        public saveShift() {
            //console.log('saving')            
            this.shiftError = false;
			let requiredError = false;
			
			if (!this.selectedStartTime) {
					this.startTimeState = false;
					requiredError = true;
			} else {
                    this.selectedStartTime = this.autoCompleteTime(this.selectedStartTime);
					this.startTimeState = true;
			}
			if (!this.selectedEndTime) {
					this.endTimeState = false;
					requiredError = true;
			} else {
                    this.selectedEndTime = this.autoCompleteTime(this.selectedEndTime);
					this.endTimeState = true;
            }
            if (this.selectedStartTime && this.selectedEndTime && this.selectedStartTime >= this.selectedEndTime ) {
                    this.startTimeState = false;
                    this.endTimeState = false;
					requiredError = true;
            } 
            
			if (this.selectedDays.length < 1) {
					this.selectedDayState = false;
					requiredError = true;
			} else {
					this.selectedDayState = true;
			}

			if (!requiredError) {
                    this.startTimeState = true;
                    this.endTimeState = true;
					this.createShift();
			} else {
					console.log('Error required')
			}
        }
        
        public autoCompleteTime(time){
            const tail="00:00";
            let result = "";
            
            if(time.length==1) result = '0'+time+ tail.slice(2);
            else if(time.length==4) {
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result = time+(time.slice(-1)=='1' || time.slice(-1)=='4'?'5':'0');
            }
            else if(time.length==5){
                if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
                else result =time.slice(0,4)+(time.slice(3,4)=='1' || time.slice(3,4)=='4'?'5':'0');
            }
            else  result = time+ tail.slice(time.length);
            return result
        }

        public isChanged(){
            if( this.selectedStartTime ||
                this.selectedEndTime ||
                this.selectedDays.length >0) return true;
            return false;           
        }

		public closeShiftWindow(){
			if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseShiftWindow();
        }
        
        public confirmedCloseShiftWindow(){
            this.resetShiftWindowState();
            this.showCancelWarning = false;
			this.showShiftDetails = false;
        } 

		public resetShiftWindowState() {
            this.shift = {} as shiftInfoType;
            this.ClearFormState();
		}

		public ClearFormState(){
            this.allDaysSelected = false;
            this.weekDaysSelected = false;
            this.selectedStartTime ='';
            this.selectedEndTime = '';
            this.selectedDays = [] ;				
            this.selectedDayState = true;
            this.startTimeState = true;
            this.endTimeState = true;
            this.shiftError = false;
            this.shiftErrorMsg = '';
            this.shiftErrorMsgDesc = '';
            this.LoanedInDesc = '';
            this.comment = '';
        }
        
        public getListOfDates(){
            const listOfDates: editedShiftInfoType[] = [];   
			for(const day of this.dayOptions) {
                if(this.selectedDays.includes(day.diff)){

                    const startTime = this.completeDate(day.diff, this.selectedStartTime);
                    const endTime = this.completeDate(day.diff, this.selectedEndTime);
                    const shifts = [{start:startTime, end:endTime}]
                    if(shifts.length==0){
                        this.shiftErrorMsg = 'Too many shift conflicts on '+day.name +'.';
                        this.shiftErrorMsgDesc = '';
                        this.shiftError = true;
                        return
                    }

                    for(const shift of shifts){
                        if(shift.start>=shift.end)continue;
                        const editedShift = {
                            id: 0,
                            startDate: moment(shift.start).utc().format(),
                            endDate: moment(shift.end).utc().format(),
                            courtAdminId: this.courtAdminId,
                            locationId: this.location.id,
                            timezone: this.location.timezone
                        }
                        if(this.comment) editedShift['comment'] = this.comment;				
                        listOfDates.push(editedShift)
                    }
                }                
			}
			return listOfDates;
        }      

        public roundTime(time, floor){
            const minutes = moment(time).minutes()
            let minOffset = 0
            if(minutes%15 == 0){
                return time;
            } 

            if(minutes/15 >= 3) minOffset= floor? 45-minutes: 60-minutes;
            else if(minutes/15 >= 2) minOffset= floor? 30-minutes: 45-minutes;
            else if(minutes/15 >= 1) minOffset= floor? 15-minutes: 30-minutes;
            else minOffset= floor? 0-minutes: 15-minutes;
            return moment(time).add(minOffset,'minutes').format()           
        }

        public completeDate(offset, time){
            const startOfdate = moment(this.shiftRangeInfo.startDate).add(offset,'days').format().substring(0,10);
            return(moment.tz(startOfdate + 'T'+time, this.location.timezone).format()); 
        }

		public createShift() {
            const body = this.getListOfDates();
            if(!body || (body && body.length==0)) return
            const url = 'api/shift';
            this.$http.post(url, body )
                .then(response => {
                    if(response.data){
                        this.resetShiftWindowState();
                        this.closeShiftWindow;
                        this.$emit('change');
                    }
                }, err => {
                    const errMsg = err.response.data.error;
                    this.shiftErrorMsg = errMsg.slice(0,45) + (errMsg.length>45?' ...':'');
                    this.shiftErrorMsgDesc = errMsg;
                    this.shiftError = true;
                })
		}
           
        public timeFormat(value , event) {
			if(isNaN(Number(value.slice(-1))) && value.slice(-1) != ':') return value.slice(0,-1)
			if(value.length!=3 && value.slice(-1) == ':') return value.slice(0,-1);

			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==0 || value.slice(-1)==3)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'0';
			if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==1 || value.slice(-1)==4)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'5';
			if(value.length==2 && event.data && value.slice(0,1)==2 && value.slice(-1)>=5) return value.slice(0,-1);
			if(value.length==2 && event.data && value.slice(0,1)==2 && (value.slice(-1)==4 || value.slice(-1)==4)) return '02:45';
			if(value.length==2 && event.data && value.slice(-1)!=0 && value.slice(-1)!=1 && value.slice(-1)!=3 && value.slice(-1)!=4) return value.slice(0,2)+':';
			if(value.length==2 && event.data) return '0'+value.slice(0,1)+':'+value.slice(1,2);
			if(value.length==3 && value.slice(-1)!=':' ) return value.slice(0,2)+':';
			if(value.length==4 && event.data && (value.slice(0,1)>0||value.slice(1,2)>1) && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==0 || value.slice(-1)==3)) return value.slice(0,4)+'0';
			if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==1 || value.slice(-1)==4)) return value.slice(0,4)+'5';
			if(value.length==4 && event.data && value.slice(0,1)==0 && value.slice(1,2)<2 && value.slice(-1)>=5 ) return value.slice(1,2)+value.slice(3,4)+':';
			if(value.length==5 && (value.slice(0,2)>=24 || value.slice(3,5)>=60)) return '';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==0 || value.slice(3,4)==3)) return value.slice(0,4)+'0';
			if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==1 || value.slice(3,4)==4)) return value.slice(0,4)+'5';
			if(value.length==6 && value.slice(0,1)==0 && (value.slice(4,6)==0||value.slice(4,6)==15||value.slice(4,6)==30||value.slice(4,6)==45) && (value.slice(1,2)+value.slice(3,4))<24) return value.slice(1,2)+value.slice(3,4)+':'+value.slice(4,5)+value.slice(5,6);
		
			if(value.length>5) return value.slice(0,5);
			if(value.length==5 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,5)) || value.slice(2,3)!=':') )return '';
			if(value.length==4 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,4)) || value.slice(2,3)!=':') )return '';
			return value
        }
        
        public commentFormat(value) {
			return value.slice(0,100);
        }

        public getCourtAdmins() {
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
            const url = 'api/courtAdmin/' + userId;
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

            user.leave = userJson.leave;
            user.training = userJson.training;
            user.userRoles = userJson.roles
            user.actingRank = userJson.actingRank;
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

    .team-member-view:hover{
        cursor: pointer;
        background-image: linear-gradient(to bottom right, rgb(200, 207, 91),rgb(243, 232, 232), white);     
    }

</style>