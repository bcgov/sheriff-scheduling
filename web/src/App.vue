<template>    
    <div class="app-outer fill-body" id="app"  style="user-select: none;">
        <div v-if= "isCommonDataReady">
            <navigation-topbar />
            <router-view></router-view>
            <navigation-footer id="footer" v-if="displayFooter"/>
        </div>
        <div v-else>
            <b-card v-if= "displayError && errorText" border-variant="white" class="bg-warning">
            {{errorText}}
            </b-card>
        </div>
    </div>
</template>

<script lang="ts">
    import NavigationTopbar from "@components/NavigationTopbar.vue";
    import NavigationFooter from "@components/NavigationFooter.vue";
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import {commonInfoType, locationInfoType, courtAdminRankInfoType, userInfoType, regionInfoType} from './types/common';
    import {courtAdminRankJsonType} from './types/common/jsonTypes'
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import * as _ from 'underscore';
    import moment from 'moment-timezone';
    

    @Component({
        components: {
            NavigationTopbar,
            NavigationFooter
        }
    })    
    export default class App extends Vue {

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

        @commonState.Action
        public UpdateUser!: (newUser: userInfoType) => void

        @commonState.Action
        public UpdateRegionList!: (newRegionList: regionInfoType[]) => void

        @commonState.State
        public locationList!: locationInfoType[];
        
        @commonState.Action
        public UpdateLocationList!: (newLocationList: locationInfoType[]) => void

        @commonState.State
        public allLocationList!: locationInfoType[];
        
        @commonState.Action
        public UpdateAllLocationList!: (newAllLocationList: locationInfoType[]) => void

        @commonState.State
        public displayFooter!: boolean;

        errorCode = 0;
        errorText = '';
        displayError=false;
        isCommonDataReady= false;
        courtAdminRankList: courtAdminRankInfoType[] = []
        currentLocation;
       
        mounted() {
            this.displayError = false;
            this.errorText = '';
            this.isCommonDataReady = false; 
            //console.log(Vue.$cookies.get("logout"))           
            if (Vue.$cookies.isKey("logout"))
                this.isCommonDataReady = true;            
            else 
                this.loadUserDetails();
        }

        public loadUserDetails() {
            const url = 'api/auth/info'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        const userData = response.data;
                        if(userData.permissions.length == 0){
                            this.isCommonDataReady = true;
                            if(this.$route.name != 'RequestAccess')
                                this.$router.push({path:'/request-access'}) 
                        }
                        else {                            
                            this.UpdateUser({
                                firstName: userData.firstName,
                                lastName: userData.lastName,
                                roles: userData.roles,
                                homeLocationId: userData.homeLocationId,
                                permissions: userData.permissions,
                                userId: userData.userId
                            }) 
                            this.getAllLocations()  
                        }                      
                    } 
                },err => {
                    this.errorText = err + '  - ' + moment().format();
                    if (this.errorText.indexOf('401') == -1) {                        
                        this.displayError = true;
                    }                    
                }) 
        }

        public loadCourtAdminRankList(){  
            const url = 'api/managetypes?codeType=CourtAdminRank'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractCourtAdminRankInfo(response.data);
                        if(this.commonInfo.courtAdminRankList.length>0 && 
                        this.userDetails.roles.length>0 && this.locationList.length>0)
                        {                              
                            this.isCommonDataReady = true;
                            //console.log(this.$route.path)
                            if(this.$route.path!='/' && this.$route.name == 'Home')
                                this.$router.push({path:'/'})
                        }
                        this.getRegions();
                    }                   
                },err => {
                    this.errorText = err + '  - ' + moment().format();
                    if (this.errorText.indexOf('401') == -1) {                        
                        this.displayError = true;
                    }                    
                })         
        }        

        public extractCourtAdminRankInfo(courtAdminRankList){

            let courtAdminRank: courtAdminRankJsonType;

            for(courtAdminRank of courtAdminRankList){                
                this.courtAdminRankList.push({id: Number(courtAdminRank.id), name: courtAdminRank.description})
            }                       
            this.UpdateCommonInfo({
                courtAdminRankList: this.courtAdminRankList 
            })
        }

        public getAllLocations() {
            const url = 'api/location/all'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractLocationInfo(response.data, true);
                        this.getLocations();
                    }                   
                },err => {
                    this.errorText = err + '  - ' + moment().format();
                    if (this.errorText.indexOf('401') == -1) {                        
                        this.displayError = true;
                    }
                    
                })  
        }

        public getRegions() {
            const url = 'api/region'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractRegionInfo(response.data);                        
                    }                   
                },err => {
                    this.errorText = err + '  - ' + moment().format();
                    if (this.errorText.indexOf('401') == -1) {                        
                        this.displayError = true;
                    }                    
                }) 
        }
        
        public getLocations() {
            const url = 'api/location'
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.extractLocationInfo(response.data, false);
                        this.loadCourtAdminRankList();
                    }                   
                },err => {
                    this.errorText = err + '  - ' + moment().format();
                    if (this.errorText.indexOf('401') == -1) {                        
                        this.displayError = true;
                    }                    
                }) 
        }
        
        public extractLocationInfo(locationListJson, allLocations: boolean){            
            const locations: locationInfoType[] = [];
            for(const locationJson of locationListJson){    
                if (locationJson.regionId > 0) {
                    const locationInfo: locationInfoType = {id: locationJson.id, name: locationJson.name, regionId: locationJson.regionId, timezone: locationJson.timezone}
                    locations.push(locationInfo);
                }
            }
            if (allLocations) {
                this.UpdateAllLocationList(_.sortBy(locations,'name'));
            } else {
                this.UpdateLocationList(_.sortBy(locations,'name'));
            } 
        }

        public extractRegionInfo(regionListJson){ 
                      
            const regions: regionInfoType[] = regionListJson.filter(region=>(region.justinId > 0)); 

            const vancouverIndex = regions.findIndex((region => region.name == 'Vancouver'));

            if(vancouverIndex>=0) regions[vancouverIndex].name = 'Coastal';            
            
            this.UpdateRegionList(_.sortBy(regions,'name'));            
        }
        
     }
</script>