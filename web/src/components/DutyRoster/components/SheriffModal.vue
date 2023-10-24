<template>
    <div v-if="dataready">
        <b-row style="width:95%; transform:translate(0,-3rem);">
            <div v-if="!multipleAssignments" class="h4 text-white ml-2 p-0 mb-0" v-b-tooltip.hover :title="assignmentName.length>25? assignmentName:''" >{{assignmentName|truncate(22)}}</div> 
            <div v-if="multipleAssignments" class="h4 text-warning ml-2 mr-4 p-0 mb-0"> Multiple assignments are selected</div> 

            <div v-if="!multipleAssignments" style="float:left;font-size:14pt;" class="h4 text-warning ml-2 p-0 mb-0"> 
                <b-row v-if="weekView" style="margin:-0.65rem 0 -0.2rem 0;font-size:12pt;">
                    <b-col class="text-center">
                        <div>{{dutyBlock.dutyDate|beautify-date-weekday}}</div>
                        <div>{{dutyBlock.startTimeString}} - {{dutyBlock.endTimeString}}</div>                        
                    </b-col>
                </b-row>
                <div v-else>{{dutyBlock.startTimeString}} - {{dutyBlock.endTimeString}}</div>

            </div>
            <b-button v-if="!multipleAssignments" style="margin:-0.45rem 0 0 1rem;" size="sm p-1" variant="info" @click="editDuty">Edit details</b-button>               

            <b-button style="margin:-0.45rem 0 0 1rem; font-size:8pt;" class="bg-success text-white p-1"  @click="drop('00000-00000-11111')">Not Required</b-button>
            <b-button style="margin:-0.45rem 0 0 0.25rem; font-size:8pt;" class="bg-danger text-white p-1"  @click="drop('00000-00000-22222')">Not Available</b-button>
            <b-button style="margin:-0.45rem -2rem 0 0.25rem; font-size:8pt;" class="bg-bright text-white p-1" @click="drop('00000-00000-33333')">Closed for Day</b-button>
        </b-row>
        <b-row style="background:#AABBCC; font-size:10pt; margin:-2.5rem -1rem 0.5rem -1rem; ">
            <div style="margin: 0.2rem auto 0.2rem auto">
                <b-row>
                    <div style="margin:0.1rem 1rem" class="text-primary h4 "><i>Sort By:</i></div>
                    <b-form-radio-group  class="mx-3 mb-n1 text-dark " size="sm"              
                        v-model="sortType">                
                        <b-form-radio value="best" v-if="!multipleAssignments">Best Match</b-form-radio>
                        <b-form-radio value="availability" v-if="!multipleAssignmentsWeekview">Availability</b-form-radio>
                        <b-form-radio value="rank">Rank</b-form-radio>
                        <b-form-radio value="name">Name</b-form-radio>
                    </b-form-radio-group>
                </b-row>
            </div>
        </b-row>
<!-- {{dutyBlock}}
{{assignmentBlock}} -->
        <b-row class="px-1">
            <b-card bg-variant="dark"                 
                style="width:7.5rem; margin:0.25rem 0.35rem;" 
                body-class="p-1" 
                v-for="member in sortedShiftAvailabilityInfo"
                @click="drop(member.courtAdminId)" 
                :key="'court-admin-modal-'+member.courtAdminId"> 
                <!-- <span class="text-white">{{sumOfArrayElements(member.availability)}} {{sumOfArrayElements(unionArrays(member.availability, assignmentBlock))}}              </span> -->
                    <duty-roster-team-member-card editingBlockId :courtAdminInfo="member" :weekView="weekView" :courtAdminModal="true"/>
                    <court-admin-availability-bar v-if="!multipleAssignmentsWeekview" :courtAdminInfo="member" :dutyBlock="dutyBlock" :weekView="weekView"/>
            </b-card>
        </b-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { myTeamShiftInfoType, selectedDutyCardInfoType} from '@/types/DutyRoster';
    import DutyRosterTeamMemberCard from './DutyRosterTeamMemberCard.vue'
    import CourtAdminAvailabilityBar from './CourtAdminAvailabilityBar.vue'

    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    import { namespace } from "vuex-class";
    
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");


    @Component({
        components: {            
            DutyRosterTeamMemberCard,
            CourtAdminAvailabilityBar
        }        
    })  
    export default class CourtAdminModal extends Vue {
        
        @Prop({required: true})
        assignmentName!: string;

        @Prop({required: true})
        assignmentBlock!: number[];

        @Prop({required: true})
        dutyBlock!: any;

        @Prop({required: true})
        weekView!: boolean;

        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.State
        public selectedDuties!: selectedDutyCardInfoType[];

        
        maxRank = 1000;
        sortType= 'best';
        multipleAssignments = false
        multipleAssignmentsWeekview = false
        dataready = false;

        mounted(){
            this.dataready = false;
            if(this.weekView) this.extractCourtAdminAvailabilities()
            if(this.selectedDuties.length>0){
                this.multipleAssignments = true;
                if(this.weekView){
                    this.multipleAssignmentsWeekview = true;
                    this.sortType = 'rank'
                }else{
                    this.multipleAssignmentsWeekview = false
                    this.sortType = 'availability'
                }
            }else{
                this.multipleAssignmentsWeekview = false;
                this.multipleAssignments = false;
                this.sortType = 'best'
            }
            this.dataready = true;
        }

        public extractCourtAdminAvailabilities(){
            const dutyDate=this.dutyBlock.dutyDate.slice(0,10)            

            for(const courtAdminInfo of this.shiftAvailabilityInfo){
                const shifts = courtAdminInfo.shifts.filter( shift => shift.startDate.slice(0,10)==dutyDate)                        
                let availability = Array(96).fill(0)
                if(shifts.length>0){
                    const shiftStartTime = moment(shifts[0].startDate).tz(shifts[0].timezone);
                    const startOfDay = moment(shiftStartTime).startOf("day");
                    for(const shift of shifts){
                        const shiftBin = this.getTimeRangeBins(shift.startDate, shift.endDate, startOfDay, shift.timezone);
                        availability = this.fillInArray(availability, 1, shiftBin.startBin, shiftBin.endBin);
                    }
                }
                
                const dutiesDetail = courtAdminInfo.dutiesDetail.filter(
                    duty => {
                        if(duty.startTime)
                            return duty.startTime.slice(0,10)==dutyDate
                        else 
                            return false 
                    }
                )
                let duties =  Array(96).fill(0)
                for(const dutydetail of dutiesDetail){
                    duties = this.fillInArray(duties, 1, dutydetail.startBin, dutydetail.endBin)
                } 
                
                courtAdminInfo.availability = this.subtractOfTwoArrays(availability, this.unionArrays(availability, duties))

            }
        }

        public drop(courtAdminId){
            this.$emit('drop','member-'+courtAdminId, true )
        }

        get sortedShiftAvailabilityInfo() {
            const teamList = this.shiftAvailabilityInfo 
            if(this.sortType=='best'){
                return _.chain(teamList)
                    .sortBy(member =>{return (member['rankOrder']? member['rankOrder'] : this.maxRank + 1)})
                    .sortBy(member =>{return (member.availability.reduce((acc, arr) => acc - (arr>0?1:0), 0))})
                    .sortBy( member =>{
                        const intersection = this.unionArrays(member.availability, this.assignmentBlock);
                        return -this.sumOfArrayElements(intersection);
                    })                    
                    .value()
            }
            else if(this.sortType=='rank'){       
                return  _.chain(teamList)
                    .sortBy(member =>{return (member['lastName']? member['lastName'].toUpperCase() : '')})
                    .sortBy(member =>{return (member['rankOrder']? member['rankOrder'] : this.maxRank + 1)})
                    .value()
            }
            else if(this.sortType=='availability'){
                return  _.chain(teamList)
                    .sortBy(member =>{return (member['rankOrder']? member['rankOrder'] : this.maxRank + 1)})
                    .sortBy(member =>{return (member.availability.reduce((acc, arr) => acc - (arr>0?1:0), 0))})
                    .value()
            }
            else{       
                return _.sortBy(teamList, member =>{return (member['lastName']? member['lastName'].toUpperCase() : '')})
            }
        }

        public unionArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr*arrayB[index]});
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public subtractOfTwoArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr-arrayB[index]});
        }

        public editDuty(){
            if(this.weekView) 
                this.$emit('editDuty', this.dutyBlock['day'])
            else
                this.$emit('editDuty')
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public getTimeRangeBins(startDate, endDate, startOfDay, timezone){
            const startTime = moment(startDate).tz(timezone);
            const endTime = moment(endDate).tz(timezone);
            const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
            const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
            return( {startBin: startBin, endBin:endBin } )
        }
    }
</script>

<style scoped lang="scss">

</style>