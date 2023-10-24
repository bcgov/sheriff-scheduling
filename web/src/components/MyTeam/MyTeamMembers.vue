<template>
    <b-card bg-variant="white">
        <b-row class="bg-white">
            <b-col cols="10">
                <page-header :pageHeaderText="sectionHeader"></page-header>
                <b-card>
                    <b-row>
                        <b-col>  
                            <b-form-group class="mr-1" style="width: 20rem"><label class="ml-1">Searching keyword:</label>
                                <b-form-input v-model="searchPhrase" placeholder="Enter Keyword"></b-form-input>
                                <b-form-text class="text-light font-italic"> Name/Rank/Location/Badge Number </b-form-text>
                            </b-form-group>
                        </b-col>
                        <b-col style="margin-top: 35px;">
                            <b-pagination
                                v-model="currentPage"
                                :total-rows="totalRows"
                                :per-page="itemsPerPage" 
                                first-number
                                last-number                               
                                first-text="First"
                                prev-text="Prev"
                                next-text="Next"
                                last-text="Last">
                            </b-pagination>
                        </b-col>
                    </b-row>
                </b-card> 
            </b-col>
            <b-col style="padding: 0;">
                <div :class="expiredViewChecked?'my-4 bg-warning':'my-4'" :style="expiredViewChecked?'max-width: 160px;':''">
                    <b-form-checkbox v-model="expiredViewChecked" :style="expiredViewChecked?'max-width: 160px; margin-left:10px;':''" @change="getSheriffs()" size="lg"  switch>
                        {{viewStatus}}
                    </b-form-checkbox>
                </div>                
                <b-button v-if="hasPermissionToAddNewUsers" style="max-height: 40px;" size="sm" variant="success" @click="AddMember()" class="my-2"><b-icon-plus/>Add User</b-button>  
            </b-col>
        </b-row>

        <loading-spinner v-if= "!isMyTeamDataMounted" />

        <div v-else class="container mb-5" style="float: left;" id="app">
            <div class="row" :key="photokey">
                <div v-for="teamMember in myTeamData" :key="teamMember.badgeNumber" class="col-3  my-1">
                    <div  class="card h-100 bg-dark">                        
                        <div class="card-header bg-dark border-dark mb-0 pb-0 " style="width: 13.4rem; height: 2.5rem;">
                            <b-row class="ml-3">                                                
                                <user-location-summary v-if="teamMember.awayLocation.length>0" class="mx-3" :homeLocation="teamMember.homeLocationNm" :awayJson="teamMember.awayLocation" :index="teamMember.badgeNumber"/>
                                <user-training-summary class="mx-2" v-if="teamMember.training.length>0" :trainingJson="teamMember.training" :index="teamMember.badgeNumber" :timezone="teamMember.homeLocation?teamMember.homeLocation.timezone:'UTC'"/>
                                <user-leave-summary class="mx-2" v-if="teamMember.leave.length>0" :leaveJson="teamMember.leave" :index="teamMember.badgeNumber" :timezone="teamMember.homeLocation?teamMember.homeLocation.timezone:'UTC'"/>
                            </b-row>
                        </div>
                        <div @click="openMemberDetails(teamMember.id)" class="card-body my-1 py-0">
                            <user-summary-template v-on:photoChange="photoChanged" :user="teamMember" :editMode="false" />
                        </div>
                        <div class="card-footer text-white bg-dark border-dark mt-0 pt-0" style="width: 13.4rem; height: 2.5rem;">                                                
                            <expire-court-admin-profile :disabled="!hasPermissionToExpireUsers" :userID="teamMember.id" :userIsEnable="teamMember.isEnabled" @change="getSheriffs()" />                        
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <b-modal size="xl" v-model="showMemberDetails" id="bv-modal-team-member-details" header-class="bg-primary text-light">            
            <template v-slot:modal-title>                
                 <h2 v-if="editMode" class="mb-0 text-light"> Updating User Profile </h2>
                 <h2 v-else-if="createMode" class="mb-0 text-light"> Creating User Profile </h2>                
            </template>
            <b-card v-if="isUserDataMounted" no-body style="user-select: none;">
                <b-row>
                    <b-col cols="3">
                        <user-summary-template v-on:photoChange="photoChanged" :user="userToEdit" :editMode="editMode"/>
                    </b-col>
                    <b-col cols="9">
                        <b-card no-body >
                            <b-tabs  card v-model="tabIndex" @activate-tab="onTabChanged">
                                <b-tab title="Identification">
                                    <identification-tab 
                                        :runMethod="identificationTabMethods"                                         
                                        v-on:closeMemberDetails="closeMemberDetailWindow()" 
                                        v-on:profileUpdated="getSheriffs()"
                                        v-on:enableSave="enableSave()"   
                                        v-on:changeTab="changeTab"                                     
                                        :createMode="createMode" 
                                        :editMode="editMode" />
                                </b-tab>

                                <b-tab v-if="editMode" title="Locations"> 
                                    <location-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                                <b-tab v-if="editMode" title="Leaves">
                                    <leave-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                    
                                </b-tab>

                                <b-tab v-if="editMode"  title="Training"> 
                                    <training-tab
                                        v-on:refresh="refreshProfile"
                                        v-on:change="getSheriffs()"/>
                                </b-tab>

                                <b-tab v-if="editMode && hasPermissionToAssignRoles" title="Roles" class="p-0">
                                    <role-assignment-tab  
                                        v-on:change="getSheriffs()"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>
                                </b-tab>

                                <b-tab v-if="editMode" title="Acting Rank"> 
                                    <rank-tab 
                                        v-on:change="getSheriffs()"
                                        v-on:refresh="refreshProfile"
                                        v-on:closeMemberDetails="closeProfileWindow()"/>                                   
                                </b-tab>

                            </b-tabs>
                        </b-card>
                    </b-col>
                </b-row>        
            </b-card>
            <template v-slot:modal-footer>
                <b-button
                    variant="secondary" 
                    @click="closeProfileWindow()"                  
                ><b-icon-x font-scale="1.5" style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>{{getCancelLabel}}</b-button>
                <b-button     
                    v-if="tabIndex<1"
                    variant="success"
                    :disabled="(editMode && !hasPermissionToEditUsers) || saving" 
                    @click="saveMemberProfile()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button
                  variant="outline-primary"
                  class="text-light closeButton"
                  @click="closeProfileWindow()"                  
                  >
                  &times;</b-button>
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

        
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation"); 

    import "@store/modules/TeamMemberInformation";
    const TeamMemberState = namespace("TeamMemberInformation");
    
    import PageHeader from "@components/common/PageHeader.vue";    
    
    import ExpireSheriffProfile from './Tabs/ExpireSheriffProfile.vue';
    import RoleAssignmentTab from './Tabs/RoleAssignmentTab.vue';
    import IdentificationTab from './Tabs/IdentificationTab.vue';
    import UserSummaryTemplate from "./Tabs/UserSummaryTemplate.vue";
    import LocationTab from './Tabs/LocationTab.vue';
    import LeaveTab from './Tabs/LeaveTab.vue';
    import RankTab from './Tabs/RankTab.vue'
    import UserLocationSummary from './Tabs/UserLocationSummary.vue';
    import UserTrainingSummary from './Tabs/UserTrainingSummary.vue';
    import UserLeaveSummary from './Tabs/UserLeaveSummary.vue';
    import TrainingTab from './Tabs/TrainingTab.vue';

    import { teamMemberJsonType } from '@/types/MyTeam/jsonTypes';
    import {commonInfoType, locationInfoType, userInfoType} from '@/types/common';
    import {teamMemberInfoType} from '@/types/MyTeam';

    enum gender {'Male'=0, 'Female', 'Other'}

    @Component({
        components: {
            PageHeader,
            UserSummaryTemplate,
            UserLocationSummary,
            UserTrainingSummary,
            UserLeaveSummary,
            ExpireSheriffProfile,
            RoleAssignmentTab,
            IdentificationTab,
            LocationTab,
            LeaveTab,
            TrainingTab,
            RankTab
        }        
    })    
    export default class MyTeamMembers extends Vue {

        @commonState.State
        public commonInfo!: commonInfoType;

        @commonState.Action
        public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void

        @commonState.State
        public location!: locationInfoType;

        @commonState.Action
        public UpdateLocation!: (newLocation: locationInfoType) => void

        @commonState.State
        public userDetails!: userInfoType;

        @TeamMemberState.State
        public userToEdit!: teamMemberInfoType;

        @TeamMemberState.Action
        public UpdateUserToEdit!: (userToEdit: teamMemberInfoType) => void

        identificationTabMethods = new Vue();
        
        expiredViewChecked = false;
        showMemberDetails = false;
        hasPermissionToAddNewUsers = false;
        hasPermissionToExpireUsers = false;
        hasPermissionToEditUsers = false;
        hasPermissionToAssignRoles = false;

        maxRank = 1000;

        itemsPerRow = 4;//Define
        rowsPerPage = 13;//Define
        currentPage = 1;
        itemsPerPage = 1;// itemsPerRow*rowsPerPage

        tabIndex = 0;
        isUserDataMounted = false;
        editMode = false;
        createMode = false;
        saving = false;
        sectionHeader = '';
        photokey = 0;
        // userAllRoles: any[] = [];

        errorText=''
		openErrorModal=false;

        searchPhrase = '';

        isMyTeamDataMounted = false;
        newTabIndex = 0;
        firstNavigation = true;
    
        allMyTeamData: teamMemberInfoType[] =[];
        myTeam: teamMemberInfoType[] = [];
        
        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isMyTeamDataMounted) {
                this.getSheriffs()
                this.sectionHeader = "My Team - " + this.location.name;
            }            
        }  

        mounted() {
            this.maxRank = this.commonInfo.sheriffRankList.reduce((max, rank) => rank.id > max ? rank.id : max, this.commonInfo.sheriffRankList[0].id);
            this.hasPermissionToAddNewUsers = this.userDetails.permissions.includes("CreateUsers");
            this.hasPermissionToExpireUsers = this.userDetails.permissions.includes("ExpireUsers");
            this.hasPermissionToEditUsers = this.userDetails.permissions.includes("EditUsers");
            this.hasPermissionToAssignRoles = this.userDetails.permissions.includes("CreateAndAssignRoles");
            this.getSheriffs();
            this.sectionHeader = "My Team - " + this.location.name;
            this.itemsPerPage = this.itemsPerRow * this.rowsPerPage;
        }
        
        get viewStatus() {
            if(this.expiredViewChecked) return 'All Profiles';else return 'Active Profiles'
        }

        public getSheriffs() {            
            this.isMyTeamDataMounted = false;
            const url = 'api/sheriff';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractMyTeamFromSheriffs(response.data);                        
                    }
                    this.isMyTeamDataMounted = true;
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }     
                    this.isMyTeamDataMounted=true;
                })
        }
        
        public onTabChanged(newTabIndex , prevTabIndex, bvEvt) {
            this.newTabIndex = newTabIndex;            
            if(prevTabIndex == 0 && this.firstNavigation) {
                this.firstNavigation = false;
                bvEvt.preventDefault();
                this.identificationTabMethods.$emit('switchTab');   
            }            
        }

        public changeTab(changingTab) {
            if(changingTab) this.tabIndex = this.newTabIndex;
            Vue.nextTick().then(()=>{this.firstNavigation = true;});
        }

        public extractMyTeamFromSheriffs(data: teamMemberJsonType[]) {    
            this.allMyTeamData = [];
            // let myteaminfo:             
            for(const myteaminfo of data)
            {                
                const myteam = {} as teamMemberInfoType;
                myteam.fullName = Vue.filter('capitalizefirst')(myteaminfo.firstName) + ' ' + Vue.filter('capitalizefirst')(myteaminfo.lastName);
                myteam.firstName = myteaminfo.firstName;
                myteam.lastName = myteaminfo.lastName;
                myteam.rank = (myteaminfo.actingRank?.length>0)?  (myteaminfo.actingRank[0].rank)+' (A)': myteaminfo.rank;
                myteam.rankOrder = this.getRankOrder(myteam.rank)[0]?this.getRankOrder(myteam.rank)[0].id:0;
                myteam.badgeNumber = myteaminfo.badgeNumber;
                myteam.id = myteaminfo.id;
                myteam.image = myteaminfo.photoUrl? myteaminfo.photoUrl: '';
                myteam.isEnabled = myteaminfo.isEnabled;
                myteam.homeLocationId = myteaminfo.homeLocationId;
                myteam.homeLocationNm = myteaminfo.homeLocation? myteaminfo.homeLocation.name: '';               
                
                myteam.leave = myteaminfo.leave? myteaminfo.leave: [];
                myteam.training = myteaminfo.training? myteaminfo.training: [];
                myteam.awayLocation = myteaminfo.awayLocation;
                
                if(myteaminfo.homeLocation)
                    myteam.homeLocation = {id: myteaminfo.homeLocation.id, name: myteaminfo.homeLocation.name, regionId: myteaminfo.homeLocation.regionId, timezone: myteaminfo.homeLocation.timezone};
                
                this.allMyTeamData.push(myteam);
            }
                     
        }

        get totalRows() {
            return this.myTeam.length
        }

        get myTeamData() {
            this.myTeam =this.allMyTeamData.filter(member => {
                if (this.expiredViewChecked || member.isEnabled)
                {
                    if(this.searchPhrase==''){
                        if(member.homeLocationId == this.location.id) return true
                        for(const awayInx in member.awayLocation)
                            if(member.awayLocation[awayInx].locationId == this.location.id ) return true
                    }
                    else{ 
                        if(this.searchForKeyword(member.firstName)) return true
                        if(this.searchForKeyword(member.lastName))  return true
                        if(this.searchForKeyword(member.rank))      return true
                        if(this.searchForKeyword(member.badgeNumber))    return true
                        if(this.searchForKeyword(member.homeLocationNm)) return true
                    }
                }
            })

            const sortedTeam = this.sortTeamMembers(this.myTeam);            
            return sortedTeam.slice((this.itemsPerPage)*(this.currentPage-1), (this.itemsPerPage)*(this.currentPage-1) + this.itemsPerPage);
        }

        public sortTeamMembers(teamList) {            
            return _.chain(teamList)
                    .sortBy(member =>{return (member['lastName']? member['lastName'].toUpperCase() : '')})
                    .sortBy(member =>{return (member['rankOrder']? member['rankOrder'] : this.maxRank + 1)})
                    .value()
        }

        public getRankOrder(rankName: string) {
            if(rankName?.includes(' (A)'))
                rankName = rankName.replace(' (A)','');
            return this.commonInfo.sheriffRankList.filter(rank => {
                if (rank.name == rankName) {
                    return true;
                }
            })
        }

        public searchForKeyword(phrase){
            if(phrase && phrase.toLowerCase().startsWith(this.searchPhrase.toLowerCase())) return true;else return false;
        }


        public photoChanged(id: string, image: string){           
            const index = this.allMyTeamData.findIndex(myteam => {if(myteam.id == id) return true;else return false})
            if( index >=0 ){
                this.allMyTeamData[index].image = image;
                this.photokey++;
            }
        }

        public refreshProfile(userId){
            this.closeProfileWindow()
            this.openMemberDetails(userId)
        }      

        public openMemberDetails(userId){           
            this.createMode = false;
            this.editMode = true;            
            this.loadUserDetails(userId);
        }

        public saveMemberProfile() {
            this.saving = true; 
            this.identificationTabMethods.$emit('saveMemberProfile');
        }
        
        public enableSave() {
            this.saving = false;
        }

        public closeProfileWindow(){            
            if(this.tabIndex ==0 || this.createMode)
            {  
                this.identificationTabMethods.$emit('closeProfileWindow');
            }
            else 
                this.closeMemberDetailWindow()
        }

        public closeMemberDetailWindow(){            
            this.showMemberDetails = false;           
        }

        public resetProfileWindowState(){
            this.createMode = false;
            this.editMode = false;
            this.tabIndex = 0;
            const user = {} as teamMemberInfoType;
            this.UpdateUserToEdit(user);  
        }

        public AddMember(){
            const user = {} as teamMemberInfoType;
            this.UpdateUserToEdit(user); 
            this.tabIndex = 0;
            this.createMode = true;
            this.editMode = false;
            this.saving = false;
            this.isUserDataMounted = true;
            this.showMemberDetails = true;
        }

        public loadUserDetails(userId): void {
            this.resetProfileWindowState();  
            this.editMode = true;            
            const url = 'api/sheriff/' + userId;
            this.$http.get(url)
                .then(response => {
                    if(response.data){                                              
                        this.extractUserInfo(response.data);
                        this.isUserDataMounted = true;
                        this.showMemberDetails=true;                                              
                    }                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }   
                });
        }

        public extractUserInfo(userJson): void {            
            const user = {} as teamMemberInfoType;            
            user.idirUserName =  userJson.idirName;
            user.firstName = userJson.firstName;
            user.lastName = userJson.lastName;
            user.fullName = Vue.filter('capitalizefirst')(userJson.firstName) + ' ' + Vue.filter('capitalizefirst')(userJson.lastName);
            user.gender = gender[userJson.gender];
            user.rank = userJson.rank;
            user.email = userJson.email;
            user.badgeNumber = userJson.badgeNumber;
            user.id = userJson.id;
            user.homeLocationId = userJson.homeLocationId;
            user.homeLocationNm = userJson.homeLocation? userJson.homeLocation.name: '';
            user.image = userJson['photoUrl']?userJson['photoUrl']:'';
            if(userJson.homeLocation)
                user.homeLocation  = {id: userJson.homeLocation.id, name: userJson.homeLocation.name, regionId: userJson.homeLocation.regionId, timezone: userJson.homeLocation.timezone};
          
            if(userJson.awayLocation && userJson.awayLocation.length>0)
                user.awayLocation = userJson.awayLocation;

            user.actingRank = userJson.actingRank;
            user.leave = userJson.leave;
            user.training = userJson.training;
            user.userRoles = userJson.roles
            this.UpdateUserToEdit(user);  
        }

        get getCancelLabel(){
            if(this.tabIndex<1) return 'Cancel'; else return 'Close'
        }
    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .closeButton {
        font-size: 20px;
        padding: 0;
        margin: 0;
    }

    .pagination {
        height: calc(1.6em + 0.75rem + 2px);
        border-width: 1px;
        border-radius: 0.25rem;
    }

    .form-group.required .label:after {
        content:"*";
        color:red;
    }

</style>