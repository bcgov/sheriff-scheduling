<template>
    <div v-if="scheduleInfo && isMounted" id="shiftBox" ref="shiftBox" :key="updateBoxes">             

        <div style="font-size: 7pt; border:none;" class="m-0 p-0" >
            <!-- {{ courtAdminEvent }} -->
            <b-row v-if="courtAdminEvent.type == 'Shift'" class="mx-1" style="">

                <div style="text-align: left; font-weight: 700; width:30%; ">
                    <div v-if="courtAdminEvent.startTime && courtAdminEvent.endTime">
                        <div>
                            <span style="font-size: 6.5pt; margin-right:0.4rem; ">In: </span> {{courtAdminEvent.startTime}} 
                        </div>
                        <div>
                            <span style="font-size: 6pt;" >Out:</span> {{courtAdminEvent.endTime}}
                        </div>
                    </div>                    
                    
                    <b-row class="m-0" v-if="courtAdminEvent.startTime && courtAdminEvent.endTime">
                        <div class="m-0" >
                            <b-form-checkbox
                                style="transform:translate(0px,-1px);"                                
                                v-model="checked"
                                @change="cardSelected">
                            </b-form-checkbox>
                        </div>
                        <div class="m-0" >
                            <b-button  
                                class="p-0"
                                style="transform:translate(-4px,0px);" 
                                size="sm"
                                variant="transparent" 
                                @click="editDuties(courtAdminEvent)"
                                v-b-tooltip.hover                                
                                title="Edit"
                                ><b-icon-pencil-square font-scale="1.27" variant="primary" />
                            </b-button>
                        </div>
                    </b-row>
                </div>

                <div style=" width:70%;">
                    <div :style="{fontSize:(currentTime?'10pt':'6pt'), border:'none'}" class="m-0 p-0" v-for="duty,inx in sortEvents(courtAdminEvent.duties)" :key="'duty-name-'+inx+'-'+duty.startTime">                                
                        <div :style="'color: ' + duty.color">
                            <b v-if="duty.isOvertime">*</b>
                            <font-awesome-icon v-else-if="duty.dutyType=='Training'" style="font-size: 0.5rem;" icon="graduation-cap" />
                            <font-awesome-icon v-else-if="duty.dutyType=='Leave'" style="font-size: 0.5rem;" icon="suitcase" />
                            <font-awesome-icon v-else-if="duty.dutyType=='Loaned'" style="font-size: 0.5rem;transform:rotate(180deg);" icon="sign-out-alt" />
                            <b> {{duty.startTime}}-{{duty.endTime}}</b> {{ duty.dutyType.replace('Role','').replace('Assignment','').replace('EscortRun','Transport') }} 
                            <span v-if="duty.dutyType!='Training' && duty.dutyType!='Leave' && duty.dutyType!='Loaned'" > {{duty.dutySubType}} </span>
                        </div>                            
                    </div>                    
                </div>                    
            </b-row>

            <div v-else-if="courtAdminEvent.type == 'Unavailable'" class="text-center">                                         
                <div  class="m-0 p-0" style="">                    
                    <div :style="{background:getColor(courtAdminEvent.subType)}" class="bdg text-white">Unavailable</div>
                </div>
            </div>
            
            <div v-else-if="courtAdminEvent.type == 'Leave'" class="text-center">                                         
                <div  class="m-0 p-0" style="">                    
                    <div :style="{background:getColor(courtAdminEvent.subType)}" class="bdg text-white">{{ courtAdminEvent.subType }} Leave</div>
                </div>
            </div> 

            <div v-else-if="courtAdminEvent.type == 'Training'" class="text-center" style="display:inline;">                  
                <div style="" class="m-0 p-0">
                    <div class="bg-training-leave text-white bdg" v-b-tooltip.hover :title="courtAdminEvent.subType">
                        <font-awesome-icon style="font-size: 0.9rem;" icon="graduation-cap" /> Training
                    </div>
                </div> 
            </div>   

            <div v-else-if="courtAdminEvent.type == 'Loaned'" class="text-center" style="display:inline;">  
                <div style="" class="m-0 p-0"> 
                    <div class="bg-loaned text-white bdg" v-b-tooltip.hover :title="'Loaned to '+courtAdminEvent.location">
                        <font-awesome-icon  style="transform:translate(0,0) rotate(180deg); font-size: .9rem;"  icon="sign-out-alt" /> Loaned
                    </div>
                </div>                     
            </div>                
        </div>

        
        <assignment-modal 
            :showModal="showEditAssignmentsDetails" 
            :courtAdminName="courtAdminName"
            :courtAdminId="courtAdminId"
            :shiftDate="shiftDate"
            :shiftEndTime="shiftEndTime"
            :shiftStartTime="shiftStartTime"
            :dutyDate="dutyDate"
            :scheduleInfo="scheduleInfo"
            :dutyBlocks="dutyBlocks"
            :courtAdminAvailabilityArray="courtAdminAvailabilityArray"
            v-on:change="getData"
            />
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    import "@store/modules/AssignmentScheduleInformation";
	const assignmentState = namespace("AssignmentScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import { userInfoType, locationInfoType } from '@/types/common';
    import { manageAssignmentDutyInfoType, manageAssignmentsScheduleInfoType } from '@/types/DutyRoster';

    import AssignmentModal from './AssignmentComponents/AssignmentModal.vue';
    import { selectShiftInfoType } from '@/types/ShiftSchedule';

    @Component({
        components: {
            AssignmentModal
        }
    })
    export default class AssignmentCard extends Vue {

        @Prop({required: true})
        scheduleInfo!: manageAssignmentsScheduleInfoType[];

        @Prop({required: true})
        courtAdminId!: string;

        @Prop({required: true})
        courtAdminName!: string;

        @Prop({required: true})
        showAllDuties!: boolean;

        @Prop({required: true})
        cardDate!: string;

        @commonState.State
        public userDetails!: userInfoType;
        
        @commonState.State
        public location!: locationInfoType;

        @assignmentState.State
        public selectedShifts!: selectShiftInfoType[];

        @assignmentState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: selectShiftInfoType[]) => void  
        

        @assignmentState.State
        public editDutyModalID!: string;

        @assignmentState.Action
        public UpdateEditDutyModalID!: (newEditDutyModalID: string) => void 
        
        updateBoxes =0;
        hasPermissionToEditShifts = false;
        hasPermissionToEditDuty = false;
        hasPermissionToAddAssignDuty = false;
        isMounted = false; 
        
        editDutyError = false;
        editDutyErrorMsg = '';

        deleteErrorMsg = '';
        deleteError = false;

        showEditAssignmentsDetails = { show: false };

        isAssignmentDataMounted = false;

        addNewDutySlotForm = false;
        addFormColor = 'secondary';
        checked = false;

        isEditOpen = false;
        latestEditData;

        dutySlotToUnassign = {} as manageAssignmentDutyInfoType;

        assignmentName = 'assignment name!!!!!';
        shiftStartTime = '';
        shiftEndTime = '';
        shiftDate = '';
        dutyDate = '';
        currentTime='';

        dutyBlocks: manageAssignmentDutyInfoType[] = [];
        courtAdminAvailabilityArray: number[]|null=[]

        courtAdminEvent = {} as manageAssignmentsScheduleInfoType;

        mounted() { 
            this.isMounted = false;
            this.currentTime='';
            if(this.showAllDuties==false){
                if(moment().format('YYYY-MM-DD')==moment(this.cardDate).format('YYYY-MM-DD'))
                    this.currentTime=moment().format('HH:mm')
            }
            this.hasPermissionToEditShifts = this.userDetails.permissions.includes("EditShifts");
            this.hasPermissionToEditDuty = this.userDetails.permissions.includes("EditDuties");            
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");            
            this.extractCourtAdminEvents();
            this.isMounted = true;
            Vue.nextTick(()=>this.checkOpenModal()) 
        }
        
        public extractCourtAdminEvents(){            
            // if(this.scheduleInfo.length) console.log(this.scheduleInfo)
            this.courtAdminAvailabilityArray = null
            this.courtAdminEvent = {} as manageAssignmentsScheduleInfoType;
            const duties: manageAssignmentDutyInfoType[] = []            
            for(const courtAdminEvent of this.sortEvents(this.scheduleInfo)){
                if(courtAdminEvent.fullday){
                    this.courtAdminEvent=courtAdminEvent;
                    return
                }
                // console.error(courtAdminEvent.overtime)
                // console.log(courtAdminEvent.type)
                if(courtAdminEvent.type != 'Shift'){
                    // console.log(courtAdminEvent)
                    const subtype = (courtAdminEvent.type=='Leave'? courtAdminEvent.subType:courtAdminEvent.type)
                    // console.log(subtype)
                    duties.push({                    
                        startTime: courtAdminEvent.startTime,
                        endTime: courtAdminEvent.endTime, 
                        dutyType: courtAdminEvent.type,
                        dutySubType: courtAdminEvent.subType,
                        color: Vue.filter('subColors')(subtype)                                        
                    })
                }
                else if(courtAdminEvent.type == 'Shift' && courtAdminEvent.overtime){
                    // console.log('Overtime')
                    // console.log(courtAdminEvent.duties)
                    for(const duty of courtAdminEvent.duties){
                        duty.isOvertime=true
                        duty.color= Vue.filter('subColors')('overtime')
                        duties.push(duty)
                    }
                }
                else{
                    //console.log(courtAdminEvent)
                    this.courtAdminAvailabilityArray = Vue.filter('startEndTimesToArray')(
                        this.courtAdminAvailabilityArray, 1, 
                        courtAdminEvent.date.slice(0,10), 
                        courtAdminEvent.startTime, 
                        courtAdminEvent.endTime, 
                        this.location.timezone
                    )
                    duties.push(...courtAdminEvent.duties)
                    if(!this.courtAdminEvent.type){
                        this.courtAdminEvent=courtAdminEvent
                    }else{
                        const start = this.courtAdminEvent.startTime
                        const end = this.courtAdminEvent.endTime
                        this.courtAdminEvent.startTime = start < courtAdminEvent.startTime? start :courtAdminEvent.startTime;
                        this.courtAdminEvent.endTime = end > courtAdminEvent.endTime? end :courtAdminEvent.endTime;
                    }
                }
            }
            
            if(this.currentTime){
                this.courtAdminEvent.duties = duties.filter( duty =>  
                    duty.startTime && this.currentTime>=duty.startTime &&
                    duty.endTime && this.currentTime<=duty.endTime
                ) 
            }else
                this.courtAdminEvent.duties = duties;

            this.courtAdminEvent.allDuties = duties;
            
            //__Empty_Shift
            if(!this.courtAdminEvent.type && duties.length>0){
                this.courtAdminEvent={
                    date: this.dutyDate,
                    dayOffset: 0,
                    duties: duties,
                    endTime: "",
                    fullday: false,                    
                    location:"",
                    overtime:0,
                    startTime:"",
                    subType:"",
                    type:"Shift",
                    workSection:"",
                    workSectionColor:""
                }
            }
                        
            // console.log(this.courtAdminEvent)
            //console.log(duties)
            //console.log(this.courtAdminAvailabilityArray)
        }

        public checkOpenModal(){
            const editModalID=this.courtAdminEvent.date?.slice(0,10)+'-'+this.courtAdminId;
            if(this.editDutyModalID==editModalID) this.editDuties(this.courtAdminEvent)
        }

        public cardSelected(check){            
            // console.log(check)
            // console.log(this.scheduleInfo)  
            
            const a ={
                id:9,
                courtAdmin:'Alex',
                date: this.cardDate
            }
            
            const selectedCardsIds =  this.selectedShifts.map(shift => shift.id)
            let selectedCards = this.selectedShifts;
            for(const block of this.scheduleInfo){
                if(block.type=='Shift' && block.id){
                    // console.log(block)
                    if (check && !selectedCardsIds.includes(block.id))
                        selectedCards.push({
                            id: block.id,
                            courtAdmin: this.courtAdminName,
                            date: this.cardDate
                        });
                    else if (!check)
                        selectedCards = selectedCards.filter(sc => sc.id != block.id);                    
                }
            }
            this.UpdateSelectedShifts(selectedCards);
        }

        public editDuties(block: manageAssignmentsScheduleInfoType){ 
            // console.log(block)           
            this.isAssignmentDataMounted = false;
            this.shiftDate = Vue.filter('beautify-date-weekday')(block.date);
            this.shiftStartTime = block.startTime;
            this.shiftEndTime = block.endTime;
            this.dutyBlocks = block.allDuties?block.allDuties:[];  
            this.dutyDate = block.date;
            const editModalID=this.courtAdminEvent.date.slice(0,10)+'-'+this.courtAdminId;
            this.UpdateEditDutyModalID(editModalID)
            this.showEditAssignmentsDetails.show = true;           
            this.isAssignmentDataMounted = true;            
        }

        public sortEvents (events: any) {            
            const eventsCopy = JSON.parse(JSON.stringify(events))
            return _.sortBy(eventsCopy, "startTime");
        }

        public getColor(subtype){
            return Vue.filter('subColors')(subtype)
        }

        public getData(){
            this.$emit('change')
        }

    }
</script>

<style scoped>   

    .bdg{
        font-size: 10pt;
        margin: 0rem 0.2rem;
        border-radius: 5px;
        padding:0.5rem;
    }
    .card {
        border: white;
    }

</style>