<template>    
    <b-row v-if="dataReady"  style="float: right; margin-left: auto; margin-right: auto; padding: 0;" class="mt-4">
        <b-dropdown
            style="height: 30% !important;"
            class="mr-3 py-0"      
            variant="primary">
            <template #button-content >
                <div style="display:inline; font-size: 0.75rem; line-height: 0.75rem !important; height: 40% !important;">
                    Items Per Page: {{itemsPerPage}}
                </div>
            </template>
            <b-dropdown-item v-for="page in pages" :key="'page'+page" @click="itemsPerPage=page;pageChanged();">{{page}}</b-dropdown-item>            
        </b-dropdown>

        <b-pagination  
            @change="pageChanged"                         
            v-model="currentPage"
            :total-rows="totalRows"
            :per-page="itemsPerPage" 
            first-number
            last-number                             
            first-text="First"
            prev-text="Prev"
            next-text="Next"
            last-text="Last">
        </b-pagination>
    
    </b-row>
          
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';


@Component
export default class CustomPagination extends Vue {

    @Prop({required: true})
    pages!: number[];

    @Prop({required: true})
    totalRows!: number;
    
    @Prop({required:true})
    initCurrentPage!: number;

    @Prop({required:true})
    initItemPerPage!: number;
    
    currentPage=1;
    itemsPerPage = 1;// Default
    dataReady = false

    mounted(){
        this.dataReady = false
        this.currentPage = this.initCurrentPage
        if(this.initItemPerPage)
            this.itemsPerPage = this.initItemPerPage
        else
            this.itemsPerPage = this.pages[0]
        // console.log('mount ' + this.initCurrentPage)
        this.dataReady = true
    }
   
    pageChanged(){
        

        Vue.nextTick(()=>{   
        // console.log('itemsPerPage '+this.itemsPerPage)
        // console.log('currentPage '+this.currentPage)
        
            this.$emit('paginationChanged', this.currentPage, this.itemsPerPage)
        })
    }    

}
</script>
