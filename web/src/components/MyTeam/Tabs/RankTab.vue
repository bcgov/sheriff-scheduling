<template>
    <div>
        <b-card  style="height:400px;overflow: auto;" no-body> 
            <b-card no-body>                                       
                <h2 v-if="rankError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="rankErrorMsgDesc" style="word-break: break-word;white-space: normal;"  variant="danger"> {{rankErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="rankError = false" /></b-badge></h2>
            </b-card>
            
            <b-card v-if="!addNewRankForm && hasPermissionToEditUsers">
                <b-button size="sm" variant="success" @click="addNewRank"> <b-icon icon="plus" /> Add </b-button>
            </b-card> 

            <b-card v-if="addNewRankForm" id="addRankForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-rank-form :formData="{}" :isCreate="true" v-on:submit="saveRank" v-on:cancel="closeRankForm" />              
            </b-card>
            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedActingRanks.length">
                        <span class="text-muted ml-4 my-5">No acting ranks have been assigned.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="assignedActingRanks"
                        :fields="fields"
                        head-row-variant="primary"
                        borderless
                        small
                        sort-by="startDate"
                        :sort-desc="true"
                        responsive="sm"
                        >  
                            <template v-slot:cell(isFullDay)="data" >
                                <span v-if="data.value">Full</span> 
                                <span v-else>Partial</span> 
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
                            <template v-slot:cell(edit)="data" >
                                <b-button  size="sm" variant="transparent" style="margin:0; padding:0; width:1.2rem; float: left;">
                                    <b-icon-chat-square-text-fill v-if="data.item.comment" v-b-tooltip.hover.left="data.item.comment"  class="mr-2" variant="info" font-scale=".99"/>                                       
                                </b-button>                                      
                                <b-button v-if="hasPermissionToEditUsers" class="my-0 py-0" size="sm" variant="transparent" @click="confirmDeleteRank(data.item)"><b-icon icon="trash-fill" font-scale="1.25" variant="danger"/></b-button>
                                <b-button v-if="hasPermissionToEditUsers" class="my-0 py-0" size="sm" variant="transparent" @click="editRank(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'RK-Date-'+data.item.startDate.substring(0,10)" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-rank-form :formData="data.item" :isCreate="false" v-on:submit="saveRank" v-on:cancel="closeRankForm" />
                                </b-card>
                            </template>
                            
                    </b-table> 
                </b-card> 
            </div>      
        </b-card>


        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Delete Acting Rank</h2>                    
            </template>
            <h4>Are you sure you want to delete the "{{rankToDelete.rank}}" acting rank?</h4>
            <b-form-group style="margin: 0; padding: 0; width: 20rem;"><label class="ml-1">Reason for Deletion:</label> 
                <b-form-select
                    size = "sm"
                    v-model="rankDeleteReason">
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
                <b-button variant="danger" @click="deleteRank()" :disabled="rankDeleteReason.length == 0">Confirm</b-button>
                <b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="cancelDeletion()"
                 >&times;</b-button>
            </template>
        </b-modal>
        <b-modal v-model="confirmOverride" id="bv-modal-confirm-override" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Conflicting Event</h2>                    
            </template>
            <h4>The following events conflict with this location</h4>
                <ul>
                    <li v-for="event in overlappingList"
                        :key="event"
                        class="mb-1"> {{event}}
                    </li>
                </ul>
            <h4 class="mt-4 mb-0 text-danger">Do you want to override the conflicting event(s) listed above? </h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="saveRank(rankToSave, create, true)">Confirm</b-button>
                <b-button variant="primary" @click="cancelActingRankOverride()">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="cancelActingRankOverride()"
                >&times;</b-button>
            </template>
        </b-modal> 

    </div>
</template>

<script lang="ts">
    import { Component, Vue} from 'vue-property-decorator';
    import moment from 'moment-timezone';

    import { namespace } from 'vuex-class';
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");
    
    import AddRankForm from "./AddForms/AddRankForm.vue"
    
    import { userInfoType } from '@/types/common';
    import { teamMemberInfoType } from '@/types/MyTeam';
    import { actingRankJsontype } from '@/types/MyTeam/jsonTypes';
    
    @Component({
        components: {
            AddRankForm
        }        
    })    
    export default class RankTab extends Vue {

        @commonState.State
        public userDetails!: userInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        hasPermissionToEditUsers = false;
        addNewRankForm = false;
        rankError = false;
        rankErrorMsg = '';
        rankErrorMsgDesc = '';
        overlappingList = [] as string[];

        confirmDelete = false;
        confirmOverride = false;
        rankToDelete = {} as actingRankJsontype;

        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;
        rankDeleteReason = '';

        rankToSave = {};
        create = false;
        currentTime = '';
        
        assignedActingRanks: actingRankJsontype[] = [];


        fields =  
        [     
            {key:'isFullDay', label:'Type',sortable:false, tdClass: 'border-top', },       
            {key:'rank',      label:'Rank',sortable:false, tdClass: 'border-top',  }, 
            {key:'startDate', label:'Start Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'startTime', label:'Start Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},
            {key:'endDate',   label:'End Date',  sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'endTime',   label:'End Time',  sortable:false, tdClass: 'border-top', thClass:'h6 align-middle',},  
            {key:'edit',      label:'',    sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        mounted()
        {
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");                         
            this.extractActingRanks();          
        }

        public extractActingRanks ()
        {
            this.assignedActingRanks = this.userToEdit.actingRank? this.userToEdit.actingRank: [];
            
            
            for(const inx in this.assignedActingRanks)
            {
                if(Vue.filter('isDateFullday')(this.assignedActingRanks[inx].startDate,this.assignedActingRanks[inx].endDate)){ 
                    this.assignedActingRanks[inx]['isFullDay'] = true;
                    this.assignedActingRanks[inx]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedActingRanks[inx]['isFullDay'] = false;
                    this.assignedActingRanks[inx]['_cellVariants'] = {isFullDay:'success'}                    
                }
                const timezone = this.assignedActingRanks[inx].timezone? this.assignedActingRanks[inx].timezone : 'UTC';
                this.currentTime = moment(new Date()).tz(timezone).format();
                this.assignedActingRanks[inx].startDate = moment(this.assignedActingRanks[inx].startDate).tz(timezone).format();
                this.assignedActingRanks[inx].endDate = moment(this.assignedActingRanks[inx].endDate).tz(timezone).format();
                this.currentTime = moment(new Date()).tz(timezone).format();            
                this.assignedActingRanks[inx]['_rowVariant'] = '';
                if(this.assignedActingRanks[inx].endDate < this.currentTime)
                        this.assignedActingRanks[inx]['_rowVariant'] = 'info';                            
            }
        }

        public addNewRank(){
            if(this.isEditOpen){
                location.href = '#RK-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'
            }else{
                this.addNewRankForm = true;
                this.$nextTick(()=>{location.href = '#addRankForm';})
            }
        }

        public confirmDeleteRank(rank) {
            //console.log(rank)
            this.rankToDelete = rank;           
            this.confirmDelete=true; 
        }
        
        public cancelDeletion() {
            this.confirmDelete = false;
            this.rankDeleteReason = '';
        }

        public deleteRank(){
            if (this.rankDeleteReason.length) {
                this.confirmDelete = false;
                this.rankError = false; 
                const url = 'api/courtAdmin/actingrank?id='+this.rankToDelete.id+'&expiryReason='+this.rankDeleteReason;
                this.$http.delete(url)
                    .then(response => {
                        //console.log(response)
                        // console.log('delete success')
                        const index = this.assignedActingRanks.findIndex(assignedrank=>{if(assignedrank.id == this.rankToDelete.id) return true;})
                        if(index>=0) this.assignedActingRanks.splice(index,1);
                        this.$emit('change');
                    }, err=>{
                        const errMsg = err.response.data.error;
                        this.rankErrorMsg = errMsg;//.slice(0,60) + (errMsg.length>60?' ...':'');
                        this.rankErrorMsgDesc = errMsg;
                        this.rankError = true;
                        location.href = '#RankError'
                    });
                    this.rankDeleteReason = '';
            }
        }

        public editRank(data){
            //console.log('edit location')
            //console.log(data)
            if(this.addNewRankForm){
                location.href = '#addRankForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#RK-Date-'+this.latestEditData.item.startDate.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                Vue.nextTick().then(()=>{
                    location.href = '#RK-Date-'+this.latestEditData.item.startDate.substring(0,10)
                });
            }                        
        }

        public saveRank(body, iscreate, overrideConflicts){
            this.rankError  = false;                        
            body['courtAdminId']= this.userToEdit.id;
            const method = iscreate? 'post' :'put';
            const url = overrideConflicts?'api/courtAdmin/actingrank?overrideConflicts=true':'api/courtAdmin/actingrank'  
            const options = { method: method, url:url, data:body}
            this.$http(options)
                .then(response => {                    
                    if(iscreate) 
                        this.addToAssignedRankList(response.data);
                    else
                        this.modifyAssignedRankList(response.data);
                    if (overrideConflicts){
                        this.cancelActingRankOverride();
                        this.closeRankForm();
                        this.$emit('refresh', this.userToEdit.id)
                    }
                    this.closeRankForm();
                }, err=>{   
                    // console.log(err.response.data);
                    const errMsg = err.response.data.error;
                    this.rankErrorMsg = errMsg;                    
                    this.rankErrorMsgDesc = errMsg;
                    if (errMsg.toLowerCase().includes("overlaps")) {
                        //console.log("overlap")
                        this.overlappingList = this.rankErrorMsg.split('||');
                        this.rankToSave = body;
                        this.create = iscreate;
                        this.confirmOverride = true;
                    } else {
                        this.rankError = true;
                        location.href = '#RankError'
                    }
                });                
        }

        public addToAssignedRankList(addedRankInfo){
            
            const timezone = addedRankInfo.timezone? addedRankInfo.timezone : 'UTC';
            const assignedActingRank: actingRankJsontype =
            {
                id: addedRankInfo.id,
                courtAdminId : addedRankInfo.courtAdminId,    
                rank: addedRankInfo.rank,
                timezone: timezone,
                startDate: moment(addedRankInfo.startDate).tz(timezone).format(),
                endDate: moment(addedRankInfo.endDate).tz(timezone).format(), 
                comment: addedRankInfo.comment,              
            }
            
            if(Vue.filter('isDateFullday')(assignedActingRank.startDate,assignedActingRank.endDate)){ 
                assignedActingRank['isFullDay'] = true;
                assignedActingRank['_cellVariants'] = {isFullDay:'danger'}                 
            }else{
                assignedActingRank['isFullDay'] = false;
                assignedActingRank['_cellVariants'] = {isFullDay:'success'}                    
            }
            this.currentTime = moment(new Date()).tz(timezone).format();

            assignedActingRank['_rowVariant'] = '';
            if(assignedActingRank.endDate < this.currentTime)
                assignedActingRank['_rowVariant'] = 'info';                

            this.assignedActingRanks.push(assignedActingRank); 
            this.$emit('change');                     
        }

        public modifyAssignedRankList(modifiedRankInfo){

            const index = this.assignedActingRanks.findIndex(assignedRank =>{ if(assignedRank.id == modifiedRankInfo.id) return true})
            if(index>=0){  
                const timezone =  modifiedRankInfo.timezone?  modifiedRankInfo.timezone : 'UTC';          
                this.assignedActingRanks[index].rank =  modifiedRankInfo.rank;
                this.assignedActingRanks[index].startDate = moment(modifiedRankInfo.startDate).tz(timezone).format();
                this.assignedActingRanks[index].endDate = moment(modifiedRankInfo.endDate).tz(timezone).format();                
                this.assignedActingRanks[index].timezone = timezone ;
                this.assignedActingRanks[index].comment = modifiedRankInfo.comment? modifiedRankInfo.comment :'' ;

                if(Vue.filter('isDateFullday')( this.assignedActingRanks[index].startDate, this.assignedActingRanks[index].endDate)){ 
                    this.assignedActingRanks[index]['isFullDay'] = true;
                    this.assignedActingRanks[index]['_cellVariants'] = {isFullDay:'danger'}                 
                }else{
                    this.assignedActingRanks[index]['isFullDay'] = false;
                    this.assignedActingRanks[index]['_cellVariants'] = {isFullDay:'success'}                    
                }
                this.currentTime = moment(new Date()).tz(timezone).format();
                this.assignedActingRanks[index]['_rowVariant'] = '';
                if(this.assignedActingRanks[index].endDate < this.currentTime)
                    this.assignedActingRanks[index]['_rowVariant'] = 'info';

                this.$emit('change');
            } 
        }

        public cancelActingRankOverride() {
            this.confirmOverride = false;
        }

        public closeRankForm(){        
            this.addNewRankForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }
        
    }
</script>

<style scoped>
    .card {
        border: white;
    }
    td {
        margin: 0rem 1rem 0.1rem 0rem;
        padding: 0rem 1rem 0.1rem 0rem;
        
        background-color: whitesmoke ;
    }
</style>