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
                <b-col cols="4">
                    <v-app class="vuetify">
                        <label class="h4 p-0 float-left mr-auto"> Sheriff </label>
                        <v-autocomplete class="mt-2" v-model="reportParameters.sheriffId" :items="sheriffList" item-text="fullName" item-value="id" solo>
                            <template v-slot:item="data">
                                <template>
                                    <v-list-item-content v-text="data.item.fullName"></v-list-item-content>
                                </template>
                            </template>    
                        </v-autocomplete>
                    </v-app>
                </b-col> 
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
            </b-row>
            <b-row class="mt-4 mx-0">              
                <b-col cols="4" v-if="reportParameters.reportType && reportParameters.reportType == 'Training'" >
                    <v-app class="vuetify">
                        <label class="h4 mb-2 p-0 float-left mr-auto"> Training Type </label>
                        <v-select
                            @change="clearReports()"
                            v-model="reportParameters.reportSubtype"
                            :items="trainingTypeOptions"                                
                            label="Select"
                            item-text="code"
							item-value="id"
                            :error="!reportSubTypeState"                            
                            multiple
                            flat
                            hide-details
                            solo>
                            <template v-slot:prepend-item>
                                <v-list-item
                                ripple
                                @mousedown.prevent
                                @click="toggleAllSubtypes"
                                >
                                <v-list-item-action>
                                    <v-icon :color="reportParameters.reportSubtype.length > 0 ? 'indigo darken-4' : ''">
                                    {{ subtypeSelectionIcon }}
                                    </v-icon>
                                </v-list-item-action>
                                <v-list-item-content>
                                    <v-list-item-title>
                                    Select All
                                    </v-list-item-title>
                                </v-list-item-content>
                                </v-list-item>
                                <v-divider class="mt-2"></v-divider>
                            </template>
                            <template v-slot:selection="{ item, index }">
                                <span v-if="reportParameters.reportSubtype.length==1">
                                    {{item.code | truncate(25)}}
                                </span>                                
                                <span v-else-if="index === 0" class="grey--text text-caption">
                                    <span v-if="reportParameters.reportSubtype.length<trainingTypeOptions.length">{{ reportParameters.reportSubtype.length }} training types selected</span>
                                    <span v-else>All training types selected</span>
                                </span>
                            </template>
                        </v-select>
                    </v-app>
                </b-col>
                <b-col cols="8">
                    <date-range :dateRange="reportDateRange" @rangeChanged="clearReports()"/>
                </b-col>
            </b-row>
            <b-row class="mt-4 mx-0">              
                <b-col cols="6" class="text-left">                    
                    <label class="h4 mb-2 p-0 ml-4" >Filter By</label>                                        
                    <b-form-checkbox-group  
                        class="ml-4"
                        @change="filterReports()"                  
                        v-model="reportFilter"
                        size="lg"
                        :options="Object.values(statusOptions)"                        
                    />
                </b-col>
                <b-col cols="6">                    
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
                <custom-pagination
                    :key="'pagination-top-'+paginationKey"                                         
                    :pages="[15,25,50,75,100]"
                    :totalRows="totalRows"
                    :initCurrentPage="currentPage"
                    :initItemPerPage="itemsPerPage"
                    @paginationChanged="paginationChanged"/>
                
                <b-table            
                    :items="filteredTrainingReportData"
                    :fields="trainingFields"     
                    sort-by="name"
                    class="mt-3"       
                    bordered
                    head-variant="light"            
                    small                     
                    :currentPage="currentPage"
                    :perPage="itemsPerPage"
                    responsive="sm">
                    <template v-slot:cell(name) ="data"><div v-b-tooltip.hover.right.noninteractive.v-primary :title="data.item.location" >{{data.value}}</div></template>
                    <template v-slot:cell(end) ="data"> {{data.value | beautify-date}} </template>
                    <template v-slot:cell(status) ="data">                   
                        <b-badge :variant="data.item['_rowVariant']" style="width:6rem;" >{{data.value}}</b-badge>                        
                    </template>
                    <template v-slot:cell(expiryDate) ="data">{{data.value | beautify-date}}</template>
                    <template v-slot:cell(excluded) ="data"><b-form-checkbox v-model="data.item.excluded" @change="excludeFromReports(true, data.item.sheriffId)"/></template>
                </b-table>

                <b-row class="mt-5 mx-0">                
                    <b-button                        
                        style="margin:0 0 0 auto; padding: 0.25rem 1rem;" 
                        :disabled="generatingReport"                   
                        variant="primary"
                        @click="downloadReport()"
                        ><spinner color="#FFF" v-if="generatingReport" style="margin:0; padding: 0; height:2rem; transform:translate(0px,-24px);"/>
                            <b-row class="mx-1" style="font-size: 18px;" v-else>
                                <b-icon-download class="mr-2"/>                            
                                Download CSV File
                                <div style="line-height:1rem; transform:translate(5px,6px)">        
                                    <i style="font-size:20pt;" class="mdi mdi-file-excel"></i>
                                </div>
                            </b-row>
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
                :currentPage="currentExcludedPage"
                :perPage="itemsPerPageExcluded" 
                responsive="sm">                
                <template v-slot:cell(end) ="data"> {{data.value | beautify-date}} </template>
                <template v-slot:cell(status) ="data">                   
                    <b-badge :variant="data.item['_rowVariant']" style="width:6rem;" >{{data.value}}</b-badge>                        
                </template>
                <template v-slot:cell(expiryDate) ="data">{{data.value | beautify-date}}</template>
                <template v-slot:head(excluded)>Excused</template>
                <template v-slot:cell(excluded) ="data"><b-form-checkbox v-model="data.item.excluded" @change="excludeFromReports(false, data.item.sheriffId)"/></template>
            </b-table>
            <custom-pagination
                :key="'pagination-excluded-'+paginationExcludedKey"                                         
                :pages="[15,25,50,75,100]"
                :totalRows="totalExcludedRows"
                :initCurrentPage="currentExcludedPage"
                :initItemPerPage="itemsPerPageExcluded"
                @paginationChanged="paginationExcludedChanged"/>
        </b-card>         

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import * as _ from 'underscore';  
    import { ExportToCsv } from 'export-to-csv';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");      
    import PageHeader from "@components/common/PageHeader.vue";
    import Spinner from "@components/Spinner.vue";
    import DateRange from "./Components/DateRange.vue"
    import CustomPagination from "./Components/CustomPagination.vue"

    import {reportInfoType, locationInfoType, regionInfoType, dateRangeInfoType} from '@/types/common';
    import { sheriffOption, trainingReportInfoType, trainingStatusInfoType } from '@/types/MyTeam';
    import { leaveTrainingTypeInfoType } from '@/types/ManageTypes';

    @Component({
        components: {
            PageHeader,
            Spinner,
            DateRange,
            CustomPagination
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
        reportParameters = {region: 'All', location: 'All', reportSubtype: [] as number[], reportType:'Training',} as reportInfoType;        
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

        currentPage = 1;
        itemsPerPage = 15;// Default
        paginationKey = 0;
        totalRows = 0;

        currentExcludedPage = 1;
        itemsPerPageExcluded = 15;// Default
        paginationExcludedKey = 0;
        totalExcludedRows = 0;

        statusOptions: trainingStatusInfoType = {danger:'Not Taken', alert:'Expired', warning:'Requalification',  notify:'Notified'}

        trainingFields = [
            {key:"excluded",     label:"Excuse",                    thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: false,thStyle:'width:5%; line-height:1rem;'},
            {key:"name",         label:"Name",                      thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:25%;'},
            {key:"trainingType", label:"Training Type",             thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:35%;'},            
            {key:"end",          label:"Completion Date",           thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:10%;'},
            {key:"expiryDate",   label:"Certification Expiry Date", thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:15%;'},
            {key:"status",       label:"Status",                    thClass: 'border-bottom align-middle text-center', tdClass:'align-middle text-center', sortable: true, thStyle:'width:10%;'}
        ];

        sheriffList: sheriffOption[] = [];
        
        mounted() {   
            this.dataReady = false;
            this.clearReports();
            this.reportParameters.region = 'All';     
            this.reportParameters.location = 'All';
            this.searching = false;          
            this.generatingReport = false;     
            this.locationOptionsList = this.locationList;
            this.getTrainingTypes();     
        }

        public clearReports(){
            this.dataLoaded = false;
            this.reportSubTypeState = true
            this.trainingReportData = [];
            this.excludedTrainingReportData = [];
        }
        
        public find(){
             
            this.clearReports()
            this.reportTypeState = true;
            this.reportSubTypeState = true;

            if(!this.reportDateRange.valid){
                this.reportDateRange.startDate = ''
                this.reportDateRange.endDate = ''
            }

            if (!this.reportParameters.reportType){
                this.reportTypeState = false;
            } 
            else if (this.reportParameters.reportSubtype?.length==0){
                this.reportSubTypeState = false;
            } 
            else {
                
                const body = {
                    regionId: this.reportParameters.region == 'All'? null : this.reportParameters.region,
                    locationId: this.reportParameters.location == 'All'? null : this.reportParameters.location, 
                    // reportType: this.reportParameters.reportType,
                    reportSubtypeIds: this.isAllSubtypesSelected? [] : this.reportParameters.reportSubtype,
                    startDate: this.reportDateRange.valid? this.reportDateRange.startDate : null,
                    endDate: this.reportDateRange.valid? this.reportDateRange.endDate : null,
                    sheriffId: this.reportParameters.sheriffId? this.reportParameters.sheriffId : null
                }

                this.searching = true;
                this.$http.post("api/sheriff/training/reports", body)
                .then(response => {
                    if(response.data){                        
                        this.trainingReportData = response.data;
                        this.filterReports()
                        this.dataLoaded = true;
                    }
                    this.searching = false;                   
                },err => {
                    console.log(err.response)
                    this.error = err.response.data
                    this.searching = false;
                })
            } 
        }

        public getSheriffs() {            
            const url = 'api/sheriff';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.sheriffList = response.data.map((s) => {
                            return {
                                id: s.id, 
                                fullName: Vue.filter('capitalizefirst')(s.firstName) + ' ' + Vue.filter('capitalizefirst')(s.lastName)
                            }
                        });    

                        // after getting the sheriffs; read query params and set the sheriffId
                        const queryParams = this.$route.query;
                        if (queryParams?.sheriffId) {
                            this.reportParameters.sheriffId = (queryParams.sheriffId || '') as string;
                            // remove the query params from the url
                            this.$router.replace({});

                            // generate the report
                            this.find();
                        }
                    }
                    
                    this.dataReady = true;
                },err => {
                    console.log(err.response);
                    
                    this.dataReady = true;
                })
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
                headers: ['Location', 'Name', 'Training Type', 'End Date', 'Expiry Date', 'Status']
            };

            const reportData: trainingReportInfoType[] = [];

            for (const trainingData of this.filteredTrainingReportData){
                if(trainingData.excluded) continue
                const trainingInfo = {} as trainingReportInfoType;
                trainingInfo.location = trainingData.location;
                trainingInfo.name = trainingData.name;
                trainingInfo.trainingType = trainingData.trainingType;                
                trainingInfo.end = trainingData.end?.length>0?Vue.filter('beautify-date')(trainingData.end):'';
                trainingInfo.expiryDate = trainingData.expiryDate?.length>0?Vue.filter('beautify-date')(trainingData.expiryDate):''; 
                trainingInfo.status = this.beautifyTrainingStatusForExcel(trainingData.status);
                reportData.push(trainingInfo)                
            }

            const csvExporter = new ExportToCsv(options);
            csvExporter.generateCsv(_.sortBy(reportData, 'name'));
            this.generatingReport = false;
        }

        public beautifyTrainingStatusForExcel(status){
            if(!status || status?.trim()?.length==0) return "Qualified"
            else if(status == 'Requalification') return status+' Required'
            else return status
        }

        public getTrainingTypes() {                      
                
            const url = 'api/managetypes?codeType=TrainingType&showExpired=false';
            this.$http.get(url)
                .then(response => {
                    if(response.data){
                        this.trainingTypeOptions = response.data;                  
                    }
                    this.reportParameters.reportSubtype = this.trainingTypeOptions.map(t => t.id)

                    // get sheriffs after getting the training types
                    this.getSheriffs();
                   
                },err => {
                    console.log(err.response)
                    // try gettin sheriffs after getting the training types
                    this.getSheriffs(); 
                })                   
        }

        public excludeFromReports(exclude, sheriffId){
            Vue.nextTick(()=>{
                const body = {               
                    excused: exclude,
                    id: sheriffId
                }

                const url = 'api/sheriff/updateExcused';
                this.$http.put(url, body)
                    .then(() => {
                        this.find()                                   
                    }, err => {               
                        this.error = err.response.data.error;
                        this.find()                   
                    });
            })
        }

        public filterReports() {
            Vue.nextTick(() => {
                
                this.excludedTrainingReportData = this.trainingReportData.filter(training => training.excluded)

                if(this.reportFilter.length>0){
                    this.filteredTrainingReportData = this.trainingReportData.filter(training => !training.excluded && training.status && this.reportFilter.includes(training.status))
                }
                else
                    this.filteredTrainingReportData = this.trainingReportData.filter(training => !training.excluded)
                
                this.totalRows = this.filteredTrainingReportData.length;
                this.currentPage = 1;
                this.paginationChanged(this.currentPage, this.itemsPerPage)

                this.totalExcludedRows = this.excludedTrainingReportData.length;
                this.currentExcludedPage = 1;
                this.paginationExcludedChanged(this.currentExcludedPage, this.itemsPerPageExcluded)
            })
        }

        public paginationChanged(currentPage, itemsPerPage){
            this.currentPage = currentPage
            this.itemsPerPage = itemsPerPage
            this.paginationKey++;
        }

        public paginationExcludedChanged(currentPage, itemsPerPage){
            this.currentExcludedPage = currentPage
            this.itemsPerPageExcluded = itemsPerPage
            this.paginationExcludedKey++;
        }

        public toggleAllSubtypes() {
            Vue.nextTick(() => {
                if (this.isAllSubtypesSelected) {
                    this.reportParameters.reportSubtype = []
                } else {
                    this.reportParameters.reportSubtype = this.trainingTypeOptions.map(t => t.id)
                }
            })
        }

        get isAllSubtypesSelected(){
            return this.trainingTypeOptions.length==this.reportParameters.reportSubtype.length
        }
        
        get isSomeSubtypesSelected() {
            return this.reportParameters.reportSubtype.length > 0 && !this.isAllSubtypesSelected
        }
        
        get subtypeSelectionIcon() {
            if (this.isAllSubtypesSelected) return 'mdi-close-box'
            if (this.isSomeSubtypesSelected) return 'mdi-minus-box'
            return 'mdi-checkbox-blank-outline'
        }

}
</script>

<style scoped  lang="scss">

    ::v-deep .vuetify{
        @import "@/styles/vuetify.scss";
        .v-application--wrap{
            height: 2rem;
            min-height:2rem !important;
        }
        
        .v-text-field .v-input__control .v-input__slot {
            min-height: 2.4rem !important;
            display: flex !important;
            align-items: center !important;
            border: 1px solid #adb5bd;
            background-color: #fff;
        }

        .v-text-field.error--text .v-input__control .v-input__slot {
            min-height: 2.4rem !important;
            display: flex !important;
            align-items: center !important;
            border: 1px solid #f71b03;
        }

        .v-list-item__content {
            flex: none;
            margin-left: 1rem;
        }
    }

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
