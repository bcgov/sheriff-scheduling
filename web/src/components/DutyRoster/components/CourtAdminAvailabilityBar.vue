<template>
    <div v-if="dataReady" class="gridcourtAdmin" :style="{gridTemplateColumns: gridTemplateStyle}">                
        <div class="grid" v-for="i in barLength" :key="i" :style="{backgroundColor: '#F1FEF1', gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/2'}"></div>
        <div 
            v-for="(block,index) in findAvailabilitySlots(availability)"
            :key="index+3000"
            :style="{gridColumnStart: (offset+block.startBin),gridColumnEnd:(offset+block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0 }"
            v-b-tooltip.hover.bottom                             
            :title="block.name">                                
        </div>
        <div 
            v-for="(block,index) in dutiesDetail"
            :key="index+4000"
            :style="{gridColumnStart: (offset+block.startBin),gridColumnEnd:(offset+block.endBin), gridRow:'1/1',  backgroundColor: block.color, fontSize:'9px', textAlign: 'center', margin:0, padding:0, color:'white' }"
            v-b-tooltip.hover.bottom                             
            :title="block.name +'-'+ block.code">                           
        </div>        
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { myTeamShiftInfoType, dutiesDetailInfoType} from '@/types/DutyRoster';
    
    @Component
    export default class CourtAdminAvailabilityBar extends Vue {

        @Prop({required: true})
        courtAdminInfo!: myTeamShiftInfoType;

        @Prop({required: true})
        dutyBlock!: any;
        
        @Prop({required: true})
        weekView!: boolean;
        
        offset= 1
        barLength = 96
        gridTemplateStyle
        dataReady =  false
        dutiesDetail: dutiesDetailInfoType[] = []
        availability: number[] = []

        mounted(){
            this.dataReady = false
            this.getDutiesAndAvailabilities()
            this.getBarArea()
            this.dataReady = true
        }

        public getDutiesAndAvailabilities(){
            
            if(this.weekView){                
                const dutyDate=this.dutyBlock.dutyDate.slice(0,10)
                this.dutiesDetail = this.courtAdminInfo.dutiesDetail.filter(
                    duty => {
                        if(duty.startTime)
                            return duty.startTime.slice(0,10)==dutyDate
                        else 
                            return false 
                    }
                )                
            }
            else{
                this.dutiesDetail = this.courtAdminInfo.dutiesDetail;
            }
            
            this.availability = this.courtAdminInfo.availability;
            
        }

        public getBarArea(){
        
            let duties =  Array(96).fill(0)
            for(const dutydetail of this.dutiesDetail){
               duties = this.fillInArray(duties, 1, dutydetail.startBin, dutydetail.endBin)
            } 
            duties = this.sumOfTwoArrays(duties,this.availability );
            const indexFirst = duties.findIndex(val => val>0)
            const indexLast = duties.reverse().findIndex(val => val>0)
            this.offset = 1-indexFirst
            this.barLength = 96 - indexLast -indexFirst-1

            this.gridTemplateStyle = 'repeat('+this.barLength+', '+(100/this.barLength)+'%)'
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

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }

        public fillInArray(array, fillInNum, startBin, endBin){
            return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
        }

        public sumOfTwoArrays(arrayA, arrayB){
            return arrayA.map((arr,index) =>{return arr+arrayB[index]});
        }

    }
</script>

<style scoped lang="scss">

    .gridcourtAdmin {        
        display:grid;
        grid-template-columns: repeat(96, 1.04%);
        grid-template-rows: repeat(1,1rem);
        inline-size: 100%; 
        background-color: #fcf5f5; 
        height: 1rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;       
           
    }

</style>