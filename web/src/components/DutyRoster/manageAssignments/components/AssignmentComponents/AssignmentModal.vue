<template>
    <div>
        <b-modal v-model="showModal.show" no-close-on-backdrop hide-footer centered header-class="bg-primary text-light" size="lg">
            <template v-slot:modal-header>                
                <div class="h3 my-auto text-light"> {{courtAdminName}} </div>
                <div>
                    <div class="h4 text-center ml-2 p-0 mb-0">{{shiftDate}}</div>             
                    <div class="h3 text-center text-warning ml-2 p-0 mb-0"> {{shiftStartTime}} - {{shiftEndTime}}</div>                        
                </div>
                <b-button
                    v-if="hasPermissionToAddAssignDuty && hasPermissionToEditDuty"
                    class=""                    
                    @click="manageDuties();"
                    variant="success"> 
                    Manage Duties
                </b-button>
                <b-button
                    variant="dark"
                    class="text-white px-3"
                    @click="closeModal()">
                    Close
                </b-button>

            </template>

            <b-alert :show="errMsg!=''" dismissible @dismissed="errMsg='';"  variant="danger"> 
            <div v-html="errMsg"></div></b-alert>
            <loading-spinner v-if="loadingData" />
            <b-card v-else no-body style="font-size: 14px;user-select: none;" >	                
                <div>                    
                    <b-card no-body border-variant="light" bg-variant="white">
                        <b-table
                            class="mb-0"
                            :items="dutyBlocksExtra"
                            :fields="fields"
                            head-row-variant="primary"
                            striped
                            borderless
                            small
                            sort-by="startTimeString"
                            responsive="sm">  

                            <template v-slot:head(duty)="data" >
                                <div style="">
                                    <div style="float: left; margin:.15rem .25rem 0  0; font-size:14px">
                                        {{data.label}}
                                    </div>
                                    
                                </div>
                            </template>

                            <template v-slot:cell(duty)="data" >                               
                                <div v-if="data.index==0">
                                    <b-form-select
                                        size="sm"
                                        v-model="selectedDuty"
                                        @input="dutySelected"                                        
                                    >
                                        <b-form-select-option 
                                            v-for="opt,inx in availabeDutySlots" 
                                            :key="'duty-'+inx"
                                            :style="{color: opt.color.colorCode}"                                             
                                            :value="opt">
                                            {{ opt.code + (opt.name? ' ('+opt.name+') ':' ') + opt.start + '-'+opt.end }}
                                        </b-form-select-option>
                                    </b-form-select>
                                </div>
                                <div v-else :style="{color:data.item.color}">
                                    <!-- <b v-if="data.item.isOvertime">*</b> -->
                                    {{data.item.dutyType.replace('Role','').replace('Assignment','').replace('EscortRun','Transport') +' '+ data.item.dutySubType}}
                                </div>
                                
                            </template>

                            <template v-slot:cell(note)="data" >
                                <div  v-if="data.index==0">
                                    <b-form-input
                                        v-model="selectedComment"                                        
                                        size="sm"
                                        type="text"                                        
                                    ></b-form-input>
                                </div>                                
                                <div v-else>
                                    <span v-if="data.item.isOvertime" class="text-overtime-shift">(Overtime) </span>
                                    {{data.item.dutyNotes}}
                                </div>
                            </template>

                            <template v-slot:cell(startTime)="data" >
                                <div  v-if="data.index==0">
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
                                </div>
                                <div v-else>{{ data.value }}</div>
                            </template>

                            <template v-slot:cell(endTime)="data" >
                                <div  v-if="data.index==0">
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
                                </div>
                                <div v-else>{{ data.value }}</div>
                            </template>

                            <template v-slot:cell(edit)="data" >          
                                <b-button v-if="hasPermissionToAddAssignDuty && data.index==0"
                                    style="width:2.1rem;"
                                    :disabled="!selectedDuty['dutyId']" 
                                    class="my-0"
                                    size="sm"
                                    variant="transparent" 
                                    @click="addNewDuty()"
                                    v-b-tooltip.hover                                
                                    :title="selectedDuty['dutyId']?'Assign':''"
                                ><b-icon-plus-square variant="success" scale="1.5" /></b-button>
                                <b-button v-if="hasPermissionToEditDuty && data.index!=0"                                
                                    style="width:1.2rem;float:right" 
                                    class="ml-1 mr-0 my-0 py-0"
                                    size="sm"
                                    variant="transparent" 
                                    @click="confirmUnassignDutySlot(data.item)"
                                    v-b-tooltip.hover                                
                                    title="Unassign"
                                    ><b-icon
                                        icon="person-x-fill" 
                                        font-scale="1.25" 
                                        variant="danger" 
                                        style="transform: translate(-8px,0);"/>
                                </b-button>
                                <b-button v-if="hasPermissionToEditDuty && data.index!=0" 
                                    style="width:.75rem;float:right" 
                                    class="mx-1 my-0 py-0" 
                                    size="sm"
                                    variant="transparent" 
                                    @click="editDutySlotInfo(data)"
                                    v-b-tooltip.hover                                
                                    title="Edit"
                                    ><b-icon
                                        icon="pencil-square" 
                                        font-scale="1.25" 
                                        variant="primary" 
                                        style="transform: translate(-8px,0);"/>
                                </b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Le-Date-'+data.item.startTime.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="dutyFormColor" style="border:2px solid">                                   
                                    <duty-slot-form 
                                        :dutySlot="data.item" 
                                        :dutyBlocks="dutyBlocks"     
                                        :duties="duties" 
                                        :date="dutyDate"
                                        :courtAdminName="courtAdminName"
                                        :dutyDate="dutyDate"
                                        :courtAdminAvailabilityArray="courtAdminAvailabilityArray"                                                                         
                                        v-on:save="assignDuty" 
                                        v-on:cancel="closeDutySlotForm" />
                                </b-card>
                            </template>
                        </b-table>                            
                    </b-card> 
                </div>                
            </b-card>

            <b-card class="border-0 mb-0" no-body>
                <b-row style="margin:0.75rem 0 -0.65rem auto; ">
                    <div
                        class="mx-1 p-0"
                        v-for="color in Object.keys(dutyColorsCode)" 
                        :key="color">
                        <b-row class="m-0 p-0">
                            <div :style="{backgroundColor:dutyColorsCode[color], width:'0.7rem', height:'0.7rem', borderRadius:'25px', margin:'0.1rem .2rem 0 0'}"/>
                            <div style="font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0"> {{color.replace('Role','').replace('Assignment','').replace('EscortRun','Transport')}}</div>
                        </b-row>
                    </div>
                </b-row>
            </b-card>
        </b-modal>

        <all-assignments-management-modal :showModal="showManageAssignments" :shiftDate="shiftDate" v-on:change="getData"/>
        <unassign-duty-modal :showModal="showUnassignConfirm" @unassign="assignDuty(true)" :dutySlot="dutySlotToUnassign"/>
        <confirm-overtime-modal :showModal="showOvertimeConfirm" @assign="assignDuty(false)" :courtAdminName="courtAdminName" />
    </div>    
</template>

<script lang="ts">
    import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    import "@store/modules/AssignmentScheduleInformation";
	const assignmentState = namespace("AssignmentScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import DutySlotForm from './ManageDuty/DutySlotForm.vue';
    import AllAssignmentsManagementModal from './ManageAssignment/AllAssignmentsManagementModal.vue'
    import UnassignDutyModal from './ManageDuty/UnassignDutyModal.vue'
    import ConfirmOvertimeModal from './ManageDuty/ConfirmOvertimeModal.vue'
    import { manageAssignmentDutyInfoType, assignDutyInfoType, dutySlotInfoType, manageAssignmentsScheduleInfoType } from '@/types/DutyRoster';
    import { locationInfoType, userInfoType } from '@/types/common';

    @Component({
        components: {
            DutySlotForm,
            AllAssignmentsManagementModal,
            UnassignDutyModal,
            ConfirmOvertimeModal
        }
    })
    export default class AssignmentModal extends Vue {

        @Prop({required: true})
        showModal!: {show: boolean};

        @Prop({required: true})
        courtAdminId!: string;

        @Prop({required: true})
        courtAdminName!: string;

        @Prop({required: true})
        courtAdminAvailabilityArray!: number[]|null;

        @Prop({required: true})
        shiftStartTime!: string;

        @Prop({required: true})
        shiftEndTime!: string;

        @Prop({required: true})
        shiftDate!: string;

        @Prop({required: true})
        dutyDate!: string;

        @Prop({required: true})
        scheduleInfo!: manageAssignmentsScheduleInfoType[];

        @Prop({required: true})
        dutyBlocks!: manageAssignmentDutyInfoType[];

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @assignmentState.Action
        public UpdateEditDutyModalID!: (newEditDutyModalID: string) => void 

        @assignmentState.State
        public manageDutiesModalID!: string;

        @assignmentState.Action
        public UpdateManageDutiesModalID!: (newManageDutiesModalID: string) => void 

        @Watch('showModal.show')
        openModal(newValue){
            // console.log(newValue)
            if(newValue){
                this.loadDutyData(this.dutyDate)
            }
        }

        shiftDay=0
        hasPermissionToEditDuty=false
        hasPermissionToAddAssignDuty=false
        loadingData=false

        availabeDutySlots: any[] =[]
        duties: any[] =[]

        dutyBlocksExtra: manageAssignmentDutyInfoType[] = [];
        startOfDay = ""
        selectedDuty = {}
        selectedComment =""
        selectedStartTime=""
        selectedEndTime=""
        startTimeState=true
        endTimeState=true
        errMsg=""

        dutyFormColor=""
        isEditOpen=false
        latestEditData;

        dutyColorsCode= {}

        dutySlotToUnassign = {} as  manageAssignmentDutyInfoType
        showUnassignConfirm={show: false}
        
        showOvertimeConfirm={show: false}

        showManageAssignments={show: false}

        fields = [      
            {key:'duty',         label:'Duty',        sortable:false, thStyle:'width:35%;', tdClass: 'border-top'  },
            {key:'startTime',    label:'Start Time',  sortable:false, thStyle:'width:12%;', tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endTime',      label:'End Time',    sortable:false, thStyle:'width:12%;', tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'note',         label:'Note',        sortable:false, thStyle:'width:33%;', tdClass: 'border-top', thClass:'h6 align-middle'  },
            {key:'edit',         label:'',            sortable:false, thStyle:'width:8%;', tdClass: 'border-top', thClass:'',}       
        ];

        mounted(){
            this.dutyColorsCode=Vue.filter('WSColors')()
            this.shiftDay = moment(this.shiftDate).tz(this.location.timezone).day();
            this.hasPermissionToEditDuty = this.userDetails.permissions.includes("EditDuties");            
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
        }

        public getDutyRosters(startDate, endDate){            
            const url = 'api/dutyroster?locationId='+this.location.id+'&start='+startDate+'&end='+endDate;
            return this.$http.get(url).then(res=> {return res.data}, e => console.log(e))
        }

        async loadDutyData(date){
            this.loadingData=true
            this.selectedDuty = {}
            this.selectedComment =""
            this.selectedStartTime=""
            this.selectedEndTime=""
            const endDate = moment.tz(date, this.location.timezone).endOf('day').utc().format();
            const startDate = moment.tz(date, this.location.timezone).startOf('day').utc().format();            
            const duties = await this.getDutyRosters(startDate, endDate)
            // console.log( duties)
            this.extractAvailableDutySlots(duties, date);
            this.extractDutyBlocksExtra();
            const manageModalID=this.dutyDate.slice(0,10)+'-'+this.courtAdminId;
            if(this.manageDutiesModalID==manageModalID) this.manageDuties()
            this.loadingData=false            
        }

        public extractAvailableDutySlots(duties, date){
            const availabeDutySlots: any[] =[]
            const startOfday = moment.tz(date, this.location.timezone).startOf('day').format();
            this.startOfDay = startOfday
            for(const duty of duties){
                const dutyRangeBin = Vue.filter('getTimeRangeBins')(duty.startDate, duty.endDate, this.location.timezone);
                // console.log(dutyRangeBin)
                let dutyArray = Vue.filter('fillInArray')(Array(96).fill(0), 1 , dutyRangeBin.startBin,dutyRangeBin.endBin);
                
                for(const dutySlot of duty.dutySlots){
                    const dutySlotRangeBin = Vue.filter('getTimeRangeBins')(dutySlot.startDate, dutySlot.endDate, this.location.timezone);
                    const dutySlotArray = Vue.filter('fillInArray')(Array(96).fill(0), 1 , dutySlotRangeBin.startBin,dutySlotRangeBin.endBin);
                    dutyArray =Vue.filter('subtractUnionOfArrays')(dutyArray,dutySlotArray)
                }
                // console.log(dutyArray)
                const availableSlots = this.findAvailableSlots(dutyArray, startOfday, duty)  
                availabeDutySlots.push(...availableSlots)          
            }
            this.duties=duties;
            this.availabeDutySlots=availabeDutySlots
            // console.log(availabeDutySlots)
        }

        public findAvailableSlots(dutyArray, startOfday, duty){

            const availableDutySlots: any[] = []
            const discontinuity = Vue.filter('findDiscontinuity')(dutyArray);            
            const iterationNum = Math.floor((Vue.filter('sumOfArrayElements')(discontinuity) +1)/2);
            //console.log(iterationNum)
            for(let i=0; i< iterationNum; i++){
                const inx1 = discontinuity.indexOf(1)
                let inx2 = discontinuity.indexOf(2)
                discontinuity[inx1]=0
                if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
                // console.error(inx1 + ' ' +inx2)
                const dutySlotRange = Vue.filter('convertTimeRangeBinsToTime')(startOfday, inx1, inx2)
                const dutyType = duty.assignment?.lookupCode?.type
                availableDutySlots.push(
                    {...dutySlotRange, 
                        dutyId: duty.id,
                        type: dutyType,
                        code: duty.assignment?.lookupCode?.code,
                        name: duty.assignment?.name,
                        comment: duty.comment? duty.comment:"",
                        color: Vue.filter('getColorByType')(dutyType),
                        assignmentNotes: duty.assignment?.comment
                    } )
            }

            return  availableDutySlots            
        }

        public manageDuties(){
            const manageModalID=this.dutyDate.slice(0,10)+'-'+this.courtAdminId;
            this.UpdateManageDutiesModalID(manageModalID)
            this.showManageAssignments.show = true
        }

        public getData(){
            this.$emit('change')
        }

        public confirmUnassignDutySlot(dutyslotinfo){
            this.errMsg=""
            this.dutySlotToUnassign = dutyslotinfo
            this.showUnassignConfirm.show = true;
        }

        public extractDutyBlocksExtra(){
            const newDuty = {} as manageAssignmentDutyInfoType;
            const dutyBlocks = JSON.parse(JSON.stringify(this.dutyBlocks))
            const shiftDutyBlocks = dutyBlocks.filter(block =>
                block.dutyType != "Leave" && 
                block.dutyType != "Loaned" && 
                block.dutyType != "Training" &&
                block.dutyType != "Unavailable" 
            )
            const sortedShiftDutyBlocks = _.sortBy(shiftDutyBlocks,'startTime')
            newDuty['_rowVariant']= 'info'
            this.dutyBlocksExtra = [newDuty]
            this.dutyBlocksExtra.push(...sortedShiftDutyBlocks)
        }
        
        public dutySelected(){
            this.selectedStartTime = this.selectedDuty['start']
            this.selectedEndTime = this.selectedDuty['end']
            this.selectedComment = this.selectedDuty['comment']
            this.startTimeState=true
            this.endTimeState=true
            this.errMsg=""
            // console.log(this.selectedDuty)
        }

        public editDutySlotInfo(data) {
            this.errMsg=""
            if(this.isEditOpen){
                // location.href = '#Ds-Time-'+this.latestEditData.item.startTimeString.substring(0,10)
                this.dutyFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                // Vue.nextTick().then(()=>{
                //     location.href = '#Ds-Time-'+this.latestEditData.item.startTimeString.substring(0,10)
                // });
            }   
        }

        public closeDutySlotForm() {                     
            this.dutyFormColor = 'secondary';
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }
        
        public checkStates(){            
            this.startTimeState = !(
                (this.selectedStartTime<this.selectedDuty['start']) ||
                (this.selectedStartTime>this.selectedEndTime)
            )
            this.endTimeState=!(
                (this.selectedEndTime>this.selectedDuty['end']) ||
                (this.selectedStartTime>this.selectedEndTime)
            )
            return (this.startTimeState && this.endTimeState)
        }

        public addNewDuty(){
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
                this.assignDuty(false)
            }
        }

        public assignDuty(unassign, dutyid?, slotid?, start?, end?) {
            
            this.closeDutySlotForm()
            this.errMsg=""
            const dutyId = dutyid? dutyid : (unassign? this.dutySlotToUnassign.dutyId :this.selectedDuty['dutyId'])

            const dutyInfo = this.duties.filter(duty => duty.id==dutyId)[0]
            let dutySlots: dutySlotInfoType[] = [];
            
            // console.log(dutyInfo)
            

            if(unassign && dutyInfo){
                for(const dutySlot of dutyInfo.dutySlots){
                    delete dutySlot.assignmentLookupCode
                    delete dutySlot.concurrencyToken
                }                
                dutySlots.push(...dutyInfo.dutySlots)                
                dutySlots = dutySlots.filter(slot => slot.id != this.dutySlotToUnassign.id )
                this.postDutyChanges(dutyInfo, dutySlots)
            }
            else if(this.checkStates() && dutyInfo){
                
                const selectedStartTime=start? Vue.filter('autoCompleteTime')(start):Vue.filter('autoCompleteTime')(this.selectedStartTime)
                const selectedEndTime=end? Vue.filter('autoCompleteTime')(end):Vue.filter('autoCompleteTime')(this.selectedEndTime)
                const dutySlotID=slotid? slotid:null
                
                if(this.checkCourtAdminConflict(selectedStartTime, selectedEndTime, dutySlotID)){
                    const duties=this.dutyBlocks.map(d=>
                    d.dutyType=='Training' || d.dutyType=='Leave' || d.dutyType=='Loaned'?
                    ('<br/>'+d.startTime+'-'+d.endTime+' '+d.dutyType+' '+d.dutySubType):'')
                    this.errMsg="The selected duty has a conflict with the CourtAdmin's other duties on this date."+duties.join(' ')
                    return
                }
                
                for(const dutySlot of dutyInfo.dutySlots){
                    delete dutySlot.assignmentLookupCode
                    delete dutySlot.concurrencyToken
                }

                dutySlots.push(...dutyInfo.dutySlots)
                dutySlots = dutySlots.filter(slot => slot.id != dutySlotID )


                const dutyArray = Vue.filter('startEndTimesToArray')(null, 1, this.dutyDate.slice(0,10), selectedStartTime, selectedEndTime, this.location.timezone)
                let allShiftsArray = Vue.filter('initArray')();
                // console.log(dutyArray)
                // console.log(this.scheduleInfo)
                for(const sch of this.scheduleInfo){                    
                    const shiftArray = Vue.filter('startEndTimesToArray')(null, 1, this.dutyDate.slice(0,10), sch.startTime, sch.endTime, this.location.timezone)
                    // console.log(shiftArray)
                    const dutyOnShiftArray = Vue.filter('unionArrays')(dutyArray, shiftArray)
                    if(Vue.filter('sumOfArrayElements')(dutyOnShiftArray)>0){
                        const bins = Vue.filter('getArrayRangeBins')(dutyOnShiftArray)
                        allShiftsArray = Vue.filter('fillInArray')(allShiftsArray, 1, bins.startBin, bins.endBin)
                        // console.log(bins)
                        const dutyOnShiftTime = Vue.filter('convertTimeRangeBinsToTime')(this.startOfDay, bins.startBin, bins.endBin)
                        // console.log(dutyOnShiftTime)
                        dutySlots.push({  
                            id: dutySlotID,                                              
                            startDate: dutyOnShiftTime.startTime,
                            endDate: dutyOnShiftTime.endTime,
                            dutyId: dutyInfo.id,
                            courtAdminId: this.courtAdminId,
                            shiftId: null,
                            timezone: dutyInfo.timezone,
                            isNotRequired: false,
                            isNotAvailable: false,
                            isClosed: false,
                            isOvertime: sch.overtime? true: false                           
                        })
                    }
                }

                const reminderOfDutyOnShiftsArray = Vue.filter('subtractUnionOfArrays')(dutyArray, allShiftsArray)
                // console.log(reminderOfDutyOnShiftsArray)
                const discontinuity = Vue.filter('findDiscontinuity')(reminderOfDutyOnShiftsArray)                
                const iterationNum = Math.floor((Vue.filter('sumOfArrayElements')(discontinuity) +1)/2);

                for(let i=0; i< iterationNum; i++){
                    const inx1 = discontinuity.indexOf(1)
                    let inx2 = discontinuity.indexOf(2)
                    discontinuity[inx1]=0
                    if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
                    const dutyOnShiftTime = Vue.filter('convertTimeRangeBinsToTime')(this.startOfDay, inx1, inx2)
                    // console.log(dutyOnShiftTime)
                    dutySlots.push({  
                        id: dutySlotID,                                              
                        startDate: dutyOnShiftTime.startTime,
                        endDate: dutyOnShiftTime.endTime,
                        dutyId: dutyInfo.id,
                        courtAdminId: this.courtAdminId,
                        shiftId: null,
                        timezone: dutyInfo.timezone,
                        isNotRequired: false,
                        isNotAvailable: false,
                        isClosed: false,
                        isOvertime: true                           
                    })
                }
       
                // console.log(dutySlots)
                this.postDutyChanges(dutyInfo, dutySlots)
            }
        }

        public postDutyChanges(dutyInfo, dutySlots){
            const body: assignDutyInfoType[] = [{
                id: dutyInfo.id,
                startDate: dutyInfo.startDate,
                endDate: dutyInfo.endDate,
                locationId: dutyInfo.locationId,
                assignmentId: dutyInfo.assignmentId,
                dutySlots: dutySlots,
                timezone: dutyInfo.timezone,
                comment: this.selectedComment
            }];

            //console.log(body)
            this.loadingData=true

            if(body?.length>0){
                const url = 'api/dutyroster';
                this.$http.put(url, body )
                    .then(response => {
                        if(response.data){
                            this.$emit('change')
                        }
                    }, err => {
                        const errMsg = err.response.data.error;
                        this.errMsg = errMsg;
                        this.loadingData=false                            
                    })
            }
        }

        public checkCourtAdminConflict(startTime, endTime, blkID){
            const timezone = this.location.timezone
            const date = this.dutyDate.slice(0,10)
            const dutyslotArray = Vue.filter('startEndTimesToArray')(null, 1, date, startTime, endTime, timezone)          
            // console.log(dutyslotArray)
            let blkArray = Array(96).fill(0)
            for(const blk of this.dutyBlocks){
                // console.log(blk)
                if(blk.id && blk.id == blkID) continue
                blkArray = Vue.filter('startEndTimesToArray')(blkArray, 1, date, blk.startTime, blk.endTime, timezone)
            }

            // console.log(blkArray)
            const conflictArray = Vue.filter('unionArrays')(blkArray, dutyslotArray)
            // console.log(conflictArray)
            const hasConflict = Vue.filter('sumOfArrayElements')(conflictArray)
            // console.log(hasConflict)
            return (hasConflict>0)
        }
        
        public timeFormat(value , event){
            return Vue.filter('timeFormat')(value , event)
        }

        public closeModal(){
            this.UpdateEditDutyModalID("")
            this.showModal.show=false
        }
       
    }
</script>

