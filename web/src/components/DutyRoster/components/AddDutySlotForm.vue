<template>
    <div>        
        <b-table-simple small borderless class="m-0 p-0">
            <b-tbody >
                <b-tr >
                    <b-td >
                        <b-tr class="bg-white">
                            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem">                            
                                <label class="h6 m-0 p-0">CourtAdmin<span class="text-danger">*</span></label>
                                <b-form-select 
                                    size="sm"
                                    v-model="selectedCourtAdmin"
                                    :state = "courtAdminState?null:false">
                                        <b-form-select-option
                                            v-for="courtAdmin in notAvailableNotRequired"
                                            :key="courtAdmin.courtAdminId"
                                            :value="courtAdmin">
                                                    {{courtAdmin.lastName}}
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="courtAdmin in shiftAvailabilityInfo"
                                            :key="courtAdmin.courtAdminId"
                                            :value="courtAdmin">
                                                    {{courtAdmin.lastName|capitalize}}, {{courtAdmin.firstName|capitalize}} 
                                        </b-form-select-option>
                                </b-form-select>
                            </b-form-group>
                        </b-tr>
                        <b-tr v-if="showErrorMsg">
                            <div style="font-size:12px; border-radius:3px;" class="bg-danger text-white mx-2 my-1 px-1">
                                {{errorMsg}} <b-button @click="showErrorMsg=false;" class="m-0 p-0" style="height:.8rem" size="sm"><b-icon-x style="transform: translate(0,-9px);" font-scale=".75" /> </b-button>
                            </div>
                        </b-tr>                        
                    </b-td>
                    <b-td>
                        <b-tr>
                            <b-td>
                                <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem">
                                    <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                                    <b-form-input
                                        v-model="selectedStartTime"
                                        @click="startTimeState=true;showErrorMsg=false;"
                                        size="sm"
                                        type="text"
                                        autocomplete="off"
                                        @paste.prevent
                                        :formatter="timeFormat"
                                        placeholder="HH:MM"
                                        :state = "startTimeState?null:false"
                                    ></b-form-input>
                                </b-form-group>
                            </b-td>
                            <b-td>
                                <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem;">
                                    <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                                    <b-form-input
                                        v-model="selectedEndTime"
                                        @click="endTimeState=true;showErrorMsg=false;"
                                        @change="reassignDutyChanged"
                                        size="sm"
                                        type="text"
                                        autocomplete="off"
                                        @paste.prevent
                                        :formatter="timeFormat"
                                        placeholder="HH:MM"
                                        :state = "endTimeState?null:false"
                                    ></b-form-input>
                                </b-form-group>
                            </b-td>	
                        </b-tr>
                        <b-tr>
                            <b-td>	
                                <b-button                                    
                                    style="margin: .5rem .5rem .5rem .5rem ; padding:0 .5rem 0 .5rem; "
                                    variant="secondary"
                                    @click="closeForm()">
                                    Cancel
                                </b-button>
                            </b-td>
                            <b-td >		       
                                <b-button                                    
                                    style="margin: .5rem 0 .5rem .25rem; padding:0 1rem 0 1rem; "
                                    variant="success"                        
                                    @click="saveForm()">
                                    Save
                                </b-button>
                            </b-td>	
                        </b-tr>
                    </b-td>                   
                </b-tr>
                <b-tr v-if="!reassign">
                    <b-button @click="reassign=true;" size="sm" variant="warning" style="margin:0 0.7rem; width:165% !important">Reassign <b-icon-arrow-left-right  font-scale="0.99" class="ml-2" /></b-button>
                </b-tr>
                <b-tr v-else class="bg-warning">
                    <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem">                            
                        <label class="h6 m-0 p-0">Reassign <b class="text-danger">{{selectedCourtAdmin.lastName|capitalize}}, {{selectedCourtAdmin.firstName|capitalize}}</b> to <span class="text-danger">*</span></label>
                        <b-form-select 
                            size="sm"
                            @change="reassignDutyChanged"
                            v-model="selectedDuty"
                            :state = "dutyState?null:false">
                                <b-form-select-option
                                    v-for="duty,inx in dutyList"
                                    :key="inx"
                                    :style="{color:duty.type.colorCode}"
                                    :value="duty">                                        
                                            {{duty.fullname}}
                                </b-form-select-option>
                        </b-form-select>
                    </b-form-group>
                    <b-td class="bg-warning">
                        <b-td class="bg-warning">
                            <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem">
                                <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                                <b-form-input
                                    v-model="reassignStartTime"
                                    @click="reassignStartTimeState=true;showErrorMsg=false;"
                                    size="sm"
                                    type="text"
                                    autocomplete="off"
                                    @paste.prevent
                                    :formatter="timeFormat"
                                    placeholder="HH:MM"
                                    :state = "reassignStartTimeState?null:false"
                                ></b-form-input>
                            </b-form-group>
                        </b-td>
                        <b-td class="bg-warning">
                            <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem;">
                                <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                                <b-form-input
                                    v-model="reassignEndTime"
                                    @click="reassignEndTimeState=true;showErrorMsg=false;"
                                    size="sm"
                                    type="text"
                                    autocomplete="off"
                                    @paste.prevent
                                    :formatter="timeFormat"
                                    placeholder="HH:MM"
                                    :state = "reassignEndTimeState?null:false"
                                ></b-form-input>
                            </b-form-group>
                        </b-td>
                    </b-td>	
                </b-tr>                
            </b-tbody>
        </b-table-simple>  

        <b-modal v-model="showCancelWarning" id="bv-modal-duty-slot-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                <h2 class="mb-0 text-light"> Unsaved Duty Slot Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-duty-slot-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-duty-slot-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="confirmAssignOverTimeDuty" id="bv-modal-confirm-assign-edit-overtime" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light">Confirm Assign Overtime</h2>                   
            </template>
            <h4 style="line-height:1.5rem" v-if="selectedCourtAdmin" >Are you sure you want to assign a duty that extends {{selectedCourtAdmin.firstName|capitalize}} {{selectedCourtAdmin.lastName|capitalize}}'s shift?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="confirmedAssignOverTimeDuty()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-assign-edit-overtime')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-assign-edit-overtime')"
                >&times;</b-button>
            </template>
        </b-modal>            
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {allEditingDutySlotsInfoType, assignDutySlotsInfoType, assignmentCardInfoType, dutyBlockInfoType, myTeamShiftInfoType } from '@/types/DutyRoster';
    import { namespace } from 'vuex-class';
    import "@store/modules/DutyRosterInformation";
    import moment from 'moment-timezone';
    const dutyState = namespace("DutyRosterInformation");

    @Component
    export default class AddDutySlotForm extends Vue {        

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.State
        public dutyRosterAssignments!: assignmentCardInfoType[];

        @Prop({required: true})
        formData!: dutyBlockInfoType;

        @Prop({required: true})
        dutyBlocks!: dutyBlockInfoType[];

        @Prop({required: true})
        fullDutyStartTime!: string;

        @Prop({required: true})
        fullDutyEndTime!: string;

        @Prop({required: true})
        timezone!: string;

        @Prop({required: true})
        startOfDay!: string;        

        selectedCourtAdmin = {} as myTeamShiftInfoType | undefined;
        courtAdminState = true;        

        selectedStartTime = '';
        startTimeState = true;

        selectedEndTime = '';
        endTimeState = true; 

        reassignStartTime = '';
        reassignStartTimeState = true;

        reassignEndTime = '';
        reassignEndTimeState = true; 

        originalCourtAdmin = {} as myTeamShiftInfoType | undefined;       
        originalStartTime = '';
        originalEndTime = '';

        errorMsg = ''
        showErrorMsg = false;
        reassign = false;

        confirmAssignOverTimeDuty = false;
        editedDutySlots: assignDutySlotsInfoType[] = [];

        reassignEditedDutySlot = {} as assignDutySlotsInfoType

        dutyList: assignmentCardInfoType[] = []
        selectedDuty = {} as assignmentCardInfoType
        dutyState = true

        formDataId = '0';
        showCancelWarning = false;

        notAvailableNotRequired: myTeamShiftInfoType[] =[]; 
        
        mounted()
        { 
            this.reassign = false;
            this.addNotAvailableNotRequired();
            this.clearSelections();
            this.extractDuties();
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        } 
        
        public addNotAvailableNotRequired(){
            this.notAvailableNotRequired = [];
            this.notAvailableNotRequired.push({
                courtAdminId: '00000-00000-11111',
                shifts: [],
                badgeNumber: 0,
                firstName: ' ',
                lastName: 'NOT REQUIRED',
                rank: ' ',
                availability: Array(96).fill(1),
                duties: Array(96).fill(0),
                dutiesDetail: []
            })
            this.notAvailableNotRequired.push({
                courtAdminId: '00000-00000-22222',
                shifts: [],
                badgeNumber: 0,
                firstName: ' ',
                lastName: 'NOT AVAILABLE',
                rank: '',
                availability: Array(96).fill(1),
                duties: Array(96).fill(0),
                dutiesDetail: []
            })
            this.notAvailableNotRequired.push({
                courtAdminId: '00000-00000-33333',
                shifts: [],
                badgeNumber: 0,
                firstName: ' ',
                lastName: 'CLOSED',
                rank: '',
                availability: Array(96).fill(1),
                duties: Array(96).fill(0),
                dutiesDetail: []
            })
        }

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:'0';
            
            const index = this.shiftAvailabilityInfo.findIndex(courtAdmin=>{if(courtAdmin.courtAdminId == this.formData.courtAdminId)return true})
            const indexNANQ = this.notAvailableNotRequired.findIndex(courtAdmin=>{if(courtAdmin.courtAdminId == this.formData.courtAdminId)return true})
            this.originalCourtAdmin = this.selectedCourtAdmin = (index>=0)? this.shiftAvailabilityInfo[index]: (indexNANQ>=0)? this.notAvailableNotRequired[indexNANQ]: {} as myTeamShiftInfoType;            
              
            this.originalStartTime = this.selectedStartTime = this.formData.startTimeString;            
            this.originalEndTime = this.selectedEndTime = this.formData.endTimeString;
        }

        
        public extractDuties(){
            this.dutyList = []
            for(const dutyRosterAssignment of this.dutyRosterAssignments){
                if(dutyRosterAssignment.attachedDuty){
                    const duty = dutyRosterAssignment;
                    duty['fullname']= Vue.filter('capitalize')(duty.type.name) +
                        (duty.code? '-'+duty.code:'') +
                        (duty.name?' ('+duty.name+') ':'')+
                        (duty.FTEnumber>0?' slot_'+(duty.FTEnumber+1):'');
                    this.dutyList.push(duty) 
                }
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

        public saveForm(){
            this.courtAdminState  = true;                
            this.startTimeState = true;
            this.endTimeState   = true;
            this.showErrorMsg = false;
            let requiredError = false;

            if(!this.selectedCourtAdmin || (this.selectedCourtAdmin && !this.selectedCourtAdmin.courtAdminId) ){
                this.courtAdminState  = false;
                requiredError = true;            
            } else {
                this.courtAdminState  = true;
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
                this.errorMsg = "The End time is before or equal to the Start time."
                this.showErrorMsg = true;
                requiredError = true;
            } 

            if(this.reassign && this.selectedDuty.FTEnumber != undefined){ 
                this.reassignStartTimeState = true
                this.reassignEndTimeState = true               
                requiredError = this.checkReassignConflicts()
            }
            
            if (!requiredError) {
                   
                this.courtAdminState  = true;                    
                this.endTimeState   = true;
                this.startTimeState = true;
                this.reassignStartTimeState = true
                this.reassignEndTimeState = true

                const selectedTimeBins = this.getTimeRangeBins(this.selectedStartTime, this.selectedEndTime, this.startOfDay, this.timezone)

                if(this.validateTime(selectedTimeBins)){
                    this.submitEditingSlotInfo(selectedTimeBins)                    
                }                                
            }
        }

        public submitEditingSlotInfo(selectedTimeBins){
            const selectedTimeArray = this.fillInArray(Array(96).fill(0), 1, selectedTimeBins.startBin, selectedTimeBins.endBin);
            
            if(this.selectedCourtAdmin){
                const courtAdminId = this.selectedCourtAdmin.courtAdminId
                let courtAdmin = this.shiftAvailabilityInfo.filter(courtAdmin=>{if(courtAdmin.courtAdminId==courtAdminId)return true})[0];
                if(!courtAdmin) courtAdmin =  this.selectedCourtAdmin;
                
                let availability = courtAdmin.availability
                let duties = courtAdmin.duties
                let shiftId = this.formData.shiftId

                const isNotRequiredOrAvailable = (courtAdminId =='00000-00000-11111' || courtAdminId =='00000-00000-22222' || courtAdminId =='00000-00000-33333')

                
                if(!isNotRequiredOrAvailable  && courtAdminId == this.formData.courtAdminId){
                    const dutyArrayOfOriginalSlot = this.fillInArray(Array(96).fill(0), 1, this.formData.startTime-1, this.formData.endTime-1);
                    availability = this.addArrays(dutyArrayOfOriginalSlot, availability);
                    duties = this.subtractUnionOfArrays(duties,dutyArrayOfOriginalSlot)                    
                }

                const dutiesConflictWithNewTimeSlot = this.unionArrays(selectedTimeArray, duties);
                if(this.sumOfArrayElements(dutiesConflictWithNewTimeSlot)>0){
                    this.errorMsg = "The CourtAdmin has conflicting duties with the selected time range." 
                    this.showErrorMsg = true;
                    return
                }

                const unionSelectedRangeAvail = this.unionArrays(selectedTimeArray, availability);
//___overtime___
                const subtractSelectedRangeAvail = this.subtractUnionOfArrays(selectedTimeArray, availability)
                if(this.sumOfArrayElements(subtractSelectedRangeAvail)>0){
                    const editedDutySlots: assignDutySlotsInfoType[] = [];

                    const discontinuityOvertime = this.findDiscontinuity(subtractSelectedRangeAvail);
                    const iterationNumOvertime = Math.floor((this.sumOfArrayElements(discontinuityOvertime) +1)/2);
                    for(let i=0; i< iterationNumOvertime; i++){
                        const inx1 = discontinuityOvertime.indexOf(1)
                        let inx2 = discontinuityOvertime.indexOf(2)
                        discontinuityOvertime[inx1]=0
                        if(inx2>=0) discontinuityOvertime[inx2]=0; else inx2=discontinuityOvertime.length 
                        const slotTime = this.convertTimeRangeBinsToTime(this.startOfDay, inx1, inx2)
                        editedDutySlots.push({
                            startDate: slotTime.startTime,
                            endDate: slotTime.endTime,                        
                            isOvertime:true,
                            //shiftId: null,
                            dutySlotId: null,
                        })
                    }

                    const discontinuity = this.findDiscontinuity(unionSelectedRangeAvail);
                    const iterationNum = Math.floor((this.sumOfArrayElements(discontinuity) +1)/2);
                   
                    for(let i=0; i< iterationNum; i++){
                        const inx1 = discontinuity.indexOf(1)
                        let inx2 = discontinuity.indexOf(2)
                        discontinuity[inx1]=0
                        if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length                         
                        const slotTime = this.convertTimeRangeBinsToTime(this.startOfDay, inx1, inx2)
                        editedDutySlots.push({
                            startDate: slotTime.startTime,
                            endDate: slotTime.endTime,                        
                            //shiftId: unionSelectedRangeAvail.slice(inx1,inx2).includes(1)? this.formData.shiftId: unionSelectedRangeAvail[inx1],
                            isOvertime: false,
                            dutySlotId: unionSelectedRangeAvail.slice(inx1,inx2).includes(1)? this.formData.dutySlotId:null,
                        })
                    }
                    this.editedDutySlots = editedDutySlots;
                    this.confirmAssignOverTimeDuty = true;
                    return
                }
                
                const indexShiftId = unionSelectedRangeAvail.findIndex((element)=>{return(element>0);})
                if(!shiftId) shiftId = unionSelectedRangeAvail[indexShiftId]; 

                const startTime = moment(this.startOfDay).add(this.selectedStartTime).format();
                const endTime =  moment(this.startOfDay).add(this.selectedEndTime).format();
                const editedDutySlot: assignDutySlotsInfoType ={
                    startDate: startTime,
                    endDate: endTime, 
                    isOvertime: false,  
                    dutySlotId: this.formData.dutySlotId,
                }

                const allEditingDutySlots: allEditingDutySlotsInfoType[] = [{
                    selectedDuty: null,
                    editedDutySlot: editedDutySlot
                }]

                if(this.reassignEditedDutySlot.startDate && this.reassignEditedDutySlot.endDate){
                    allEditingDutySlots.push({
                        selectedDuty: this.selectedDuty,
                        editedDutySlot:this.reassignEditedDutySlot
                    })
                }
                
                this.$emit('submit',this.selectedCourtAdmin?this.selectedCourtAdmin.courtAdminId:0, allEditingDutySlots , false);  
            }
        }

        public validateTime(selectedTimeBins){
            const fulldutyBins = this.getTimeRangeBins(this.fullDutyStartTime, this.fullDutyEndTime, this.startOfDay, this.timezone)            
            
            if(selectedTimeBins.startBin < fulldutyBins.startBin){
                this.startTimeState = false;
                this.errorMsg = "The Start time is before the duty start time ("+this.fullDutyStartTime+")." 
                this.showErrorMsg = true;
                return false
            }
            else if(selectedTimeBins.endBin > fulldutyBins.endBin){
                this.endTimeState = false;
                this.errorMsg = "The End time is after the duty end time ("+this.fullDutyEndTime+")."
                this.showErrorMsg = true;
                return false;
            }

            for(const dutyBlock of this.dutyBlocks){
                if(dutyBlock.courtAdminId && (dutyBlock.startTime != this.formData.startTime)){
                
                    const name = dutyBlock.firstName? dutyBlock.lastName+", "+dutyBlock.firstName:dutyBlock.lastName

                    if(selectedTimeBins.startBin >= (dutyBlock.startTime-1) && selectedTimeBins.startBin < (dutyBlock.endTime-1)){
                        this.startTimeState = false;
                        this.errorMsg = "The Start time conflicts with "+name+"'s duty." 
                        this.showErrorMsg = true;
                        return false;
                    }

                    if(selectedTimeBins.endBin > (dutyBlock.startTime-1) && selectedTimeBins.endBin <= (dutyBlock.endTime-1)){
                        this.endTimeState = false;
                        this.errorMsg = "The End time conflicts with "+name+"'s duty." 
                        this.showErrorMsg = true;
                        return false;
                    }

                    // if(this.selectedCourtAdmin?.courtAdminId == dutyBlock.courtAdminId && (selectedTimeBins.startBin == (dutyBlock.endTime-1)||selectedTimeBins.endBin == (dutyBlock.startTime-1)) ){
                    //     this.errorMsg = "Please modify the duty of "+name +" instead."
                    //     this.showErrorMsg = true; 
                    //     return false;   
                    // }
                }
            }
            return true            
        }

        public confirmedAssignOverTimeDuty(){
            this.confirmAssignOverTimeDuty = false;
            this.$emit('submit',this.selectedCourtAdmin?this.selectedCourtAdmin.courtAdminId:0, this.editedDutySlots , false);
        }

        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){            
            if((this.originalCourtAdmin && this.selectedCourtAdmin && (this.originalCourtAdmin.courtAdminId != this.selectedCourtAdmin.courtAdminId)) ||
                (this.originalStartTime != this.selectedStartTime) || 
                (this.originalEndTime != this.selectedEndTime)) return true;
            return false;
            
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedCourtAdmin = {} as myTeamShiftInfoType;           
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.courtAdminState  = true;
            this.startTimeState = true;
            this.endTimeState   = true;            
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public addArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public subtractUnionOfArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
        }
        
        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public findDiscontinuity(array){
            return array.map((arr,index)=>{
                if((index==0 && arr>0)||(arr>0 && array[index-1]==0)) return 1 ;
                else if(index>0 && arr==0 && array[index-1]>0) return 2 ;
                else return 0
            })
        }
        
        public getTimeRangeBins(startTime, endTime, startOfDay, timezone){
            const startDate = moment(startOfDay).tz(timezone).add(startTime);
            const endDate = moment(startOfDay).tz(timezone).add(endTime);
            const startBin = moment.duration(startDate.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endDate.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public convertTimeRangeBinsToTime(dutyDate, startBin, endBin){            
            const startTime = moment(dutyDate).add(startBin*15, 'minutes').format();
            const endTime = moment(dutyDate).add(endBin*15, 'minutes').format();
            return( {startTime: startTime, endTime:endTime } )
        }

        public reassignDutyChanged(){

            this.reassignStartTimeState = true
            this.reassignEndTimeState = true
            const timezone = this.selectedDuty?.attachedDuty?.timezone? this.selectedDuty.attachedDuty.timezone : this.timezone
            const startTime = this.selectedDuty?.attachedDuty?.startDate? moment(this.selectedDuty.attachedDuty.startDate).tz(timezone).format("HH:mm") : ''            
            const endTime = this.selectedDuty?.attachedDuty?.endDate? moment(this.selectedDuty.attachedDuty.endDate).tz(timezone).format("HH:mm") : ''
            this.reassignStartTime = (this.selectedEndTime > startTime) ? this.autoCompleteTime(this.selectedEndTime): startTime
            this.reassignEndTime = endTime

            if(this.reassignStartTime > this.reassignEndTime){
                this.reassignStartTimeState = false
                this.reassignEndTimeState = false
            }
        }

        public checkReassignConflicts(){
            
            this.reassignStartTimeState = false
            this.reassignEndTimeState = false

            this.reassignStartTime = this.autoCompleteTime(this.reassignStartTime)
            this.reassignEndTime = this.autoCompleteTime(this.reassignEndTime)

            const timezone = this.selectedDuty?.attachedDuty?.timezone? this.selectedDuty.attachedDuty.timezone : this.timezone

            if(this.reassignStartTime >= this.reassignEndTime){                
                this.errorMsg = "The Reassigned End Time is before or equal to the Start Time."
                this.showErrorMsg = true;
                return true
            }

            const dutyStartTime = this.selectedDuty?.attachedDuty?.startDate? moment(this.selectedDuty.attachedDuty.startDate).tz(timezone).format("HH:mm") : ''
            if(dutyStartTime > this.reassignStartTime){
                this.errorMsg = `The Reassigned Start Time is before the duty's Start time (${dutyStartTime}).`
                this.showErrorMsg = true;
                return true
            }

            const dutyEndTime =  this.selectedDuty?.attachedDuty?.endDate? moment(this.selectedDuty.attachedDuty.endDate).tz(timezone).format("HH:mm") : ''
            if(dutyEndTime < this.reassignEndTime){
                this.errorMsg = `The Reassigned End Time is after the duty's End time (${dutyEndTime}).`
                this.showErrorMsg = true;
                return true
            }
            

            const reassignedDutyBin = this.getTimeRangeBins(this.reassignStartTime, this.reassignEndTime, this.startOfDay, this.timezone)
            const reassignArray = this.fillInArray(Array(96).fill(0), 1, reassignedDutyBin.startBin, reassignedDutyBin.endBin)

            for(const dutySlot of this.selectedDuty?.attachedDuty?.dutySlots){

                const timezone = dutySlot.timezone
                const start = moment(dutySlot.startDate).tz(timezone)
                const startTime = moment(dutySlot.startDate).tz(timezone).format("HH:mm")
                const endTime = moment(dutySlot.endDate).tz(timezone).format("HH:mm")
                const startOfDay = moment(start).startOf("day");
                const occupiedDutyBin = this.getTimeRangeBins(startTime, endTime, startOfDay, timezone)
                const occupiedArray = this.fillInArray(Array(96).fill(0), 1, occupiedDutyBin.startBin, occupiedDutyBin.endBin)
                const conflictArray = this.unionArrays(reassignArray, occupiedArray)
                if(this.sumOfArrayElements(conflictArray)>0){
                    this.errorMsg = `The reassign range has conflict with another courtAdmin (${startTime} to ${endTime}).`
                    this.showErrorMsg = true;
                    return true
                }

     
            }

            if(this.selectedDuty?.attachedDuty?.id){
                const reassignStartTime = moment(this.startOfDay).add(this.reassignStartTime).format();
                const reassignEndTime =  moment(this.startOfDay).add(this.reassignEndTime).format();
                this.reassignEditedDutySlot =
                {
                    startDate: reassignStartTime,
                    endDate: reassignEndTime, 
                    isOvertime: false,                                       
                    dutySlotId: null ,
                }

            }
            return false
        }
    }
</script>

<style scoped>
    td {
        margin: 0rem 0.5rem 0.1rem 0rem;
        padding: 0rem 0.5rem 0.1rem 0rem;
        
        background-color: white ;
    }
</style>