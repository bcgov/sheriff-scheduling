<template>
    <b-card bg-variant="white" class="home">

        <b-card style="width:40rem; margin: 2rem auto 0 auto;" bg-variant="success " class="text-center text-white" >
            <b-card-title class="h2"> You Successfully Signed Out </b-card-title>    
        </b-card>

        <b-card style="width:35rem; margin: 2rem auto 0 auto;">
            <b-button  variant="primary" class="py-2 px-5 text-warning" @click="login()"> <img 
							class="img-fluid mr-3"          
							src="../../public/images/bcid-logo-rev-en.svg"
							width="177"
							height="44"
							alt="B.C. Government Logo"
						/> Login To CourtAdmin Scheduling</b-button>
        </b-card>

    </b-card>

</template>

<script lang="ts">
    import { Component } from 'vue-property-decorator';
    import Vue from "vue";

    @Component
    export default class Logout extends Vue {

        public login(){
            Vue.$cookies.remove("logout");
            window.location.replace(`${process.env.BASE_URL}api/auth/login?redirectUri=%2Fcourt-admin-scheduling`);
        }

        mounted(){
            Vue.$cookies.remove("logout");
            console.log(document.cookie)
        }

    }
</script>

<style scoped>
    .card {
        border: white;
    }
</style>
