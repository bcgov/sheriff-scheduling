<template>
    <div>
        <b-table-simple small borderless>
            <b-tbody>
                <b-tr>
                    <b-td>   
                        <b-tr class="mt-0 bg-white">   
                            <b class="ml-3 h6" v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day Training: </b>                          
                            <b class="ml-3 px-1" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day Training: </b> 
                            <b class="ml-3 px-1" style="background-color: #aed4bc" v-else >Partial Day Training: </b>
                        </b-tr>
                        <b-tr >
                            <b-form-group style="margin: 0.05rem 0 0 0.5rem;width: 15rem"> 
                                <b-form-select
                                    tabindex="1"
                                    size = "sm"
                                    @change="setExpiryDate()"
                                    :disabled="!editable"
                                    v-model="selectedTrainingType"
                                    :state = "trainingTypeState?null:false">
                                        <b-form-select-option :value="{}">
                                            Select a Training Type*
                                        </b-form-select-option>
                                        <b-form-select-option
                                            v-for="training in trainingTypeInfoList" 
                                            :key="training.id"
                                            :value="training">
                                                {{training.description}}
                                        </b-form-select-option>     
                                </b-form-select>
                                <b-form-input
                                    tabindex="2"
                                    class="mt-1 mb-0"
                                    v-if="selectedTrainingType && selectedTrainingType.code=='Other'"
                                    v-model="selectedTrainingTypeComment"
                                    :disabled="!editable"
                                    placeholder="Comment*"
                                    :formatter="commentFormat"
                                    :state = "trainingTypeCommentState?null:false"
                                    size = "sm">
                                </b-form-input>
                            </b-form-group>
                        </b-tr>                        
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> From: </label>
                        <b-input-group  style="padding 0; margin:0 ;width: 10rem">  
                            <b-form-datepicker
                                tabindex="3"
                                class="mb-1"
                                @input="setExpiryDate()"
                                size="sm"                                
                                :disabled="!editable"
                                v-model="selectedStartDate"
                                placeholder="Start Date*"
                                :state = "startDateState?null:false"
                                :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                                @context="startTimePicked"
                                locale="en-US">
                            </b-form-datepicker>
                        </b-input-group>
                        <b-row  style="padding 0; margin:0; height: 1.95rem; width: 10rem">
                            <b-input-group  style="padding 0; margin:0 ;width: 5.4rem">
                                <b-form-checkbox 
                                    tabindex="6"
                                    v-model="addTime" 
                                    @change="addTimeCheckBoxChanged" 
                                    size="sm" 
                                    class="mt-1 mx-0">
                                    <span class="h6 mx-0 px-0">Add Time</span>                                
                                </b-form-checkbox>
                            </b-input-group>                        
                            <b-input-group  style="width: 4.2rem; margin-left: auto;">
                                <b-form-input v-if="addTime"
                                    tabindex="7"
                                    v-model="selectedStartTime"
                                    @click="startTimeState=true"
                                    size="sm" 
                                    :disabled="!editable"
                                    type="text"
                                    autocomplete="off"
                                    @paste.prevent
                                    :formatter="timeFormat"
                                    placeholder="HH:MM"
                                    :state = "startTimeState?null:false"
                                ></b-form-input>
                            </b-input-group>
                        </b-row>                       
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> To: </label>
                        <b-form-datepicker
                            tabindex="4"
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            @input="setExpiryDate()"
                            :disabled="!editable"
                            v-model="selectedEndDate"
                            placeholder="End Date*"
                            :state = "endDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker>
                        <b-input-group v-if="addTime" style="width: 4.2rem; margin-left: auto;">
                            <b-form-input
                                tabindex="8"
                                v-model="selectedEndTime"
                                @click="endTimeState=true"
                                size="sm"
                                type="text"
                                :disabled="!editable"
                                autocomplete="off"
                                @paste.prevent
                                :formatter="timeFormat"
                                placeholder="HH:MM"
                                :state = "endTimeState?null:false"
                            ></b-form-input>
                        </b-input-group>
                    </b-td>
                    <b-td >
                        <label class="h6 m-0 p-0"> Expiry Date: 
                            <b-button
                                @click="refreshExpiryDate()"
                                class="refresh-expiry-date"                                
                                style="margin:0 0.5rem; padding:0; transform:translate(0,-2px);"
                                variant="trasparant">
                                <b-icon-arrow-repeat scale="1.2" :animation="refresh? 'spin': 'none'" class="text-success m-0 p-0"/>
                            </b-button> 
                        </label>
                        <b-form-datepicker
                            tabindex="5"
                            disabled
                            style="margin:-.2rem 0 .5rem 0;"
                            class="pt-0"
                            size="sm"
                            v-model="selectedExpiryDate"
                            placeholder="Expiry Date"
                            :state = "expiryDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker> 
                        <b-button                                    
                            style="margin: 0rem .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                            variant="secondary"
                            @click="closeForm()">
                            Cancel
                        </b-button>   
                        <b-button                                    
                            style="margin: 0rem 0 0 0; padding:0 0.7rem 0 0.7rem; "
                            variant="success"                        
                            @click="saveForm()">
                            Save
                        </b-button>  
                    </b-td>
                </b-tr>
                <b-tr  class="m-0 p-0">
                    <b-td colspan="4">
                        <b class="p-0 h6" style="margin: 0.75rem 0.5rem 0 0.7rem; float:left;">Note:</b>
                        <b-form-group class="p-0 mr-2 mt-1 mb-0" style="float:left; width: 41rem">                        
                            <b-form-input
                                tabindex="9"
                                v-model="selectedComment"
                                size="sm"
                                type="text"
                                :formatter="commentFormat"                            
                            ></b-form-input>
                        </b-form-group> 
                    </b-td>
                </b-tr>  

            </b-tbody>
        </b-table-simple> 

        <b-modal v-model="showCancelWarning" id="bv-modal-training-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Training </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Training Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-training-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-training-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="showSaveWarning" id="bv-modal-training-save-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                              
                <h2 class="mb-0 text-light"> Training Without Expiry Date </h2>                                 
            </template>
            <p>Are you sure you want to save training without specifying an expiry date?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-training-save-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedSave()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-training-save-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>              
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';
    
    import {teamMemberInfoType ,userTrainingInfoType} from '@/types/MyTeam';
    import {trainingInfoType} from '@/types/common';
    
    import "@store/modules/TeamMemberInformation"; 
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component
    export default class AddTrainingForm extends Vue {        

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @Prop({required: true})
        formData!: userTrainingInfoType;

        @Prop({required: true})
        isCreate!: boolean;

        @Prop({required: true})
        trainingTypeInfoList!: trainingInfoType[]; 
        
        @Prop({required: true})
        editable!: boolean;

        selectedTrainingType = {} as trainingInfoType | undefined;
        trainingTypeState = true;

        selectedTrainingTypeComment =''
        trainingTypeCommentState = true

        selectedEndDate = ''
        endDateState = true

        selectedStartDate = ''
        startDateState = true

        selectedStartTime = ''
        startTimeState = true

        selectedEndTime = ''
        endTimeState = true

        selectedExpiryDate = ''
        expiryDateState = true; 

        originalTrainingType = {} as trainingInfoType | undefined;
        originalTrainingTypeComment =''
        originalEndDate = ''
        originalStartDate = ''
        originalStartTime = ''
        originalEndTime = ''
        originalExpiryDate = ''

        originalComment = '';
        selectedComment = '';

        addTime = false;
        refresh = false;

        formDataId = 0;
        showCancelWarning = false;
        showSaveWarning = false;
        
        mounted()
        { 
            this.clearTrainingSelections();
            if(this.formData.id) {
                this.extractFormInfo();
            }               
        }        

        public extractFormInfo(){
            this.formDataId = this.formData.id? this.formData.id:0;
            
            const index = this.trainingTypeInfoList.findIndex(training=>{if(training.id == this.formData.trainingTypeId)return true})
            this.originalTrainingType = this.selectedTrainingType = (index>=0)? this.trainingTypeInfoList[index]: {} as trainingInfoType; 
            this.originalExpiryDate = this.selectedExpiryDate = this.formData.expiryDate? this.formData.expiryDate.substring(0,10):'';
            this.originalTrainingTypeComment = this.selectedTrainingTypeComment = this.formData.comment? this.formData.comment:'';          
            this.originalStartDate = this.selectedStartDate = this.formData.startDate.substring(0,10)            
            this.originalEndDate = this.selectedEndDate =  this.formData.endDate.substring(0,10)
            
            const displayTime = Vue.filter('isDateFullday')(this.formData.startDate,this.formData.endDate);
            this.addTime = !displayTime;           
            this.originalStartTime = this.selectedStartTime = displayTime? '' :this.formData.startDate.substring(11,16)            
            this.originalEndTime = this.selectedEndTime = displayTime? '' :this.formData.endDate.substring(11,16)
            this.originalComment = this.selectedComment = this.formData.note? this.formData.note: ''
        }

        public timeFormat(value , event){        
            if(isNaN(Number(value.slice(-1))) && value.slice(-1) != ':') return value.slice(0,-1) 
            if(value.length!=3 && value.slice(-1) == ':') return value.slice(0,-1);
            if(value.length==2 && event.data && value.slice(0,1)>=6 && value.slice(-1)>=6) return value.slice(0,-1);
            if(value.length==2 && event.data && value.slice(-1)<6) return '0'+value.slice(0,1)+':'+value.slice(1,2);
            if(value.length==2 && event.data && value.slice(0,1)>=2 && value.slice(0,1)<6 && value.slice(-1)>=6) return '00:'+value.slice(0,2);            
            if(value.length==2 && event.data && value.slice(0,1)<2 && value.slice(-1)>=6) return value.slice(0,2)+':';
            if(value.length==4 && value.slice(-1)>=6) return value.slice(0,-1);
            if(value.length==3 && value.slice(0,1)!=':' && value.slice(1,2)!=':' && value.slice(-1)<6) return value.slice(0,2)+':'+value.slice(2,3);
            if(value.length==3 && value.slice(-1)>=6 ) return value.slice(0,-1);
            if(value.length==6 && value.slice(0,1)==0 && value.slice(4,6)<60 && (value.slice(1,2)+value.slice(3,4))<24) return value.slice(1,2)+value.slice(3,4)+':'+value.slice(4,5)+value.slice(5,6);           
            if(value.length>5) return value.slice(0,5);
            if(value.length==5 && (value.slice(0,2)>=24 || value.slice(3,5)>=60)) return '';
            if(value.length==5 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,5)) || value.slice(2,3)!=':') )return '';
            if(value.length==4 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,4)) || value.slice(2,3)!=':') )return '';
            return value
        }

        public saveForm(){
            this.trainingTypeState  = true;
            this.endDateState    = true;
            this.startDateState  = true;
            this.startTimeState  = true;
            this.endTimeState    = true;
            this.expiryDateState = true;

            if(this.selectedTrainingType && !this.selectedTrainingType.id ){
                this.trainingTypeState  = false;
            }else if(this.selectedTrainingType && this.selectedTrainingType.code=='Other' && this.selectedTrainingTypeComment == ""){
                this.trainingTypeState  = true;
                this.trainingTypeCommentState = false;
            }else if(this.selectedStartDate == ""){
                this.trainingTypeState  = true;
                this.trainingTypeCommentState = true;
                this.startDateState = false;
            }else if(this.selectedEndDate == ""){
                this.trainingTypeState  = true;
                this.trainingTypeCommentState = true;
                this.startDateState = true;
                this.endDateState   = false;
            }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                this.trainingTypeState  = true;
                this.trainingTypeCommentState = true;
                this.startDateState = true;
                this.endDateState   = true;
                this.startTimeState = true;
                this.endTimeState   = false;
            }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                this.trainingTypeState  = true;
                this.trainingTypeCommentState = true;
                this.startDateState = true;
                this.endDateState   = true;
                this.endTimeState   = true;
                this.startTimeState = false;
            // }else if(this.selectedExpiryDate == ""){
            //     this.trainingTypeState  = true;
            //     this.trainingTypeCommentState = true;
            //     this.startDateState = true;
            //     this.endDateState   = true;
            //     this.endTimeState   = true;
            //     this.startTimeState = true;
            //     this.showSaveWarning = true;
            //     this.expiryDateState = false;

            }else{
                this.confirmedSave(); 
            }
        }

        public confirmedSave() {            
            this.showSaveWarning = false;
            this.trainingTypeState  = true;
            this.trainingTypeCommentState = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
            this.expiryDateState = true;

            const timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
            const startDate = Vue.filter('convertDate')(this.selectedStartDate,this.selectedStartTime, 'StartTime',timezone);
            const endDate =   Vue.filter('convertDate')(this.selectedEndDate,this.selectedEndTime,'EndTime',timezone);

            const body = {
                trainingTypeId: this.selectedTrainingType?this.selectedTrainingType.id:0,
                startDate: startDate,
                endDate: endDate,                      
                trainingCertificationExpiry: this.selectedExpiryDate,
                comment: this.selectedTrainingTypeComment,
                id: this.formDataId,
                timezone: timezone
            } 
            if(this.selectedComment) body['note'] = this.selectedComment;
            this.$emit('submit', body, this.isCreate);                  
                
        }

        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){
            if(this.isCreate){
                if((this.selectedTrainingType && this.selectedTrainingType.id) ||
                    this.selectedStartDate || this.selectedEndDate || this.selectedComment ||
                    this.selectedExpiryDate || this.selectedTrainingTypeComment ||
                    this.selectedStartTime || this.selectedEndTime) return true;
                return false;
            }else{
                if((this.originalTrainingType && this.selectedTrainingType && (this.originalTrainingType.id != this.selectedTrainingType.id)) ||
                    (this.originalStartDate != this.selectedStartDate)|| 
                    (this.originalEndDate != this.selectedEndDate) ||
                    (this.originalExpiryDate != this.selectedExpiryDate) ||
                    (this.originalTrainingTypeComment != this.selectedTrainingTypeComment) ||
                    (this.originalStartTime != this.selectedStartTime) ||
                    (this.originalComment != this.selectedComment) || 
                    (this.originalEndTime != this.selectedEndTime)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){  
            this.clearTrainingSelections();
            this.$emit('cancel');
        }

        public clearTrainingSelections(){
            this.selectedTrainingType = {} as trainingInfoType | undefined;
            this.selectedTrainingTypeComment = '';
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.selectedComment ='';

            this.trainingTypeState  = true;
            this.trainingTypeCommentState = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true;
            this.expiryDateState = true;            
        }

        public addTimeCheckBoxChanged() {
            Vue.nextTick(()=>{                
                if(this.addTime){
                    if(this.isCreate) this.selectedEndDate = this.selectedStartDate;
                }
                else{
                    this.selectedStartTime = '';
                    this.selectedEndTime = '';
                }            
            })
        }

        public startTimePicked(){
            this.startDateState = true;
            this.endDateState = true;
            if(this.isCreate) this.selectedEndDate = this.selectedStartDate;
        }

        get isFullDay(){    
            if(this.selectedStartTime == '' && this.selectedEndTime == '')
                return true
            else if(this.selectedStartDate && this.selectedEndDate){
                const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00')//+":00.000Z";
                const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00')//+":00.000Z";
                return  Vue.filter('isDateFullday')(startDate,endDate)
            }else
                return false           
        }

        public commentFormat(value) {
			return value.slice(0,100);
		}

        refreshExpiryDate(){
            this.refresh = true;
            this.setExpiryDate()
            setTimeout(() => this.refresh = false, 500)
        }

        public isRotatingTraining(trainingType){
            const yearsInDays = [365, 730, 1095, 1461, 1826, 2191, 2556, 2922, 3287, 3652]; 
            return trainingType.rotating || !yearsInDays.includes(trainingType.validityPeriod);               
        }

        public setExpiryDate() {
            Vue.nextTick(() => {
                const timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
                if(this.selectedEndDate && this.selectedTrainingType?.validityPeriod){                    
                    if(!this.isRotatingTraining(this.selectedTrainingType)){
                        const years = Math.floor(this.selectedTrainingType.validityPeriod /365) -1;
                        this.selectedExpiryDate = moment.tz(this.selectedEndDate, timezone).endOf('year').add(years,'year').format();
                    }else{
                        this.selectedExpiryDate = moment.tz(this.selectedEndDate, timezone).add(this.selectedTrainingType.validityPeriod, 'days').format();
                    }
                }else{
                    this.selectedExpiryDate = ''
                }
            })
        }
    }
</script>

<style scoped>
    td {
        margin: 0rem 0.5rem 0.1rem 0rem;
        padding: 0rem 0.5rem 0.1rem 0rem;        
        background-color: white ;
    }
    .refresh-expiry-date:focus{       
        outline: none !important;
        box-shadow: none;
    }
</style>