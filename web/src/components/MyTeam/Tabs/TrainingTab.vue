<template>
    <div>
        <b-card v-if="trainingTabDataReady"  style="height:400px;overflow: auto;" no-body>
            <b-card id="TrainingError" no-body>
                <h2 v-if="trainingError" class="mx-1 mt-0"><b-badge v-b-tooltip.hover :title="trainingErrorMsgDesc" style="word-break: break-word;white-space: normal;" variant="danger"> {{trainingErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="trainingError = false" /></b-badge></h2>
            </b-card>
            <b-card v-if="!addNewTrainingForm && hasPermissionToEditUsers">
                <div style="display: flex; justify-content: space-between;">
                    <b-button size="sm" variant="success" @click="addNewTraining"><b-icon icon="plus"/> Add </b-button>
                    <router-link v-if="userDetails.permissions.includes('GenerateReports')" :to="`/reports?sheriffId=${userToEdit.id}`" class="btn btn-sm btn-info">
                        <b-icon icon="file-earmark-text"/> Go to Reports
                    </router-link>
                </div>
            </b-card>

            <b-card v-if="addNewTrainingForm" id="addTrainingForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-training-form :formData="{}" :editable="true" :isCreate="true" :trainingTypeInfoList="trainingTypeInfoList" v-on:submit="saveTraining" v-on:cancel="closeTrainingForm" />              
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedTrainings.length">
                        <span class="text-muted ml-4 my-5">No trainings have been assigned.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="filteredAssignedTrainings"
                        :fields="fields"
                        head-row-variant="primary"
                        borderless
                        small
                        sort-by="startDate"
                        :sort-desc="true"
                        responsive="sm"
                        >  
                            <template v-slot:head(editTraining)>
                                <b-form-group style="margin: 0; padding: 0; width: 7rem;"> 
                                    <b-form-select
                                        size = "sm"
                                        v-model="selectedPastTrainings">
                                            <b-form-select-option value="pastyear">
                                                Past Year
                                            </b-form-select-option>
                                            <b-form-select-option value="allyears">
                                                View All
                                            </b-form-select-option>     
                                    </b-form-select>
                                </b-form-group>
                            </template>

                            <template v-slot:cell(isFullDay)="data" >
                                <span v-if="data.value" class="fullDay">Full</span> 
                                <span v-else class="partialDay">Partial</span> 
                            </template>
                            <template v-slot:cell(trainingType)="data" >
                                <span 
                                    class="text-primary"
                                    v-b-tooltip.hover.right                                
                                    :title="data.item.comment?data.item.comment:data.value.code"> 
                                        {{data.value.description}}
                                </span>
                            </template>
                            <template v-slot:cell(startDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(endDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(startTime)="data" >
                                <span v-if="!data.item.isFullDay">{{data.item.startDate | beautify-time}}</span> 
                            </template>
                            <template v-slot:cell(endTime)="data" >
                                <span v-if="!data.item.isFullDay">{{data.item.endDate | beautify-time }}</span> 
                            </template>
                            <template v-slot:cell(expiryDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(editTraining)="data" >
                                <b-button  size="sm" variant="transparent" style="margin:0; padding:0; width:1.2rem; float: left;">
                                    <b-icon-chat-square-text-fill v-if="data.item.note" v-b-tooltip.hover.left="data.item.note"  class="mr-2" variant="info" font-scale="0.99"/>                                       
                                </b-button>
                                <b-button :disabled="!hasPermissionToEditCompletedTraining && (data.item['_rowVariant']?true:false)" v-if="hasPermissionToEditUsers" class="my-0 py-0" size="sm" variant="transparent" @click="editTraining(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>                                       
                                <b-button class="my-0 py-0" size="sm" variant="transparent" :disabled="!hasPermissionToRemoveCompletedTraining && (data.item['_rowVariant']?true:false)" v-if="hasPermissionToEditUsers" @click="confirmDeleteTraining(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>                                
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Tr-Date-'+data.item.startDate.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-training-form :formData="data.item" :editable="true" :isCreate="false" :trainingTypeInfoList="trainingTypeInfoList" v-on:submit="saveTraining" v-on:cancel="closeTrainingForm" />
                                </b-card>
                            </template>                            
                    </b-table> 
                </b-card> 
                <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
                    <template v-slot:modal-title>
                            <h2 class="mb-0 text-light">Confirm Delete Training</h2>                    
                    </template>
                    <h4>Are you sure you want to delete the "{{trainingToDelete.trainingType?trainingToDelete.trainingType.description:''}}" training?</h4>
                    <b-form-group style="margin: 0; padding: 0; width: 20rem;"><label class="ml-1">Reason for Deletion:</label> 
                        <b-form-select
                            size = "sm"
                            v-model="trainingDeleteReason">
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
                    <template v-slot:modal-footer>
                        <b-button variant="danger" @click="deleteTraining()" :disabled="trainingDeleteReason.length == 0">Confirm</b-button>
                        <b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
                    </template>            
                    <template v-slot:modal-header-close>                 
                        <b-button variant="outline-warning" class="text-light closeButton" @click="cancelDeletion()"
                        >&times;</b-button>
                    </template>
                </b-modal>
            </div>                                     
        </b-card>

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
    import moment from 'moment-timezone';
    import { namespace } from 'vuex-class';

    import {teamMemberInfoType, userTrainingInfoType} from '@/types/MyTeam'; 
    import { trainingTypeJson } from '@/types/common/jsonTypes';    
    import { trainingInfoType, userInfoType } from '@/types/common';    
    import AddTrainingForm from './AddForms/AddTrainingForm.vue'   
    
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/TeamMemberInformation";    
    const TeamMemberState = namespace("TeamMemberInformation");

    @Component({
        components: {
            AddTrainingForm
        }        
    }) 
    export default class TrainingTab extends Vue {

        @commonState.State
        public userDetails!: userInfoType;
        
        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;
        
        hasPermissionToEditUsers = false;
        hasPermissionToEditCompletedTraining = false;
        hasPermissionToRemoveCompletedTraining = false;
        trainingTabDataReady = false;
        addNewTrainingForm = false;
        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;

        trainingToSave = {};
        create = false;

        trainingError = false;
        trainingErrorMsg = '';
        trainingErrorMsgDesc = '';
        overlappingList = [] as string[];
        updateTable = 0;

        confirmDelete = false;
        trainingToDelete = {} as userTrainingInfoType;
        
        assignedTrainings: userTrainingInfoType[] = [];
        trainingTypeInfoList: trainingInfoType[] =[];

        currentTime = '';
        lastyear = '';
        selectedPastTrainings = 'pastyear';
        timezone = 'UTC';
        trainingDeleteReason = '';

        errorText =''
        openErrorModal=false

        fields =  
        [     
            {key:'isFullDay',  label:'Day',sortable:false, tdClass: 'border-top', thClass:'align-middle'}, 
            {key:'trainingType', label:'Type',sortable:false, tdClass: 'border-top', thClass:'align-middle'}, 
            {key:'startDate', label:'Start Date',  sortable:false, tdClass: 'border-top', thClass:'align-middle',},
            {key:'startTime', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endDate',   label:'End Date',  sortable:false, tdClass: 'border-top', thClass:'align-middle',},
            {key:'endTime',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'expiryDate',label:'Expiry Date',  sortable:false, tdClass: 'border-top', thClass:'align-middle',},    
            {key:'editTraining',      label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");
            this.hasPermissionToRemoveCompletedTraining = this.userDetails.permissions.includes("RemovePastTraining"); 
            this.hasPermissionToEditCompletedTraining = this.userDetails.permissions.includes("EditPastTraining");                                
            this.timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
            this.currentTime = moment(new Date()).tz(this.timezone).format();
            // console.log(this.currentTime)  
            this.lastyear = moment().subtract(1,'year').tz(this.timezone).format(); 
            // console.log(this.lastyear)   
            this.trainingTabDataReady = false;
            this.extractTrainings();                        
        }
   
        public loadTrainingTypes(){
            const url = 'api/managetypes?codeType=TrainingType';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractTrainingTypeInfo(response.data)                                                                
                    }                                   
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }  
                })
        }

        public extractTrainingTypeInfo(trainingTypeListJson){
            let trainingType: trainingTypeJson;
            for(trainingType of trainingTypeListJson){
                const trainingTypeInfo = {} as trainingInfoType;
                trainingTypeInfo.id = trainingType.id;
                trainingTypeInfo.code = trainingType.code;
                trainingTypeInfo.validityPeriod = trainingType.validityPeriod
                trainingTypeInfo.description = trainingType.description;
                trainingTypeInfo.rotating = trainingType.rotating;
                this.trainingTypeInfoList.push(trainingTypeInfo)
            }
            this.trainingTabDataReady = true;
        }

        public extractTrainings (){
            const url = `api/sheriff/${this.userToEdit.id}/trainings`;

            this.$http.get(url).then((response) => {
                if(response.data)
                    for(const training of response.data)
                    {
                        const assignedTraining = {} as userTrainingInfoType;
                        assignedTraining.id = training.id;
                        assignedTraining.trainingType = training.trainingType;
                        assignedTraining.trainingTypeId = training.trainingTypeId;
                        assignedTraining.startDate = moment(training.startDate).tz(this.timezone).format();
                        assignedTraining.endDate = moment(training.endDate).tz(this.timezone).format();
                        assignedTraining.expiryDate = training.trainingCertificationExpiry;
                        assignedTraining.comment = training.comment;
                        assignedTraining.note = training['note'];
                        assignedTraining['_rowVariant'] = '';
                        if(assignedTraining.endDate < this.currentTime)
                            assignedTraining['_rowVariant'] = 'info';

                        if(Vue.filter('isDateFullday')(assignedTraining.startDate,assignedTraining.endDate)){ 
                            assignedTraining.isFullDay = true;                                        
                        }else{
                            assignedTraining.isFullDay = false;                    
                        }
                        
                        this.assignedTrainings.push(assignedTraining)
                    }
                this.loadTrainingTypes();
            });
        }

        get filteredAssignedTrainings(){
            const pastYear = this.selectedPastTrainings == 'pastyear'?this.lastyear:'1900-01-01T00:00:00Z';
            return this.assignedTrainings.filter(training =>{if(training.endDate> pastYear ) return true})
        }

        public addNewTraining(){
            if(this.isEditOpen){
                location.href = '#Tr-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'
            }else
            {
                this.addNewTrainingForm = true;
                this.$nextTick(()=>{location.href = '#addTrainingForm';})
            }
        }

        public confirmDeleteTraining(training) {
            this.trainingToDelete = training;           
            this.confirmDelete=true; 
        }

        public cancelDeletion() {
            this.confirmDelete = false;
            this.trainingDeleteReason = '';
        }

        public deleteTraining(){
            if (this.trainingDeleteReason.length) {
                this.confirmDelete = false;
                this.trainingError = false;
                const url = 'api/sheriff/training?id='+this.trainingToDelete.id+'&expiryReason='+this.trainingDeleteReason;
                this.$http.delete(url)
                    .then(response => {
                        const index = this.assignedTrainings.findIndex(assignedtraining=>{if(assignedtraining.id == this.trainingToDelete.id) return true;})
                        if(index>=0) this.assignedTrainings.splice(index,1);
                        this.$emit('change');
                    }, err=>{
                        const errMsg = err.response.data.error;
                        this.trainingErrorMsg = errMsg;
                        this.trainingErrorMsgDesc = errMsg;
                        this.trainingError = true;                       
                    });
                    this.trainingDeleteReason = '';
            }            
        }

        public editTraining(training){
            if(this.addNewTrainingForm){
                location.href = '#addTrainingForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Tr-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !training.detailsShowing){
                training.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = training
                Vue.nextTick().then(()=>{
                    location.href = '#Tr-Date-'+this.latestEditData.item.startDate.substring(0,10)
                });
            }  
        }

        public saveTraining(body, iscreate){
            this.trainingError   = false; 
            body['sheriffId']= this.userToEdit.id;
            const method = iscreate? 'post' :'put';            
            
            const url = 'api/sheriff/training?allowConflictingEvents=true';
            
            const options = { method: method, url:url, data:body}
            
            this.$http(options)
                .then(response => {
                    if(iscreate) 
                        this.addToAssignedTrainingList(response.data);
                    else
                        this.modifyAssignedTrainingList(response.data);
                    this.closeTrainingForm();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.trainingErrorMsg = errMsg;
                    this.trainingErrorMsgDesc = errMsg;
                    if (errMsg.toLowerCase().includes("overlaps")) {
                        //console.log("overlap")
                        this.overlappingList = this.trainingErrorMsg.split('||');
                        this.trainingToSave = body;
                        this.create = iscreate;
                    } else {
                        this.trainingError = true;
                        location.href = '#TrainingError'
                    }
                });                
        }

        public modifyAssignedTrainingList(modifiedTrainingInfo){            

            const index = this.assignedTrainings.findIndex(assignedtraining =>{ if(assignedtraining.id == modifiedTrainingInfo.id) return true})
            if(index>=0){
                this.assignedTrainings[index].id =  modifiedTrainingInfo.id;
                this.assignedTrainings[index].startDate = moment(modifiedTrainingInfo.startDate).tz(this.timezone).format();
                this.assignedTrainings[index].endDate = moment(modifiedTrainingInfo.endDate).tz(this.timezone).format();
                this.assignedTrainings[index].trainingTypeId = modifiedTrainingInfo.trainingTypeId; 
                const trainingType = this.getTrainingType(this.assignedTrainings[index].trainingTypeId);               
                this.assignedTrainings[index].trainingType = trainingType;
                this.assignedTrainings[index].trainingName = trainingType.description;
                this.assignedTrainings[index].expiryDate = modifiedTrainingInfo.trainingCertificationExpiry? modifiedTrainingInfo.trainingCertificationExpiry:'';
                this.assignedTrainings[index].comment = modifiedTrainingInfo.comment?modifiedTrainingInfo.comment:'';
                this.assignedTrainings[index].note = modifiedTrainingInfo.note?modifiedTrainingInfo.note:'';
                if(Vue.filter('isDateFullday')( this.assignedTrainings[index].startDate, this.assignedTrainings[index].endDate)){ 
                    this.assignedTrainings[index]['isFullDay'] = true;                 
                }else{
                    this.assignedTrainings[index]['isFullDay'] = false;                   
                }

                this.assignedTrainings[index]['_rowVariant'] = '';
                if(this.assignedTrainings[index].endDate < this.currentTime)
                    this.assignedTrainings[index]['_rowVariant'] = 'info';

                this.$emit('change');
            } 
        }

        public addToAssignedTrainingList(addedTrainingInfo)
        {
            const assignedTraining: userTrainingInfoType =
            {
                id: addedTrainingInfo.id,
                trainingType: addedTrainingInfo.trainingType,
                trainingTypeId: addedTrainingInfo.trainingTypeId,
                sheriffId : addedTrainingInfo.sheriffId,
                startDate: moment(addedTrainingInfo.startDate).tz(this.timezone).format(),
                endDate: moment(addedTrainingInfo.endDate).tz(this.timezone).format(),
                expiryDate: addedTrainingInfo.trainingCertificationExpiry,
                comment: addedTrainingInfo.comment,
                note: addedTrainingInfo.note              
            }
            
            if(Vue.filter('isDateFullday')(assignedTraining.startDate,assignedTraining.endDate)){ 
                assignedTraining['isFullDay'] = true;                 
            }else{
                assignedTraining['isFullDay'] = false;                   
            }

            assignedTraining['_rowVariant'] = '';
            if(assignedTraining.endDate < this.currentTime)
                assignedTraining['_rowVariant'] = 'info';

            this.assignedTrainings.push(assignedTraining); 
            this.$emit('change');                     
        }

        public closeTrainingForm() {                     
            this.addNewTrainingForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public getTrainingType(trainingTypeId){
            const index = this.trainingTypeInfoList.findIndex(training=>{if(training.id == trainingTypeId)return true})
            if(index>=0){
                return this.trainingTypeInfoList[index]
            }
            else
                return {} as trainingInfoType;
        }       
       
    }
</script>

<style scoped>
    .card {
        border: white;
    }
    .fullDay{
        padding: 0.25rem 1rem 0.25rem 1rem; 
        background-color: rgb(232, 181, 181); 
        color: rgb(82, 78, 78);
        font-weight: bold;        
    }
    .partialDay{
        padding: 0.25rem 0.25rem 0.25rem 0.25rem; 
        background-color: rgb(174, 212, 188); 
        color:rgb(82, 78, 78); 
        font-weight: bold;       
    }
</style>