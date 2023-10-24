<template>
	<div>
		<header id="dutyRosterNav" variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">                
				<b-navbar-nav>
					<h3 style="width:11rem; margin-bottom: 0px;" class="text-white ml-2 mr-auto font-weight-normal">Duty Roster</h3>
				</b-navbar-nav>
				<b-navbar-nav v-if="activetab =='Day'">
					<b-tabs nav-wrapper-class = "bg-primary text-dark"
							active-nav-item-class="text-uppercase font-weight-bold text-warning bg-primary"                     
							pills							
							no-body
							class="mx-3"> 
						<b-tab 
							v-for="(tabMapping, index) in tabsMyTeamToggle" 
							:key="'toggle-myteam-'+index"                 
							:title="tabMapping"
							v-on:click="tabMyTeamChanged(tabMapping)" 
							v-bind:class="[ activeMyTeamTab === tabMapping ? 'active p-0 my-0' : 'p-0 my-0' ]"
							/>
					</b-tabs>
				</b-navbar-nav>
				<b-navbar-nav>
					<b-button class="mr-1 text-white" @click="zoomInOut(10)" variant="transparant"><b-icon-zoom-in /></b-button>
					<b-button class="text-white" @click="zoomInOut(-10)" variant="transparant"><b-icon-zoom-out /></b-button>
				</b-navbar-nav>
				<b-navbar-nav v-if="activetab!='Day'">
					<h3 style="width:8rem; margin-bottom: 0px;" class="text-white ml-2 mr-auto font-weight-normal"></h3>
				</b-navbar-nav>
				<b-navbar-nav :class="{'custom-navbar':true, 'full-view':courtAdminFullview}">
                    <b-col class="my-1">
                        <b-row :style="activetab=='Day'?'width:17.75rem':'width:25.6rem'">
                            <b-button style=" height: 2rem;" size="sm" variant="secondary" @click="previousDateRange" class="my-0 mx-1"><b-icon-chevron-left /></b-button>
                            
							<div v-if="activetab=='Day'" class="m-0 bg-white" style="padding:0.2rem 0.52rem; border-radius:3px; font-weight:bold;">{{selectedDate|beautify-date-weekday}}</div>
							<div v-else class="m-0 p-1 bg-white" style=" border-radius:3px; font-weight:bold;">{{selectedDateBegin|beautify-date-weekday}} - {{selectedDateEnd|beautify-date-weekday}}</div>
						
							<b-form-datepicker
                                    class="my-0 py-0 mx-1"
                                    size = "sm"
                                    style="height: 2rem;"
                                    v-model = "selectedDate"
                                    @context = "dateChanged"
                                    @shown = "datePickerOpened = true"
                                    @hidden = "datePickerOpened = false"
                                    button-only
									today-button
									close-button
                                    locale="en-US">
                            </b-form-datepicker>
                            <b-button style="height: 2rem;" size="sm" variant="secondary" @click="nextDateRange" class="my-0"><b-icon-chevron-right/></b-button>
                        </b-row>
                        
                    </b-col>
                </b-navbar-nav>
				<b-navbar-nav v-if="activetab!='Day'" >
					<b-tabs nav-wrapper-class = "bg-primary text-dark"
							active-nav-item-class="text-uppercase font-weight-bold text-warning bg-primary"                     
							pills							
							no-body
							class="mx-3">
						<b-tab 
							v-for="(tabMapping, index) in tabs12h24h" 
							:key="'tab-24h-'+index"                 
							:title="tabMapping"
							v-on:click="tab12h24hChanged(tabMapping)" 
							v-bind:class="[ active24htab === tabMapping ? 'active p-0 my-0' : 'p-0 my-0' ]"
							/>
					</b-tabs>
				</b-navbar-nav>
				<b-navbar-nav v-if="!courtAdminFullview">
					<b-tabs nav-wrapper-class = "bg-primary text-dark my-1 p-0"
							active-nav-item-class="text-uppercase font-weight-bold text-warning bg-primary"                     
							pills							
							no-body
							class="mx-3">
						<b-tab 
							v-for="(tabMapping, index) in tabs" 
							:key="'tab-day'+index"                 
							:title="tabMapping"                 
							v-on:click="tabChanged(tabMapping)" 
							v-bind:class="[ activetab === tabMapping ? 'active p-0 my-0' : 'p-0 my-0' ]"
							/>
					</b-tabs>
				</b-navbar-nav>
				<b-navbar-nav v-if="courtAdminFullview">
					<b-button
						v-b-tooltip.hover.noninteractive
						title="Print CourtAdmin Duties"							
						style="max-height: 40px;" 
						size="sm"
						variant="white"						
						@click="printCourtAdminDuties()" 
						class="my-1 mr-3">
						<b-icon icon="printer-fill" font-scale="2.0" variant="white"/>
					</b-button>						
				</b-navbar-nav>
			</b-navbar>
		</header>


		<b-modal v-model="showAssignmentDetails" id="bv-modal-assignment-details" centered header-class="bg-primary text-light">
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Creating Assignment </h2>
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
							@change="loadSubTypes"
							v-model="assignment.type"
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
							:disabled="!isSubTypeDataReady"
							v-model="assignment.lookupCodeId"
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
							v-model="assignment.name" 
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
						size="sm"
						variant="secondary"
						@click="closeAssignmentWindow()"
				><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
				<b-button
						size="sm"
						variant="success"
						@click="saveAssignment()"
				><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
			</template>
			<template v-slot:modal-header-close>
				<b-button
					variant="outline-primary"
					class="text-light closeButton"
					@click="closeAssignmentWindow()"
					>
					&times;</b-button>
			</template>
		</b-modal>

		<b-modal v-model="showCancelWarning" id="bv-modal-assignment-cancel-warning" header-class="bg-warning text-light">            
			<template v-slot:modal-title>
				<h2 class="mb-0 text-light"> Unsaved New Assignments </h2>                                 
			</template>
			<p>Are you sure you want to cancel without saving your changes?</p>
			<template v-slot:modal-footer>
				<b-button variant="secondary" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"                   
				>No</b-button>
				<b-button variant="success" @click="confirmedCloseAssignmentWindow()"
				>Yes</b-button>
			</template>            
			<template v-slot:modal-header-close>                 
				<b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"
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
 
	import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
	import moment from 'moment-timezone';	
	import { namespace } from "vuex-class";   

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");
	
    import { locationInfoType, userInfoType } from '@/types/common';
    import { assignmentInfoType, assignmentSubTypeInfoType, dutyRangeInfoType} from '@/types/DutyRoster';
	
	@Component
	export default class DutyRosterHeader extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @dutyState.Action
		public UpdateDutyRangeInfo!: (newDutyRangeInfo: dutyRangeInfoType) => void
		
		@dutyState.Action
        public UpdateView24h!: (newView24h: boolean) => void

		@commonState.State
		public userDetails!: userInfoType;

		@dutyState.State
        public courtAdminFullview!: boolean;
		
		@dutyState.Action
        public UpdateDisplayFuelGauge!: (newDisplayFuelGauge: boolean) => void
        
        @dutyState.Action
        public UpdateCourtAdminFullview!: (newCourtAdminFullview) => void

		@dutyState.Action
        public UpdatePrintCourtAdminFullview!: (newPrintCourtAdminFullview: boolean) => void;

		@dutyState.State
        public zoomLevel!: number;

		@dutyState.Action
		public UpdateZoomLevel!: (newZoomLevel: number) => void;
		
		@Prop({required: true})
		runMethod!: any

		active24htab = '12h';
		tabs12h24h = ['12h','24h'];

		activetab = 'Day';
		tabs =['Day', 'Week']

		activeMyTeamTab = 'Assignments View'
		tabsMyTeamToggle = ['Assignments View', 'My Team View']
		
		selectedDate = '';
		selectedDateBegin = '';
		selectedDateEnd = '';
		datePickerOpened = false;
		userIsAdmin = false;

		selectedStartTime = '';
		selectedEndTime = '';
		selectedStartDate = '';
		selectedEndDate = '';
		selectedDays: number[] = [];
		showAssignmentDetails = false;
		showCancelWarning = false;
		isAssignmentDataMounted = false;
		isSubTypeDataReady = false;
		nonReoccuring = false;

		selectedComment = '';

		errorText=''
		openErrorModal=false;

		
		assignment = {} as assignmentInfoType;

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

        mounted() {
			this.runMethod.$on('addassign', this.addAssignment)			
			this.selectedDate = moment().format().substring(0,10);			
			this.loadNewDateRange();
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

		public toggleReoccurring(checked) {
			this.nonReoccuring = checked;
			if (checked) {
				this.selectedDays = [];				
			} else {
				this.enableAllDayOptions();
			}
		}

		public startDatePicked(){
            this.startDateState = true;
			this.endDateState = true;
			this.toggleAllDays(false);
			this.toggleWeekDays(false);
			this.selectedDays = [] ;
			this.selectedEndDate = this.selectedStartDate;
			this.disableOutOfRangeDays();			
		}
		
		public endDatePicked(){
			this.toggleAllDays(false);
			this.toggleWeekDays(false);
			this.selectedDays = [] ;            
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

        public addAssignment(){
			this.isSubTypeDataReady = false;
			this.enableAllDayOptions();
			this.isAssignmentDataMounted = true;
			this.showAssignmentDetails = true;           
		}
		
		public loadSubTypes(type) {
			Vue.nextTick(()=>{
				this.isSubTypeDataReady = false;
				const url = 'api/managetypes?codeType='+ type.name +'&locationId='+this.location.id+'&showExpired=false';
				this.$http.get(url)
					.then(response => {
						if(response.data){
							this.extractSubTypes(response.data);
						}
					},err => {
						this.errorText = err.response.statusText+' '+err.response.status + '  - ' + moment().format();
						if (err.response.status != '401') {
							this.openErrorModal=true;
						}  
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
		}

		public saveAssignment() {
			let requiredError = false;
			// if (!this.assignment.name) {
			// 	this.nameState = false;
			// 	requiredError = true;
			// } else {
			// 	this.nameState = true;
			// }
			if (!this.assignment.type) {
				this.selectedTypeState = false;
				requiredError = true;
			} else {
				this.selectedTypeState = true;
			}
			if (!this.assignment.lookupCodeId) {
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
					
					this.createAssignment();
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
			if( this.assignment.name ||
				this.assignment.type ||
				this.selectedComment ||
				this.selectedStartTime || this.selectedEndTime ||
                this.nonReoccuring || this.selectedDays.length >0) return true;
            return false;           
		}

		public closeAssignmentWindow(){
			if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseAssignmentWindow();
		}

		public confirmedCloseAssignmentWindow(){
            this.resetAssignmentWindowState();
            this.showCancelWarning = false;
			this.showAssignmentDetails = false;
        }

		public resetAssignmentWindowState() {
			this.assignment = {} as assignmentInfoType;
			this.ClearFormState();
		}

		public ClearFormState(){
			this.allDaysSelected = false;
			this.weekDaysSelected = false;
			this.assignment = {} as assignmentInfoType;
            this.selectedStartTime ='';
			this.selectedEndTime = '';
			this.selectedStartDate = '';
			this.selectedEndDate = '';
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
			this.selectedComment = '';
		}

		public enableAllDayOptions() {
			for (let dayOfWeek = 0; dayOfWeek < this.dayOptions.length; dayOfWeek++) {
				this.dayOptions[dayOfWeek].enabled = true;
			}
		}

		public createAssignment() {

			this.assignment.sunday = this.selectedDays.includes(0)
			this.assignment.monday = this.selectedDays.includes(1)
			this.assignment.tuesday = this.selectedDays.includes(2)
			this.assignment.wednesday = this.selectedDays.includes(3)
			this.assignment.thursday = this.selectedDays.includes(4)
			this.assignment.friday = this.selectedDays.includes(5)
			this.assignment.saturday = this.selectedDays.includes(6);

			this.assignment.locationId = this.location.id;
			this.assignment.timezone = this.location.timezone;			

			if (this.nonReoccuring) {
				this.assignment.adhocStartDate = moment.tz(this.selectedStartDate, this.location.timezone).utc().format();
				this.assignment.adhocEndDate = moment.tz(this.selectedEndDate, this.location.timezone).utc().format();
			} else {
				this.assignment.adhocStartDate = null;
				this.assignment.adhocEndDate = null;
			}

			this.assignment.start = this.selectedStartTime;
			this.assignment.end = this.selectedEndTime;
			
			this.assignment.comment = this.selectedComment;

			const body = this.assignment;	
			const url = 'api/assignment';
			this.$http.post(url, body )
				.then(response => {
					if(response.data){
							this.confirmedCloseAssignmentWindow();
							this.$emit('change',this.activetab);
					}
				}, err => {
					const errMsg = err.response.data;
					this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
					this.assignmentErrorMsgDesc = errMsg;
					this.assignmentError = true;
				})
		}

		public tabChanged(tabInfo){
			this.activetab = tabInfo;
			this.loadNewDateRange();
		}

		public tab12h24hChanged(tabInfo){
			this.active24htab = tabInfo;
			if(tabInfo == '12h')
				this.UpdateView24h(false)
			else
				this.UpdateView24h(true)
			this.$emit('change',this.activetab);			
		}

		public tabMyTeamChanged(tabInfo){
			this.activeMyTeamTab = tabInfo;
			if(tabInfo == this.tabsMyTeamToggle[0]){                
                this.UpdateCourtAdminFullview(false)
			}			
			else{
                this.UpdateCourtAdminFullview(true)
				this.UpdateDisplayFuelGauge(false)
			}			
		}

        
        public dateChanged(event) {
			if(this.datePickerOpened)this.loadNewDateRange();
		}

		public nextDateRange() {
			const days =(this.activetab == 'Day')? 1 :7;		
			this.selectedDate = moment(this.selectedDate).add(days, 'days').format().substring(0,10);
			this.loadNewDateRange(); 
		}

		public previousDateRange() {
			const days =(this.activetab == 'Day')? 1 :7;
			this.selectedDate = moment(this.selectedDate).subtract(days, 'days').format().substring(0,10);
			this.loadNewDateRange();
		}

		public loadNewDateRange() {

			const dateType = (this.activetab == 'Day')?'day':'week'
			this.selectedDateBegin = moment(this.selectedDate).startOf(dateType).format()
			this.selectedDateEnd = moment(this.selectedDate).endOf(dateType).format();
			const dateRange = {startDate: this.selectedDateBegin, endDate: this.selectedDateEnd}
			this.UpdateDutyRangeInfo(dateRange);
			this.$emit('change',this.activetab); 
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

		public printCourtAdminDuties(){
			this.UpdatePrintCourtAdminFullview(true);
		}

		public zoomInOut(percent){
			let zoom = this.zoomLevel + percent
			if(zoom>130) zoom = 130
			if(zoom<60) zoom = 60
			document.body.style.zoom = zoom +"%";
			Vue.nextTick(()=>this.UpdateZoomLevel(zoom))
		}
    }
</script>

<style scoped lang="scss">

	.card {
		border: white;
	}

	.custom-navbar {
		float: none;
		margin:0 auto 0 auto;
		display: block;
		text-align: center;
		width:20rem;
		&.full-view{
			margin:0 auto 0 18rem;
		}
	}

	.custom-navbar > li {
		display: inline-block;
		float:none;
	}

</style>