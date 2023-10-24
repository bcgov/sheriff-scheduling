<template>
    <div :class="{'gridcourtAdmin':true, 'fullview':fullview}" :style="pdfView?'':{gridTemplateColumns: 'repeat(96, '+zoomLevel*2.0833/100+'%)'}">
        <div class="grid" v-for="i in 96" :key="i" :style="{backgroundColor: '#F1FEF1', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/2'}"></div>
        <div 
            v-for="(block,index) in courtAdminInfo.availabilityDetail"
            :key="index+2000"
            :style="{gridColumnStart: (1+block.startBin),gridColumnEnd:(1+block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0 }"
            v-b-tooltip.hover.bottom                             
            :title="block.name">
                <div class="text" :style="{textTransform: 'capitalize', margin:fullview?'0.2rem 0 0 0':'0', padding:'0', fontSize: fullview?'15px':'13px',transform:'translate(0,-4px)'}">
                    {{block.name|truncate((block.endTime - block.startTime-1)*2)}}
                </div>                
        </div>
        <div 
            v-for="(block,index) in courtAdminInfo.courtAdmin.dutiesDetail"
            :key="index+1000"
            :style="{gridColumnStart: (1+block.startBin),gridColumnEnd:(1+block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0, color:'white' }"
            v-b-tooltip.hover.bottom                             
            :title="block.code">
                <div class="text" :style="{textTransform: 'capitalize', margin:fullview?'0.2rem 0 0 0':'0', padding:'0', fontSize: fullview?'15px':'13px', transform:'translate(0,-4px)'}">
                    {{getBlockTitle(block.name,block.code,(block.endTime - block.startTime-1)*2)}}
                </div>                
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { myTeamShiftInfoType} from '@/types/DutyRoster';
    
    import { namespace } from "vuex-class";
    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    import * as _ from 'underscore';

    @Component
    export default class CourtAdminAvailabilityCard extends Vue {

        @Prop({required: true})
        courtAdminInfo!: myTeamShiftInfoType;

        @Prop({required: false, default:false})
        fullview!: boolean;

        @Prop({required: false, default:false})
        pdfView!: boolean;

        @dutyState.State
        public zoomLevel!: number;

        styleGauge = "text-transform: capitalize; margin:0; padding: 0; font-size: 13px; transform:translate(0,-4px)"
        styleFullview = "text-transform: capitalize; margin-top:1rem; padding: 0; font-size: 16px;"

        public getBlockTitle(name,code,len){
           return Vue.filter('truncate')( code,len)
        }

    }
</script>

<style scoped lang="scss">

    .gridcourtAdmin {        
        display:grid;
        grid-template-columns: repeat(96, 2.0833%);
        grid-template-rows: repeat(1,.9rem);
        inline-size: 100%; 
        background-color: #fcf5f5; 
        height: 1rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;

        &.fullview {
            height: 1.75rem;
            grid-template-rows: repeat(1,1.75rem);
        }
           
    }

    .gridcourtAdmin > * { 
        padding: 0px 0;
        border: 1px dotted rgb(202, 202, 202);
    }

</style>