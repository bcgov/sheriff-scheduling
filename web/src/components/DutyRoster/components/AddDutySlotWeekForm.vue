<template>
    <div v-if="isDataReady">
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
                                            v-for="courtAdmin in filteredShiftAvailabilityInfo"
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
    import {assignDutySlotsInfoType, dutyBlockWeekInfoType, myTeamShiftInfoType } from '@/types/DutyRoster';
    import { namespace } from 'vuex-class';
    import "@store/modules/DutyRosterInformation";
    import moment from 'moment-timezone';
    const dutyState = namespace("DutyRosterInformation");

    @Component
    export default class AddDutySlotWeekForm extends Vue {        

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @Prop({required: true})
        formData!: dutyBlockWeekInfoType;

        @Prop({required: true})
        dutyBlocks!: dutyBlockWeekInfoType[];

        @Prop({required: true})
        fullDutyStartTime!: string;

        @Prop({required: true})
        fullDutyEndTime!: string;

        @Prop({required: true})
        timezone!: string;

        @Prop({required: true})
        startOfDay!: string;
        
        filteredShiftAvailabilityInfo: myTeamShiftInfoType[] = [];

        selectedCourtAdmin = {} as myTeamShiftInfoType | undefined;
        courtAdminState = true;        

        selectedStartTime = '';
        startTimeState = true;

        selectedEndTime = '';
        endTimeState = true; 

        originalCourtAdmin = {} as myTeamShiftInfoType | undefined;       
        originalStartTime = '';
        originalEndTime = '';

        errorMsg = ''
        showErrorMsg = false;

        confirmAssignOverTimeDuty = false;
        editedDutySlots: assignDutySlotsInfoType[] = [];

        formDataId = '0';
        showCancelWarning = false;

        isDataReady = false;

        notAvailableNotRequired: myTeamShiftInfoType[] =[]; 
        
        mounted()
        { 
            this.isDataReady = false;
            this.filterCourtAdmins()
            this.addNotAvailableNotRequired();
            this.clearSelections();
            if(this.formData.id) {
                this.extractFormInfo();
            }  
            this.isDataReady = true;             
        }
        
        public filterCourtAdmins(){
            this.filteredShiftAvailabilityInfo = this.shiftAvailabilityInfo.filter(courtAdmin=>{
                for(const shift of courtAdmin.shifts){
                    if(shift.startDate.substring(0,10)==this.formData.dutyDate.substring(0,10)) return true
                }
            })
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
            
            if (!requiredError) {
                   
                this.courtAdminState  = true;                    
                this.endTimeState   = true;
                this.startTimeState = true;

                const selectedTimeBins = this.getTimeRangeBins(this.selectedStartTime, this.selectedEndTime, this.startOfDay, this.timezone)

                if(this.validateTime(selectedTimeBins)){
                    this.submitEditingSlotInfo(selectedTimeBins)                    
                }                                
            }
        }

        public getCourtAdminAvailability(courtAdmin){
            if(courtAdmin){
                const shifts = courtAdmin.shifts.filter(shift=>{if(shift.startDate.substring(0,10)==this.startOfDay.substring(0,10))return true})
                let availability = Array(96).fill(0)
                for(const shift of shifts){
                    const rangeBin = this.getTimeRangeBinsFullDate(shift.startDate, shift.endDate, this.timezone);
                    availability = this.fillInArray(availability, shift.id , rangeBin.startBin,rangeBin.endBin)
                }
                return availability
            }else{
                return Array(96).fill(1)
            }
        }

        public getCourtAdminDuties(courtAdmin){
            if(courtAdmin){
                const dutiesDetail = courtAdmin.dutiesDetail.filter(duty=>{if(duty.startTime.substring(0,10)==this.startOfDay.substring(0,10))return true})
                let duties = Array(96).fill(0)
                for(const duty of dutiesDetail){                    
                    duties = this.fillInArray(duties, duty.id , duty.startBin, duty.endBin)
                }
                return duties
            }else{
                return Array(96).fill(0)
            }
        }

        public submitEditingSlotInfo(selectedTimeBins){

            const selectedTimeArray = this.fillInArray(Array(96).fill(0), 1, selectedTimeBins.startBin, selectedTimeBins.endBin);
            
            if(this.selectedCourtAdmin){
                const courtAdminId = this.selectedCourtAdmin.courtAdminId
                const isNotRequiredOrAvailable = (courtAdminId =='00000-00000-11111' || courtAdminId =='00000-00000-22222' || courtAdminId =='00000-00000-33333')

                let courtAdmin = this.shiftAvailabilityInfo.filter(courtAdmin=>{if(courtAdmin.courtAdminId==courtAdminId)return true})[0];
                               
                if(!courtAdmin && !isNotRequiredOrAvailable) courtAdmin =  this.selectedCourtAdmin;
                
                let availability =  this.getCourtAdminAvailability(courtAdmin);
                let duties = this.getCourtAdminDuties(courtAdmin);
                availability = this.subtractUnionOfArrays(availability,duties)
                let shiftId = this.formData.shiftId

                
                if(!isNotRequiredOrAvailable  && courtAdminId == this.formData.courtAdminId){
                    //console.log(this.formData)
                    const dutytimeBins = this.getTimeRangeBins(this.formData.startTimeString, this.formData.endTimeString, this.startOfDay, this.timezone);
                    const startTime = dutytimeBins.startBin;
                    const endTime = dutytimeBins.endBin;                   
                    const dutyArrayOfOriginalSlot = this.fillInArray(Array(96).fill(0), 1, startTime, endTime);
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
                            isOvertime: true,                       
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
                            isOvertime: false,                       
                            //shiftId: unionSelectedRangeAvail.slice(inx1,inx2).includes(1)? this.formData.shiftId: unionSelectedRangeAvail[inx1],
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
                const editedDutySlot: assignDutySlotsInfoType[] =[{
                    startDate: startTime,
                    endDate: endTime,
                    isOvertime: false,                        
                    //shiftId: isNotRequiredOrAvailable?null:shiftId,
                    dutySlotId: this.formData.dutySlotId,
                }]

                this.$emit('submit',this.selectedCourtAdmin?this.selectedCourtAdmin.courtAdminId:0, editedDutySlot , false, this.formData.day);  
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
                //console.log(dutyBlock)
                const dutytimeBins = this.getTimeRangeBins(dutyBlock.startTimeString, dutyBlock.endTimeString, this.startOfDay, this.timezone);
                const startTime = dutytimeBins.startBin;
                const endTime = dutytimeBins.endBin;


                // console.log(startTime)
                // console.log(endTime)
                // console.log(selectedTimeBins.startBin)
                // console.log(selectedTimeBins.endBin)

                const name = dutyBlock.firstName? dutyBlock.lastName+", "+dutyBlock.firstName:dutyBlock.lastName

                if(dutyBlock.courtAdminId && (dutyBlock.startTime != this.formData.startTime)){
                   
                    if(selectedTimeBins.startBin >= (startTime) && selectedTimeBins.startBin < (endTime)){
                        this.startTimeState = false;
                        this.errorMsg = "The Start time conflicts with "+name+"'s duty." 
                        this.showErrorMsg = true;
                        return false;
                    }

                    if(selectedTimeBins.endBin > (startTime) && selectedTimeBins.endBin <= (endTime)){
                        
                        this.endTimeState = false;
                        this.errorMsg = "The End time conflicts with "+name+"'s duty." 
                        this.showErrorMsg = true;
                        return false;
                    }

                    // if(this.selectedCourtAdmin?.courtAdminId == dutyBlock.courtAdminId && (selectedTimeBins.startBin == (dutyBlock.endTime-1)||selectedTimeBins.endBin == (dutyBlock.startTime-1)) ){
                    //     this.errorMsg = "Please modify the duty of "+name+" instead."
                    //     this.showErrorMsg = true; 
                    //     return false;   
                    // }
                }
            }
            return true            
        }

        public confirmedAssignOverTimeDuty(){
            this.confirmAssignOverTimeDuty = false;
            this.$emit('submit',this.selectedCourtAdmin?this.selectedCourtAdmin.courtAdminId:0, this.editedDutySlots , false, this.formData.day);
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
            return arrayA.map((arr,index) =>{return arr*(arrayB[index]>0?0:1)});
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

        public getTimeRangeBinsFullDate(startDate, endDate, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startOfDay = moment(startTime).startOf("day");
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }

        public convertTimeRangeBinsToTime(dutyDate, startBin, endBin){            
            const startTime = moment(dutyDate).add(startBin*15, 'minutes').format();
            const endTime = moment(dutyDate).add(endBin*15, 'minutes').format();
            return( {startTime: startTime, endTime:endTime } )
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