<template>
    <div>        
       
        <b-row v-if="errMsg !=''">
            <div style="font-size:12px; border-radius:3px;" class="bg-danger text-white mx-2 my-1 px-1">
                {{errMsg}} <b-button @click="errMsg='';" class="m-0 p-0" style="height:.8rem" size="sm"><b-icon-x style="transform: translate(0,-9px);" font-scale=".75" /> </b-button>
            </div>
        </b-row> 
        <!-- {{ editingDuty }}  -->
        <b-row class="m-0">
            <div style="width:2%;"></div>
            <div style="width:13%;">
                <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem">
                    <label class="h6 m-0 p-0">Start Time<span class="text-danger">*</span></label>
                    <b-form-input
                        v-model="selectedStartTime"
                        @input="checkStates"
                        size="sm"
                        type="text"
                        autocomplete="off"
                        @paste.prevent
                        :formatter="timeFormat"
                        placeholder="HH:MM"
                        :state = "startTimeState?null:false"
                    ></b-form-input>
                </b-form-group>
            </div>

            <div style="width:13%;">              
                <b-form-group style="margin: 0.25rem 0 0 0; width: 5rem;">
                    <label class="h6 m-0 p-0">End Time<span class="text-danger">*</span></label>
                    <b-form-input
                        v-model="selectedEndTime"
                        @input="checkStates"                                       
                        size="sm"
                        type="text"
                        autocomplete="off"
                        @paste.prevent
                        :formatter="timeFormat"
                        placeholder="HH:MM"
                        :state = "endTimeState?null:false"
                    ></b-form-input>
                </b-form-group>
            </div>

            <div style="width:48%;">
                <b-form-group style="margin: 0.25rem 0 0 0;">
                    <label class="h6 m-0 p-0">Note</label>
                    <b-form-input
                        v-model="selectedComment"                                       
                        size="sm"
                        type="text"                                        
                    ></b-form-input>
                </b-form-group>
            </div>
            <div style="width:1%;" />
            <div style="width:11%;">          
                <b-button                                    
                    style="margin: 1.75rem .5rem 0.5rem .5rem ; padding:0 .5rem 0 .5rem; "
                    variant="secondary"
                    @click="closeForm()">
                    Cancel
                </b-button>
            </div>

            <div style="width:9%;">        
                <b-button                                    
                    style="margin: 1.75rem 0 .5rem .25rem; padding:0 1rem 0 1rem; "
                    variant="success"                        
                    @click="addEditedDuty()">
                    Save
                </b-button>
            </div>
        </b-row>  
                           

        <b-modal v-model="showCancelWarning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                <h2 class="mb-0 text-light"> Unsaved Duty Slot Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="showCancelWarning=false"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="showCancelWarning=false"
                 >&times;</b-button>
            </template>
        </b-modal>

        <confirm-overtime-modal :showModal="showOvertimeConfirm" @assign="saveForm()" :courtAdminName="courtAdminName" />
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import { manageAssignmentDutyInfoType } from '@/types/DutyRoster';
    import { locationInfoType } from '@/types/common';
    
    import ConfirmOvertimeModal from './ConfirmOvertimeModal.vue'

    @Component({
        components: {           
            ConfirmOvertimeModal
        }
    })
    export default class DutySlotForm extends Vue {        

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        courtAdminName!: string;

        @Prop({required: true})
        dutySlot!: manageAssignmentDutyInfoType

        @Prop({required: true})
        dutyBlocks!: manageAssignmentDutyInfoType[];

        @Prop({required: true})
        duties!: any[];

        @Prop({required: true})
        courtAdminAvailabilityArray!: number[]|null;

        @Prop({required: true})
        dutyDate!: string;


        selectedComment =""
        selectedStartTime=""
        selectedEndTime=""
        startTimeState=true
        endTimeState=true
        errMsg=""

        editingDuty = {}
        editingDutyStart=""
        editingDutyEnd=""

        showOvertimeConfirm={show: false}   
        showCancelWarning = false;     

        
        mounted() { 
            // this.editDutyDataMounted = false;
            this.clearSelections();            
            console.log('OPEN')
            
            this.selectedStartTime = this.dutySlot.startTime || ''
            this.selectedEndTime = this.dutySlot.endTime || ''
            this.selectedComment = this.dutySlot.dutyNotes || ''
            this.editingDuty = this.duties.filter(duty=>duty.id==this.dutySlot.dutyId)[0]
            this.editingDutyStart=moment(this.editingDuty['startDate']).tz(this.editingDuty['timezone']).format("HH:mm")
            this.editingDutyEnd=moment(this.editingDuty['endDate']).tz(this.editingDuty['timezone']).format("HH:mm")
            // this.getDuties();                           
        }
                
        public addEditedDuty(){
            if(!this.checkStates()) return
            this.selectedStartTime = Vue.filter('autoCompleteTime')(this.selectedStartTime)
            this.selectedEndTime = Vue.filter('autoCompleteTime')(this.selectedEndTime)
            const newDutyArray = Vue.filter('startEndTimesToArray')(null,1, this.dutyDate.slice(0,10), this.selectedStartTime, this.selectedEndTime, this.location.timezone)
            const overtimeArray = Vue.filter('subtractUnionOfArrays')(newDutyArray, this.courtAdminAvailabilityArray)
            const isOvertime = Vue.filter('sumOfArrayElements')(overtimeArray)>0
            // console.log(overtimeArray)
            // console.log(isOvertime)
            if(isOvertime){
                this.showOvertimeConfirm.show=true;
            }else{
                this.saveForm()
            }
        }
        
        public saveForm(){
            this.$emit('save', false, this.dutySlot.dutyId , this.dutySlot.id, this.selectedStartTime ,this.selectedEndTime )
        }
        
        public checkStates(){            
            this.startTimeState=!(
                (this.selectedStartTime<this.editingDutyStart) ||
                (this.selectedStartTime>this.selectedEndTime)
            )
            this.endTimeState=!(
                (this.selectedEndTime>this.editingDutyEnd) ||
                (this.selectedStartTime>this.selectedEndTime)
            )

            return (this.startTimeState && this.endTimeState)
        }

        public timeFormat(value , event){
            return Vue.filter('timeFormat')(value , event)
        }

        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){            
            if( (this.dutySlot.dutyNotes != this.selectedComment) ||
                (this.dutySlot.startTime != this.selectedStartTime) || 
                (this.dutySlot.endTime != this.selectedEndTime) ) return true;
            return false;            
        }

        public confirmedCloseForm(){           
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){            
            this.selectedStartTime = '';
            this.selectedEndTime = '';            
            this.startTimeState = true;
            this.endTimeState   = true;            
        }
        
    }
</script>

<style scoped>
</style>