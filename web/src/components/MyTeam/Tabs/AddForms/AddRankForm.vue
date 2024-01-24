<template>
    <div>
        <b-table-simple small borderless style="margin:0">
            <b-tbody>
                <b-tr class="bg-warning">
                    <b-td>   
                        <b-tr class="mt-0 bg-white">   
                            <b class="ml-3 h6 p-0 m-0 " v-if="!selectedStartDate || !selectedEndDate" >Full/Partial Day: </b>                          
                            <b class="ml-3 px-1" style="background-color: #e8b5b5" v-else-if="isFullDay" >Full Day: </b> 
                            <b class="ml-3 px-1" style="background-color: #aed4bc" v-else >Partial Day: </b>                            
                        </b-tr>
                        <b-tr >
                            <b-form-group style="margin: 0.05rem 0 0 0.5rem;width: 17rem"> 
                                <b-form-select
                                    tabindex="1"
                                    size = "sm"
                                    v-model="selectedRank"
                                    :state = "rankState?null:false">
                                        <b-form-select-option :value="''">
                                            Select a rank*
                                        </b-form-select-option>
                                         <b-form-select-option
                                            v-for="courtAdminRank in courtAdminRankList" 
                                            :key="courtAdminRank.id"
                                            :value="courtAdminRank.name">
                                                {{courtAdminRank.name}}
                                        </b-form-select-option>    
                                </b-form-select>
                            </b-form-group>
                        </b-tr>
                        <b-tr>
                            <b-badge v-if="selectedRank && selectedRank == userToEdit.rank" class="ml-2" variant="warning"> This is the User's Permanent Rank! </b-badge>
                        </b-tr>
                        
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> From: </label>
                        <b-input-group  style="padding 0; margin:0 ;width: 10rem">                            
                            <b-form-datepicker
                                tabindex="2"
                                class="mb-1"
                                size="sm"
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
                                    tabindex="4"
                                    v-model="addTime" 
                                    @change="addTimeCheckBoxChanged" 
                                    size="sm" 
                                    class="mt-1 mx-0">
                                    <span class="h6 mx-0 px-0">Add Time</span>                                
                                </b-form-checkbox>
                            </b-input-group>                        
                            <b-input-group  style="width: 4.2rem; margin-left: auto;">                                                        
                                <b-form-input v-if="addTime"
                                    tabindex="5"
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
                            </b-input-group>                            
                        </b-row>
                    </b-td>
                    <b-td>
                        <label class="h6 m-0 p-0"> To: </label>
                        <b-form-datepicker
                            tabindex="3"
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            v-model="selectedEndDate"
                            placeholder="End Date*"
                            :state = "endDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            locale="en-US">
                        </b-form-datepicker> 
                        <b-input-group v-if="addTime" style="width: 4.2rem; margin-left: auto;">
                            <b-form-input
                                tabindex="6"
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
                        </b-input-group>
                    </b-td>
                    <b-td >
                        <b-button                                    
                            style="margin: 1.7rem .5rem 0 0 ; padding:0 .5rem 0 .5rem; "
                            variant="secondary"
                            @click="closeForm()">
                            Cancel
                        </b-button>   
                        <b-button                                    
                            style="margin: 1.7rem 0 0 0; padding:0 0.7rem 0 0.7rem; "
                            variant="success"                        
                            @click="saveForm()">
                            Save
                        </b-button>  
                    </b-td>
                </b-tr>
                <b-tr  class="m-0 p-0">
                    <b-td colspan="4">
                        <b class="p-0 h6" style="margin: 0.75rem 0.5rem 0 0.7rem; float:left;">Comment:</b>
                        <b-form-group class="p-0 mr-2 mt-1 mb-0" style="float:left; width: 40rem">                        
                            <b-form-input
                                tabindex="7"
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

        <b-modal v-model="showCancelWarning" id="bv-modal-rank-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 v-if="isCreate" class="mb-0 text-light"> Unsaved New Acting Rank </h2>                
                <h2 v-else class="mb-0 text-light"> Unsaved Acting Rank Changes </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-rank-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseForm()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-rank-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>              
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import {teamMemberInfoType} from '@/types/MyTeam';
    import {locationInfoType, commonInfoType} from '@/types/common';
    import * as _ from 'underscore';

    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");

    import { actingRankJsontype } from '@/types/MyTeam/jsonTypes';

    @Component
    export default class AddRankForm extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public commonInfo!: commonInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @Prop({required: true})
        formData!: actingRankJsontype;

        @Prop({required: true})
        isCreate!: boolean;

        selectedRank = '';
        rankState = true;      

        selectedStartDate = '';
        startDateState = true; 

        selectedEndDate = '';
        endDateState = true;

        selectedStartTime = '';
        startTimeState = true;

        selectedEndTime = '';
        endTimeState = true;
        
        originalRank = '';
        originalStartDate = '';
        originalEndDate = '';
        originalStartTime = '';
        originalEndTime = '';

        originalComment = '';
        selectedComment = '';

        addTime = false;

        formDataId = 0;

        showCancelWarning = false;

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
        
        mounted()
        {    
            this.clearSelections();
            if(this.formData.id) this.extractFormInfo();  
        }

        public extractFormInfo(){            
            this.formDataId = this.formData.id? this.formData.id:0;

            this.originalRank = this.selectedRank = this.formData.rank;
            this.originalStartDate = this.selectedStartDate = this.formData.startDate.substring(0,10)            
            this.originalEndDate = this.selectedEndDate =  this.formData.endDate.substring(0,10)

            const displayTime = Vue.filter('isDateFullday')(this.formData.startDate,this.formData.endDate)
            this.addTime = !displayTime;
            this.originalStartTime = this.selectedStartTime = displayTime? '' :this.formData.startDate.substring(11,16)            
            this.originalEndTime = this.selectedEndTime = displayTime? '' :this.formData.endDate.substring(11,16)
            this.originalComment = this.selectedComment = this.formData.comment? this.formData.comment :''
        }

        public saveForm(){
                this.rankState  = true;
                this.endDateState   = true;
                this.startDateState = true;
                this.startTimeState = true;
                this.endTimeState   = true;
                const isFullDay = this.isFullDay

                if(!this.selectedRank){
                    this.rankState  = false;
                }else if(this.selectedStartDate == ""){
                    this.rankState  = true;
                    this.startDateState = false;
                }else if(this.selectedEndDate == ""){
                    this.rankState  = true;
                    this.startDateState = true;
                    this.endDateState   = false;
                }else if(this.selectedEndTime == "" && this.selectedStartTime != ""){
                    this.rankState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.startTimeState = true;
                    this.endTimeState   = false;
                }else if(this.selectedStartTime == "" && this.selectedEndTime != ""){
                    this.rankState  = true;
                    this.startDateState = true;
                    this.endDateState   = true;
                    this.endTimeState   = true;
                    this.startTimeState = false;
                }else{
                    this.rankState  = true;
                    this.endDateState   = true;
                    this.startDateState = true;
                    this.startTimeState = true;
                    this.endTimeState   = true;

                    const startDate = Vue.filter('convertDate')(this.selectedStartDate,this.selectedStartTime, 'StartTime',this.location.timezone);
                    const endDate =   Vue.filter('convertDate')(this.selectedEndDate,this.selectedEndTime,'EndTime',this.location.timezone);

                    const body = {
                        rank: this.selectedRank,
                        startDate: startDate,
                        endDate: endDate,                      
                        isFullDay: isFullDay,
                        id: this.formDataId,
                        timezone: this.location.timezone
                    }
                    if(this.selectedComment) body['comment'] = this.selectedComment;
                    this.$emit('submit', body, this.isCreate);                  
                }
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



        public closeForm(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseForm();
        }

        public isChanged(){
            if(this.isCreate){
                if((this.selectedRank) ||
                    this.selectedStartDate || this.selectedEndDate ||  this.selectedComment ||                  
                    this.selectedStartTime || this.selectedEndTime) return true;
                return false;
            }else{
                if((this.originalRank && this.selectedRank && (this.originalRank != this.selectedRank)) ||
                    (this.originalStartDate != this.selectedStartDate)|| 
                    (this.originalEndDate != this.selectedEndDate) ||
                    (this.originalStartTime != this.selectedStartTime) || 
                    (this.originalComment != this.selectedComment) ||
                    (this.originalEndTime != this.selectedEndTime)) return true;
                return false;
            }
        }

        public confirmedCloseForm(){  
            this.clearSelections();
            this.$emit('cancel');
        }

        public clearSelections(){
            this.selectedRank = '';
            this.selectedEndDate = '';
            this.selectedStartDate = '';
            this.selectedStartTime = '';
            this.selectedEndTime = '';
            this.rankState  = true;
            this.endDateState   = true;
            this.startDateState = true;
            this.startTimeState = true;
            this.endTimeState   = true; 
            this.selectedComment = '';           
        }

        get isFullDay(){    
            if(this.selectedStartTime == '' && this.selectedEndTime == '')
                return true
            else if(this.selectedStartDate && this.selectedEndDate){
                const startDate = this.selectedStartDate+"T"+(this.selectedStartTime?this.selectedStartTime:'00:00')//+":00.000Z";
                const endDate =   this.selectedEndDate+"T"+(this.selectedEndTime?this.selectedEndTime:'00:00')//+":00.000Z";
                return Vue.filter('isDateFullday')(startDate,endDate)
            }else
                return false           
        }

        public commentFormat(value) {
			return value.slice(0,100);
		}

        get courtAdminRankList(){
        return _.sortBy(this.commonInfo.courtAdminRankList, 'id')
    }
    }
</script>

<style scoped>
    td {
        margin: 0rem 0.35rem 0.1rem 0rem;
        padding: 0rem 0.35rem 0.1rem 0rem;
        
        background-color:white ;
    }
</style>