<template>
    <div v-if="isMounted">         
        <b-table
            :items="courtAdminSchedules" 
            :fields="dailyFields"
            small
            bordered
            style="width: 100%;"
            fixed>

            <template v-slot:cell(name) = "data" >  

                <div style="line-height:1rem; font-size: 7.5pt; font-weight: 700;">
                    {{data.value}}
                </div>
                <div style="line-height:0.75rem;"                    
                    v-if="data.item.homeLocation != location.name">
                    <div class="m-0 p-0 text-jail"> 
                        <b-icon-box-arrow-in-right style="float:left;margin:0 .2rem 0 0;"/>
                        <div style="float:left;font-size: 7.5pt; margin:0 .1rem 0 0;">Loaned in from </div>
                    </div> 
                    <div class="m-0 p-0 text-jail" style="font-size: 7.5pt;"> {{data.item.homeLocation|truncate(35)}} </div>
                </div>
            
                <div class="row m-0" style="line-height:0.75rem;">
                    <div style="font-size: 7.5pt; margin:0 .25rem 0 0;">
                        {{data.item.rank}}
                    </div>
                    <div class="m-0 p-0" style="font-size: 7.5pt;"> #{{data.item.badgeNumber}} </div>
                </div>
                
                <div style="line-height:0.75rem;">
                    <div v-if="data.item.actingRank.length>0" style="font-size: 7.5pt; font-weight: 700;"> 
                        <span class="dot">A</span> <span style="font-weight: 500;">{{data.item.actingRank[0].rank}}</span>
                    </div>
                </div>
                
            </template>               

            <template v-slot:cell(shifts) = "data"> 
                <div style="font-size: 6pt; border:none;" class="m-0 p-0" >
                    
                    <div v-if="data.item.courtAdminEvent.type == 'Shift'"  style="margin-bottom: 0.1rem;">
                        <div v-if="data.item.courtAdminEvent.startTime && data.item.courtAdminEvent.endTime" style="margin:0 1rem; text-align: center; font-weight: 600; border-bottom: 1px solid #ccc;">                    
                            <span style="font-size: 5.5pt; margin-right:0.1rem; ">In: </span> {{data.item.courtAdminEvent.startTime}}                         
                            <span style="font-size: 6pt;" >Out:</span> {{data.item.courtAdminEvent.endTime}}                    
                        </div>
                        <div style="font-size: 6pt; border: none; line-height: 0.55rem;" class="m-0 p-0" v-for="duty,inx in sortEvents(data.item.courtAdminEvent.duties)" :key="'duty-name-'+inx+'-'+duty.startTime">                                
                            <div v-if="duty.dutyType=='Training' || duty.dutyType=='Leave' || duty.dutyType=='Loaned'">
                                <div  style="margin:0 1rem; text-align: center; border-bottom: 1px solid #ccc;">
                                    <div class="badge my-1 " :style="{fontSize:'6pt', background: getColor(duty.dutySubType)}">{{ duty.dutyType }}</div>                            
                                    <div>
                                        <b> {{duty.startTime}}-{{duty.endTime}}</b>  
                                        <span > {{duty.dutySubType}} </span>
                                    </div>
                                </div>
                            </div>                            
                        </div>         
                    </div>

                    <div v-else-if="data.item.courtAdminEvent.type == 'Unavailable'" class="text-center middle-text">                                         
                        <div  class="m-0 p-0" style="">                    
                            <div :style="{background:getColor(data.item.courtAdminEvent.subType)}" class=" text-white">Unavailable</div>
                        </div>
                    </div>
                    
                    <div v-else-if="data.item.courtAdminEvent.type == 'Leave'" class="text-center middle-text">                                         
                        <div  class="m-0 p-0" style="">                    
                            <div :style="{background:getColor(data.item.courtAdminEvent.subType)}" class=" text-white">
                                <div>Leave</div> {{ data.item.courtAdminEvent.subType }}
                            </div>
                        </div>
                    </div> 

                    <div v-else-if="data.item.courtAdminEvent.type == 'Training'" class="text-center middle-text">                  
                        <div style="" class="m-0 p-0">
                            <div class="bg-training-leave">
                                <div>Training</div> {{data.item.courtAdminEvent.subType}}
                            </div>
                        </div> 
                    </div>   

                    <div v-else-if="data.item.courtAdminEvent.type == 'Loaned'" class="text-center middle-text">  
                        <div style="" class="m-0 p-0"> 
                            <div class="bg-loaned">
                                <div>Loaned</div> {{data.item.courtAdminEvent.location}}
                            </div>
                        </div>                     
                    </div>                
                </div>                

            </template>

            <template v-slot:cell(duties) = "data">
                <div style="font-size: 6pt; border: none; line-height: 0.55rem;" class="m-0 p-0" v-for="duty,inx in sortEvents(data.item.courtAdminEvent.duties)" :key="'duty-name-'+inx+'-'+duty.startTime">                                
                    <div :style="'color: ' + duty.color" v-if="duty.dutyType!='Training' && duty.dutyType!='Leave' && duty.dutyType!='Loaned'">
                        <b v-if="duty.isOvertime">*</b>                            
                        <b> {{duty.startTime}}-{{duty.endTime}}</b>  
                        <span > {{duty.dutySubType}} </span>
                        ({{ duty.dutyType.replace('Role','').replace('Assignment','').replace('EscortRun','Transport') }})
                    </div>                            
                </div>
            </template>

            <template v-slot:cell(notes) = "data">
                <div style="font-size: 6pt; border:none;" class="m-0 p-0" v-for="duty in sortEvents(data.item.courtAdminEvent.duties)" :key="duty.startTime">
                    <div :style="'color: ' + duty.color" v-if="duty.dutyNotes">{{duty.dutyNotes}}</div>
                    <div v-if="duty.assignmentNotes">{{duty.assignmentNotes}}</div>
                </div>
            </template>
                
        </b-table>        
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    import * as _ from 'underscore';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");    

    import { locationInfoType } from '@/types/common';
    import { distributeScheduleInfoType, distributeTeamMemberInfoType, dailyDistributeScheduleInfoType } from '@/types/ShiftSchedule/index';       
    import { manageAssignmentsScheduleInfoType, manageAssignmentDutyInfoType } from '@/types/DutyRoster';

    @Component
    export default class DailySchedule extends Vue {

        @Prop({required: true})
        dailyCourtAdminSchedules!: distributeScheduleInfoType[];        
        
        @commonState.State
        public location!: locationInfoType;

        isMounted = false;       

        
        courtAdminSchedules: dailyDistributeScheduleInfoType[] =[];    

        dailyFields=[
            {key:'name',        label:'Name',            tdClass:'px-1 mx-0 align-middle my-team', thStyle:'text-align: center; font-size: 8pt; width: 11rem;'},
            {key:'shifts',      label:'Scheduled Shift', tdClass:'px-1 mx-0 align-middle', thStyle:'text-align: center; font-size: 8pt; width: 8rem;'},
            {key:'duties',      label:'Assignments',     tdClass:'px-1 mx-0 align-middle', thStyle:'text-align: center; font-size: 8pt; width: 8rem;'},
            {key:'notes',       label:'Notes',           tdClass:'px-1 mx-0 align-middle my-notes', thStyle:'text-align: center; font-size: 8pt; width: 17rem;'}
        ]    

        mounted() {
            this.isMounted = false;
            this.extractCourtAdminEvents();
            this.isMounted = true;       
        }

        public sortEvents (events: any) {            
            return _.sortBy(events, "startTime");
        }
        
        public extractCourtAdminEvents(){
            this.courtAdminSchedules = []            
            for(const sherifschedule of this.dailyCourtAdminSchedules){
                const scheduleInfo = sherifschedule.conflicts
                
                let courtAdminEvent = {} as manageAssignmentsScheduleInfoType;
                const duties: manageAssignmentDutyInfoType[] = []            
                for(const shEvent of this.sortEvents(scheduleInfo)){

                    if(shEvent.fullday){
                        courtAdminEvent=shEvent;
                        break
                    }

                    if(shEvent.type != 'Shift'){
                        // console.log(shEvent)
                        const subtype = (shEvent.type=='Leave'? shEvent.subType:shEvent.type)
                        // console.log(subtype)
                        duties.push({                    
                            startTime: shEvent.startTime,
                            endTime: shEvent.endTime, 
                            dutyType: shEvent.type,
                            dutySubType: shEvent.subType,
                            color: Vue.filter('subColors')(subtype)                                        
                        })
                    }
                    else if(shEvent.type == 'Shift' && shEvent.overtime){
                        
                        for(const duty of shEvent.duties){
                            duty.isOvertime=true
                            duty.color= Vue.filter('subColors')('overtime')
                            duties.push(duty)
                        }
                    }
                    else{
                    
                        duties.push(...shEvent.duties)
                        if(!courtAdminEvent.type){
                            courtAdminEvent=shEvent
                        }else{
                            const start = courtAdminEvent.startTime
                            const end = courtAdminEvent.endTime
                            courtAdminEvent.startTime = start < shEvent.startTime? start :shEvent.startTime;
                            courtAdminEvent.endTime = end > shEvent.endTime? end :shEvent.endTime;
                        }
                    }
                }

                courtAdminEvent.duties = duties;
                
                this.courtAdminSchedules.push({
                    courtAdminId: sherifschedule.courtAdminId,   
                    courtAdminEvent: courtAdminEvent,
                    name: sherifschedule.name,
                    homeLocation: sherifschedule.homeLocation,
                    rank: sherifschedule.rank,
                    badgeNumber: sherifschedule.badgeNumber,
                    actingRank: sherifschedule.actingRank                    
                })
            }
            // console.log(this.courtAdminSchedules)
        }

        public getColor(subtype){
            return Vue.filter('subColors')(subtype)
        }

    }
</script>

<style scoped lang="scss" src="@/styles/_pdf.scss">

</style>