
<template>
    <div>
        <b-card v-if="roleTabDataReady"  style="height:400px;overflow: auto;" >
            <b-card id="RoleError" no-body>
                <h2 v-if="roleError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="roleErrorMsgDesc"  variant="danger"> {{roleErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="roleError = false" /></b-badge></h2>
            </b-card>
            <b-card  v-if="!addNewRoleForm && hasPermissionToAssignRoles && hasPermissionToEditUsers">                
                <b-button v-if="allowChanges" size="sm" variant="success" :disabled="roles.length==0" @click="addNewRole"> <b-icon icon="plus" /> Add </b-button>
            </b-card>

            <b-card v-if="addNewRoleForm" id="addRoleForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                <add-role-form :formData="{}" :isCreate="true" :roleTypeInfoList="roles" v-on:submit="saveRole" v-on:cancel="closeRoleForm" />              
            </b-card>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignedRoles.length">
                    <span class="text-muted ml-4 mb-5">No roles have been assigned.</span>
                </b-card>

                <b-card v-else no-body border-variant="light" bg-variant="white">
                    <b-table
                        :items="filteredAssignedRoles"
                        :fields="roleFields"
                        :key="refreshTable"
                        head-row-variant="primary"
                        striped
                        borderless
                        small
                        sort-by="effectiveDate"
                        responsive="sm"
                        >
                            <template v-slot:head(editRole)>
                                <b-form-group style="margin: 0; padding: 0; width: 7.30rem;"> 
                                    <b-form-select
                                        size = "sm"
                                        v-model="selectedRoleView">
                                            <b-form-select-option value="active">
                                                Active Roles
                                            </b-form-select-option>
                                            <b-form-select-option value="history">
                                                History
                                            </b-form-select-option>     
                                    </b-form-select>
                                </b-form-group>
                            </template>  

                            <template v-slot:cell(effectiveDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(expiryDate)="data" >
                                <span>{{data.value | beautify-date}}</span> 
                            </template>
                            <template v-slot:cell(editRole)="data" >                                       
                                <span><b-button class="m-0"
                                                style="padding: 1px 2px 1px 2px;" 
                                                size="sm"
                                                v-b-tooltip.hover
                                                title="Expire" 
                                                variant="warning"
                                                v-if="hasPermissionToEditUsers && allowChanges" 
                                                @click="confirmDeleteRole(data.item)">
                                                <b-icon icon="clock" 
                                                        font-scale="1.25" 
                                                        variant="white"/>
                                </b-button></span>
                                <span><b-button class="my-0 py-0" 
                                                size="sm" 
                                                variant="transparent"
                                                v-if="hasPermissionToEditUsers && allowChanges" 
                                                @click="editRole(data)">
                                                <b-icon icon="pencil-square" 
                                                        font-scale="1.25" 
                                                        variant="primary"/>
                                </b-button></span> 
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'Ro-Date-'+data.item.effectiveDate.substring(0,10)" :border-variant="addFormColor" body-class="m-0 px-0 py-1" style="border:2px solid">
                                    <add-role-form :formData="data.item" :isCreate="false" :roleTypeInfoList="roles" v-on:submit="saveRole" v-on:cancel="closeRoleForm" />
                                </b-card>
                            </template>                            
                    </b-table> 
                </b-card>
            </div>                                        
        </b-card>

         <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Expire Role</h2>                    
            </template>            
            <h4>Are you sure you want to expire the "{{roleToDelete.desc}}" role?</h4>
            <b-form-group style="margin: 0; padding: 0; width: 20rem;"><label class="ml-1">Reason for Expiration:</label> 
                <b-form-select
                    size = "sm"
                    v-model="roleDeleteReason">
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
                <b-button variant="danger" @click="deleteRole()" :disabled="roleDeleteReason.length == 0">Confirm</b-button>
                <b-button variant="primary" @click="cancelDeletion()">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="cancelDeletion()"
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
    import moment from 'moment-timezone';    
    import { namespace } from 'vuex-class';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation"); 

    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");

    import AddRoleForm from './AddForms/AddRoleForm.vue';
    import { userRoleHistoryJsonType, userRoleJsonType } from '@/types/MyTeam/jsonTypes';
    import { locationInfoType, userInfoType } from '@/types/common';
    import {roleOptionInfoType, teamMemberInfoType, userRoleInfoType} from '@/types/MyTeam';

    @Component({
        components: {
            AddRoleForm
        }        
    }) 
    export default class RoleAssignmentTab extends Vue {

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;
        
        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        hasPermissionToEditUsers = false;
        hasPermissionToAssignRoles = false;
        roleTabDataReady = false;       

        refreshTable = 0;

        roles: roleOptionInfoType[] = []
        roleAssignError = false;

        rolesJson;
        historicRolesJson;
        addNewRoleForm = false;
        addFormColor = 'secondary';
        latestEditData;
        isEditOpen = false;
        allowChanges = true;

        roleError = false;
        roleErrorMsg = '';
        roleErrorMsgDesc = '';
        updateTable=0;

        confirmDelete = false;
        roleToDelete = {} as userRoleInfoType;
        assignedRoles: userRoleInfoType[] = [];
        historicAssignedRoles: userRoleInfoType[] = [];
        roleDeleteReason = '';
        selectedRoleView = 'active';

        errorText = ''
        openErrorModal=false;

        timezone = 'UTC';

        roleFields =  
        [           
            {key:'text',    label:'Role',sortable:false, tdClass: 'border-top',  }, 
            {key:'effectiveDate', label:'Effective Date',   sortable:false, tdClass: 'border-top', thClass:'',},
            {key:'expiryDate', label:'Expiry Date',      sortable:false, tdClass: 'border-top', thClass:'',}, 
            {key:'editRole', label:'', sortable:false, tdClass: 'border-top', thClass:'text-white',},       
        ];

        mounted()
        {
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");                         
            this.hasPermissionToAssignRoles = this.userDetails.permissions.includes("CreateAndAssignRoles");
            this.timezone = this.userToEdit.homeLocation? this.userToEdit.homeLocation.timezone :'UTC';
            this.roleTabDataReady = false;
            this.allowChanges = true;
            this.loadRoles();
        }
   
        public loadRoles(){
            const url = '/api/role';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.rolesJson = response.data
                        this.extractRoles();                        
                    }                                   
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }  
                })
        }
      
        public extractRoles(){
            this.roles=[];
            this.assignedRoles =[];
            this.roleAssignError = false;

            if (this.userToEdit.userRoles && this.userToEdit.userRoles.length>0) {
                let userRole: userRoleJsonType;
                for(userRole of this.userToEdit.userRoles) 
                {                    
                    this.assignedRoles.push({
                        text:userRole.role.name, 
                        desc: userRole.role.description, 
                        value:userRole.role.id.toString(), 
                        effectiveDate:moment(userRole.effectiveDate).tz(this.timezone).format(), 
                        expiryDate:userRole.expiryDate?moment(userRole.expiryDate).tz(this.timezone).format():''
                    })
                }
            }
            
            this.refreshTable++;
            this.populateRolesDropdown();
        }

        public populateRolesDropdown() {
            this.roles=[];
            const today = moment().tz(this.location.timezone);
            
            for(const role of this.rolesJson)
            {                
                const index = this.assignedRoles.findIndex(assignrole =>{if(assignrole.value == role.id) return true;else return false});
                if(index < 0)
                {             
                    this.roles.push({text:role.name, desc: role.description, value:role.id})           
                } else {
                    const expiryDate = this.assignedRoles[index].expiryDate;
                     
                    if (expiryDate.length && today.isAfter(moment(expiryDate).tz(this.location.timezone))) {
                        this.roles.push({text:role.name, desc: role.description, value:role.id})
                    }                   
                }
            }
            this.loadHistoricRoles();            
        }

        public loadHistoricRoles(){
            const url = '/api/audit/rolehistory?courtAdminId='+ this.userToEdit.id;
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.historicRolesJson = response.data
                        this.extractHistoricRoles();                        
                    }                                   
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }    
                })
        }

        public extractHistoricRoles(){
           
            this.historicAssignedRoles = [];
            // console.log(this.historicRolesJson)
            // console.log(this.rolesJson)

            if (this.historicRolesJson && this.historicRolesJson.length>0) {
                let userRole: userRoleHistoryJsonType;
                for(userRole of this.historicRolesJson) 
                {
                    if (userRole.oldValuesJson && userRole.oldValuesJson.ExpiryDate && userRole.keyValuesJson && userRole.keyValuesJson.RoleId) {
                        const roleId = userRole.keyValuesJson.RoleId;
                        const role = this.rolesJson.filter(role =>{if(role.id == roleId ) return true})[0]
                        // console.log(role)                   
                        this.historicAssignedRoles.push({
                            text:role.name, 
                            desc: role.description, 
                            value:roleId.toString(), 
                            effectiveDate:userRole.oldValuesJson.EffectiveDate? moment(userRole.oldValuesJson.effectiveDate).tz(this.timezone).format():'', 
                            expiryDate:userRole.oldValuesJson.ExpiryDate?moment(userRole.oldValuesJson.expiryDate).tz(this.timezone).format():''
                        })
                    }                     
                }
            }            
            this.roleTabDataReady = true;
        }

        get filteredAssignedRoles(){
            if (this.selectedRoleView == 'active') {
                this.allowChanges = true;
                return this.assignedRoles;
            } else {
                this.allowChanges = false;
                return this.historicAssignedRoles;
            }
        }
        
        public addNewRole(){
            if(this.isEditOpen){
                location.href = '#Ro-Date-'+this.latestEditData.item.effectiveDate.substring(0,10)
                this.addFormColor = 'danger'
            }else{
                this.addNewRoleForm = true;
                this.$nextTick(()=>{location.href = '#addRoleForm';})
            }
        }

        public saveRole(body, iscreate){                            
            this.roleError = false; 
            body[0]['userId']= this.userToEdit.id;
            const method = 'put';   
            const url = 'api/courtAdmin/assignroles';
            const options = { method: method, url:url, data:body} 
            this.$http(options)
                .then(response => {
                    if(iscreate) 
                        this.addToRoleList(body[0]);
                    else
                        this.modifyAssignedRoleList(body[0]);
                    this.closeRoleForm();
                    this.populateRolesDropdown()
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.roleErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.roleErrorMsgDesc = errMsg;
                    this.roleError = true;
                    location.href = '#RoleError'
                });           
        }

        public modifyAssignedRoleList(modifiedRoleInfo){
            const index = this.assignedRoles.findIndex(assignedrole =>{ if(assignedrole.value == modifiedRoleInfo.roleId) return true})
            if(index>=0){
                this.assignedRoles[index].value =  modifiedRoleInfo.roleId;
                this.assignedRoles[index].effectiveDate = moment(modifiedRoleInfo.effectiveDate).tz(this.timezone).format();
                this.assignedRoles[index].expiryDate = modifiedRoleInfo.expiryDate? moment(modifiedRoleInfo.expiryDate).tz(this.timezone).format():'';
                this.assignedRoles[index].text = modifiedRoleInfo.text;
                this.assignedRoles[index].desc = modifiedRoleInfo.desc;                  
                this.$emit('change');
            } 
        }

        public addToRoleList(addedRoleInfo){
            const role = {} as userRoleInfoType;
            role.value = addedRoleInfo.roleId;
            role.text = addedRoleInfo.text;
            role.desc = addedRoleInfo.desc; 
            role.effectiveDate = moment(addedRoleInfo.effectiveDate).tz(this.timezone).format();
            role.expiryDate = addedRoleInfo.expiryDate? moment(addedRoleInfo.expiryDate).tz(this.timezone).format():'';
            this.assignedRoles.push(role); 
            this.$emit('change');                     
        }

        public closeRoleForm() {                     
            this.addNewRoleForm= false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public confirmDeleteRole(role) {
            this.roleToDelete = role;           
            this.confirmDelete = true; 
        }

        public cancelDeletion() {
            this.confirmDelete = false;
            this.roleDeleteReason = '';
        }

        public deleteRole(){
            if (this.roleDeleteReason.length) {
                this.roleAssignError = false;
                this.confirmDelete = false;                 
                const body = 
                [{
                    "userId": this.userToEdit.id,
                    "roleId": this.roleToDelete.value,
                    "expiryReason": this.roleDeleteReason                        
                }]
                const url = 'api/courtAdmin/unassignroles' ;
                this.$http.put(url, body)
                    .then(response => {
                        this.updateDeletedRole();
                        this.$emit('change');
                                                        
                    }, err=>{
                        const errMsg = err.response.data.error;
                        this.roleErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                        this.roleErrorMsgDesc = errMsg;                    
                        this.roleAssignError = true;
                    });
                this.roleDeleteReason = '';
            }
        }
        
        public updateDeletedRole(){ 
            const index = this.assignedRoles.findIndex(assignedrole =>{ if(assignedrole.value == this.roleToDelete.value) return true})
            if(index>=0){
                this.assignedRoles[index].expiryDate = new Date().toISOString();            
                this.$emit('change');
            } 
        }
        
        public editRole(data){
            if(this.addNewRoleForm){
                location.href = '#addRoleForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Ro-Date-'+this.latestEditData.item.effectiveDate.substring(0,10)
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !data.detailsShowing){
                data.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = data
                Vue.nextTick().then(()=>{
                    location.href = '#Ro-Date-'+this.latestEditData.item.effectiveDate.substring(0,10)
                });
            }
        }
    }
</script>

<style scoped>
    .card {
        border: white;
    }
</style>