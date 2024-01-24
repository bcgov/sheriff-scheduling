<template>
    <div> 
         
        <b-table            
            :items="courtAdminSchedules" 
            :fields="fields"
            small
            class="printer"
            style="width: 100%;"
            bordered
            fixed>
            
            <template v-slot:head() = "data" >
                <span class="text">{{data.column}}</span> <span> {{data.label}}</span>
            </template>

            <template v-slot:head(myteam) = "data" >  
                <span>{{data.label}}</span>
            </template>

            <template v-slot:cell(myteam) = "data" >
                <div style="line-height:1rem; font-size: 7.5pt; font-weight: 700;">
                    {{data.value.name}}
                </div>
                <div style="line-height:0.75rem;"                    
                    v-if="data.value.homeLocation != location.name">
                    <div class="m-0 p-0 text-jail"> 
                        <b-icon-box-arrow-in-right style="float:left;margin:0 .2rem 0 0;"/>
                        <div style="float:left;font-size: 7.5pt; margin:0 .1rem 0 0;">Loaned in from </div>
                    </div> 
                    <div class="m-0 p-0 text-jail" style="font-size: 7.5pt;"> {{data.value.homeLocation|truncate(35)}} </div>
                </div>
            
                <div class="row m-0" style="line-height:0.75rem;">
                    <div style="font-size: 7.5pt; margin:0 .25rem 0 0;">
                        {{data.value.rank}}
                    </div>
                    <div class="m-0 p-0" style="font-size: 7.5pt;"> #{{data.value.badgeNumber}} </div>
                </div>
                
                <div style="line-height:0.75rem;">
                    <div v-if="data.value.actingRank.length>0" style="font-size: 7.5pt; font-weight: 700;"> 
                        <span class="dot">A</span> <span style="font-weight: 500;">{{data.value.actingRank[0].rank}}</span>
                    </div>
                </div>
                
            </template>               
            
            <template v-slot:cell() = "data">
                <weekly-assignment-card :scheduleInfo="data.item[data.field.key]" />                
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
    import { weekScheduleInfoType } from '@/types/ShiftSchedule/index';
    import WeeklyAssignmentCard from './WeeklyAssignmentCard.vue'    

    @Component({
        components :{
            WeeklyAssignmentCard
        }
    })
    export default class WeeklySchedule extends Vue {

        @Prop({required: true})
        courtAdminSchedules!: weekScheduleInfoType[];

        @Prop({required: true})
        fields!: any[];

        @commonState.State
        public location!: locationInfoType;    

        public sortEvents (events: any) {            
            return _.sortBy(events, "startTime");
        }

    }
</script>

<style>
    table.printer td:has(.middle-text){  
        vertical-align: middle !important;
    }
</style>

<style scoped lang="scss" src="@/styles/_pdf.scss">

</style>