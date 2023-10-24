<template>
    <div>
        <b-card id="IdentificationError" border-variant="white" no-body>
            <h2 v-if="identificationError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="identificationErrorMsgDesc"  variant="danger"> {{identificationErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="identificationError = false" /></b-badge></h2>
        </b-card>

        <b-form-group v-if="!(editMode && !hasPermissionToEditIdir)" class="mx-1"><label>IDIR User Name<span class="text-danger">*</span></label>
            <b-form-input v-model="user.idirUserName" placeholder="Enter IDIR User Name" :state = "idirUserNameState?null:false"></b-form-input>
        </b-form-group>
        <h2 class="mx-1 mt-0"><b-badge v-if="duplicateIdir" variant="danger"> Duplicate IDIR</b-badge></h2>

        <b-row class="mx-1"> 
            <b-form-group class="mr-1" style="width: 20rem"><label>First Name<span class="text-danger">*</span></label>
                <b-form-input v-model="user.firstName" placeholder="Enter First Name" :state = "firstNameState?null:false"></b-form-input>
            </b-form-group>                                    
            <b-form-group class="ml-1" style="width: 20rem"><label>Last Name<span class="text-danger">*</span></label>
                <b-form-input v-model="user.lastName" placeholder="Enter Last Name" :state = "lastNameState?null:false"></b-form-input>
            </b-form-group>
        </b-row>

        <b-row class="mx-1">   
            <b-form-group class="mr-1" style="width: 10rem"><label>Gender<span class="text-danger">*</span></label>
                <b-form-select v-model="user.gender" :options="genderOptions" :state = "selectedGenderState?null:false"></b-form-select>
            </b-form-group>
            <b-form-group class="ml-1" style="width: 30rem"><label>Email<span class="text-danger">*</span></label>
                <b-form-input v-model="user.email" placeholder="Enter Email" :state = "emailState?null:false" type="email"></b-form-input>
            </b-form-group>
        </b-row>

        <b-row class="mx-1">
            <b-form-group class="mr-1" style="width: 20rem"><label>Badge Number<span class="text-danger">*</span></label>
                <b-form-input v-model="user.badgeNumber" type="number" placeholder="Enter Badge Number" :state = "badgeNumberState?null:false"></b-form-input>
            </b-form-group>                                            
            <b-form-group class="ml-1" style="width: 15rem"><label>Rank<span class="text-danger">*</span></label>
                <b-form-select v-model="user.rank" placeholder="Select Rank" :state = "selectedRankState?null:false">
                    <b-form-select-option
                        v-for="courtAdminRank in courtAdminRankList" 
                        :key="courtAdminRank.id"
                        :value="courtAdminRank.name">
                            {{courtAdminRank.name}}
                    </b-form-select-option>
                </b-form-select>
            </b-form-group>
        </b-row>
        <h2 v-if="duplicateBadge" class="mx-1 mt-0"><b-badge variant="danger"> Duplicate Badge</b-badge></h2>

        <b-row class="mx-1">
            <b-form-group class="mr-1" style="width: 25rem"><label>Home Location<span class="text-danger">*</span></label> 
                <b-form-select                                                                                                           
                    v-model="user.homeLocationId"
                    :state = "homeLocationState?null:false">
                        <b-form-select-option
                            v-for="homelocation in locationList" 
                            :key="homelocation.id"
                            :value="homelocation.id">
                                {{homelocation.name}}
                        </b-form-select-option>    
                </b-form-select>
            </b-form-group>
        </b-row>

        <b-modal v-model="showCancelWarning" id="bv-modal-team-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Unsaved Profile Changes </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Unsaved New Profile </h2>                
            </template>
            <p>Are you sure you want to {{cancelMessage}} without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="closeWarningWindow(false);"                   
                >No</b-button>
                <b-button variant="success" @click="closeWarningWindow(true)"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-team-cancel-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>


        <b-modal v-model="showHomeLocationWarning" id="bv-modal-team-home-location-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 class="mb-0 text-light"> <b-icon-house-door-fill class="mr-1" font-scale="1.25"/> Home Location Change </h2>                
            </template>
            <p>Are you sure you want to change the home location?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-team-home-location-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="updateProfile()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-team-home-location-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import * as _ from 'underscore';
import { namespace } from 'vuex-class';

import "@store/modules/CommonInformation";
const commonState = namespace("CommonInformation"); 

import "@store/modules/TeamMemberInformation";
const TeamMemberState = namespace("TeamMemberInformation");

import {teamMemberInfoType} from '@/types/MyTeam';
import {commonInfoType, locationInfoType, userInfoType} from '@/types/common';

enum gender {'Male'=0, 'Female', 'Other'}

@Component
export default class IdentificationTab extends Vue {

    @commonState.State
    public commonInfo!: commonInfoType;

    @commonState.State
    public location!: locationInfoType;

    @commonState.State
    public locationList!: locationInfoType[]; 
    
    @commonState.State
    public userDetails!: userInfoType;

    @TeamMemberState.State
    public userToEdit!: teamMemberInfoType;

    @TeamMemberState.Action
    public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

    @Prop({required: true})
    createMode!: boolean;

    @Prop({required: true})
    editMode!: boolean;

    @Prop({required: true})
    runMethod!: any;

    genderOptions = [{text:"Male", value: gender.Male}, {text:"Female", value: gender.Female}, {text:"Other", value: gender.Other}]
    genderValues = [0, 1, 2]
    firstNameState = true;
    lastNameState = true;
    emailState = true;
    selectedGenderState = true;
    badgeNumberState = true;
    selectedRankState = true;
    idirUserNameState = true;
    homeLocationState = true;
    duplicateBadge = false;
    duplicateIdir = false;

    hasPermissionToEditIdir = false;
    showCancelWarning = false;
    cancelMessage ='cancel';

    showHomeLocationWarning= false;

    selectedHomeLocation = {} as locationInfoType | undefined;

    identificationError = false;
    identificationErrorMsg = '';
    identificationErrorMsgDesc = '';

    user = {} as teamMemberInfoType;

    mounted(){
        this.refreshTabInformation();
        this.runMethod.$on('switchTab', this.switchTab)
        this.runMethod.$on('closeProfileWindow', this.closeProfileWindow)
        this.runMethod.$on('saveMemberProfile', this.saveMemberProfile)
        this.hasPermissionToEditIdir = this.userDetails.permissions.includes("EditIdir");            
    }

    public refreshTabInformation()
    {        
        this.ClearFormState();
        this.user = _.clone(this.userToEdit);
    }

    public switchTab(){
        
        if(this.editMode && !this.changesMade()){
            this.$emit('changeTab', true);
        }    
        else
        {            
            this.cancelMessage = 'change tabs';
            this.showCancelWarning = true;
        }
    }

    public closeProfileWindow(){
        
        if(this.createMode && this.isEmpty(this.user)){
            this.$emit('closeMemberDetails');
        } 
        else if(this.editMode && !this.changesMade()){
            this.$emit('closeMemberDetails');
        }    
        else
        {
            this.cancelMessage = 'cancel';
            this.showCancelWarning = true;
        }
    }

    public closeWarningWindow(changingTab: boolean) {
        this.showCancelWarning = false;
        if(this.cancelMessage == 'cancel' && changingTab)
        { 
            this.$emit('closeMemberDetails');
        }
        else{
            this.$emit('changeTab', changingTab);
        }
    }

    public changesMade(): boolean {
         if((this.userToEdit.homeLocationId != this.user.homeLocationId) ||
            (this.userToEdit.firstName != this.user.firstName) ||
            (this.userToEdit.lastName != this.user.lastName) ||
            (this.userToEdit.gender != this.user.gender) ||
            (this.userToEdit.email != this.user.email) ||
            (this.userToEdit.idirUserName != this.user.idirUserName) ||
            (this.userToEdit.badgeNumber != this.user.badgeNumber) ||
            (this.userToEdit.rank != this.user.rank) ||                    
            (this.userToEdit.homeLocationId != this.user.homeLocationId)) return true;
        return false;
    }

    public isEmpty(obj){
        for(const prop in obj) 
            if(obj[prop] != null)
                return false;
        return true;
    }
         
    public saveMemberProfile() {     
        let requiredError = false;

        if (!this.user.idirUserName) {
            if (this.createMode || (this.editMode && this.hasPermissionToEditIdir)) {
                this.idirUserNameState = false;
                requiredError = true;
            } else {
                this.idirUserNameState = true;
                this.duplicateIdir = false;
            }            
        } else {
            this.idirUserNameState = true;
            this.duplicateIdir = false;
        }
        if (!this.user.firstName) {
            this.firstNameState = false;
            requiredError = true;
        } else {
            this.firstNameState = true;
        }
        if (!this.user.lastName) {
            this.lastNameState = false;
            requiredError = true;
        } else {
            this.lastNameState = true;
        }
        if (this.genderValues.toString().indexOf(this.user.gender) == -1) {
            this.selectedGenderState = false;
            requiredError = true;
        } else {
            this.selectedGenderState = true;
        }
        if (!this.user.badgeNumber) {
            this.badgeNumberState = false;
            requiredError = true;
        } else {
            this.badgeNumberState = true;
            this.duplicateBadge = false;
        }
        if (!this.user.email) {
            this.emailState = false;
            requiredError = true;
        } else {
            this.emailState = true;
        }
        if (!this.user.rank) {
            this.selectedRankState = false;
            requiredError = true;
        } else {
            this.selectedRankState = true;
        }        
        if (!this.user.homeLocationId) {           
            requiredError = true;
            this.homeLocationState = false;
        } else{
            this.homeLocationState = true;
        }
        
        if (!requiredError) {
            if (this.editMode){
                if(this.userToEdit.homeLocationId == this.user.homeLocationId)
                    this.updateProfile();
                else
                    this.showHomeLocationWarning = true;
            } 
            if (this.createMode) this.createProfile();              

        } else {
            this.$emit('enableSave');
            //console.log('Error required')
        }             
    }

    public updateProfile(): void {
        const body = {               
            gender: this.user.gender,
            badgeNumber: this.user.badgeNumber,
            rank: this.user.rank,
            idirName: this.user.idirUserName,
            firstName:this.user.firstName,
            lastName: this.user.lastName,
            email: this.user.email,
            id: this.user.id
        }

        const url = 'api/courtAdmin';
        this.$http.put(url, body)
            .then(response => {
                if(response.data){
                    this.updateLocation();                           
                }                    
            }, err => {               
                const errMsg = err.response.data.error;
                if(err.response.status == 400){
                    if (errMsg.includes('already has badge number')){
                        this.badgeNumberState = false;
                        this.duplicateBadge = true;
                    } else if (errMsg.includes('has IDIR name')) {
                        this.idirUserNameState = false;
                        this.duplicateIdir = true;
                    }                        
                }
                else{                    
                    this.identificationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.identificationErrorMsgDesc = errMsg;
                    this.identificationError = true;
                }
                this.$emit('enableSave');
            });
    }

    public updateLocation(){
        
        const url = 'api/courtAdmin/updatelocation?id='+this.user.id+'&locationId='+this.user.homeLocationId;
        this.$http.put(url)
            .then(response => {                
                this.resetProfileWindowState();
                this.$emit('closeMemberDetails');
                this.$emit('profileUpdated');
                this.$emit('enableSave');                
                                                            
            }, err => {                
                const errMsg = err.response.data.error;
                this.identificationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                this.identificationErrorMsgDesc = errMsg;
                this.identificationError = true;
                this.$emit('enableSave');
            });       
    }

    public createProfile() {

        const body = {
            homeLocationId: this.user.homeLocationId,               
            gender: this.user.gender,
            badgeNumber: this.user.badgeNumber,
            rank: this.user.rank,
            idirName: this.user.idirUserName,
            firstName:this.user.firstName,
            lastName: this.user.lastName,
            email: this.user.email
        }
        const url = 'api/courtAdmin';
        this.$http.post(url, body )
            .then(response => {
                if(response.data){
                    this.resetProfileWindowState();
                    this.$emit('closeMemberDetails');
                    this.$emit('profileUpdated');
                    this.$emit('enableSave');    
                }
            }, err => {
                const errMsg = err.response.data.error;                                
                
                if(err.response.status == 400){
                    if (errMsg.includes('already has badge number')){
                        this.badgeNumberState = false;
                        this.duplicateBadge = true;
                    } else if (errMsg.includes('has IDIR name')) {
                        this.idirUserNameState = false;
                        this.duplicateIdir = true;
                    }                        
                }else{
                    this.identificationErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.identificationErrorMsgDesc = errMsg;
                    this.identificationError = true;   
                }
                this.$emit('enableSave');

            })   
    }

    public resetProfileWindowState() {
        this.user = {} as teamMemberInfoType;
        this.ClearFormState();
    }

    public ClearFormState(){        
        this.firstNameState = true;
        this.lastNameState = true;
        this.selectedGenderState = true;
        this.badgeNumberState = true;
        this.emailState = true;
        this.selectedRankState = true;
        this.idirUserNameState = true;
        this.duplicateBadge = false;
        this.duplicateIdir = false;
    }

    get courtAdminRankList(){
        return _.sortBy(this.commonInfo.courtAdminRankList, 'id')
    }

    
}
</script>