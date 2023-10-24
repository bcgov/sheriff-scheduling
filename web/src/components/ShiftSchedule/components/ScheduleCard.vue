<template>
    <div v-if="scheduleInfo.length>0 && isMounted" id="shiftBox" ref="shiftBox" :key="updateBoxes">             
        <b-card
            v-for="block in scheduleBlocks"
            :key="block.key"
            :id="'schCard'+block.key"
            :class="'m-0 p-0 ' +scheduleColorClass(block).body"
            :style="{ border: '1px solid white', float:'left', position: 'relative', left:block.startTime+'%', width:block.timeDuration+'%', height:'3rem'}"
            no-body>
                <span v-if="blockSize(block)>40" @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%"> 
                    <h6 :class="'m-0 mb-1 p-0 ' + scheduleColorClass(block).header" :style="{ color: 'white', textAlign: 'center', fontSize:'10px', lineHeight: '16px' }"
                        v-b-tooltip.hover.html="'<b>'+block.title + ' ' + block.timeStamp+' </b>'+(block.comment?'<br/><b class=\' px-1 bg-info \'>'+block.comment+'</b>':'')">
                        <font-awesome-icon v-if="block.title.includes('Loaned')" style="transform: rotate(180deg); font-size: .55rem;"  icon="sign-out-alt" /> 
                        <font-awesome-icon v-if="block.title=='Leave'" style="font-size: .55rem;"  icon="suitcase"/> 
                        <font-awesome-icon v-if="block.title=='Training'" style="font-size: .5rem;" icon="graduation-cap"/> 
                        <b-icon-person-fill v-if="block.title=='Shift'"/>
                        <b-icon-calendar2-x v-if="block.title=='Unavailable'"/>
                        <b-icon-chat-square-text-fill v-if="block.comment" font-scale="0.9" class="ml-1" />
                    </h6>
                    <div class="m-0 p-0" style="text-align: center;font-size:10px; line-height: 12px; display: block;">{{block.timeStamp}}</div>
                </span>
                <span v-else @mousedown="cardSelected(block)" @dblclick="selectOnlyCard(block)" style="height:100%">
                    <h6 
                        v-b-tooltip.hover.html="'<b>'+block.title + ' ' + block.timeStamp+' </b>'+(block.comment?'<br/><b class=\' px-1 bg-info \'>'+block.comment+'</b>':'')"
                        :class="scheduleColorClass(block).header"
                        :style="'color:white; text-align: center; font-size:6px; line-height: 16px;'">
                        ...<b-icon-chat-square-text-fill v-if="block.comment" font-scale="0.9" class="ml-1" />
                    </h6>                    
                </span>
        </b-card>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import "@store/modules/ShiftScheduleInformation";   
    const shiftState = namespace("ShiftScheduleInformation");
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");
    import {conflictsInfoType, scheduleBlockInfoType} from '@/types/ShiftSchedule/index'
    import { userInfoType } from '@/types/common';

    @Component
    export default class ScheduleCard extends Vue {

        @Prop({required: true})
        scheduleInfo!: conflictsInfoType[];

        @Prop({required: true})
        courtAdminId!: string;

        @commonState.State
        public userDetails!: userInfoType;
        
        @shiftState.State
        public selectedShifts!: string[];

        @shiftState.Action
        public UpdateSelectedShifts!: (newSelectedShifts: string[]) => void

        scheduleBlocks: scheduleBlockInfoType[] = [];

        blockDrop = false;
        updateBoxes =0;
        hasPermissionToEditShifts = false;

        isMounted = false;

        mounted()
        {
            //console.log(this.scheduleInfo)
            this.isMounted = false;
            this.hasPermissionToEditShifts = this.userDetails.permissions.includes("EditShifts");

            const sortedScheduleInfo: conflictsInfoType[] = _.sortBy(this.scheduleInfo,'startInMinutes')
           
            this.scheduleBlocks = []
            let widthOtherElements =0;           
            for(const schedule of sortedScheduleInfo){
                if(schedule.startTime){

                    this.scheduleBlocks.push({
                        id: schedule.id? schedule.id:'',
                        key: this.courtAdminId+schedule.date +'Z' +schedule.startTime,
                        startTime: schedule.startInMinutes *5 /72 -widthOtherElements,
                        timeDuration: schedule.timeDuration * 5 /72,
                        timeStamp: (schedule.type == 'Leave' || schedule.type == 'Training')?schedule.courtAdminEventType + ': ' + schedule.startTime +'-'+schedule.endTime:schedule.startTime +'-'+schedule.endTime,
                        title:schedule.type=='Loaned'? 'Loaned to ' + schedule.location: (schedule.type=='overTimeShift'? 'Shift':schedule.type),
                        color: this.getScheduleColorClass(schedule.type).body, //unused now
                        originalColor: this.getScheduleColorClass(schedule.type).body, //unused now
                        headerColor: this.getScheduleColorClass(schedule.type).header, //unused now
                        selected: false,
                        type: schedule.type,
                        subType: (schedule.type =='Leave' && schedule.courtAdminEventType)?schedule.courtAdminEventType:'',                                
                        comment: schedule.comment
                    })
                    widthOtherElements += (schedule.timeDuration * 5 /72);
                    //console.log(this.scheduleBlocks)
                }else{

                    this.scheduleBlocks.push({
                        id: schedule.id? schedule.id:'',
                        key: this.courtAdminId+schedule.date +'Z' +schedule.startTime,
                        startTime: 0,
                        timeDuration: 100,
                        timeStamp: (schedule.type == 'Leave' || schedule.type == 'Training')?schedule.courtAdminEventType + ': Full Day': 'Full Day',
                        title:schedule.type=='Loaned'? 'Loaned to ' + schedule.location: schedule.type,
                        color: this.getScheduleColorClass(schedule.type).body, //unused now
                        originalColor: this.getScheduleColorClass(schedule.type).body, //unused now
                        headerColor: this.getScheduleColorClass(schedule.type).header, //unused now
                        selected: false, //unused now
                        type: schedule.type,
                        subType: (schedule.type =='Leave' && schedule.courtAdminEventType)?schedule.courtAdminEventType:'',
                        comment: schedule.comment
                    })
                }                    
            }
            this.isMounted = true; 
        }

        public selectOnlyCard(block){
            if(this.hasPermissionToEditShifts && block.title=='Shift') {
                this.UpdateSelectedShifts([ block.id ]);
                this.$root.$emit('editShifts');
            }
        }

        public cardSelected(block){
            let selectedCards = this.selectedShifts;
            if(block.title=='Shift'){
                if (!selectedCards.includes(block.id))
                    selectedCards.push(block.id);
                else
                    selectedCards = selectedCards.filter(sc => sc != block.id);
                this.UpdateSelectedShifts(selectedCards);
            }
        }

        public getScheduleColorClass(block) {            
            let color = this.getLeaveColorClass(block)
            if(block.type=='Unavailable') color = {body:'bg-unavailable-schedule' , header:'bg-unavailable-schedule-header'}
            else if(block.type=='Shift') color = {body:'bg-shift' , header:'bg-shift-header'}
            else if (block.type=='overTimeShift') color = {body:'bg-overtime-shift' , header:'bg-overtime-shift-header'}
            if (block.id != null && this.selectedShifts.includes(block.id)) color.body ='bg-selected-shift';
            return color;
        }

        //Wrapper function, the markup wouldn't bind to getScheduleColor. 
        public scheduleColorClass(block){            
            return this.getScheduleColorClass(block);
        }

        public getLeaveColorClass(block) {
            let color = {body:'bg-default-shift' , header:'bg-default-shift-header'};
            if (block.type=='Leave' && block.subType != ''){
                if(block.subType.toUpperCase().includes('SPL')) color = {body:'bg-spl-leave', header:'bg-spl-leave-header'}
                if(block.subType.toUpperCase().includes('A/L')) color = {body:'bg-a-l-leave' , header:'bg-a-l-leave-header'}
                if(block.subType.toUpperCase().includes('MED/DENTAL')) color = {body:'bg-med-dental-leave' , header:'bg-med-dental-leave-header'}
                if(block.subType.toUpperCase().includes('STIIP')) color = {body:'bg-stiip-leave' , header:'bg-stiip-leave-header'}
                if(block.subType.toUpperCase().includes('CTO')) color = {body:'bg-cto-leave' , header:'bg-cto-leave-header'}
                if(block.subType.toUpperCase().includes('LWOP')) color = {body:'bg-lwop-leave' , header:'bg-lwop-leave-header'}
                if(block.subType.toUpperCase().includes('BEREAVEMENT')) color = {body:'bg-bereavement-leave' , header:'bg-bereavement-leave-header'}
                if(block.subType.toUpperCase().includes('TRAINING')) color = {body:'bg-training-leave' , header:'bg-training-leave-header'}
                if(block.subType.toUpperCase().includes('OVERTIME')) color = {body:'bg-overtime-leave' , header:'bg-overtime-leave-header'}               

            }
            
            return color;
        }

        public blockSize(block){
            //console.log('schCard'+block.key)
            const el = document.getElementById('schCard'+block.key)
            //console.log(el)
            if(el){
                return el.clientWidth
            }else{
                return 0
            }            
        }

        created() {
            window.addEventListener("resize", this.onResize);
        }

        destroyed() {
            window.removeEventListener("resize", this.onResize);
        }

        public onResize() {          
            this.updateBoxes++;
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

</style>