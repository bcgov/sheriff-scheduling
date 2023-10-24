<template>
    <b-card v-if="dataReady" bg-variant="white">
        <b-row>
            <b-col cols="12">
                <page-header pageHeaderText="Reports"></page-header>
            </b-col>            
        </b-row>

        <b-row class="ml-1 mb-0 mx-4 text-primary">
            <b style="font-size:15pt;"><i>Search Criteria:</i></b>
        </b-row>
        <b-card class="search-box mb-4 text-center">
            <b-row class="ml-1 mt-n2 mx-0"> 
                <b-col cols="2">
                    <b-form-group style="margin:0; height: 2rem;">
                        <label class="h4 mb-2 p-0 float-left">Region</label> 
                        <b-form-select                                                                                                           
                            v-model="reportParameters.region"
                            @change="updateRegion()">
                            <b-form-select-option value="All">
                                All Regions
                            </b-form-select-option>							
                            <b-form-select-option
                                v-for="homeRegion in regionList" 
                                :key="homeRegion.id"
                                :value="homeRegion.id">
                                    {{homeRegion.name}}
                            </b-form-select-option>    
                        </b-form-select>
                    </b-form-group>
                </b-col>
                <b-col cols="4">
                    <b-form-group :key="updateRegionId" style="margin:0; height: 2rem;">
                        <label class="h4 mb-2 p-0 float-left">Location</label> 
                        <b-form-select
                            @change="clearReports()"                                                                                                           
                            v-model="reportParameters.location">
                            <b-form-select-option value="All">
                                All Locations
                            </b-form-select-option>							
                            <b-form-select-option
                                v-for="homelocation in locationOptionsList" 
                                :key="homelocation.id"
                                :value="homelocation.id">
                                    {{homelocation.name}}
                            </b-form-select-option>    
                        </b-form-select>
                    </b-form-group>
                </b-col>   
                <b-col cols="2">
                    <b-form-group style="margin:0;"> 
                        <label class="h4 mb-2 p-0 float-left"> Report Type </label>
                        <b-form-select
                            @change="clearReports()"
                            v-model="reportParameters.reportType"
                            :state = "reportTypeState?null:false">
                                <b-form-select-option
                                    state=true
                                    v-for="reportType in reportTypeOptions" 
                                    :key="reportType"
                                    :value="reportType">
                                        {{reportType}}
                                </b-form-select-option>     
                        </b-form-select>
                    </b-form-group>
                </b-col>           
                <b-col v-if="reportParameters.reportType && reportParameters.reportType == 'Training'" >
                    <b-form-group style="margin:0;"> 
                        <label class="h4 mb-2 p-0 float-left"> Training Type </label>
                        <b-form-select 
                            @change="clearReports()"                         
                            v-model="reportParameters.reportSubtype"
                            :state = "reportSubTypeState?null:false">
                                <b-form-select-option value="All">
                                    All Training Types
                                </b-form-select-option>
                                <b-form-select-option
                                    state=true
                                    v-for="trainingType in trainingTypeOptions" 
                                    :key="trainingType.id"
                                    :value="trainingType.id">
                                        {{trainingType.code}}
                                </b-form-select-option>     
                        </b-form-select>
                    </b-form-group>
                </b-col>
            </b-row>
            <b-row class="mt-4 mx-0">                
                <b-col cols="4">
                    <date-range :dateRange="reportDateRange" @rangeChanged="clearReports()"/>
                </b-col>                
                <b-col cols="5" class="text-left">                    
                    <label class="h4 mb-2 p-0 ml-4" >Filter By</label>                                        
                    <b-form-checkbox-group  
                        class="ml-4"
                        @change="filterReports()"                  
                        v-model="reportFilter"
                        size="lg"
                        :options="Object.values(statusOptions)"                        
                    />
                </b-col>
                <b-col cols="1"/>
                <b-col cols="2">                    
                    <b-button                        
                        style="float:right; padding: 0.25rem 1rem; margin-top:1.5rem;" 
                        :disabled="searching"                        
                        variant="primary"
                        @click="find()"
                        ><spinner color="#FFF" v-if="searching" style="margin:0; padding:0 3rem; height:2rem; transform:translate(0px,-24px);"/>
                        <span style="font-size: 18px;" v-else><b-icon-table class="mr-2"/>Generate Report</span>
                    </b-button>
                </b-col> 
            </b-row>                   
        </b-card> 

        <loading-spinner color="#000" v-if="searching && !dataLoaded" waitingText="Loading ..." />  
                  
        <b-card class="table-box" v-else-if="!searching && dataLoaded"> 
            <b-row v-if="filteredTrainingReportData.length == 0" class="h4 mb-0 text-primary">
                No Records Found.
            </b-row>
            <div v-else>
                <b-table            
                    :items="filteredTrainingReportData"
                    :fields="trainingFields"     
                    sort-by="name"
                    class="mt-3"       
                    bordered
                    head-variant="light"            
                    small 
                    responsive="sm">
                    <template v-slot:cell(end) ="data"> {{data.value | beautify-date}} </template>
                    <template v-slot:cell(status) ="data">                   
                        <b-badge :variant="data.item['_rowVariant']" style="width:6rem;" >{{data.value}}</b-badge>                        
                    </template>
                    <template v-slot:cell(expiryDate) ="data">{{data.value | beautify-date}}</template>
                    <template v-slot:cell(excluded) ="data"><b-form-checkbox v-model="data.item.excluded" @change="splitExcludedReports(true, data.item.courtAdminId)"/></template>
                </b-table>

                <b-row class="mt-5 mx-0">                
                    <b-button                        
                        style="margin:0 0 0 auto; padding: 0.25rem 1rem;" 
                        :disabled="generatingReport"                   
                        variant="primary"
                        @click="downloadReport()"
                        ><spinner color="#FFF" v-if="generatingReport" style="margin:0; padding: 0; height:2rem; transform:translate(0px,-24px);"/>
                        <span style="font-size: 18px;" v-else><b-icon-download class="mr-2"/>Download SCV File</span>
                    </b-button>                
                </b-row>
            </div>
        </b-card>

        <b-card class="excluded-table-box my-5" v-if="!searching && dataLoaded && excludedTrainingReportData.length > 0">
            <b-row class="h3 ml-1 mb-1 mx-0 text-primary">
                Excluded From Report:
            </b-row>
            <b-table            
                :items="excludedTrainingReportData"
                :fields="trainingFields"     
                sort-by="start"
                class="mt-3"       
                bordered
                head-variant="light"            
                small 
                responsive="sm">                
                <template v-slot:cell(end) ="data"> {{data.value | beautify-date}} </template>
                <template v-slot:cell(status) ="data">                   
                    <b-badge :variant="data.item['_rowVariant']" style="width:6rem;" >{{data.value}}</b-badge>                        
                </template>
                <template v-slot:cell(expiryDate) ="data">{{data.value | beautify-date}}</template>
                <template v-slot:head(excluded)>Excluded</template>
                <template v-slot:cell(excluded) ="data"><b-form-checkbox v-model="data.item.excluded" @change="splitExcludedReports(false, data.item.courtAdminId)"/></template>
            </b-table>
        </b-card>         

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import moment from 'moment-timezone';
    import * as _ from 'underscore';  
    import { ExportToCsv } from 'export-to-csv';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");      
    import PageHeader from "@components/common/PageHeader.vue";
    import Spinner from "@components/Spinner.vue";
    import DateRange from "./Components/DateRange.vue"
    
    import {reportInfoType, locationInfoType, regionInfoType, dateRangeInfoType} from '@/types/common';
    import { trainingReportInfoType } from '@/types/MyTeam';
    import { leaveTrainingTypeInfoType } from '@/types/ManageTypes';

    @Component({
        components: {
            PageHeader,
            Spinner,
            DateRange
        }
    })
    export default class ViewReports extends Vue {

        @commonState.State
        public locationList!: locationInfoType[]; 

        @commonState.State
        public regionList!: regionInfoType[]; 

        dataReady = false;
        dataLoaded = false;    
        error = '';
        updateRegionId = 0;
        printReady = false;
        reportParameters = {region: 'All', location: 'All', reportSubtype: 'All', reportType:'Training'} as reportInfoType;        
        reportDateRange  = { startDate:'', endDate:'', valid:false} as dateRangeInfoType
        trainingReportData: trainingReportInfoType[] = []
        filteredTrainingReportData: trainingReportInfoType[] = []
        excludedTrainingReportData: trainingReportInfoType[] = []
        searching = false;
        generatingReport = false;
        reportTypeOptions = ['Training']
        reportTypeState = true;        
        trainingTypeOptions: leaveTrainingTypeInfoType[] = [];
        reportSubTypeState = true;        
        reportFilter: string[] = []
        locationOptionsList: locationInfoType[] = [];
        
        statusOptions = {danger:'Not Met', warning:'Expired', court:'Expiring Soon'}

        trainingFields = [
            {key:"excluded",     label:"Exclude",                   thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: false,thStyle:'width:5%; line-height:1rem;'},
            {key:"name",         label:"Name",                      thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:30%;'},
            {key:"trainingType", label:"Training Type",             thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:15%;'},            
            {key:"end",          label:"Completion Date",           thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:20%;'},
            {key:"expiryDate",   label:"Certification Expiry Date", thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:20%;'},
            {key:"status",       label:"Status",                    thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:10%;'}
        ];
        
        mounted() {   
            this.dataReady = false;
            this.clearReports();
            this.reportParameters.region = 'All';     
            this.reportParameters.location = 'All';  
            this.reportParameters.reportSubtype = 'All';   
            this.searching = false;          
            this.generatingReport = false;     
            this.locationOptionsList = this.locationList;
            this.getTrainingTypes();
        }

        public clearReports(){
            this.dataLoaded = false;
            this.trainingReportData = [];
            this.excludedTrainingReportData = [];
        }
        
        public find(){ 
            this.clearReports()
            this.reportTypeState = true;
            this.reportSubTypeState = true;
            if (!this.reportParameters.reportType){
                this.reportTypeState = false;
            } else if (!this.reportParameters.reportSubtype){
                this.reportSubTypeState = false;
            } else {
                
                this.searching = true;
                const calls: any[] =[]
                calls.push(this.$http.get("api/courtAdmin/training"));
                calls.push(this.$http.get("api/courtAdmin"));

                Promise.all(calls).then(values => {
                    if(values[0]?.data && values[0]?.data){                        
                        const courtAdminWithTrainings = values[0].data;
                        const courtAdmins = values[1].data;
                        const courtAdminWithTrainingIds = courtAdminWithTrainings.map(courtAdmin => courtAdmin.id)
                        for(const courtAdmin of courtAdmins){
                            if(!courtAdminWithTrainingIds.includes(courtAdmin.id)){
                                courtAdminWithTrainings.push(courtAdmin)
                            }
                        }
                        this.extractData(courtAdminWithTrainings);
                    }
                }, err =>{this.searching = false;this.error = err.response.data})
            } 
        }

        public extractData(data){ 

            if (this.reportParameters.reportType == 'Training'){
                this.gatherTrainingReportData(data)
            }             
        }

        public updateRegion(){
            
            if (this.reportParameters.region != 'All'){
                this.locationOptionsList = this.locationList.filter(location=>{if(location.regionId == this.reportParameters.region)return true;});                
            } else {
                this.locationOptionsList = this.locationList;
            }
            this.clearReports();
            this.updateRegionId ++; 
        }

        public gatherTrainingReportData(data: any[]){

            let reportInfo: any[] = [];               
            
            if (this.reportParameters.location && this.reportParameters.location != 'All'){
                reportInfo = data.filter(courtAdmin=>(courtAdmin.homeLocationId == this.reportParameters.location));               
                
            } else {
                reportInfo = data.filter(courtAdmin=>(this.locationOptionsList.some(location => courtAdmin.homeLocationId == location.id)))
            }
            
            const mandetoryTrainings = this.trainingTypeOptions.filter(training => training.mandatory)
            
            for (const courtAdminData of reportInfo){

                if (courtAdminData.isEnabled){
                    const courtAdminTrainingsId = courtAdminData.training? courtAdminData.training.map(training => training.trainingTypeId): [];
                    const courtAdminTrainings = courtAdminData.training? JSON.parse(JSON.stringify(courtAdminData.training)) : [];
                    
                    for(const training of mandetoryTrainings){
                        if(!courtAdminTrainingsId.includes(training.id)){
                            courtAdminTrainings.push({
                                comment: "",
                                endDate: "",
                                firstNotice: false,
                                id: null,
                                courtAdminId: courtAdminData.id,
                                startDate: "",
                                timezone: "",
                                trainingCertificationExpiry: "",
                                trainingType: training,
                                trainingTypeId: training.id
                            })
                        }
                    }

                    for (const trainingData of courtAdminTrainings){
                        if (  (this.reportParameters.reportSubtype != 'All' && this.reportParameters.reportSubtype == trainingData.trainingType.id) 
                            || this.reportParameters.reportSubtype == 'All'){
                            this.addTrainingToReport(courtAdminData, trainingData);
                        }
                    }
                }
            } 
            if(this.reportDateRange.valid){
                this.trainingReportData = this.trainingReportData.filter(training => 
                    training.end>=this.reportDateRange.startDate  &&
                    training.end<=this.reportDateRange.endDate
                )
            }
            this.filterReports()
            this.searching = false;
            this.dataLoaded = true;            
        }

        public addTrainingToReport(courtAdminData, trainingData){
            //console.log(courtAdminData)
            let rowType = ''
            const trainingInfo = {} as trainingReportInfoType;
            trainingInfo.name = courtAdminData.firstName + ' ' + courtAdminData.lastName;
            trainingInfo.courtAdminId = courtAdminData.id;
            trainingInfo.trainingType = trainingData.trainingType.description;
            const timezone = trainingData.timezone?trainingData.timezone:'America/Vancouver';
            trainingInfo.end = trainingData.endDate? moment(trainingData.endDate).tz(timezone).format():'';
            trainingInfo.expiryDate = trainingData.trainingCertificationExpiry? moment(trainingData.trainingCertificationExpiry).tz(timezone).format():'';
            trainingInfo.excluded = false;
            const todayDate = moment().tz(timezone).format();
            const advanceNoticeDate = moment().tz(timezone).add(trainingData.trainingType.advanceNotice, 'days').format()

            if(!trainingData.endDate) rowType ='danger'
            else if(trainingInfo.expiryDate && todayDate>trainingInfo.expiryDate) rowType ='warning'
            else if(trainingInfo.expiryDate && trainingData.trainingType.advanceNotice && advanceNoticeDate>trainingInfo.expiryDate) rowType ='court'
            trainingInfo['_rowVariant'] = rowType; //danger warning court
            trainingInfo.status = rowType? this.statusOptions[rowType] : ''
            this.trainingReportData.push(trainingInfo);
        }

        public downloadReport(){ 

            this.generatingReport = true;

            const fileName = 'Training Report';
            const options = { 
                fieldSeparator: ',',
                quoteStrings: '"',
                decimalSeparator: '.',
                showLabels: true, 
                showTitle: false,
                title: '',
                filename: fileName,
                useTextFile: false,
                useBom: true,
                useKeysAsHeaders: false,
                headers: ['Name', 'Training Type', 'End Date', 'Expiry Date', 'Status']
            };

            const reportData: trainingReportInfoType[] = [];

            for (const trainingData of this.filteredTrainingReportData){
                if(trainingData.excluded) continue
                const trainingInfo = {} as trainingReportInfoType;
                trainingInfo.name = trainingData.name;
                trainingInfo.trainingType = trainingData.trainingType;                
                trainingInfo.end = trainingData.end?.length>0?Vue.filter('beautify-date')(trainingData.end):'';
                trainingInfo.expiryDate = trainingData.expiryDate?.length>0?Vue.filter('beautify-date')(trainingData.expiryDate):''; 
                trainingInfo.status = trainingData.status;
                reportData.push(trainingInfo)                
            }

            const csvExporter = new ExportToCsv(options);
            csvExporter.generateCsv(_.sortBy(reportData, 'name'));
            this.generatingReport = false;
        }

        public getTrainingTypes() {                      
                
            const url = 'api/managetypes?codeType=TrainingType&showExpired=false';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.trainingTypeOptions = response.data;                  
                    }
                    this.dataReady = true;
                   
                },err => {
                    console.log(err.response)
                    this.dataReady = true; 
                }) 
                  
        }

        public splitExcludedReports(exclude, courtAdminId){
            Vue.nextTick(()=>{
                if(exclude){
                    const trainings = this.trainingReportData.filter(training => training.courtAdminId==courtAdminId);                                        
                    this.excludedTrainingReportData.push(...trainings) 
                    this.trainingReportData = this.trainingReportData.filter(training => training.courtAdminId!=courtAdminId);
                }else {
                    const trainings = this.excludedTrainingReportData.filter(training => training.courtAdminId==courtAdminId);                     
                    this.trainingReportData.push(...trainings) 
                    this.excludedTrainingReportData = this.excludedTrainingReportData.filter(training => training.courtAdminId!=courtAdminId);
                }
                this.excludedTrainingReportData.forEach(training => training.excluded = true);
                this.trainingReportData.forEach(training => training.excluded = false);
                this.filterReports()
            })            
        }

        public filterReports() {
            Vue.nextTick(() => {
                this.filteredTrainingReportData = JSON.parse(JSON.stringify(this.trainingReportData))
                if(this.reportFilter.length>0){
                    this.filteredTrainingReportData =this.filteredTrainingReportData.filter(training => training.status && this.reportFilter.includes(training.status))
                }
            })
        }

}
</script>

<style scoped>   

    .card {
        border: white;
    }

    .search-box {
        margin: 0 1.5rem;
        background: #c7d6dd;
        box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
    }

    .table-box {
        margin: 0 1.5rem;
        padding: 0 1rem;
        background: #f7f8f9;
        box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.1), 0 6px 20px 0 rgba(0, 0, 0, 0.09);
    }

    .excluded-table-box {
        margin: 0 1.5rem;
        padding: 0 1rem;
        background: #eaf0f7;
        box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.1), 0 6px 20px 0 rgba(0, 0, 0, 0.09);
    }

    ::v-deep .custom-control-label {
        font-size: 13pt;
        padding-top: 0.15rem;
        padding-right: 1rem;
    }

</style>
