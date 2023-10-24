<template>
    <b-card >
        <div v-if="!dataReady">
            <loading-spinner />
        </div>
        <div v-else>
            <b-row>
                <b-col cols="2" class="">
                    <b-img style="width:100%; margin:0 0 0 0;" 
                        src="./images/bcss-crest.png"                                                         
                        alt="B.C. Gov"/>
                </b-col>
                <b-col cols="10">
                    <b-row class="info-box">
                        <b-col >
                            <b-col class="text-light text-center mt-0">
                                <div style="font-size:25pt;" ><b>CourtAdmin Scheduling System</b></div>                                
                            </b-col>
                            <div class=" mt-n1 mb-2 text-center text-shift" style="font-size:16pt;">
                                <i>Welcome <b class="ml-1">{{courtAdminName}}</b></i>
                            </div>
                        </b-col>

                        <b-col cols="5" >
                            <div class="float-right mr-4">
                                <div class="text-info h2 mt-3"><b-icon-house /> {{location.name}}</div>                        
                                <div class="text-warning mt-n3" style="font-size:16pt;"><b-icon-calendar2 /> <b class="ml-2">{{today}}</b> </div>                                                    
                            </div> 
                        </b-col>   
                    </b-row>
                    <b-row class="mt-5">
                        <b-col cols="6" v-if="trainingAlert">
                            <b-card class="border training-box" no-body>
                                <b-card-header class="h3 bg-primary text-white">Training Status</b-card-header>
                                <b-card-body>                        
                                    <b-alert
                                        v-for="item,inx in training.notmet"
                                        :key="'not-met-'+inx"
                                        class="mx-2 my-3"
                                        variant="danger"
                                        :show="true">
                                            {{item.trainingType}}
                                        <b class="float-right">{{item.status}}</b>
                                    </b-alert>
                                    <b-alert
                                        v-for="item,inx in training.expired"
                                        :key="'expired-'+inx"
                                        class="mx-2 my-3"
                                        variant="warning"
                                        :show="true">
                                            {{item.trainingType}}
                                        <b class="float-right">{{item.status}}</b>
                                    </b-alert>
                                    <b-alert
                                        v-for="item,inx in training.expiringsoon"
                                        :key="'expiring-soon-'+inx"
                                        class="mx-2 my-3"
                                        variant="court"
                                        :show="true">
                                            {{item.trainingType}}                                         
                                            <i class="float-right ml-2"> ({{item.expiryDate | expiry-date}}) </i>
                                            <b class="float-right"> {{item.status}} </b>
                                    </b-alert>
                                </b-card-body>
                            </b-card>
                        </b-col>
                        <b-col cols="6" v-if="courtAdminEvents.length>0">
                            <b-card class="border training-box" no-body>
                                <b-card-header class="h3 bg-primary text-white">Upcoming Events</b-card-header>
                                <b-card-body>                        
                                    <b-alert
                                        v-for="item,inx in courtAdminEvents"
                                        :key="'court-admin-events-'+inx"
                                        class="mx-2 my-3"
                                        variant="info"
                                        :show="true">
                                        <b-icon-box-arrow-left v-if="item.type=='Loaned'" font-scale="1.5" v-b-tooltip.hover.v-primary.left="'Loaned To'" class="mr-1 mt-0 mb-n1" />                                
                                        <font-awesome-icon v-if="item.type=='Training'" icon="graduation-cap" v-b-tooltip.hover.v-primary.left="'Training'"  style="font-size: 1.5rem;" />
                                        <b class=""> {{item.start | event-date-time}}</b>
                                        <i class="ml-4 text-dark">{{item.name}}</i>
                                        <b-icon-chat-square-text-fill v-if="item.comment" v-b-tooltip.hover.left="item.comment"  class="ml-2" variant="info" font-scale="0.99"/> 
                                    </b-alert>
                                </b-card-body>
                            </b-card>
                        </b-col>
                    </b-row>
                </b-col>                  
            </b-row>            
        </div>
        
    </b-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { namespace } from 'vuex-class';
import moment from 'moment-timezone';
import * as _ from 'underscore';
import "@store/modules/CommonInformation";
import { locationInfoType, userInfoType } from '@/types/common';
import { teamMemberJsonType, userEventsInfoType } from '@/types/MyTeam/jsonTypes';
import { trainingReportInfoType } from '@/types/MyTeam';
import { leaveTrainingTypeInfoType } from '@/types/ManageTypes';

const commonState = namespace("CommonInformation");    

@Component
export default class Home extends Vue {

    
    @commonState.State
    public userDetails!: userInfoType;


    courtAdminName = ''
    location = {} as locationInfoType;
    
    trainingTypeOptions: leaveTrainingTypeInfoType[] = [];
    statusOptions = {danger:'Not Met', warning:'Expired', court:'Expiring Soon'}
    training: { 
        notmet: trainingReportInfoType[]; 
        expired: trainingReportInfoType[]; 
        expiringsoon: trainingReportInfoType[]; 
    } = { notmet: [], expired: [], expiringsoon: [] }

    trainingAlert=false;

    courtAdminEvents: userEventsInfoType[] = []

    today=""
    dataReady=false
    errorText=''

    mounted() {
        this.dataReady=false;
        this.trainingAlert=false;
        this.getTrainingTypes()
    }

    public getTrainingTypes() {                      
                
        const url = 'api/managetypes?codeType=TrainingType&showExpired=false';
        this.$http.get(url)
            .then(response => {
                if(response.data){
                    this.trainingTypeOptions = response.data;
                    this.loadUserDetails(this.userDetails.userId)                  
                }
                else 
                    this.dataReady = true;
                
            },err => {
                console.log(err.response)
                this.dataReady = true; 
            }) 
                
    }

    public loadUserDetails(userId): void {
        this.errorText='';
        this.dataReady=false;

        const url = 'api/courtAdmin/' + userId;
        this.$http.get(url)
            .then(response => {
                if(response.data){                                              
                   this.extractUserInfo(response.data);          
                }else  
                    this.dataReady=true;                  
            },err => {
                this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format();
                this.dataReady=true;              
            });
    }


    public extractUserInfo(user: teamMemberJsonType){
        //
        // console.log(user)
        this.location = user.homeLocation
        this.courtAdminName = user.firstName + ' ' + user.lastName; 
        this.extractTrainings(user);
        this.extractAwayLocations(user);
        this.courtAdminEvents = _.sortBy(this.courtAdminEvents, 'start')
        this.today = moment().tz(this.location.timezone).format("dddd - MMM DD, YYYY");
        this.dataReady=true;
    }

    
    public extractTrainings(courtAdminData){
        const mandetoryTrainings = this.trainingTypeOptions.filter(training => training.mandatory)
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
        this.training = { notmet:[], expired:[],expiringsoon:[] }
        for (const trainingData of courtAdminTrainings){
            this.addTrainingToReport(courtAdminData, trainingData);            
        }
    }

    public addTrainingToReport(courtAdminData, trainingData){
        //console.log(courtAdminData)
        let rowType = ''
        const trainingInfo = {} as trainingReportInfoType;
        trainingInfo.name = courtAdminData.firstName + ' ' + courtAdminData.lastName;
        trainingInfo.courtAdminId = courtAdminData.id;
        trainingInfo.trainingType = trainingData.trainingType.description;
        const timezone = trainingData.timezone?trainingData.timezone:'America/Vancouver';
        trainingInfo.start = trainingData.startDate? moment(trainingData.startDate).tz(timezone).format():'';
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
        // console.log(trainingInfo)
        
        if(rowType){
            this.trainingAlert=true;
            const type = this.statusOptions[rowType].replace(' ','').toLowerCase()
            this.training[type].push(trainingInfo)
        }
        const currentDate = moment().format()
        const nextWeekDate = moment().add(7,'days').format()
        if(currentDate<=trainingInfo.start && nextWeekDate>=trainingInfo.start){
            // console.log(trainingInfo.trainingType)
            this.courtAdminEvents.push({
                name: trainingInfo.trainingType,
                type: 'Training',
                start: trainingInfo.start,
                comment: trainingData.comment
            })
        }        
    }

    public extractAwayLocations(courtAdminData){
        for (const awayInfo of courtAdminData.awayLocation){
            const start = awayInfo.startDate? moment(awayInfo.startDate).tz(awayInfo.timezone).format():'';
            this.courtAdminEvents.push({
                name: awayInfo.location.name,
                type: 'Loaned',
                start: start,
                comment: awayInfo.comment
            })
        }
    }

}
</script>

<style scoped>

    .card {
        border: white;
    }

    .info-box {
        margin: 0 1.5rem;
        border-radius: 6px;
        /* background: #487080; */
        background: #024f6d;
        color: white;
        box-shadow: 0 6px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
    }

    .training-box {
        margin: 0 1.5rem;
        font-size: 15pt;
        background: #eef3f5;
        box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
    }

</style>
