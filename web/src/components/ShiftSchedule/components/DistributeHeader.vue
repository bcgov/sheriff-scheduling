<template>
    <div>
        <header variant="primary">
            <b-navbar toggleable="lg" class=" m-0 p-0 navbar navbar-expand-lg navbar-dark">
                <b-navbar-nav>
                    <h3 style="width:15rem; margin-bottom: 0px;" class="text-white ml-2 font-weight-normal">Distribute Schedule</h3>
                </b-navbar-nav>

                <b-navbar-nav v-if="showWeekViewChecked" class="custom-navbar">
                    <b-button style="max-height: 40px;" size="sm" variant="secondary" @click="previousDateRange" class="my-2"><b-icon-chevron-left /></b-button>
                    <b-form-datepicker
                            class="my-2 p-1"
                            size="sm"
                            style="max-height: 40px;"
                            v-model="selectedDate"
                            @context="dateChanged"
                            @shown = "datePickerOpened = true"
                            @hidden = "datePickerOpened = false"
                            button-only
                            today-button
                            close-button
                            locale="en-US">
                    </b-form-datepicker>
                    <b-button style="max-height: 40px;" size="sm" variant="secondary" @click="nextDateRange" class="my-2"><b-icon-chevron-right/></b-button>
                </b-navbar-nav>

                <b-navbar-nav v-else class="custom-navbar">
                    <b-col class="my-1">
                        <b-row style="width:17.75rem">
                            <b-button style=" height: 2rem;" size="sm" variant="secondary" @click="previousDateRange" class="my-0 mx-1"><b-icon-chevron-left /></b-button>
                            
                            <div class="m-0 bg-white" style="padding:0.2rem 0.52rem; border-radius:3px; font-weight:bold;">{{selectedDate | beautify-date-weekday}}</div>
                            
                            <b-form-datepicker
                                    class="my-0 py-0 mx-1"
                                    size = "sm"
                                    style="height: 2rem;"
                                    v-model="selectedDate"
                                    @context="dateChanged"
                                    @shown="datePickerOpened = true"
                                    @hidden="datePickerOpened = false"
                                    button-only
                                    today-button
                                    close-button
                                    locale="en-US">
                            </b-form-datepicker>
                            <b-button style="height: 2rem;" size="sm" variant="secondary" @click="nextDateRange" class="my-0"><b-icon-chevron-right/></b-button>
                        </b-row>
                        
                    </b-col>
                </b-navbar-nav>

                <b-navbar-nav class="mr-2">
                    <b-input-group class="mr-2 my-0" style="height: 40px">
                        <b-input-group-prepend is-text>
                            <b-icon icon="person-fill"></b-icon>
                        </b-input-group-prepend>
                        <b-form-select
                            style="height: 100%;"
                            v-model="selectedTeamMember"
                            @change="getSchedule()">
                            <b-form-select-option :value="{courtAdminId: '', name: 'All', email: ''}">All</b-form-select-option>
                            <b-form-select-option
                                v-for="member in teamMemberList"
                                :key="member.courtAdminId"                  
                                :value="member">{{member.name}}
                            </b-form-select-option>                  
                        </b-form-select>
                    </b-input-group>
                </b-navbar-nav>

                <b-navbar-nav class="mr-2">
                    <b-nav-form>						
                        <div :class="showWeekViewChecked?'bg-success':''" style="border:1px solid green;border-radius:5px; margin-left: 15px; width: 8.5rem;">
                            <b-form-checkbox class="mx-1 my-1" v-model="showWeekViewChecked" @change="getSchedule()" size="lg" switch>
                                <div style="font-size: 16px;" class="text-white">{{viewRange}}</div>
                            </b-form-checkbox>
                        </div>
                        <div v-b-tooltip.hover.noninteractive
                            title="Print Schedule">
                            <b-button 
                                style="max-height: 40px;" 
                                size="sm"
                                variant="white"						
                                @click="printSchedule()"
                                :disabled="loadingPdf" 
                                class="mb-0 mt-n1 ml-2">
                                <b-overlay :show="loadingPdf" rounded="sm" :opacity="0.4" spinner-small spinner-variant="primary">
                                    <b-icon icon="printer-fill" class="mt-1 mx-1" font-scale="2.0" variant="white"/>
                                </b-overlay>                               
                            </b-button>
                        </div>
                        <div v-b-tooltip.hover.noninteractive
                            title="Email Schedule">
                            <b-button 
                                style="max-height: 40px;" 
                                size="sm"
                                variant="white"		
                                @click="emailSchedule()"
                                :disabled="loadingPdf" 
                                class="my-0 ml-2">
                                <b-overlay :show="loadingPdf" rounded="sm" :opacity="0.4" spinner-small spinner-variant="primary">
                                    <b-icon icon="envelope-fill" class="mx-1" font-scale="2.0" variant="white"/>
                                </b-overlay>
                            </b-button>
                        </div>
                    </b-nav-form>
                </b-navbar-nav>
            </b-navbar>
        </header>	

        <b-modal size="xl" v-model="showEmailWindow" header-class="bg-primary text-light" title-class="h1" title="Prepare Schedule Email">
            
            <b-card style="border:none" bg-variant="white" class="my-4 container">   

                <b-row>
                    <b-col>				
                        <b-form-group class="mr-1"><label>To<span class="text-danger">*</span></label>
                            <b-form-input v-model="emailContent.to" placeholder="Enter Recipients" :state="recipientState?null:false"></b-form-input>
                        </b-form-group>        
                    </b-col>	
                    <b-col cols="3">
                        <b-dropdown
                            class="recipientList"
                            variant="primary"
                            text="Recipients"		
                            style="width: 100%; margin-top: 2rem;"
                            allow-focus>
                            <div class="ml-1">
                                <b-form-checkbox	
                                    @change="toggleAllEmails"															
                                    v-model="allSelected">All
                                </b-form-checkbox>
                                <b-form-checkbox-group                                    
                                    v-model="recipients">
                                    <b-form-checkbox
                                        v-for="member in teamMemberList"
                                        :key="member.courtAdminId"                  
                                        :value="member.email">{{member.name}}								
                                    </b-form-checkbox>
                                </b-form-checkbox-group>
                            </div>
                        </b-dropdown>
                    </b-col>
                                    
                </b-row>
                <b-row>					
                    <b-col>				
                        <b-form-group class="mr-1"><label>Subject<span class="text-danger">*</span></label>
                            <b-form-input v-model="emailContent.subject" placeholder="Enter Subject" :state = "subjectState?null:false"></b-form-input>
                        </b-form-group>        
                    </b-col>					
                </b-row>
                <b-row>					
                    <b-col>				
                        <b-form-group class="mr-1"><label>Content<span class="text-danger">*</span></label>
                            <b-form-textarea 
                                v-model="emailContent.body" 
                                placeholder="Enter Content"
                                rows="6" 
                                :state = "contentState?null:false"></b-form-textarea>
                        </b-form-group>        
                    </b-col>					
                </b-row>				
            </b-card>

            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="showEmailWindow=false"
                >&times;</b-button>
            </template>

            <template v-slot:modal-footer>
                <button-bar
                    :pdfType="pdfType"
                    :emailingPdf="emailingPdf" 
                    @emailSchedulePdf="emailSchedulePdf" 
                    @closeEmail="showEmailWindow=false" />
            </template>
        </b-modal>

        <b-modal size="xl" v-model="showSentEmail" title-class="h1 text-success" ok-only title="Email Sent Successfully">
            <sent-email-content :emailContent="emailContent" />                
        </b-modal> 

    </div>
</template>

<script lang="ts">
 
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from "vuex-class";
    import moment from 'moment-timezone';

    import "@store/modules/ShiftScheduleInformation";
    const shiftState = namespace("ShiftScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import { distributeTeamMemberInfoType, sentEmailContentInfoType, shiftRangeInfoType } from '@/types/ShiftSchedule';
    import { locationInfoType, userInfoType } from '@/types/common';

    import ButtonBar from './ButtonBar.vue';
    import SentEmailContent from './SentEmailContent.vue';

    @Component({
        components:{        
            ButtonBar,
            SentEmailContent
        }
    })
    export default class DistributeHeader extends Vue {	

        @commonState.State
        public location!: locationInfoType;

        @commonState.State
        public userDetails!: userInfoType;

        @shiftState.State
        public teamMemberList!: distributeTeamMemberInfoType[];
                
        @shiftState.State
        public shiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateShiftRangeInfo!: (newShiftRangeInfo: shiftRangeInfoType) => void	

        @shiftState.State
        public dailyShiftRangeInfo!: shiftRangeInfoType;

        @shiftState.Action
        public UpdateDailyShiftRangeInfo!: (newDailyShiftRangeInfo: shiftRangeInfoType) => void	

        selectedDate = '';
        datePickerOpened = false;		
        showWeekViewChecked = true;
        hasPermissionToViewDutyRoster = false;
        pdfType=''
        errorMsg = ''
        showEmailWindow = false;
        emailingPdf = false;
        emailContent = {} as sentEmailContentInfoType;
        showSentEmail = false;
        recipients: string[] = [];
        teamMemberEmailList: string[] = [];
        recipientState = true;
        subjectState = true;
        contentState = true;
        allSelected = false;
        loadingPdf = false;

        selectedTeamMember = {courtAdminId: '', name: 'All', email: ''} as distributeTeamMemberInfoType;

        @Watch('location.id', { immediate: true })
        locationChange()
        {
            this.selectedTeamMember = {courtAdminId: '', name: 'All', email: ''};
            this.showWeekViewChecked = true;       
        }

        @Watch('recipients', { immediate: true })
        recipientChange()
        {
            const emails = this.emailContent.to?.split(",") || [];
            const removeList = this.teamMemberEmailList.filter(email => !this.recipients.includes(email)) || [];
            const cleanList = emails?.filter(email => !removeList.includes(email)) || [];
            const reminderList = this.recipients?.filter(recipt => !emails.includes(recipt)) || [];
            this.emailContent.to = [...cleanList,...reminderList].join(",");
        }

        mounted() {
            this.loadingPdf = false;
            this.showWeekViewChecked = true;
            this.hasPermissionToViewDutyRoster = this.userDetails.permissions.includes("ViewDutyRoster");		
            this.teamMemberEmailList = this.teamMemberList.map(member => member.email);	
            
            if(!this.shiftRangeInfo.startDate || !this.dailyShiftRangeInfo.startDate){
                this.selectedDate = moment().format().substring(0,10);
                this.loadNewDateRange();
            } else {
                this.selectedDate = this.shiftRangeInfo.startDate;
                this.getSchedule();
            }			
        }

        public toggleAllEmails(checked){			
            this.recipients = checked?this.teamMemberList.map(member => member.email).slice(): [];
        }
        
        public getSchedule() {            			
            Vue.nextTick(()=>{
                this.selectedDate = this.dailyShiftRangeInfo.startDate;
                this.$emit('change', this.showWeekViewChecked, this.selectedTeamMember.courtAdminId)
            })
        }

        public emailSchedule(){
            
            this.errorMsg = "";
            this.pdfType = this.showWeekViewChecked?'Weekly Schedule': 'Daily Schedule';
            this.showEmailWindow = true;
            this.emailContent = {} as sentEmailContentInfoType;
        }

        public printSchedule() { 
            const el = document.getElementById("pdf")   
            const bottomLeftText = `" CAS ";`;
            const bottomRightText = `" "`;         
            const url = '/api/distributeschedule/print';            
            const pdfhtml = Vue.filter('printPdf')(el?.innerHTML, bottomLeftText, bottomRightText );

            const body = {
                html: pdfhtml               
            }
            
            const options = {
                responseType: "blob",
                headers: {
                "Content-Type": "application/json",
                }
            }
            this.loadingPdf = true;  
            this.$http.post(url,body, options)
            .then(res => { 
                const blob = res.data;
                const link = document.createElement("a");
                link.href = URL.createObjectURL(blob);
                document.body.appendChild(link);
                link.download = "SS.pdf";
                link.click();
                setTimeout(() => {
                    URL.revokeObjectURL(link.href);
                    this.loadingPdf = false;
                }, 1000);                                
            },err => {
                this.loadingPdf = false;
                // console.error(err);                
            });
        }

        public emailSchedulePdf() {

            let requiredError = false;

            if (!this.emailContent.to){
                this.recipientState = false;
                requiredError = true;
            } else {
                this.recipientState = true;
            }

            if (!this.emailContent.subject){
                this.subjectState = false;
                requiredError = true;
            } else {
                this.subjectState = true;
            }

            if (!this.emailContent.body){
                this.contentState = false;
                requiredError = true;
            } else {
                this.contentState = true;
            }

            this.showSentEmail = false;

            if (!requiredError){
                this.emailContent.from = Vue.filter('capitalizefirst')(this.userDetails.firstName) + ' ' + Vue.filter('capitalizefirst')(this.userDetails.lastName);
                this.sendEmail();
            }			
        }

        public sendEmail(){

            this.emailingPdf=true;
            const el= document.getElementById("pdf");            
            const bottomLeftText = `" CAS ";`;
            const bottomRightText = `" "`;
            const url = '/api/distributeschedule/email';            
            const pdfhtml = Vue.filter('printPdf')(el?.innerHTML, bottomLeftText, bottomRightText );            
            const body = {
                html: pdfhtml,
                recipients: this.emailContent.to.replace(/ /g, ''),
                emailContent: this.emailContent.body,
                emailSubject: this.emailContent.subject
            }
            
            const options = {                
                headers: {
                "Content-Type": "application/json",
                }
            }  

            this.$http.post(url,body, options)
            .then(res => {
                                
                this.emailingPdf=false;
                this.showEmailWindow=false; 
                this.showSentEmail = true;
                                
            },err => {
                console.error(err);
                this.showEmailWindow=false;
                this.emailingPdf=false;    
                this.showSentEmail = false;                            
            });

        }
        
        get viewRange() {
            if(this.showWeekViewChecked) return 'Week View';else return 'Day View'
        }
        
        public dateChanged(event) {
            if(this.datePickerOpened)this.loadNewDateRange();
        }

        public nextDateRange() {
            const days =(this.showWeekViewChecked)? 7 :1;		
            this.selectedDate = moment(this.selectedDate).add(days, 'days').format().substring(0,10);
            this.loadNewDateRange(); 
        }

        public previousDateRange() {
            const days =(this.showWeekViewChecked)? 7 :1;
            this.selectedDate = moment(this.selectedDate).subtract(days, 'days').format().substring(0,10);
            this.loadNewDateRange();
        }

        public loadNewDateRange() {
            const firstDayOfWeek = moment(this.selectedDate).startOf('week').format()
            const lastDayOfWeek = moment(this.selectedDate).endOf('week').format();
            let weeklyDateRange = {} as  shiftRangeInfoType;			
            
            weeklyDateRange = {startDate: firstDayOfWeek.substring(0,10), endDate: lastDayOfWeek.substring(0,10)}
            this.UpdateShiftRangeInfo(weeklyDateRange);

            let dailyDateRange = {} as  shiftRangeInfoType;
            
            dailyDateRange = {
                startDate: moment(this.selectedDate).startOf('day').format().substring(0,10), 
                endDate: moment(this.selectedDate).endOf('day').format().substring(0,10)
            }

            this.UpdateDailyShiftRangeInfo(dailyDateRange);
                        
            this.getSchedule(); 
        }
    }
</script>

<style scoped>

    .card {
            border: white;
    }

    .custom-navbar {
        float:none;
        margin:0 auto 0 auto;
        display: block;
        text-align: center;
    }

    .custom-navbar > li {
        display: inline-block;
        float:none;
    }

    .recipientList /deep/ .dropdown-menu {
        max-height: 300px;
        overflow-y: auto;
    }	

</style>