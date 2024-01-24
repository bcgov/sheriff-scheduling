<template> 
    <b-card align="center" style="width: 13.4rem; height: 21rem;">
        <b-card-img
            :key="photoUpdateKey"
            v-if="photo"
            v-auth-image="photo"
            src=""            
            style="width: 11rem; height: 11rem; object-fit: scale-down;"
            class="mb-3"
        ></b-card-img>
    
        <b-icon-person-circle v-else class="mb-3" variant="secondary" font-scale="7.5" style="width: 11rem; height: 11rem;"></b-icon-person-circle>
        
        <b-card no-body class="mb-3 mt-2" v-if="editMode">
                <h4 ><b-badge v-if="photoError" variant="danger"> {{photoErrorMsg}} <b-icon class="ml-1" icon = x-square-fill @click="photoError = false" /></b-badge></h4>
                                
            <label class="btn btn-default btnfile">
                Browse for File <input type="file" style="display: none;" accept="image/x-png,image/gif,image/jpeg" onclick="this.value=null;" @change="onFileSelected">
            </label>
        </b-card>

        <b-card-sub-title class="my-1">{{user.badgeNumber}}</b-card-sub-title>
        <b-card-title>{{user.fullName}}</b-card-title>        
        <b-card-sub-title class="my-1">{{rank}}</b-card-sub-title>
        <b-card-text 
            style="color: #8a3078; font-size: 0.75rem;" 
            class="my-1">
                <b-icon-house-door-fill class="mr-1" font-scale="1.25"/>{{user.homeLocationNm}}
        </b-card-text>
        

        <b-modal v-model="showPhotoReplacementWarning" id="bv-modal-photo-replacement-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>                
                 <h2 class="mb-0 text-light"> Replacing Profile Photo </h2>                
            </template>
            <p>Are you sure you want to replace the profile photo?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-photo-replacement-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="uploadPhoto()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                 <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-photo-replacement-warning')"
                 >&times;</b-button>
            </template>
        </b-modal>    
           
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import moment from 'moment-timezone';
    import { namespace } from 'vuex-class';

    import {teamMemberInfoType} from '@/types/MyTeam';
    import {locationInfoType} from '@/types/common';  

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import VueAuthImage from 'vue-auth-image';
    Vue.use(VueAuthImage);

    @Component
    export default class UserSummaryTemplate extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        user!: teamMemberInfoType;

        @Prop({required: true})
        editMode!: boolean;
        
        imageData: string | ArrayBuffer | null = null; 
        photo: string | null | undefined = ''; 
        photoError = false;
        photoErrorMsg = '';
        showPhotoReplacementWarning = false;

        photoUpdateKey =0;

        rank='';


        mounted()
        {
            const today = moment();
            
            this.photo = this.user.image;
            // this.rank =  (this.user?.actingRank?.length>0)?  (this.user?.actingRank[0].rank)+' (A)': this.user.rank;
            this.rank =  this.user.rank;
            if (this.user?.actingRank?.length>0){

                for(const actingRankInfo of this.user.actingRank) {

                    const startDate = actingRankInfo.startDate
                    const endDate = actingRankInfo.endDate

                    if (today.isBetween(startDate, endDate)){
                        this.rank = actingRankInfo.rank+' (A)'
                    }                
                }
            }
        }

        
        public onFileSelected(event)
        {
            this.imageData = null
            this.photoError = false
            this.photoErrorMsg = ''
            // console.log(event)
            if (event.target.files && event.target.files[0]) 
            {       
                        
                const reader = new FileReader();                
                reader.onload = (e) => {
                        
                    if(e && e.target)
                        this.imageData = e.target.result;
                }
                reader.readAsArrayBuffer(event.target.files[0]);
                // console.log(event.target.files[0])

                // console.log(event.target.files[0])

                const acceptableImageTypes = ["image/png","image/gif","image/jpeg"]

                reader.onloadend = (()=>{

                    if(acceptableImageTypes.includes(event.target.files[0].type))
                    {                    
                        if(this.photo)
                            this.showPhotoReplacementWarning = true
                        else
                            this.uploadPhoto();

                    }
                    else
                    {
                        console.log('EErr')
                        this.photoError = true
                        this.photoErrorMsg = 'Only .jpg , .png , .gif photos!'
                    }
                })
            
            }
        }

        public uploadPhoto()
        {
            const imageBlob = new Blob([this.imageData?this.imageData:'']);   
            const formData = new FormData();
            formData.append('file', imageBlob);

            const url = 'api/courtAdmin/uploadphoto?id='+this.user.id;        
            this.showPhotoReplacementWarning = false
            this.$http.post(url, formData)
                .then(response => {
                    // console.log(response)
                    this.photo = response.data.photoUrl
                    this.$emit('photoChange', this.user.id,this.photo)
                    this.photoUpdateKey++;
                
                }, err => {
                    console.log(err.response.data);
                    this.photoError = true; 
                    const errInx= err.response.data.indexOf('Maximum upload size')
                    if(errInx >=0)                   
                        this.photoErrorMsg = err.response.data.substring(errInx);
                    else
                        this.photoErrorMsg = 'Photo upload unsuccessful!';

                }) 
        }


    }
</script>

 <style scoped>   

    .card {
        border: white;
    }

    .btnfile {
        position: relative;
        overflow: hidden;
        border: 1px solid;
        background: lightgrey;
    }
    .btnfile:hover {
        background-color: #103c6b;
        color: white;
    }
    
   


</style>