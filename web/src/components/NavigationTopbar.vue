<template>
	<header class="app-header">
		<b-navbar toggleable="lg" class="navbar navbar-expand-lg navbar-dark">    
			<b-navbar-brand v-if="displayHeader" class="my-0">
					<img 
							class="img-fluid d-none d-lg-block"          
							src="../../public/images/bcid-logo-rev-en.svg"
							width="177"
							height="44"
							alt="B.C. Government Logo"
						/>
					<img
						class="img-fluid d-lg-none"
						src="../../public/images/bcid-symbol-rev.svg"
						width="63"
						height="44"
						alt="B.C. Government Logo"
					/>
			</b-navbar-brand>
            <b-navbar-nav v-else style="height:3rem"/>
            
			<b-navbar-nav v-if="displayHeader" class="my-0 mx-5">
				<b-nav-item-dropdown text="Duty Roster" dropdown :disabled="!hasPermissionToViewDutyRosterPage">
                    <b-dropdown-item to="/manage-duty-roster">Manage Duties</b-dropdown-item>
                    <b-dropdown-item to="/view-duty-roster">View Duties</b-dropdown-item>
                    <b-dropdown-item to="/manage-assignments" :disabled="!hasPermissionToViewSchedulePages">Manage Assignments</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="Shift Schedule" dropdown :disabled="!hasPermissionToViewSchedulePages">
                    <b-dropdown-item v-if="hasPermissionToViewManageSchedule" to="/manage-shift-schedule">Manage Schedule</b-dropdown-item>
                    <b-dropdown-item v-if="hasPermissionToViewDistributeSchedule" to="/distribute-shift-schedule">Distribute Schedule</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="My Team" dropdown :disabled="!hasPermissionToViewTeamPages">
                    <b-dropdown-item v-if="hasPermissionToViewProfilePage" to="/team-members">My Team Members</b-dropdown-item>
                    <b-dropdown-item v-if="hasPermissionToViewRolesPage" to="/define-roles-access">Define Roles & Access</b-dropdown-item>
                    <b-dropdown-item v-if="hasPermissionToGenerateReportsPage" to="/reports">Reports</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="Manage Types" dropdown :disabled="!hasPermissionToEditManageTypes">
                    <b-dropdown-item to="/assignment-types">Assignment Types</b-dropdown-item>
                    <b-dropdown-item to="/leave-training-types">Leave & Training Types</b-dropdown-item>
                </b-nav-item-dropdown>
			</b-navbar-nav>
			<b-navbar-nav v-if="displayHeader" class="ml-3 my-0 mr-3">
                <b-input-group v-if="locationDataReady" class="mr-2 mt-1" style="height: 40px">
                    <b-input-group-prepend is-text>
                        <b-icon icon="globe"></b-icon>
                    </b-input-group-prepend>
                    <b-form-select
                        style="height: 100%;"
                        v-model="selectedLocation"                
                        :disabled="disableLocationChange"
                        @change="UpdateLocation(selectedLocation)"                
                        >
                        <b-form-select-option
                        v-for="location in locationList"
                        :key="location.id"                  
                        :value="location">{{location.name}}
                        </b-form-select-option>                  
                    </b-form-select>
                </b-input-group>
                <b-nav-item-dropdown right class="my-0" menu-class="bg-info"  dropdown>
                    <template v-slot:button-content>
                        <b-icon-person-circle></b-icon-person-circle>
                    </template>
                    <b-dropdown-text class="text-primary"><b-icon-person/> {{userDetails.firstName}} {{userDetails.lastName}}</b-dropdown-text>
                    <b-dropdown-item-button variant="success" @click="dashboard()"><b-icon-display/> Dashboard</b-dropdown-item-button>
                    <b-dropdown-item-button variant="danger" @click="signout()"><b-icon-box-arrow-right/> Sign out</b-dropdown-item-button>
                </b-nav-item-dropdown>
			</b-navbar-nav>
		</b-navbar>    
	</header>
</template>

<script lang="ts">
	import { Component, Vue} from 'vue-property-decorator';
	import { namespace } from "vuex-class";
	import "@store/modules/CommonInformation";  
	import {commonInfoType, locationInfoType, userInfoType} from '@/types/common';  
	const commonState = namespace("CommonInformation");
    import store from "@/store";

	@Component
	export default class NavigationTopbar extends Vue {

        @commonState.State
        public displayHeader!: boolean;

		@commonState.State
		public userDetails!: userInfoType;
		
		@commonState.State
		public commonInfo!: commonInfoType;

		@commonState.Action
		public UpdateCommonInfo!: (newCommonInfo: commonInfoType) => void

		@commonState.State
		public locationList!: locationInfoType[];      
		
		@commonState.State
		public location!: locationInfoType;

		@commonState.Action
		public UpdateLocation!: (newLocation: locationInfoType) => void
		
		disableLocationChange = false;
		userIsAdmin = false;
		selectedLocation ={} as locationInfoType;
		locationDataReady = false;
        hasPermissionToEditManageTypes = false;    
        hasPermissionToViewDistributeSchedule = false;
        hasPermissionToViewManageSchedule = false;
        hasPermissionToViewSchedulePages = false;
        hasPermissionToViewDutyRosterPage = false;
        hasPermissionToViewProfilePage = false;
        hasPermissionToViewRolesPage = false;
        hasPermissionToGenerateReportsPage = true;
        hasPermissionToViewTeamPages = false;
        
        mounted() {
            this.getModulePermissions();
            this.getCurrentLocation();
            this.disableLocationChange = false;
        }

        public getModulePermissions() {
            this.hasPermissionToEditManageTypes = this.userDetails.permissions.includes("EditTypes");
            this.hasPermissionToViewDistributeSchedule = this.userDetails.permissions.includes("ViewDistributeSchedule");
            this.hasPermissionToViewManageSchedule = this.userDetails.permissions.includes("ViewShifts");
            this.hasPermissionToViewSchedulePages = this.hasPermissionToViewDistributeSchedule || this.hasPermissionToViewManageSchedule;
            this.hasPermissionToViewDutyRosterPage = this.userDetails.permissions.includes("ViewDutyRoster");
            this.hasPermissionToGenerateReportsPage = this.userDetails.permissions.includes("GenerateReports");
            this.hasPermissionToViewRolesPage = this.userDetails.permissions.includes("ViewRoles");
            const hasPermissionToViewProvinceProfiles = this.userDetails.permissions.includes("ViewProvince");
            const hasPermnissionToViewProfilesInOwnLocation = this.userDetails.permissions.includes("ViewHomeLocation") || this.userDetails.permissions.includes("ViewAssignedLocation");
            const hasPermnissionToViewRegionProfiles = this.userDetails.permissions.includes("ViewRegion");
            this.hasPermissionToViewProfilePage = hasPermissionToViewProvinceProfiles || hasPermnissionToViewProfilesInOwnLocation || hasPermnissionToViewRegionProfiles;
            this.hasPermissionToViewTeamPages = this.hasPermissionToViewProfilePage || this.hasPermissionToViewRolesPage || this.hasPermissionToGenerateReportsPage;
        }

		public getCurrentLocation()
		{
			let currentLocation;
			if (this.userDetails.homeLocationId) {
				const matchingLocation =this.locationList.filter(locationInfo => {
						if (locationInfo.id == this.userDetails.homeLocationId) {
								return true
						}
				});
				currentLocation = matchingLocation[0]

			} else {
				currentLocation = this.locationList[0]
			}      
			
			this.UpdateLocation(currentLocation);
			this.selectedLocation = this.location;
			if (this.selectedLocation && this.selectedLocation.name.length > 0) this.locationDataReady = true;
        }
        
        public signout(){            
            store.commit('CommonInformation/setToken','');
            store.commit('CommonInformation/setTokenExpiry','');
            Vue.$cookies.set("logout","1",)
            window.location.replace(`${process.env.BASE_URL}api/auth/logout`);            
        }

        public dashboard(){
            if(this.$route.path!='/' )
                this.$router.push({path:'/'})
        }

	}
</script>

<style scoped>   

    ul >>> .dropdown-menu {
        width: 270px !important;
    }
</style>