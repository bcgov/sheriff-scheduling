<template>
    <div>
		<b-overlay :show="loadingData" rounded="sm" :opacity="0.8" spinner-small spinner-variant="primary">
			<b-row v-if="assignment.FTEnumber>0"
				@click="editAssignment()" 
				:style="{
					borderTop: '0px solid #BBBBBB',
					borderBottom: getBorderBottom,
					height:'2.925rem',
					backgroundColor: assignment.type.colorCode,
					borderRadius:getBorderRadius,
					margin:'0rem 0.1rem 0 0.1rem'}">
			</b-row>
			<b-row v-else :style="{
				borderTop: '1px solid #BBBBBB',
				borderBottom: getBorderBottom,
				height:'2.785rem',
				backgroundColor:assignment.type.colorCode,
				borderRadius:getBorderRadius,
				margin:'0.15rem 0.1rem 0 0.1rem'}" > 
				<b-col cols="10" class="m-0 p-0 text-white" @click="editAssignment()">                                   
					<b-row                                  
						style="text-transform: capitalize;" 
						class="h6 p-0 mt-2 mb-0 ml-1">
						<div 
							v-b-tooltip.hover                            
							:title="assignmentTitle.length>15? assignmentTitle:''">
							{{assignmentTitle | truncate(12)}} 
						</div>                    
					</b-row>
					<b-row v-if="dutyError" >
						<h6 class="ml-3 my-0 p-0"
							><b-badge v-b-tooltip.hover
								:title="dutyErrorMsg"
								variant="danger"> {{dutyErrorMsg | truncate(14)}}
								<b-icon class="ml-2"
									icon = x-square-fill
									@click="dutyError = false"
						/></b-badge></h6>
					</b-row>     
					<b-row v-else class="h7 p-0 m-0 ml-2">
						<div>
							<b-button  
									class="p-0"
									style="transform:translate(-5px,1px); height:0.9rem;" 
									size="sm"
									variant="light" 
									@click="editAssignment()"
									v-b-tooltip.hover                                
									title="Edit Assignment"
									><b-icon-pencil-square style="transform:translate(0px,-7px);"  font-scale="0.9" variant="dark" />
								</b-button>
						</div>
						<div 
							v-b-tooltip.hover                            
							:title="assignment.name?assignment.name.length>17? assignment.name:'':''"> 
								{{assignment.name | truncate(14)}} 
						</div>                    
					</b-row>
				</b-col>
				<b-col cols="2" class="m-0 p-0"> 
					<b-row class="m-0 p-0">
						<b-button
							class="bg-white"
							style="padding:0; height:1.2rem; width:1.2rem; margin:0.35rem 0"
							:disabled="isDeleted || !hasPermissionToAddAssignDuty" 
							v-b-tooltip.hover.righttop
							:title="getTimeRange"
							@click="addDuty();"
							size="sm"> 
								<b-icon-plus class="text-dark" font-scale="1" style="transform:translate(0,-2px);"/></b-button>
					</b-row>
					<b-row class="m-0 p-0" >
						<div v-if="assignment.assignmentDetail.comment"
							v-b-tooltip.hover.right.v-info
							:title="assignment.assignmentDetail.comment">
							<b-icon-chat-square-text-fill variant="white" font-scale=".8" class="ml-1 mb-2 p-0" style="transform:translate(0,-3px);"/>
						</div>
					</b-row>
					
				</b-col>
			</b-row>
		</b-overlay>

		<b-modal v-model="showEditAssignmentDetails" id="bv-modal-edit-assignment-details" centered header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Editing Assignment </h2>
			</template>

			<b-card v-if="isAssignmentDataMounted" no-body style="font-size: 14px;user-select: none;" >

				<b-card id="AssignmentError" no-body>
					<h2 v-if="assignmentError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="assignmentErrorMsgDesc"
							variant="danger"> {{assignmentErrorMsg}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="assignmentError = false"
					/></b-badge></h2>
				</b-card>

				<b-row class="mx-1 mt-0 mb-2 p-0">
					<b-form-group class="my-auto ml-auto" style="width: 8.6rem">	
						<b-form-checkbox
							size="sm"									
							v-model="nonReoccuring"
							@change="toggleReoccurring">
								Non Reoccuring
						</b-form-checkbox>						
					</b-form-group>
				</b-row>              

				<b-row class="mx-1 my-0 p-0">
					<b-form-group class="mr-1" style="width: 12rem">
						<label class="h6 m-0 p-0">Assignment Category<span class="text-danger">*</span></label>
						<b-form-select 
							size="sm"
							:disabled="true"
							@change="loadSubTypes"
							v-model="assignmentToEditType"
							:state = "selectedTypeState?null:false">
								<b-form-select-option
									v-for="type in assignmentTypeOptions"
									:key="type.name"
									:value="type">
											{{type.label}}
								</b-form-select-option>
						</b-form-select>
					</b-form-group>
					<b-form-group class="ml-4" style="width: 14.35rem">
						<label class="h6 my-0 ml-1 p-0">Assignment Sub category<span class="text-danger">*</span></label>
						<b-form-select 
							size="sm"
							:disabled="true"
							v-model="assignmentToEditSubType.id"
							:state = "selectedSubTypeState?null:false">
								<b-form-select-option
									v-for="subType in assignmentSubTypeOptions"
									:key="subType.id"
									:value="subType.id">
											{{subType.code}}
								</b-form-select-option>
						</b-form-select>
					</b-form-group>
				</b-row>

				<b-row class="mx-1 my-0 p-0">
					<b-form-group class="mr-1" style="width: 12rem">
						<label class="h6 m-0 p-0">Name</label>
						<b-form-input 
						size="sm"
							v-model="assignmentToEdit.name" 
							placeholder="Enter Name" 
							:state = "nameState?null:false">
						</b-form-input>
					</b-form-group>
				</b-row>

				<b-row v-if="nonReoccuring" class="mx-1 my-1 p-0">
					<b-form-group class="mr-1" style="width: 12rem">					
						<label class="h6 m-0 p-0"> From <span class="text-danger">*</span></label>
						<b-form-datepicker
							tabindex="2"
							class="mb-1"
							size="sm"
							v-model="selectedStartDate"
							placeholder="Start Date*"
							:state = "startDateState?null:false"
							:date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
							@context="startDatePicked"
							locale="en-US">
						</b-form-datepicker>
					</b-form-group>
					<b-form-group class="ml-4" style="width: 14.35rem">
						<label class="h6 m-0 p-0"> To<span class="text-danger">*</span></label>
						<b-form-datepicker
							tabindex="3"
							class="mb-1 mt-0 pt-0"
							size="sm"
							v-model="selectedEndDate"
							placeholder="End Date*"
							:state = "endDateState?null:false"                                    
							:date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
							@context="endDatePicked"
							locale="en-US">
						</b-form-datepicker>
					</b-form-group>
				</b-row>

				<b-row class="mx-1 mt-3 mb-0">
					<b-form-group class="bg-light">							
						<b-row>
							<label class="h6 ml-3 mr-5">Days<span class="text-danger">*</span></label>								
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
								<b-form-checkbox
									@change="weekdaysChanged"									
									:class="day.diff? 'ml-2 pl-3' :'ml-1 pl-4' +'align-middle'"
									v-for="day in dayOptions"
									:disabled="!day.enabled"
									:key="day.diff"
									:value="day.diff">
										{{day.name}}
								</b-form-checkbox>
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

					<b-form-group style="width: 7rem;">
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
                            v-model="selectedComment"
                            size="sm"
                            type="text" 
							:formatter="commentFormat"                           
                        ></b-form-input>
                    </b-form-group>                                    
                </b-row>
			</b-card>

			<template v-slot:modal-footer>
				<b-button
						v-if="hasPermissionToExpireAssignment"
						size="sm"
						variant="danger"
						class="mr-auto"
						@click="confirmDeleteAssignment()"
				><b-icon-trash-fill style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-trash-fill>Delete</b-button>
				<b-button
						size="sm"
						variant="secondary"
						@click="closeEditAssignmentWindow()"
				><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
				<b-button
						:disabled="!hasPermissionToEditAssignment"
						size="sm"
						variant="success"
						@click="saveAssignment()"
				><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
			</template>
			<template v-slot:modal-header-close>
				<b-button
					variant="outline-primary"
					class="text-light closeButton"
					@click="closeEditAssignmentWindow()"
					>
					&times;</b-button>
			</template>
		</b-modal>

		<b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            
			<template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Assignment</h2>                    
            </template>

			<b-card id="DeleteError" no-body>
					<h2 v-if="deleteError" class="mx-1 mt-2"
						><b-badge v-b-tooltip.hover
							:title="deleteErrorMsg"
							variant="danger"> {{deleteErrorMsg | truncate(40)}}
							<b-icon class="ml-3"
								icon = x-square-fill
								@click="deleteError = false"
					/></b-badge></h2>
			</b-card>

            <h4>Are you sure you want to delete the "{{assignmentToEdit.name}}" assignment?</h4>
            <b-form-group style="float:left; margin: 0; padding: 0; width: 15rem;"><label class="ml-1">Reason for Deletion:</label> 
                <b-form-select
					tabindex="1"
                    size = "sm"
                    v-model="assignmentDeleteReason">
                        <b-form-select-option value="OPERDEMAND">
                            Cover Operational Demands
                        </b-form-select-option>
                        <b-form-select-option value="PERSONAL">
                            Personal Decision
                        </b-form-select-option>
                        <b-form-select-option value="ENTRYERR">
                            Entry Error
                        </b-form-select-option>     
                </b-form-select>
            </b-form-group>
			<b-form-group style="float:left; margin: 0 0 0 1rem; padding: 0; width: 13rem;">					
				<label class="ml-1"> Expiry Date</label>
				<b-form-datepicker
					tabindex="2"
					class="mb-1"
					size="sm"
					v-model="selectedExipryDate"
					placeholder="Exipry Date*"
					today-button
					:date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
					locale="en-US">
				</b-form-datepicker>
			</b-form-group>
            <template v-slot:modal-footer>
				<b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
                <b-button variant="danger" @click="deleteAssignment()" :disabled="assignmentDeleteReason.length == 0">Confirm</b-button>                
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="cancelDeletion()"
                >&times;</b-button>
            </template>
        </b-modal> 

		<b-modal v-model="showEditCancelWarning" id="bv-modal-edit-assignment-cancel-warning" header-class="bg-warning text-light">            
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Unsaved Assignment </h2>                                 
			</template>
			<p>Are you sure you want to cancel without saving your changes?</p>
			<template v-slot:modal-footer>
				<b-button variant="secondary" @click="$bvModal.hide('bv-modal-edit-assignment-cancel-warning')"                   
				>No</b-button>
				<b-button variant="success" @click="confirmedCloseEditAssignmentWindow()"
				>Yes</b-button>
			</template>            
			<template v-slot:modal-header-close>                 
				<b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-edit-assignment-cancel-warning')"
				>&times;</b-button>
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

    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/DutyRosterInformation";   
	const dutyState = namespace("DutyRosterInformation");

	import "@store/modules/AssignmentScheduleInformation";
	const assignmentState = namespace("AssignmentScheduleInformation");

	import * as _ from 'underscore';
    import { localTimeInfoType, locationInfoType, userInfoType } from '@/types/common';
    import { assignmentCardInfoType, assignmentInfoType, assignmentSubTypeInfoType, dutyRangeInfoType} from '@/types/DutyRoster';

    @Component
    export default class ManageDutyRosterAssignment extends Vue {

        @Prop({required: true})
		assignment!: assignmentCardInfoType;
		
		@Prop({required: true})
		weekview!: boolean;

        @commonState.State
		public location!: locationInfoType;
		
		@commonState.State
        public userDetails!: userInfoType;

        // @dutyState.State
		// public dutyRangeInfo!: dutyRangeInfoType;

		@assignmentState.State
		public assignmentRangeInfo!: dutyRangeInfoType;

		@commonState.State
        public localTime!: localTimeInfoType;
		
		assignmentTitle = '';

        dutyError = false;
        dutyErrorMsg = '';
        showDutyDetails = false;

		selectedStartTime = '';
		selectedEndTime = '';
		selectedStartDate = '';
		selectedEndDate = '';
		assignmentToEditType = {name:'', label:''};
		assignmentToEditSubType = {} as assignmentSubTypeInfoType;
		selectedDays: number[] = [];
		showEditAssignmentDetails = false;
		showEditCancelWarning = false;
		isAssignmentDataMounted = false;
		hasPermissionToEditAssignment = false;
		hasPermissionToExpireAssignment = false;
        hasPermissionToAddAssignDuty = false;
		confirmDelete = false;
		assignmentDeleteReason = '';
		initialLoad = false;
		isSubTypeDataReady = false;
		nonReoccuring = false;
		isDeleted = false;

		selectedExipryDate = ''
		selectedComment = ''

		nameState = true;
		selectedTypeState = true;
		selectedSubTypeState = true;
		startDateState = true;
		endDateState = true;	
		startTimeState = true;
		endTimeState = true;
		selectedDayState = true;		
		allDaysSelected = false;
		weekDaysSelected = false;

		initialStartDate = false;
		initialEndDate = false;
		loadingData = false;

		weekDayNames = ['sunday','monday','tuesday','wednesday','thursday','friday','saturday'];

		dayOptions = [
			{name:'Sun', diff:0, enabled: true},
			{name:'Mon', diff:1, enabled: true},
			{name:'Tue', diff:2, enabled: true},
			{name:'Wed', diff:3, enabled: true},
			{name:'Thu', diff:4, enabled: true},
			{name:'Fri', diff:5, enabled: true},
			{name:'Sat', diff:6, enabled: true},
		]

		assignmentTypeOptions = [
			{name:'CourtRoom', label:'Court Room'},
			{name:'CourtRole', label:'Court Assignment'},
			{name:'JailRole', label:'Jail Assignment'},
			{name:'EscortRun', label:'Transport Assignment'},
			{name:'OtherAssignment', label:'Other Assignment'}
		]

		assignmentSubTypeOptions = [] as assignmentSubTypeInfoType[];
		assignmentError = false;
		assignmentErrorMsg = '';
		assignmentErrorMsgDesc = '';

		assignmentToEdit = {} as assignmentInfoType;
		originalAssignmentToEdit = {} as assignmentInfoType;

		deleteErrorMsg = '';
        deleteErrorMsgDesc = '';
		deleteError = false;

		errorText='';
		openErrorModal=false;

        mounted(){
			this.loadingData = false;
			this.hasPermissionToEditAssignment = this.userDetails.permissions.includes("EditAssignments");
            this.hasPermissionToExpireAssignment = this.userDetails.permissions.includes("ExpireAssignments");    
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");    
			// this.assignmentTitle = Vue.filter('capitalize')(this.assignment.type.name) +'-' + this.assignment.code;
			this.assignmentTitle = Vue.filter('capitalize')(this.assignment.code);
            this.selectedExipryDate = this.localTime.timeString;
			this.isDeleted = this.determineExpired();
		}

		public determineExpired(){
			const currentTime = this.assignmentRangeInfo.endDate
			const currentDay = moment(currentTime).tz(this.location.timezone).day();

			if (this.assignment.assignmentDetail) {
				if (!this.weekview) {
					if (currentDay == 0 && !this.assignment.assignmentDetail.sunday) return true;
					if (currentDay == 1 && !this.assignment.assignmentDetail.monday) return true;
					if (currentDay == 2 && !this.assignment.assignmentDetail.tuesday) return true;
					if (currentDay == 3 && !this.assignment.assignmentDetail.wednesday)	return true;
					if (currentDay == 4 && !this.assignment.assignmentDetail.thursday) return true;
					if (currentDay == 5 && !this.assignment.assignmentDetail.friday) return true;
					if (currentDay == 6 && !this.assignment.assignmentDetail.saturday) return true;
				}
				if (this.assignment.assignmentDetail.expiryDate) {
					const expiryDate = moment(this.assignment.assignmentDetail.expiryDate).tz(this.location.timezone)
					if (expiryDate.isBefore(currentTime)) return true;
					return false;
				} 
			}
			return false;
		}
		
		public confirmDeleteAssignment(){
			this.deleteError = false;
			this.confirmDelete = true;
		}
		
		public cancelDeletion() {
			this.confirmDelete = false;
            this.assignmentDeleteReason = '';
        }

        public deleteAssignment(){
			if (this.assignmentDeleteReason.length) {
				this.confirmDelete = false;
				this.deleteError = false;
				const expDate = moment.tz(this.selectedExipryDate, this.location.timezone).utc().format();
				const url = 'api/assignment?id=' + this.assignmentToEdit.id + '&expiryReason=' + this.assignmentDeleteReason+ '&expiryDate='+expDate;
			
				this.$http.delete(url)
					.then(response => {
						this.confirmDelete = false;
						this.$emit('change', this.scrollPositions());                    
					}, err=>{
						const errMsg = err.response.data.error;
						console.log(err.response)
						this.deleteErrorMsg = errMsg;
						this.deleteError = true;
					});
					this.assignmentDeleteReason = '';
			}			
		}

        public editAssignment(){
			this.isSubTypeDataReady = false;
			this.enableAllDayOptions();
			this.initialLoad = true;
			this.initialStartDate = true;
			this.initialEndDate = true; 
			this.loadAssignmentDetails();					           
		}

		public startDatePicked(){
			if(this.initialStartDate){
				this.initialStartDate=false
			}else if(!this.initialEndDate){
				this.toggleAllDays(false);
				this.toggleWeekDays(false);
				this.selectedDays = [] ;
			this.selectedDays = [] ;            
				this.selectedDays = [] ;
			}           
			if (this.selectedEndDate.length) {
				this.disableOutOfRangeDays();
			}
			
		}

		public endDatePicked(){
			if(this.initialEndDate){
				this.initialEndDate=false
			}else if (!this.initialStartDate) {
				this.toggleAllDays(false);
				this.toggleWeekDays(false);
				this.selectedDays = [] ;
			this.selectedDays = [] ;            
				this.selectedDays = [] ;
			}                 
            if (this.selectedStartDate.length) {
				this.disableOutOfRangeDays();
			}
		}

		public disableOutOfRangeDays() {
			const numberOfDaysInRange = moment(this.selectedEndDate).diff(moment(this.selectedStartDate), 'days');
			if (numberOfDaysInRange < 7) {
				const daysInRange: number[] = [];					

				for (let i = 0; i <= numberOfDaysInRange; i++) {
					const dayOfWeek = moment(this.selectedStartDate).add(i, 'days').day();
					daysInRange.push(dayOfWeek)
				}

				for (let dayOfWeek = 0; dayOfWeek < this.dayOptions.length; dayOfWeek++) {
					if (!daysInRange.includes(dayOfWeek)) {
						this.dayOptions[dayOfWeek].enabled = false;
					} else {
						this.dayOptions[dayOfWeek].enabled = true;
					}
				}
			} else {
				this.enableAllDayOptions();
			}
		}
		
		get selectAllDisabled(){
            for(const day of this.dayOptions)
                if(!day.enabled) return true;
            return false;
        }

        get selectWeekDisabled(){
            for(const day in this.dayOptions)
                if(!this.dayOptions[day].enabled && this.dayOptions[day].diff !=0 && this.dayOptions[day].diff !=6) return true;
            return false;
        }
		
		public loadAssignmentDetails() {

			const assignmentInfo = this.assignment.assignmentDetail;
			this.originalAssignmentToEdit.id = this.assignmentToEdit.id = assignmentInfo.id;
			this.originalAssignmentToEdit.name = this.assignmentToEdit.name = assignmentInfo.name;			
			this.originalAssignmentToEdit.start = this.selectedStartTime = assignmentInfo.start.substring(0,5);
			this.originalAssignmentToEdit.end = this.selectedEndTime = assignmentInfo.end.substring(0,5);
			this.originalAssignmentToEdit.locationId = this.assignmentToEdit.locationId = assignmentInfo.locationId;
			this.originalAssignmentToEdit.timezone = this.assignmentToEdit.timezone = assignmentInfo.timezone;
			this.originalAssignmentToEdit.comment = this.selectedComment = assignmentInfo.comment?assignmentInfo.comment:''

			if (assignmentInfo.adhocStartDate) {
				this.nonReoccuring = true;
				this.selectedStartDate = assignmentInfo.adhocStartDate? assignmentInfo.adhocStartDate: '';
				this.selectedEndDate = assignmentInfo.adhocEndDate? assignmentInfo.adhocEndDate: '';
				this.originalAssignmentToEdit.adhocStartDate = moment.tz(this.selectedStartDate, this.originalAssignmentToEdit.timezone).utc().format();
				this.originalAssignmentToEdit.adhocEndDate = moment.tz(this.selectedEndDate, this.originalAssignmentToEdit.timezone).utc().format();
			} else {
				this.nonReoccuring = false;
				this.selectedStartDate = '';
				this.selectedEndDate = '';
				this.originalAssignmentToEdit.adhocStartDate = null;
				this.originalAssignmentToEdit.adhocEndDate = null;
			}
			this.originalAssignmentToEdit.reoccuring = !this.nonReoccuring;			
			this.originalAssignmentToEdit.sunday = assignmentInfo.sunday;
			this.originalAssignmentToEdit.monday = assignmentInfo.monday;
			this.originalAssignmentToEdit.tuesday = assignmentInfo.tuesday;
			this.originalAssignmentToEdit.wednesday = assignmentInfo.wednesday;
			this.originalAssignmentToEdit.thursday = assignmentInfo.thursday;
			this.originalAssignmentToEdit.friday = assignmentInfo.friday;
			this.originalAssignmentToEdit.saturday = assignmentInfo.saturday;

			if (assignmentInfo.sunday) this.selectedDays.push(0)
			if (assignmentInfo.monday) this.selectedDays.push(1)
			if (assignmentInfo.tuesday) this.selectedDays.push(2)
			if (assignmentInfo.wednesday) this.selectedDays.push(3)
			if (assignmentInfo.thursday) this.selectedDays.push(4)
			if (assignmentInfo.friday) this.selectedDays.push(5)
			if (assignmentInfo.saturday) this.selectedDays.push(6)
			
			const assignmentType = assignmentInfo.lookupCode.type;
			this.assignmentToEditType = this.assignmentTypeOptions.filter(option => {if (option.name == assignmentType) return true})[0];			
			this.assignmentToEdit.type = assignmentType;
			this.originalAssignmentToEdit.type = this.assignmentToEdit.type;
			this.loadSubTypes(this.assignmentToEditType);
		}

		public weekdaysChanged(){
			Vue.nextTick(()=>{
				this.allDaysSelected = this.selectedDays.length==7? true: false
				this.weekDaysSelected = this.selectedDays.includes(1) && this.selectedDays.includes(2) 
										&& this.selectedDays.includes(3) && this.selectedDays.includes(4) 
										&& this.selectedDays.includes(5);
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

		public toggleReoccurring(checked) {			
			this.nonReoccuring = checked;
			if (checked) {
				this.selectedDays = [];				
			} else {
				if (this.originalAssignmentToEdit.sunday) this.selectedDays.push(0)
				if (this.originalAssignmentToEdit.monday) this.selectedDays.push(1)
				if (this.originalAssignmentToEdit.tuesday) this.selectedDays.push(2)
				if (this.originalAssignmentToEdit.wednesday) this.selectedDays.push(3)
				if (this.originalAssignmentToEdit.thursday) this.selectedDays.push(4)
				if (this.originalAssignmentToEdit.friday) this.selectedDays.push(5)
				if (this.originalAssignmentToEdit.saturday) this.selectedDays.push(6)
				this.enableAllDayOptions();
			}
		}
		
		public loadSubTypes(type) {
			Vue.nextTick(()=>{
				this.loadingData=true;
				this.isSubTypeDataReady = false;
				const url = 'api/managetypes?codeType='+ type.name +'&locationId='+this.location.id+'&showExpired=false';
				this.$http.get(url)
					.then(response => {
						if(response.data){
							this.extractSubTypes(response.data);
						}
						this.loadingData=false;
					},err => {
						this.errorText = err.response.statusText+' '+err.response.status + '  - ' + moment().format();
						if (err.response.status != '401') {
							this.openErrorModal=true;
						}
						this.loadingData=false; 
					})
			});
		}

		public extractSubTypes(subTypesJson) {
			this.assignmentSubTypeOptions = [];
			for(const subTypeJson of subTypesJson) {
				const subType = {} as assignmentSubTypeInfoType;
				subType.id = subTypeJson.id;
				subType.code = subTypeJson.code;
				this.assignmentSubTypeOptions.push(subType)
			}
			this.isSubTypeDataReady = true;
			
			if (this.initialLoad) {
				const assignmentInfo = this.assignment.assignmentDetail;
				this.assignmentToEditSubType = {id: assignmentInfo.lookupCode.id, code: assignmentInfo.lookupCode.code}
				this.originalAssignmentToEdit.lookupCodeId = this.assignmentToEdit.lookupCodeId = assignmentInfo.lookupCode.id;
				this.isAssignmentDataMounted = true;
				this.showEditAssignmentDetails = true;
				this.initialLoad = false;
			}
		}

		public saveAssignment() {
			let requiredError = false;
		
			if (!this.assignmentToEditType) {
				this.selectedTypeState = false;
				requiredError = true;
			} else {
				this.selectedTypeState = true;
			}
			if (!this.assignmentToEditSubType.id) {
				this.selectedSubTypeState = false;
				requiredError = true;
			} else {
				this.selectedSubTypeState = true;
			}
			if (this.nonReoccuring && !this.selectedStartDate) {
				this.startDateState = false;
				requiredError = true;
			} else {
				this.startDateState = true;
			}
			if (this.nonReoccuring && !this.selectedEndDate) {
				this.endDateState = false;
				requiredError = true;
			} else {				
				this.endDateState = true;
			}
			if (!this.selectedStartTime) {
				this.startTimeState = false;
				requiredError = true;
			} else {
				this.selectedStartTime = Vue.filter('autoCompleteTime')(this.selectedStartTime);					
				this.startTimeState = true;
			}
			if (!this.selectedEndTime) {
				this.endTimeState = false;
				requiredError = true;
			} else {
				this.selectedEndTime = Vue.filter('autoCompleteTime')(this.selectedEndTime);					
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
					this.saveAssignmentChanges();
			} else {
					console.log('Error required')
			}
		}

		
		public isChanged(){
			this.readEditedAssignment();
			return !_.isEqual(this.originalAssignmentToEdit, this.assignmentToEdit);
		}

		public closeEditAssignmentWindow(){
			if(this.isChanged())
                this.showEditCancelWarning = true;
            else
                this.confirmedCloseEditAssignmentWindow();
		}

		public confirmedCloseEditAssignmentWindow(){
            this.resetAssignmentWindowState();
            this.showEditCancelWarning = false;
			this.showEditAssignmentDetails = false;
        }

		public resetAssignmentWindowState() {
			this.assignmentToEdit = {} as assignmentInfoType;
			this.ClearFormState();
		}

		public ClearFormState(){
			this.allDaysSelected = false;
			this.weekDaysSelected = false;
			this.assignmentToEdit = {} as assignmentInfoType;
            this.selectedStartTime ='';
			this.selectedEndTime = '';
			this.selectedStartDate = '';
			this.selectedEndDate = '';
			this.assignmentToEditType = {name:'', label:''}
			this.assignmentToEditSubType = {} as assignmentSubTypeInfoType;
			this.selectedDays = [] ;
			this.nameState = true;	
			this.selectedTypeState = true;
			this.selectedSubTypeState = true;
			this.selectedDayState = true;
			this.startTimeState = true;
			this.endTimeState = true;
			this.startDateState = true;
			this.endDateState = true;
			this.assignmentError = false;
            this.assignmentErrorMsg = '';
			this.assignmentErrorMsgDesc = '';
			this.nonReoccuring = false;
			this.enableAllDayOptions();
		}

		public enableAllDayOptions() {
			for (let dayOfWeek = 0; dayOfWeek < this.dayOptions.length; dayOfWeek++) {
				this.dayOptions[dayOfWeek].enabled = true;
			}
		}

		public readEditedAssignment() {

			this.assignmentToEdit.sunday = this.selectedDays.includes(0)
			this.assignmentToEdit.monday = this.selectedDays.includes(1)
			this.assignmentToEdit.tuesday = this.selectedDays.includes(2)
			this.assignmentToEdit.wednesday = this.selectedDays.includes(3)
			this.assignmentToEdit.thursday = this.selectedDays.includes(4)
			this.assignmentToEdit.friday = this.selectedDays.includes(5)
			this.assignmentToEdit.saturday = this.selectedDays.includes(6);

			if (this.nonReoccuring) {
				this.assignmentToEdit.reoccuring = false;
				this.assignmentToEdit.adhocStartDate = moment.tz(this.selectedStartDate, this.location.timezone).utc().format();
				this.assignmentToEdit.adhocEndDate = moment.tz(this.selectedEndDate, this.location.timezone).utc().format();
			} else {
				this.assignmentToEdit.reoccuring = true;
				this.assignmentToEdit.adhocStartDate = null;
				this.assignmentToEdit.adhocEndDate = null;
			}

			this.assignmentToEdit.start = this.selectedStartTime;
			this.assignmentToEdit.end = this.selectedEndTime;
			this.assignmentToEdit.comment = this.selectedComment;	
		}

		public saveAssignmentChanges() {
			this.readEditedAssignment();
			const body = this.assignmentToEdit;
			const url = 'api/assignment';
			this.$http.put(url, body )
				.then(response => {
					if(response.data){
							this.confirmedCloseEditAssignmentWindow();
							this.$emit('change', this.scrollPositions());
					}
				}, err => {
					const errMsg = err.response.data;
					this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
					this.assignmentErrorMsgDesc = errMsg;
					this.assignmentError = true;
				})
		}

        public addDuty(){
            this.createDuty();
        }

        public createDuty(){
			this.dutyError = false;

			const date = this.assignmentRangeInfo.startDate.substring(0,10);
			const startDate = moment.tz(date+'T'+this.assignment.assignmentDetail.start, this.location.timezone).utc();
			const endDate = moment.tz(date+'T'+this.assignment.assignmentDetail.end, this.location.timezone).utc();

			const body: any[] = [];

			if(this.weekview){
				for(const dayIndexStr in this.weekDayNames){
					const day = this.weekDayNames[dayIndexStr];
					const dayIndex = Number(dayIndexStr)
					const newStartDate = moment(startDate).add(dayIndex,'days').format();
					const newEndDate = moment(endDate).add(dayIndex,'days').format();

					if(this.assignment.assignmentDetail[day]){				
						body.push({
							startDate: moment.tz(newStartDate, this.location.timezone).utc().format(), 
							endDate: moment.tz(newEndDate, this.location.timezone).utc().format(),
							locationId: this.location.id,
							assignmentId: this.assignment.assignmentDetail.id,
							timezone: this.location.timezone,
							concurrencyToken: 0
						})						
					}
				}
			}else{			
				
				body.push({
					startDate: startDate.format(), 
					endDate: endDate.format(),
					locationId: this.location.id,
					assignmentId: this.assignment.assignmentDetail.id,
					timezone: this.location.timezone,
					concurrencyToken: 0
				})			
			}
			this.loadingData = true;
			const url = 'api/dutyroster';
			this.$http.post(url, body )
				.then(response => {
					if(response.data){
							this.$emit('change', this.scrollPositions());
					}
					this.loadingData = false;
				}, err => {
					this.dutyErrorMsg = err.response.data.error;
					this.dutyError = true;
					this.loadingData = false;
				})
		}		
		
		get getBorderRadius(){
			if(this.assignment.totalFTE<2) return '5px'
			else if(this.assignment.FTEnumber == 0) return'5px 5px 0 0'
			else if(this.assignment.FTEnumber == (this.assignment.totalFTE-1))	return'0 0 5px 5px'
			else return '0'
		}

		get getBorderBottom(){
			if(this.assignment.totalFTE<2) return '1px solid #BBBBBB'
			else if(this.assignment.FTEnumber == 0) return'0px solid #BBBBBB'
			else if(this.assignment.FTEnumber == (this.assignment.totalFTE-1))	return'1px solid #BBBBBB'
			else return '0px solid #BBBBBB'
		}

		get getTimeRange(){
			const days: string[]= []
			for(const day of this.weekDayNames) if(this.assignment.assignmentDetail[day]) days.push(day[0].toUpperCase()+day.slice(1))
			return this.assignment.assignmentDetail.start.substring(0,5)+' - '+this.assignment.assignmentDetail.end.substring(0,5)+' '+ days.join(', ')
		}

		public timeFormat(value , event){
            return Vue.filter('timeFormat')(value , event)
        }

		public commentFormat(value) {
			return value.slice(0,100);
		}

		public scrollPositions(){
            const el = document.getElementsByClassName('b-table-sticky-header')
            const scrollDuty = el[0]? el[0].scrollTop : 0;
            const scrollGauge = el[1]? el[1].scrollTop : 0;

            const eltm = document.getElementById('dutyrosterteammember');
            const scrollTeamMember = eltm? eltm.scrollTop : 0;

            return {scrollDuty: scrollDuty, scrollGauge: scrollGauge, scrollTeamMember:scrollTeamMember }
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
        grid-template-rows: 2.56rem;
        inline-size: 100%;
        font-size: 10px;         
    }

    .grid24 > * {      
        padding: .75rem 0;
        border: 1px dotted rgb(185, 143, 143);
    }

</style>