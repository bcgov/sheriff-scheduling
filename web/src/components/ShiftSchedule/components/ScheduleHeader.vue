<template>
	<div v-on:edit-shifts="EditShift()">
		<header variant="primary">
			<b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
				<b-navbar-nav>
					<h3 style="width:11rem; margin-bottom: 0px;" class="text-white ml-2 font-weight-normal">Shift Schedule</h3>
				</b-navbar-nav>

				<b-navbar-nav class="custom-navbar">
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="previousDateRange" class="my-2"><b-icon-chevron-left /></b-button>
					<b-form-datepicker
							class="my-2 p-1"
							size="sm"
							style="max-height: 40px;"
							v-model="selectedDate"
							@context="dateChanged"
							@shown = "datePickerOpened = true"
							@hidden = "datePickerOpened = false"
							button-only
							today-button
							close-button
							locale="en-US">
					</b-form-datepicker>
					<b-button style="max-height: 40px;" size="sm" variant="secondary" @click="nextDateRange" class="my-2"><b-icon-chevron-right/></b-button>
				</b-navbar-nav>

				<b-navbar-nav class="mr-2">
					<b-nav-form> 
						<div v-b-tooltip.hover
							v-if="hasPermissionToEditShifts"
							:title="selectedShifts.length==0?'Please select shifts':''">
								<b-button 
									style="max-height: 40px;" 
									size="sm"
									variant="transparent"
									:disabled="selectedShifts.length==0?true:false"							
									@click="EditShift()" 
									class="my-2">
									<b-icon icon="pencil-square" font-scale="2.0" variant="warning"/>
								</b-button>
						</div>
						<div v-b-tooltip.hover
							v-if="hasPermissionToExpireShifts"
							:title="selectedShifts.length==0?'Please select shifts':''">
								<b-button 
									style="max-height: 40px;" 
									size="sm"
									variant="transparent"
									:disabled="selectedShifts.length==0?true:false"							
									@click="confirmDeleteShift()" 
									class="my-2">
									<b-icon icon="trash-fill" font-scale="2.0" variant="danger"/>
								</b-button>
						</div>
						<div v-b-tooltip.hover
							v-if="hasPermissionToImportShifts"
							title="Import shifts from previous week">
								<b-button 
									style="max-height: 40px;" 
									size="sm"
									variant="warning"						
									@click="confirmImportShift()" 
									class="my-2 ml-2">Import Shift									
								</b-button>
						</div>
					</b-nav-form>
				</b-navbar-nav>
			</b-navbar>
		</header>

		<b-modal v-model="importConflict" size="lg" id="bv-modal-import-conflict" header-class="bg-primary text-light">
			<b-table
            :items="importConflictMsg"
			:fields="importConflictFields"        
            thead-class="d-none"
            responsive="sm"
            borderless 
            small              
            striped
            >
            </b-table>
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Import Shift Conflicts</h2>                   
            </template>
            
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-import-conflict')">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-import-conflict')"
                >&times;</b-button>
            </template>
        </b-modal> 

		<b-modal v-model="confirmImport" id="bv-modal-confirm-import" header-class="bg-primary text-light">
			<b-row v-if="importError" id="ImportError" class="h4 mx-2">
				<b-badge class="mx-1 mt-2"
					style="width: 20rem;"
					v-b-tooltip.hover
					:title="importErrorMsgDesc"
					variant="danger"> {{importErrorMsg}}
					<b-icon class="ml-3"
						icon = x-square-fill
						@click="importError = false"
				/></b-badge>                    
            </b-row>         
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Confirm Import Shifts</h2>                   
            </template>
            <h4 >Are you sure you want to import shifts from the previous week?</h4>
            <template v-slot:modal-footer>
                <b-button variant="success" @click="importShift()">Confirm</b-button>
                <b-button variant="primary" @click="confirmedCloseImportForm(false)">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-import')"
                >&times;</b-button>
            </template>
        </b-modal> 

		<b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
			<b-row v-if="deleteError" id="DeleteError" class="h4 mx-2">
				<b-badge class="mx-1 mt-2"
					style="width: 20rem;"
					v-b-tooltip.hover
					:title="deleteErrorMsgDesc"
					variant="danger"> {{deleteErrorMsg}}
					<b-icon class="ml-3"
						icon = x-square-fill
						@click="deleteError = false"
				/></b-badge>                    
            </b-row>            
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Confirm Delete Shift</h2>                   
            </template>
            <h4 >Are you sure you want to delete the selected shifts?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteShift()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-delete')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-delete')"
                >&times;</b-button>
            </template>
        </b-modal>  

		<b-modal  v-model="showEditShiftDetails" id="bv-modal-shift-edit-details" centered header-class="bg-primary text-light">
            <template v-slot:modal-title>
                <span class="mb-0 text-light"> 
                    <h3 class="m-0 p-0" >Updating Shift </h3>                   
                </span>                
            </template>

            <b-card v-if="isShiftDataMounted" no-body style="font-size: 14px; user-select: none;" >
                
                <b-row v-if="shiftError" id="ShiftError" style="border-radius:5px; max-width: 30rem;" class="h4 mx-2 py-2 text-white bg-danger">
					<b-col cols="11">
						<span class="p-0">{{shiftErrorMsg}}</span>
					</b-col>
					<b-col cols="1" style=" padding:0; margin:auto 0;">
						<b-icon
                        icon = x-square-fill
                        @click="shiftError = false"/>
					</b-col>
                </b-row>

                <b-row v-if="shiftsDiffer" id="shiftsDifferError" style="border-radius:5px; max-width: 30rem;" class="h4 mx-2 py-2 bg-warning">
					<b-col cols="11">
						<span class="p-0">{{shiftsDifferMsg}}</span>
					</b-col>
					<b-col cols="1" style=" padding:0; margin:auto 0;">
						<b-icon
							icon = x-square-fill
							@click="shiftsDiffer = false"/>
					</b-col>
                </b-row>

                <b-row class="mx-auto my-0 p-0">
                    <b-form-group class="mr-3" style="width: 7rem">
                        <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedStartTime"
                            @click="startTimeState=true;shiftsDiffer=false;"
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
                            @click="endTimeState=true;shiftsDiffer=false;"
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
                        @click="closeShiftEditWindow()"
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
                    @click="closeShiftEditWindow()"
                    >
                    &times;</b-button>
            </template>
		</b-modal>

		<b-modal v-model="showEditShiftCancelWarning" id="bv-modal-edit-shift-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>             
                <h2 class="mb-0 text-light"> Unsaved Shift Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-edit-shift-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-edit-shift-cancel-warning')"
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
 
	import { Component, Vue } from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import moment from 'moment-timezone';

	import "@store/modules/ShiftScheduleInformation";
	const shiftState = namespace("ShiftScheduleInformation");

	import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

	import { importConflictMessageType, shiftInfoType, shiftRangeInfoType } from '@/types/ShiftSchedule';
	import { locationInfoType, userInfoType } from '@/types/common';
	
	@Component
	export default class ScheduleHeader extends Vue {		

		@commonState.State
		public location!: locationInfoType;

		@commonState.State
        public userDetails!: userInfoType;
				
		@shiftState.State
		public shiftRangeInfo!: shiftRangeInfoType;

		@shiftState.Action
		public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void

		@shiftState.State
		public selectedShifts!: string[];

		@shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

		shiftsToEdit = [] as shiftInfoType[]; 

		selectedDate = '';
		selectedStartTime = '';
		selectedEndTime = '';
		comment = '';
		originalSelectedStartTime = '';
		originalSelectedEndTime = '';
		originalComment = '';
		showEditShiftDetails = false;
		showEditShiftCancelWarning = false;
		confirmDelete = false;
		confirmImport = false;
		isShiftDataMounted = false;
		startTimeState = true;
		endTimeState = true;

		datePickerOpened = false;
		
        hasPermissionToEditShifts = false;
		hasPermissionToExpireShifts = false;		
        hasPermissionToImportShifts = false;

		shiftError = false;
		shiftErrorMsg = '';

		shiftsDiffer = false;
		shiftsDifferMsg = '';

		deleteErrorMsg = '';
        deleteErrorMsgDesc = '';
		deleteError = true;
		
		importErrorMsg = '';
        importErrorMsgDesc = '';
		importError = true;

		errorText = ''
		openErrorModal=false
		
		importConflictMsg: importConflictMessageType[] = [];
		importConflict = false;
		importConflictFields = [{key:"ConflictFieldName", tdClass: 'border-top my-2', label: "Field Name"}]

		mounted() {
			//console.log('header')

			this.hasPermissionToImportShifts = this.userDetails.permissions.includes("ImportShifts");
			this.hasPermissionToExpireShifts = this.userDetails.permissions.includes("ExpireShifts");
			this.hasPermissionToEditShifts = this.userDetails.permissions.includes("EditShifts");            
			
			if(!this.shiftRangeInfo.startDate){
				this.selectedDate = moment().format().substring(0,10);
				this.loadNewDateRange();
			} else {
				this.selectedDate = this.shiftRangeInfo.startDate;
				this.$emit('change');
			}
			//console.log(this.selectedDate)
			

			this.$root.$on('editShifts', () => {
				this.EditShift();
			});
		}

		public EditShift() {
			this.isShiftDataMounted = false;
			this.shiftError = false;
			this.shiftErrorMsg = '';

			this.shiftsDiffer = false;
			this.shiftsDifferMsg = '';
			this.getShiftsToEdit();
		}
		
		public getShiftsToEdit() {
			this.shiftsToEdit = [];
			const startTimes = [] as string[];
			const endTimes = [] as string[];
			const comments = [] as string[];
			let numberOfStartTimes = 0;
			let numberOfEndTimes= 0;
			let numberOfComments= 0;
			this.shiftsDiffer = false;
			this.shiftsDifferMsg = '';

			const endDate = moment(this.shiftRangeInfo.endDate).endOf('day').format();

			const url = 'api/shift?locationId='+this.location.id+'&start='+this.shiftRangeInfo.startDate+'&end='+endDate;
            this.$http.get(url)
                .then(response => {
                    if(response.data){						
						this.shiftsToEdit = response.data.filter(shift=>{if(this.selectedShifts.indexOf(shift.id) != -1) return true});						
						for (const shift of this.shiftsToEdit) {							
							startTimes.push(this.extractTime(shift.startDate, shift.timezone));
							endTimes.push(this.extractTime(shift.endDate, shift.timezone));
							if (shift.comment) {
								comments.push(shift.comment)
							}else{
								comments.push('')
							}
						}
						
						numberOfStartTimes = (new Set(startTimes)).size;
						numberOfEndTimes = (new Set(endTimes)).size;
						numberOfComments = (new Set(comments)).size;

						if (numberOfStartTimes == 1) {
							this.startTimeState = true;
							this.originalSelectedStartTime = this.selectedStartTime = startTimes[0];
						} else {
							this.originalSelectedStartTime = this.selectedStartTime = '';
							this.startTimeState = false;
							this.shiftsDiffer = true;							
						}

						if (numberOfEndTimes == 1) {
							this.endTimeState = true;
							this.originalSelectedEndTime = this.selectedEndTime = endTimes[0];
						} else {
							this.originalSelectedEndTime = this.selectedEndTime = '';
							this.endTimeState = false;
							this.shiftsDiffer = true;
						}

						if (numberOfComments == 1) {							
							this.originalComment = this.comment = comments[0];
						}else {
							this.originalComment = this.comment = '';
						}

						if (this.shiftsDiffer) {
							if (numberOfEndTimes > 1 && numberOfStartTimes > 1 && numberOfComments > 1) {
								this.shiftsDifferMsg = "The comments, start and end times of the selected shifts do not match."
							} else if (numberOfEndTimes > 1 && numberOfStartTimes > 1) {
								this.shiftsDifferMsg = "The start and end times of the selected shifts do not match."
							} else if (numberOfStartTimes > 1) {
								this.shiftsDifferMsg = "The start times of the selected shifts do not match."
							} else if (numberOfEndTimes > 1) {
								this.shiftsDifferMsg = "The end times of the selected shifts do not match."
							} else if (numberOfComments > 1) {
								this.shiftsDifferMsg = "The comments of the selected shifts do not match."
							}
						}

						this.isShiftDataMounted = true;
						this.showEditShiftDetails = true;
                    }                                   
                },err => {
					this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
					if (err.response.status != '401') {
						this.showEditShiftDetails = false;
						this.openErrorModal=true;
					}   
				})
		}

		public saveShift() {         
            this.shiftsDiffer = false;
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
			if (!requiredError) {
                    this.startTimeState = true;
                    this.endTimeState = true;
					this.updateShift();
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

		public completeDate(date, time){
            const startOfdate = moment(date).startOf("day").format().substring(0,10);
            return(moment.tz(startOfdate + 'T'+time, this.location.timezone).format()); 
        }

		public updateShift() {
			const body = [] as shiftInfoType[];

			for (const shift of this.shiftsToEdit) {							
				const newStartDate = this.completeDate(shift.startDate,this.selectedStartTime);
				const newEndDate = this.completeDate(shift.endDate,this.selectedEndTime);
				//console.log(shift)
				const editedShift: shiftInfoType = {
					id: shift.id,
					startDate: newStartDate,
					endDate: newEndDate,
					timezone: shift.timezone,
					locationId: shift.locationId ,     
					courtAdminId: shift.courtAdminId,
					overtimeHours: shift.overtimeHours
				};

				if(this.comment) editedShift.comment = this.comment;				
				body.push(editedShift)
			}
			const url = 'api/shift';
			this.$http.put(url, body)
				.then(response => {
					if(response.data){
						this.confirmedCloseForm();
						this.$emit('change');
					}
				}, err => {
					const errMsg = err.response.data.error;
					this.shiftErrorMsg = errMsg;
					this.shiftError = true;
				})

			this.UpdateSelectedShifts([]);
		}
		
		public isChanged(){      
			if((this.originalSelectedStartTime != this.selectedStartTime) || (this.originalSelectedEndTime != this.selectedEndTime)) return true;
			return false;            
		}
		
		public commentFormat(value) {
			return value.slice(0,100);
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

		public extractTime(dateTime: string, timezone: string) {
			return moment(dateTime).tz(timezone).format("HH:mm")
		}

		public closeShiftEditWindow(){
			if(this.isChanged())
                this.showEditShiftCancelWarning = true;
            else {
				this.confirmedCloseForm();
			}			
		}

		public confirmedCloseImportForm(refresh: boolean) {
			this.confirmImport = false;
			this.resetImportWindowState();
			if (refresh) this.$emit('change');
		}

		public resetImportWindowState() {
			this.importConflictMsg = [];
			this.importErrorMsg = '';
            this.importErrorMsgDesc = '';
            this.importError = false;
		}

		public confirmedCloseForm() {
			this.showEditShiftCancelWarning = false;
			this.resetShiftWindowState();
			this.UpdateSelectedShifts([]);
			this.showEditShiftDetails = false;
			this.$emit('change'); 
		}

		public resetShiftWindowState() {
			this.shiftsToEdit = [];
			this.ClearFormState();
		}

		public ClearFormState(){
			this.startTimeState = true;
			this.endTimeState = true;
			this.comment = '';
		}

		public confirmDeleteShift(){
			this.deleteError = false;
			this.confirmDelete = true;
        }

        public deleteShift(){
			const url = 'api/shift';
			const body = this.selectedShifts;
            this.$http.delete(url, {data: body})
                .then(response => {
                    // console.log(response);
                    this.confirmDelete = false;
                    this.$emit('change');                    
                }, err=>{
					const errMsg = err.response.data.error;
					console.log(err.response)
                    this.deleteErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.deleteErrorMsgDesc = errMsg;
                    this.deleteError = true;
                });
		}
		
		public confirmImportShift(){
			this.importError = false;
			this.confirmImport = true;
        }

        public importShift(){
			const startDate = moment(this.shiftRangeInfo.startDate).subtract(7, 'days').format().substring(0,10);
			const url = 'api/shift/importweek?locationId=' + this.location.id + '&start=' + startDate;
			this.importConflictMsg = [];
            this.$http.post(url)
                .then(response => {
					this.confirmedCloseImportForm(true);                     
					if (response.data.conflictMessages.length> 0) {
						for (const message of response.data.conflictMessages) {
							this.importConflictMsg.push({ConflictFieldName: message});
						}						
						this.importConflict = true;
					}                   
                }, err=>{
					const errMsg = err.response.data.error;
					console.log(err.response)
                    this.importErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.importErrorMsgDesc = errMsg;
                    this.importError = true;
                });
        }

		public dateChanged(event) {
			if(this.datePickerOpened)this.loadNewDateRange();
		}

		public nextDateRange() {			
			this.selectedDate = moment(this.selectedDate).add(7, 'days').format().substring(0,10);
			this.loadNewDateRange(); 
		}

		public previousDateRange() {
			this.selectedDate = moment(this.selectedDate).subtract(7, 'days').format().substring(0,10);
			this.loadNewDateRange();
		}

		public loadNewDateRange() {
			const firstDayOfWeek = moment(this.selectedDate).startOf('week').format()
			const lastDayOfWeek = moment(this.selectedDate).endOf('week').format();
			const dateRange = {startDate: firstDayOfWeek.substring(0,10), endDate: lastDayOfWeek.substring(0,10)}
			this.UpdateShiftRangeInfo(dateRange);
			this.$emit('change'); 
		}
	}
</script>

<style scoped>

	.card {
				border: white;
		}

	.custom-navbar {
			float:none;
			margin:0 auto 0 auto;
			display: block;
			text-align: center;
	}

	.custom-navbar > li {
			display: inline-block;
			float:none;
	}

</style>