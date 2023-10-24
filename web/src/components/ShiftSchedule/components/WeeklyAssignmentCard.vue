<template>
    <div v-if="scheduleInfo && isMounted" >             

        <div style="font-size: 6pt; border:none;" class="m-0 p-0" >
            <!-- {{ scheduleInfo }} -->
            <!-- {{ courtAdminEvent }} -->
            <div v-if="courtAdminEvent.type == 'Shift'"  style="margin-bottom: 0.1rem;">

                <div v-if="courtAdminEvent.startTime && courtAdminEvent.endTime" style="margin:0; text-align: center; font-weight: 600; width:100%; border-bottom: 1px solid #ccc;">                    
                    <span style="font-size: 5.5pt; margin-right:0.1rem; ">In: </span> {{courtAdminEvent.startTime}}                         
                    <span style="font-size: 6pt;" >Out:</span> {{courtAdminEvent.endTime}}                    
                </div>

                <div style=" width:100%;">
                    <div style="font-size: 6pt; border: none; line-height: 0.55rem;" class="m-0 p-0" v-for="duty,inx in sortEvents(courtAdminEvent.duties)" :key="'duty-name-'+inx+'-'+duty.startTime">                                
                        <div :style="'color: ' + duty.color">
                            <b v-if="duty.isOvertime">*</b>                            
                            <b> {{duty.startTime}}-{{duty.endTime}}</b>  
                            <span v-if="duty.dutyType!='Training' && duty.dutyType!='Leave' && duty.dutyType!='Loaned'" > {{duty.dutySubType}} </span>
                            {{ duty.dutyType | getTypeAbrv }}
                        </div>                            
                    </div>                    
                </div>                    
            </div>

            <div v-else-if="courtAdminEvent.type == 'Unavailable'" class="text-center middle-text">                                         
                <div  class="m-0 p-0" style="">                    
                    <div :style="{background:getColor(courtAdminEvent.subType)}" class=" text-white">Unavailable</div>
                </div>
            </div>
            
            <div v-else-if="courtAdminEvent.type == 'Leave'" class="text-center middle-text">                                         
                <div  class="m-0 p-0" style="">                    
                    <div :style="{background:getColor(courtAdminEvent.subType)}" class=" text-white">
                        <div>Leave</div> {{ courtAdminEvent.subType }}
                    </div>
                </div>
            </div> 

            <div v-else-if="courtAdminEvent.type == 'Training'" class="text-center middle-text">                  
                <div style="" class="m-0 p-0">
                    <div class="bg-training-leave">
                        <div>Training</div> {{courtAdminEvent.subType}}
                    </div>
                </div> 
            </div>   

            <div v-else-if="courtAdminEvent.type == 'Loaned'" class="text-center middle-text">  
                <div style="" class="m-0 p-0"> 
                    <div class="bg-loaned">
                        <div>Loaned</div> {{courtAdminEvent.location}}
                    </div>
                </div>                     
            </div>                
        </div>

    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import { locationInfoType } from '@/types/common';
    import { manageAssignmentDutyInfoType, manageAssignmentsScheduleInfoType } from '@/types/DutyRoster';

    @Component
    export default class WeeklyAssignmentCard extends Vue {

        @Prop({required: true})
        scheduleInfo!: manageAssignmentsScheduleInfoType[];

        @commonState.State
        public location!: locationInfoType;
        
        courtAdminEvent = {} as manageAssignmentsScheduleInfoType;

        isMounted = false;

        mounted() { 
            this.isMounted = false;
            this.extractCourtAdminEvents();
            this.isMounted = true;
        }
        
        public extractCourtAdminEvents(){            

            this.courtAdminEvent = {} as manageAssignmentsScheduleInfoType;
            const duties: manageAssignmentDutyInfoType[] = []            
            for(const courtAdminEvent of this.sortEvents(this.scheduleInfo)){
                if(courtAdminEvent.fullday){
                    this.courtAdminEvent=courtAdminEvent;
                    return
                }
                // console.error(courtAdminEvent.overtime)
                // console.log(courtAdminEvent.type)
                if(courtAdminEvent.type != 'Shift'){
                    // console.log(courtAdminEvent)
                    const subtype = (courtAdminEvent.type=='Leave'? courtAdminEvent.subType:courtAdminEvent.type)
                    // console.log(subtype)
                    duties.push({                    
                        startTime: courtAdminEvent.startTime,
                        endTime: courtAdminEvent.endTime, 
                        dutyType: courtAdminEvent.type,
                        dutySubType: courtAdminEvent.subType,
                        color: Vue.filter('subColors')(subtype)                                        
                    })
                }
                else if(courtAdminEvent.type == 'Shift' && courtAdminEvent.overtime){
                    // console.log('Overtime')
                    // console.log(courtAdminEvent.duties)
                    for(const duty of courtAdminEvent.duties){
                        duty.isOvertime=true
                        duty.color= Vue.filter('subColors')('overtime')
                        duties.push(duty)
                    }
                }
                else{
                    //console.log(courtAdminEvent)
                   
                    duties.push(...courtAdminEvent.duties)
                    if(!this.courtAdminEvent.type){
                        this.courtAdminEvent=courtAdminEvent
                    }else{
                        const start = this.courtAdminEvent.startTime
                        const end = this.courtAdminEvent.endTime
                        this.courtAdminEvent.startTime = start < courtAdminEvent.startTime? start :courtAdminEvent.startTime;
                        this.courtAdminEvent.endTime = end > courtAdminEvent.endTime? end :courtAdminEvent.endTime;
                    }
                }
            }
            
            this.courtAdminEvent.duties = duties;
                       
            // console.log(this.courtAdminEvent)
            //console.log(duties)
            //console.log(this.courtAdminAvailabilityArray)
        }

        public sortEvents (events: any) {            
            const eventsCopy = JSON.parse(JSON.stringify(events))
            return _.sortBy(eventsCopy, "startTime");
        }

        public getColor(subtype){
            return Vue.filter('subColors')(subtype)
        }

    }
</script>