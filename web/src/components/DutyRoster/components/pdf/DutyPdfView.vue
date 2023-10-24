<template>
    <b-card class="bg-white" no-body > 

        <b-row class="border-bottom mt-n2 pb-1"> 
            <b-col>          
                <b-button class="mr-3" size="sm" variant="secondary" @click="closePrint()">
                    <b-icon-x /> Close
                </b-button>
            </b-col>
            <b-col>
                <b-button size="sm px-3 float-right" variant="success" @click="print()">
                    <b-icon-printer /> Print
                </b-button>
            </b-col>
        </b-row>

        <b-card id="print" bg-variant="white" class="mt-2 mb-4 pdf-container" no-body>
            
            <div v-for="page,inx in sheriffPages" :key="'pdf-'+inx">
                <b-row class="mt-4 mb-0 mx-4">
                    <b-col>
                        <b-row>
                            <div style="width:20%"  class="mr-0">
                                <b-img 
                                    class="img-fluid"
                                    width="50rem"
                                    height="40rem"
                                    :src="require('../../../../../public/images/bcss-crest.png')" 
                                alt="B.C. Government Logo">
                                </b-img>
                            </div>
                            <div style="width:80%" class="ml-0">
                                    <h4 class="mt-3">B.C. Sheriff Service</h4>
                                    <h5 class="mt-n2 text-secondary font-italic">Honour - Integrity - Commitment</h5>
                            </div>
                        </b-row>
                    </b-col>                    
                    <b-col>
                        <b-card body-class="p-1" class="mb-1 border border-dark text-center">
                            <div class="mb-0 text-secondary h5">{{location.name}} Schedule </div>
                            <b-card-title class="h5 mb-0">{{dutyRangeInfo.startDate | beautify-full-date}}</b-card-title>
                            <b-card-text class="text-secondary">Summary as of: <i class="h6">{{today | beautify-date-time-weekday}}</i></b-card-text>
                        </b-card>
                    </b-col> 
                </b-row>

                <b-table                    
                    :items="sortedMyTeamMembers.slice(page.start,page.end)" 
                    :fields="gaugeFields"                    
                    borderless
                >
                    <template v-slot:table-colgroup> 
                        <col style="width:10rem !important">
                        <col>
                    </template>
                    <template v-slot:head(name)="data" >
                        <div class="bg-info border border-info">{{data.label}}</div>
                    </template>
                    <template v-slot:cell(name)="data" >
                        <div                                                                                                                    
                            style="height:2rem; font-size:14px; line-height: 1rem; text-transform: capitalize; margin:0; padding:0.5rem 0 0 0.25rem"
                            class="txt-primary" >
                                {{data.value}}
                        </div>
                    </template>
                    <template v-slot:head(availability) >
                        <div class="gridfuel24">
                            <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                        </div>
                    </template>
                    <template v-slot:cell(availability)="data" >
                        <court-admin-availability-card class="m-0 p-0 gridsheriff " :sheriffInfo="data.item" :pdfView="true"/>
                    </template>
                </b-table>
           
                <b-row :style="{marginTop:((inx==sheriffPages.length-1)?marginToLastPageRows:'0rem')}" >
                    <div style="width:80%"> B.C. Sheriff Service </div>
                    <div style="width:12%"> Page {{inx+1}} of {{sheriffPages.length}}</div>
                </b-row>
               
                <div v-if="(inx+1)<sheriffPages.length" class="new-page" />

            </div>

        </b-card>

        <b-row v-if="sheriffPages.length>1" class="border-top pt-1"> 
            <b-col>          
                <b-button class="mr-3" size="sm" variant="secondary" @click="closePrint()">
                    <b-icon-x /> Close
                </b-button>
            </b-col>
            <b-col>
                <b-button size="sm px-3 float-right" variant="success" @click="print()">
                    <b-icon-printer /> Print
                </b-button>
            </b-col>
        </b-row>

    </b-card>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import moment from 'moment';
import { Printd } from 'printd'
import SheriffAvailabilityCard from '../SheriffAvailabilityCard.vue'
import { locationInfoType } from '@/types/common';
import { dutyRangeInfoType } from '@/types/DutyRoster';

import { namespace } from "vuex-class";

import "@store/modules/CommonInformation";
const commonState = namespace("CommonInformation");

import "@store/modules/DutyRosterInformation";   
const dutyState = namespace("DutyRosterInformation");

import * as _ from 'underscore';
const bootstrapCss = require('!!raw-loader!@/styles/bootstrapMIN.css').default;

@Component({
    components:{        
        SheriffAvailabilityCard
    }
})
export default class DutyPDFView extends Vue {
    
    @Prop({required:true})
    myTeamMembers!: any[]

    @commonState.State
    public location!: locationInfoType;

    @dutyState.State
    public dutyRangeInfo!: dutyRangeInfoType;

    @dutyState.Action
    public UpdatePrintSheriffFullview!: (newPrintSheriffFullview: boolean) => void;

    dataReady = false;
    today = ''
    sheriffPages: any[] =[]
    marginToLastPageRows = '0rem'

       
    gaugeFields = [
        {key:'name', label:'Sheriff Name', stickyColumn: true, thClass:'text-center text-white', tdClass:'border-bottom py-0 my-0', thStyle:'margin:0; padding:0;'},
        {key:'availability', label:'', thClass:'', tdClass:'p-0 m-0 bg-white', thStyle:'margin:0; padding:0;'},        
    ]

    mounted(){
        this.today = moment().tz(this.location.timezone).format();
        this.splitSheriffPages()
    }

    get sortedMyTeamMembers(){
        return _.sortBy(this.myTeamMembers, function(member){return member.name.toLowerCase()})
    }


    public splitSheriffPages(){
        const PAGE_ITEMS=16
        const len = this.myTeamMembers.length
        for(let page=1; page<=(Math.ceil(len/PAGE_ITEMS)); page++){
            this.sheriffPages.push({
                start:(page-1)*PAGE_ITEMS,
                end:Math.min(len, page*PAGE_ITEMS)
            })
        }  
        const mod = this.myTeamMembers.length % PAGE_ITEMS
        this.marginToLastPageRows = (mod==0)? '0rem' :((PAGE_ITEMS-mod)*2.1)+'rem'
    }

    public print(){
        const pdfPage: Printd = new Printd()
        const styles = [				
            bootstrapCss,
            `@media print {
                @page {
                    size:11in 8.5in !important;
                    font-size: 10pt !important;                                 
                }                            
				.new-page{
					page-break-before: always;
					position: relative; top: 8em;
				}
				section{
					page-break-inside: avoid;
				}
				.print-block{
					page-break-inside: avoid;
				}                
			}
            .pdf-container {
                padding: 2px 10px !important; 
                margin-right: auto !important;
                margin-left: auto !important;
                width: 100% !important;
                max-width: 970px !important;
                min-width: 970px !important;   
                font-size: 10pt !important;        
                color: #313132 !important;
            }
            .card {
                border: white;
            }
            .txt-primary{
                color: #003366;
            }
            .gridfuel24 {        
                display:grid;
                grid-template-columns: repeat(24, 2.1rem);
                grid-template-rows: 1.57rem;
                inline-size: 100%;
                font-size: 9px;         
            }
            .gridfuel24 > * {      
                padding: .25rem 0 0 0.2rem;
                border: 1px dotted rgb(185, 143, 143);
                background-color: #003366;
                color: white;
                font-size: 9.5px;
            }
            .gridsheriff {        
                display:grid;
                grid-template-columns: repeat(96, 0.525rem);
                grid-template-rows: repeat(1,1.95rem);
                inline-size: 100%; 
                background-color: #fcf5f5; 
                height: 1.95rem; 
                margin: 0; 
                padding: 0;
                column-gap: 0;
                row-gap: 0;
                ::v-deep div.grid{           
                    border: 1px dotted rgb(202, 202, 202);
                }
                ::v-deep div.text{
                    margin-top: 0.5rem !important ;
                    line-height: 0.85rem !important;            
                    display:flex; 
                    align-items:center; 
                    justify-content:center; 
                    text-align:center;
                }
            }            
            .gridsheriff  .text {
                margin-top: 0.5rem !important ;
                line-height: 0.85rem !important;            
                display:flex; 
                align-items:center; 
                justify-content:center; 
                text-align:center;
            }
            .gridsheriff > div.grid {                          
                border: 1px dotted rgb(202, 202, 202);
            }
            `
        ]

        // for(const stylesheet of document.styleSheets){
        //    if(stylesheet['rules'][0]['selectorText'].includes(".pdf-container")){
        //        for(const style of stylesheet['rules']){                   
        //            styles.push(style.cssText)
        //        }
        //    }
        // }
        const pageToPrint = document.getElementById("print")
        if(pageToPrint) pdfPage.print(pageToPrint, styles)
        this.closePrint()
    }

    public closePrint(){
        this.UpdatePrintSheriffFullview(false);
    }

    public getBeautifyTime(hour: number){
        return( hour>9? hour+':00': '0'+hour+':00')
    }

}
</script>
<style scoped lang="scss">

    .pdf-container {
        padding: 2px 10px !important; 
        margin-right: auto !important;
        margin-left: auto !important;
        width: 100% !important;
        max-width: 970px !important;
        min-width: 970px !important;   
        font-size: 10pt !important;        
        color: #313132 !important;
    }

    .card {
        border: white;
    }

    .txt-primary{
        color: #003366;
    }
    
    .gridfuel24 {        
        display:grid;
        grid-template-columns: repeat(24, 2.1rem);
        grid-template-rows: 1.57rem;
        inline-size: 100%;
        font-size: 9px;         
    }

    .gridfuel24 > * {      
        padding: .25rem 0 0 0.2rem;
        border: 1px dotted rgb(185, 143, 143);
        background-color: #003366;
        color: white;
        font-size: 9.5px;
    }

    .gridsheriff {        
        display:grid;
        grid-template-columns: repeat(96, 0.525rem);
        grid-template-rows: repeat(1,1.95rem);
        inline-size: 100%; 
        background-color: #fcf5f5; 
        height: 1.95rem; 
        margin: 0; 
        padding: 0;
        column-gap: 0;
        row-gap: 0;
        ::v-deep div.grid{           
            border: 1px dotted rgb(202, 202, 202);
        }
        ::v-deep div.text{
            margin-top: 0.5rem !important ;
            line-height: 0.85rem !important;            
            display:flex; 
            align-items:center; 
            justify-content:center; 
            text-align:center;
        }
    }


</style>