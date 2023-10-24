<template>
    <div>
        <b-row  class="m-0 p-0" cols="2" >
            <b-col class="m-0 p-0" cols="11" >
                <b-table
                    :items="myTeamMembers" 
                    :fields="gaugeFields"
                    small
                    head-row-variant="transparant"
                    style="overflow-x:hidden"
                    sort-by="availability"
                    :sort-desc="true"
                    class="gauge"                   
                    sticky-header="7rem"                        
                    borderless
                    fixed>
                        <template v-slot:table-colgroup> 
                            <col style="width:9rem">
                            <col>
                        </template>

                        <template v-slot:head(availability) >
                            <div class="gridfuel24">
                                <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                            </div>
                        </template>

                        <template v-slot:cell(availability)="data" >
                            <court-admin-availability-card class="m-0 p-0" :sheriffInfo="data.item" />
                        </template>

                        <template v-slot:head(name) > 
                             My Team                          
                            <b-button
                                @click="closeDisplayMyteam()"
                                v-b-tooltip.hover.right                            
                                title="Close Footer Display"
                                style="font-size:10px; width:1.15rem; margin:0 0 0 .75rem; padding:0; background-color:white;color:#189fd4;" 
                                size="sm">
                                    <b-icon-bar-chart-steps /> 
                            </b-button>                            
                        </template>

                         <template v-slot:cell(name)="data" >
                            <div
                                :id="'gauge--'+data.item.sheriff.sheriffId"
                                :draggable="hasPermissionToAddAssignDuty" 
                                v-on:dragstart="DragStart"
                                style="height:1rem; font-size:12px; line-height: 16px; text-transform: capitalize; margin:0; padding:0"
                                v-b-tooltip.hover.right                            
                                :title="data.item.fullName">
                                    {{data.value}}
                            </div>
                        </template>
                </b-table> 
            </b-col>
            <b-col class="m-0 p-0" cols="1" >
                <b-card 
                class="bg-light"
                    header="Colours" 
                    header-class=" m-0 p-0 bg-primary text-white text-center no-top-rounding" 
                    no-body>
                    <b-row style="margin:0 0 .25rem .25rem; width:7.6rem;">
                        <div
                            style="width:3.8rem;"
                            class="m-0 p-0"
                            v-for="color in dutyColors" 
                            :key="color.colorCode"> 
                            <div :style="{backgroundColor:color.colorCode, width:'.65rem', height:'.65rem', borderRadius:'15px', margin:'.2rem .2rem 0 0', float:'left'}"/>
                            <div style="font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0">{{color.name}}</div>
                        </div>
                    </b-row>
                </b-card>
            </b-col>
        </b-row>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import SheriffAvailabilityCard from './SheriffAvailabilityCard.vue'
    import { myTeamShiftInfoType, dutiesDetailInfoType} from '@/types/DutyRoster';
    import { userInfoType } from '@/types/common';
    
    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            SheriffAvailabilityCard
        }
    })
    export default class SheriffFuelGauge extends Vue {
       
        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

        @dutyState.Action
        public UpdateDisplayFuelGauge!: (newDisplayFuelGauge: boolean) => void
       
        @commonState.State
        public userDetails!: userInfoType;
        
        hasPermissionToAddAssignDuty = false;

        myTeamMembers: any[] = []

        gaugeFields = [
            {key:'name', label:'My Team', stickyColumn: true, thClass:'text-center text-white', tdClass:'border-bottom py-0 my-0', thStyle:'margin:0; padding:0;background-color:#556077;'},
            {key:'availability', label:'', thClass:'', tdClass:'p-0 m-0 bg-white', thStyle:'margin:0; padding:0;'},
            
        ]

        @Watch('shiftAvailabilityInfo')
        shiftAvailability() 
        {
            this.extractSheriffAvailability()
        }

        mounted()
        {
            //console.log(this.shiftAvailabilityInfo)
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
            this.extractSheriffAvailability() 
        }

        dutyColors = [
            {name:'court' , colorCode:'#189fd4'},
            {name:'jail' ,  colorCode:'#A22BB9'},
            {name:'transport', colorCode:'#ffb007'},
            {name:'other',  colorCode:'#7a4528'},
            {name:'overtime',colorCode:'#e85a0e'},
            {name:'free',   colorCode:'#e6d9e2'}            
        ]

        public extractSheriffAvailability(){
            this.myTeamMembers = [];
            for(const sheriff of this.shiftAvailabilityInfo){
                //console.log(sheriff.availability)
                //this.findIsland(sheriff.availability)
                this.myTeamMembers.push({                     
                    name: Vue.filter('truncate')(sheriff.lastName,10) + ', '+ sheriff.firstName.charAt(0).toUpperCase(),
                    fullName: sheriff.firstName + ' ' + sheriff.lastName,
                    availability: this.sumOfArrayElements(sheriff.availability),
                    sheriff: sheriff,
                    availabilityDetail: this.findAvailabilitySlots(sheriff.availability)
                })
            }
        }

        public findAvailabilitySlots(array){
            const availabilityDetail: dutiesDetailInfoType[] =[]
            const discontinuity = this.findDiscontinuity(array);
            const iterationNum = Math.floor((this.sumOfArrayElements(discontinuity) +1)/2);
            //console.log(iterationNum)
            for(let i=0; i< iterationNum; i++){
                const inx1 = discontinuity.indexOf(1)
                let inx2 = discontinuity.indexOf(2)
                discontinuity[inx1]=0
                if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
                //console.error(inx1 + ' ' +inx2)
                availabilityDetail.push({
                        id: 10000+inx1 , 
                        startBin:inx1, 
                        endBin: inx2,
                        name: 'free' ,
                        colorCode: '#e6d9e2',
                        color: '#e6d9e2',
                        type: '',
                        code: ''
                    })
            }

            return availabilityDetail            
        }


        public findDiscontinuity(array){
            return array.map((arr,index)=>{
                if((index==0 && arr>0)||(arr>0 && array[index-1]==0)) return 1 ;
                else if(index>0 && arr==0 && array[index-1]>0) return 2 ;
                else return 0
            })
        }


        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
        }

        public closeDisplayMyteam(){
            this.UpdateDisplayFuelGauge(false)
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public DragStart(event: any) 
        { 
            event.dataTransfer.setData('text', event.target.id);
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .gauge {       
        position: sticky;
        overflow-y: scroll;
        margin-bottom: 0;
    }

    .gridfuel24 {        
        display:grid;
        grid-template-columns: repeat(24, 8.333%);
        grid-template-rows: 1.57rem;
        inline-size: 100%;
        font-size: 9px;         
    }

    .gridfuel24 > * {      
        padding: .25rem 0;
        border: 1px dotted rgb(185, 143, 143);
        background-color: #003366;
        color: white;
        font-size: 12px;
    }
</style>