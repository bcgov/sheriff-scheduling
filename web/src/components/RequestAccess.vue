<template>
    <b-card bg-variant="white" class="home">

        <b-card style="width:50rem; margin: 2rem auto 0 auto;" bg-variant="dark" class="text-center text-white" >
            <b-card-header class="h3 bg-secondary mb-5">
                You currently don't have access to the Court Admin Scheduling Application!                
            </b-card-header>
            <b-row style="">                
                <img 
                    class="img-fluid ml-5 mr-2"          
                    src="../../public/images/bcid-logo-rev-en.svg"
                    width="177"
                    height="44"
                    alt="B.C. Government Logo"
                /> 
                <div class="mt-4 h3" >Requesting Access To Court Admin Scheduling:</div>
            </b-row>
            <b-row class="mx-auto my-0 p-0">
                <b-form-group class="mx-auto my-4" style="width: 20rem">
                    <label class="h6 m-0 p-0">Email:<span class="text-danger">*</span></label>
                    <b-form-input 
                        size="sm"
                        v-model="selectedEmail" 
                        placeholder="Enter Your Email" 
                        :state = "emailState?null:false">
                    </b-form-input>
                </b-form-group>
            </b-row>
            <b-button :disabled="submitted" variant="primary" class="py-2 px-5 text-warning" @click="requestAccess()"> Submit Your Request</b-button>  
            <b-row class="mx-auto my-4 p-0">
                <b-badge v-if="submitted" style="font-size:20px;" class="px-5 mx-auto bg-success" > Your request has been submitted successfully!<br/> We will get back to you soon.</b-badge>
            </b-row>
            <b-card v-if="errorText" class="text-danger">An unexpected error occurred ({{errorText}})
            </b-card>
        </b-card>

    </b-card>

</template>

<script lang="ts">
    import { Component } from 'vue-property-decorator';
    import Vue from "vue";
    import store from "@/store";

    @Component
    export default class RequestAccess extends Vue {

        selectedEmail = '';
        emailState = true;
        submitted = false;
        errorText ='';

        public requestAccess(){
            if(this.selectedEmail){
                this.emailState = true;
                const url = 'api/auth/requestaccess?currentEmailAddress='+this.selectedEmail
                this.$http.get(url)
                    .then(response => {
                       this.submitted = true;
                       console.log(response)
                       window.setTimeout(()=>this.signout(),5000);                 
                    },err => this.errorText = err) 
            }else{
                this.emailState = false
            }
             
        }

        public signout(){            
            store.commit('CommonInformation/setToken','');
            store.commit('CommonInformation/setTokenExpiry','');
            Vue.$cookies.set("logout","1",)
            window.location.replace(`${process.env.BASE_URL}api/auth/logout`);            
        }

    }
</script>

<style scoped>
    .card {
        border: white;
    }
</style>
