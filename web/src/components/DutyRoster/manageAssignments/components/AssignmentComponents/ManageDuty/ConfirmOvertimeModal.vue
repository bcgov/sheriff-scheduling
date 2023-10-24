<template>
    <div>
        <b-modal v-model="showModal.show" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 class="mb-0 text-light">Confirm Assign Overtime</h2>                    
            </template>            
           
            <h4 style="line-height:1.5rem" v-if="courtAdminName" >Are you sure you want to assign a duty that extends {{courtAdminName}}'s shift?</h4>
            
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="assignOvertimeDuty()">Confirm</b-button>
                <b-button variant="primary" @click="showModal.show=false;">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" 
                    class="text-light closeButton" 
                    @click="showModal.show=false;"
                    >&times;
                </b-button>
            </template>
        </b-modal>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';

    @Component({})  
    export default class ConfirmOvertimeModal extends Vue {

        @Prop({required: true})
        showModal!: {show: boolean};

        @Prop({required: true})
        courtAdminName!: string;

        assignOvertimeDuty(){
            this.$emit('assign')
            this.showModal.show=false;
        }
    }
</script>
