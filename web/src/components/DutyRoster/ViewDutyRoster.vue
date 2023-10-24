<template>
    <b-card bg-variant="white" class="home" no-body>             
                  
        <b-overlay opacity="0.6" :show="!isViewDutyDataMounted">
            <template #overlay>
                <loading-spinner :inline="true"/>
            </template>   
            <b-card class="bg-primary">
                <b-row style="padding: 0; margin: 0;">                
                    <b-col cols="8" style="padding: 0; margin: 0;" class="text-right text-white h2">{{today | beautify-date-time-weekday}}</b-col>
                    <b-col cols="4" style="padding: 0; margin: 0;" class="text-right text-white h2">{{currentPage}} / {{maxPages}}</b-col>
                </b-row>               
            </b-card> 
            <b-table
                :key="updateTable"
                :items="displayedDuties" 
                :fields="fields"
                small
                head-row-variant="primary"   
                bordered
                striped
                fixed>  

                <template v-slot:cell(displayName) = "data" >
                    <span style="font-size:12px; line-height: 14px; text-align: center;">{{data.item.rank}}</span>
                    <span 
                        style="font-size:14px; line-height: 16px; font-weight: bold; text-transform: Capitalize; display: block;">
                        {{data.value}}
                    </span>
                </template>

            </b-table>
            
        </b-overlay>
       

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
    import { namespace } from "vuex-class";   
   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import { commonInfoType, locationInfoType } from '@/types/common';
   
    import moment from 'moment-timezone';
    import * as _ from 'underscore';
    import { attachedDutyInfoType, viewDutyInfoType } from '@/types/DutyRoster';
    import { teamMemberJsonType } from '@/types/MyTeam/jsonTypes';

    @Component
    export default class ViewDutyRoster extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public commonInfo!: commonInfoType;

        isViewDutyDataMounted = false;    
        maxRank = 1000;   
        updateTable=0;
        numberOfRecords = 0;
        numberOfRecordsPerPage = 10;
        maxPages = 1;
        currentPage = 1;
        pageIndex = 0;
        displayedDuties: viewDutyInfoType[] = [];
        sortedDuties: viewDutyInfoType[] = [];
        duties: viewDutyInfoType[] = [];
        dutiesJson: attachedDutyInfoType[] =[];
        courtAdminsJson: teamMemberJsonType[] = [];
        errorText =''
        openErrorModal=false;  
        today = ''; 
        
        timeHandle1
        timeHandle2

        fields=[
            {key:'displayName', label:'Name',             tdClass:'text-center', thClass:'text-center'},
            {key:'startTime',   label:'Start',            tdClass:'text-center', thClass:'text-center'},
            {key:'endTime',     label:'End',              tdClass:'text-center', thClass:'text-center'},
            {key:'assignment',  label:'Assignment',       tdClass:'text-center', thClass:'text-center'},
            {key:'note',        label:'Note',             tdClass:'text-center', thClass:'text-center'}
        ]        

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isViewDutyDataMounted) {
                this.getData();
            }            
        } 

        mounted()
        {
            this.numberOfRecords = 0;
            this.numberOfRecordsPerPage = 10;
            this.maxPages = 1;
            this.pageIndex = 0;
            this.currentPage = 1;
            this.timeHandle1 = window.setTimeout(this.updateCurrentDutiesCallBack, 1000); 
        }

        beforeDestroy() {
            clearTimeout(this.timeHandle1);
            clearTimeout(this.timeHandle2);                    
        }

        public updateCurrentDutiesCallBack() {

            if (this.pageIndex == 0){
                this.currentPage = 1;
                this.getData();
            } else {
                this.currentPage = this.pageIndex + 1;
                this.displayDutyInformation();
            }
            
            this.timeHandle2 = window.setTimeout(this.updateCurrentDutiesCallBack, 7000);
        }

        public getData() {            
            
            this.today = moment().tz(this.location.timezone).format();

            const endDate = moment.tz(moment().format().substring(0,10), this.location.timezone).endOf('day').utc().format();
            const startDate = moment.tz(moment().format().substring(0,10), this.location.timezone).startOf('day').utc().format();

            const url = 'api/dutyroster?locationId='+this.location.id+'&start='+startDate+'&end='+endDate;
        
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.dutiesJson = response.data;    
                        this.getTeamData();                    
                    }
                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }     
                    this.isViewDutyDataMounted=true;
                })              
        }

        public getTeamData() {             
            const url = 'api/courtAdmin';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.courtAdminsJson = response.data;    
                        this.extractTeamDutyInfo();                    
                    }
                    
                },err => {
                    this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                    if (err.response.status != '401') {
                        this.openErrorModal=true;
                    }     
                    this.isViewDutyDataMounted=true;
                })
        }      

        public extractTeamDutyInfo() {             

            this.duties = []; 

            for(const dutyJson of this.dutiesJson) {
                
                const dutyData = {} as viewDutyInfoType;

                for (const dutySlot of dutyJson.dutySlots){
                    if (!dutySlot.isNotAvailable && !dutySlot.isNotRequired && !dutySlot.isClosed && dutySlot.courtAdminId && !this.isFinished(dutySlot.endDate)){
                        const courtAdmin = this.courtAdminsJson.filter(courtAdmin => {if (courtAdmin.id == dutySlot.courtAdminId) return true})[0];
                        dutyData.firstName = courtAdmin.firstName;
                        dutyData.lastName = courtAdmin.lastName;
                        dutyData.rank = courtAdmin.rank;
                        dutyData.rankOrder = this.getRankOrder(dutyData.rank)[0]?this.getRankOrder(dutyData.rank)[0].id:0;
                        dutyData.displayName = Vue.filter('capitalizefirst')(courtAdmin.lastName) + ', ' + Vue.filter('capitalizefirst')(courtAdmin.firstName);
                        dutyData.courtAdminId = courtAdmin.id;
                        dutyData.assignment =  dutyJson.assignment.lookupCode.code + (dutyJson.assignment.name?(' (' + dutyJson.assignment.name + ')'):'');
                        dutyData.startTime = Vue.filter('beautify-time')(moment(dutySlot.startDate).tz(this.location.timezone).format());
                        dutyData.endTime = Vue.filter('beautify-time')(moment(dutySlot.endDate).tz(this.location.timezone).format());
                        dutyData.note = dutyJson.comment?dutyJson.comment:'';
                        this.duties.push(dutyData);
                    }
                }
                
            }
            
            this.numberOfRecords = this.duties.length;
            this.maxPages = Math.ceil(this.numberOfRecords / this.numberOfRecordsPerPage);
            if(this.maxPages == 0) this.maxPages =1;          
            this.displayDutyInformation();            
        }

        public displayDutyInformation(){

            this.sortedDuties = _.chain(this.duties)
                                .sortBy(duty =>{return (duty.lastName? duty.lastName.toUpperCase() : '')})
                                .sortBy(duty =>{return (duty.rankOrder? duty.rankOrder : this.maxRank + 1)})
                                .value()

            this.getRecordsToDisplay(this.pageIndex * this.numberOfRecordsPerPage);        
            
            this.updateTable ++;
            this.isViewDutyDataMounted = true;

        }      

        public getRankOrder(rankName: string) {
            return this.commonInfo.courtAdminRankList.filter(rank => {
                if (rank.name == rankName) {
                    return true;
                }
            })
        }

        public isFinished(dutySlotEndDate){

            let isFinished = false;
            const endTime = moment(dutySlotEndDate).tz(this.location.timezone).format();
            if (this.today < endTime) {               
                isFinished = false;
            } else {                
                isFinished = true;                
            }
            return isFinished;
        }

        public getRecordsToDisplay(startIndex: number){

            this.displayedDuties = this.sortedDuties.slice(startIndex, startIndex + this.numberOfRecordsPerPage);

            if (this.maxPages > this.pageIndex + 1) {
                this.pageIndex ++;

            } else {               
                this.pageIndex = 0;               
                
            }

        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>