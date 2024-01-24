import Vue from 'vue'
import moment from 'moment-timezone';


Vue.filter('beautify-date', function(date){
    enum MonthList {'Jan' = 1, 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'}
    if(date)
        return date.substr(8,2) + ' ' + MonthList[Number(date.substr(5,2))] + ' ' + date.substr(0,4);
    else
        return ''
})

Vue.filter('beautify-full-date', function(date){
    
    if(date)
        return moment(date).format('MMMM DD, YYYY');
    else
        return ''
})

Vue.filter('beautify-date-weekday', function(date){
    if(date)
        return	moment(date).format('ddd DD MMM YYYY');
    else
        return ''
})

Vue.filter('beautify-date-time-weekday', function(date){
    if(date)
        return	moment(date).format('dddd MMM DD, YYYY HH:mm');
    else
        return ''
})

Vue.filter('beautify-date-time', function(date){
    enum MonthList {'Jan' = 1, 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'}
    if(date)
        return date.substr(8,2) + ' ' + MonthList[Number(date.substr(5,2))] + ' ' + date.substr(0,4)+ ' ' + date.substr(11,5);
    else
        return ''
})

Vue.filter('beautify-time', function(date){
    
    if(date)
        return date.substr(11,5);
    else
        return ''
})

Vue.filter('event-date-time', function(date){
    
    if(date){
        if(date.substr(11,5)=='00:00')
            return moment(date).format('MMM DD, YYYY');
        else
            return moment(date).format('MMM DD, YYYY @HH:mm');
    }
    else
        return ''
})

Vue.filter('expiry-date', function(date){
    
    if(date){        
        return moment(date).format('MMM DD, YYYY');        
    }
    else
        return ''
})

Vue.filter('capitalize', function(str: string){
    
    if(str)
        return str.charAt(0).toUpperCase() + (str.slice(1)).toLowerCase();
    else
        return ''
    
})

Vue.filter('capitalizefirst', function(str: string){
    
    if(str)
        return str.charAt(0).toUpperCase() + (str.slice(1));
    else
        return ''
    
})

Vue.filter('truncate', function (text: string, stop: number) {
    if(text)
        return (stop+3 < text.length) ? text.slice(0, stop)+'...' : text
    else
        return ''
})

Vue.filter('truncateleft', function (text: string, stop: number) {
    return (stop+3 < text.length) ? '...'+text.slice(0, stop) : text
})

Vue.filter('convertDate', function(date: string, time: string, type: string, timezone: string){
    const tail="00:00:00.000";
    let result = "";
    if(!time && type=='StartTime') result = date+'T'+tail;
    else if(!time && type=='EndTime') result = date+"T23:59:00.000";
    else if(time.length==1) result = date+'T0'+time+ tail.slice(2);
    else  result = date+'T'+time+ tail.slice(time.length);
    return moment.tz(result, timezone).utc().format();
})

Vue.filter('isDateFullday', function(startDate: string, endDate: string){
    const tail="1900-01-01T00:00:00";
    const start = moment(startDate+ tail.slice(startDate.length)); 
    const end = moment(endDate+ tail.slice(endDate.length));
    const duration = moment.duration(end.diff(start));
    if(duration.asMinutes() < 1439 && duration.asMinutes()> -1439 ){
        return false; 
    }else{
        return true;
    }	 	
})

Vue.filter('rawDateStartEndTimesToUTC', function(date, start, end, timezone){
    const startDate = Vue.filter('convertDate')(date, start, '', timezone);
    const endDate = Vue.filter('convertDate')(date, end, '', timezone);
    return {startDate, endDate}
})

Vue.filter('initArray', function(){
    return Array(96).fill(0)
})

Vue.filter('startEndTimesToArray', function(array, fillNum, date, start, end, timezone){
    const startDate = Vue.filter('convertDate')(date, start, '', timezone);
    const endDate = Vue.filter('convertDate')(date, end, '', timezone);
    const bins = Vue.filter('getTimeRangeBins')(startDate, endDate, timezone)
    if(!array) array = Array(96).fill(0)
    return Vue.filter('fillInArray')(array, fillNum, bins.startBin, bins.endBin)
})

Vue.filter('fillInArray', function(array, fillInNum, startBin, endBin){
    return array.map((arr,index) =>{if(index>=startBin && index<endBin) return fillInNum; else return arr;});
})

Vue.filter('unionArrays', function(arrayA, arrayB){
    return arrayA.map((arr,index) =>{return arr*arrayB[index]});
})

Vue.filter('subtractUnionOfArrays', function(arrayA, arrayB){
    return arrayA.map((arr,index) =>{return arr&&(arrayB[index]>0?0:1)});
})

Vue.filter('sumOfArrayElements', function(arrayA){
    return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
})

Vue.filter('findDiscontinuity', function(array){
    return array.map((arr,index)=>{
        if((index==0 && arr>0)||(arr>0 && array[index-1]==0)) return 1 ;
        else if(index>0 && arr==0 && array[index-1]>0) return 2 ;
        else return 0
    })
})

Vue.filter('getTimeRangeBins', function(startDate, endDate, timezone){
    const startTime = moment(startDate).tz(timezone);
    const endTime = moment(endDate).tz(timezone);
    const startOfDay = moment(startTime).startOf("day");
    const startBin = moment.duration(startTime.diff(startOfDay)).asMinutes()/15;
    const endBin = moment.duration(endTime.diff(startOfDay)).asMinutes()/15;
    return( {startBin: startBin, endBin:endBin } )
})

Vue.filter('convertTimeRangeBinsToTime', function(dutyDateSD, startBin, endBin){            
    const startTime = moment(dutyDateSD).add(startBin*15, 'minutes');
    const endTime = moment(dutyDateSD).add(endBin*15, 'minutes');

    return( {
        startBin: startBin,
        startTime: startTime.format(), 
        start: startTime.format('HH:mm'), 
        endBin: endBin,
        endTime: endTime.format() ,
        end: endTime.format('HH:mm')
    } )
})

Vue.filter('getArrayRangeBins', function(arrayOriginal){
    const array = JSON.parse(JSON.stringify(arrayOriginal));
    const startBin: number = array.findIndex(arr=> arr>0);
    const binValue: number = startBin>=0? array[startBin]: 1;
    return({
        startBin: startBin ,
        endBin: (96-array.reverse().findIndex(arr=> arr>0)),
        binValue: binValue
    })
})

Vue.filter('autoCompleteTime', function(time){
    const tail="00:00";
    let result = "";
    
    if(time.length==1) result = '0'+time+ tail.slice(2);
    else if(time.length==4) {
        if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
        else result = time+(time.slice(-1)=='1' || time.slice(-1)=='4'?'5':'0');
    }
    else if(time.length==5){
        if(time.slice(3,4)=='2') result = time.slice(0,3)+'15';
        else result =time.slice(0,4)+(time.slice(3,4)=='1' || time.slice(3,4)=='4'?'5':'0');
    }
    else  result = time+ tail.slice(time.length);
    return result
})

Vue.filter('getColorByType', function(type: string){
    const dutyColors = [
        {name:'courtroom',  colorCode:'#189fd4'},
        {name:'court',      colorCode:'#189fd4'},
        {name:'jail' ,      colorCode:'#A22BB9'},
        {name:'escort',     colorCode:'#ffb007'},
        {name:'other',      colorCode:'#7a4528'}, 
        {name:'overtime',   colorCode:'#e85a0e'},
        {name:'free',       colorCode:'#e6d9e2'}                        
    ]
    for(const color of dutyColors){
        if(type.toLowerCase().includes(color.name))return color
    }
    return dutyColors[3]
})

Vue.filter('WSColors', function(){
    return {
        'CourtRole':'#189fd4',
        'CourtRoom':'#189fd4',
        'JailRole':'#A22BB9',
        'EscortRun':'#ffb007',
        'OtherAssignment':'#7a4528',
        'Overtime':'#e85a0e',
    }
})

Vue.filter('getTypeAbrv', function(type){
    if(!type) return ''
    const typesAbrv = {
        'CourtRole':'CA',
        'CourtRoom':'CR',
        'JailRole':'J',
        'EscortRun':'T',
        'Transport':'T',
        'OtherAssignment':'O',        
    }
    if(typesAbrv[type]) return '('+typesAbrv[type]+')'
    else return type
})

Vue.filter('subColors', function(subtype){
    const leaveColor = {
        'spl':'#ffee07',
        'lwop':'#6e42c19a',
        'medical':'#fd7e14',
        'med':'#fd7e14',
        'dental':'#fd7e14',
        'illness':'#fd7e14',
        'stiip':'#dc3545',
        'stip':'#dc3545',
        'annual':'#007bff',
        'a/l':'#007bff',
        'cto':'#33b652',
        'bereavement':'#6c757d',
        'bereavemnt':'#6c757d',

        'overtime':'#e82b0e',
        'training':'#b8c6d4',
        'loaned':'#b8c6d4',
        'unavailable':'#CFCFCF'
    }

    for(const key of Object.keys(leaveColor)){        
        if(subtype.toLowerCase().includes(key)) return leaveColor[key]
    }

    return leaveColor['unavailable']
})

Vue.filter('timeFormat', function(value , event) {
    if(isNaN(Number(value.slice(-1))) && value.slice(-1) != ':') return value.slice(0,-1)
    if(value.length!=3 && value.slice(-1) == ':') return value.slice(0,-1);
    if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
    if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==0 || value.slice(-1)==3)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'0';
    if(value.length==2 && event.data && value.slice(0,1)>=3 && (value.slice(-1)==1 || value.slice(-1)==4)) return '0'+ value.slice(0,1)+':'+value.slice(1,2)+'5';
    if(value.length==2 && event.data && value.slice(0,1)==2 && value.slice(-1)>=5) return value.slice(0,-1);
    if(value.length==2 && event.data && value.slice(0,1)==2 && (value.slice(-1)==4 || value.slice(-1)==4)) return '02:45';
    if(value.length==2 && event.data && value.slice(-1)!=0 && value.slice(-1)!=1 && value.slice(-1)!=3 && value.slice(-1)!=4) return value.slice(0,2)+':';
    if(value.length==2 && event.data) return '0'+value.slice(0,1)+':'+value.slice(1,2);
    if(value.length==3 && value.slice(-1)!=':' ) return value.slice(0,2)+':';
    if(value.length==4 && event.data && (value.slice(0,1)>0||value.slice(1,2)>1) && (value.slice(-1)>=5 || value.slice(-1)==2)) return value.slice(0,-1);
    if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==0 || value.slice(-1)==3)) return value.slice(0,4)+'0';
    if(value.length==4 && event.data && value.slice(0,1)>0 && (value.slice(-1)==1 || value.slice(-1)==4)) return value.slice(0,4)+'5';
    if(value.length==4 && event.data && value.slice(0,1)==0 && value.slice(1,2)<2 && value.slice(-1)>=5 ) return value.slice(1,2)+value.slice(3,4)+':';
    if(value.length==5 && (value.slice(0,2)>=24 || value.slice(3,5)>=60)) return '';
    if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==0 || value.slice(3,4)==3)) return value.slice(0,4)+'0';
    if(value.length==5 && value.slice(0,2)>=3 && (value.slice(3,4)==1 || value.slice(3,4)==4)) return value.slice(0,4)+'5';
    if(value.length==6 && value.slice(0,1)==0 && (value.slice(4,6)==0||value.slice(4,6)==15||value.slice(4,6)==30||value.slice(4,6)==45) && (value.slice(1,2)+value.slice(3,4))<24) return value.slice(1,2)+value.slice(3,4)+':'+value.slice(4,5)+value.slice(5,6);

    if(value.length>5) return value.slice(0,5);
    if(value.length==5 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,5)) || value.slice(2,3)!=':') )return '';
    if(value.length==4 && ( isNaN(value.slice(0,2)) || isNaN(value.slice(3,4)) || value.slice(2,3)!=':') )return '';
    return value
})


Vue.filter('printPdf', function(html, pageFooterLeft, pageFooterRight){

    const body = 
        `<!DOCTYPE html>
        <html lang="en">
        <head>		
        <meta charset="UTF-8">
        <title>Court Admin Scheduling</title>`+
        `<style>`+
            `@page {
                size: 11in 8.5in !important;
                margin: 0.2in 0.2in 0.2in 0.2in !important;
                font-size: 10pt !important;			
                @bottom-left {
                    content:`+ pageFooterLeft +
                    `white-space: pre;
                    font-size: 7pt;
                    color: #606060;
                }
                @bottom-right {
                    content:`+pageFooterRight+` "  Page " counter(page) " of " counter(pages);
                    font-size: 7pt;
                    color: #606060;
                }
            }`+
            `@media print{
                div.cas-header {
					position: fixed;
					top: 0in;
                    bottom: 1.5in;
					width:100%; 
					display:inline-block;
				}
                div.cas-body {
                    margin-top: 6rem;
                }                
                .new-page{
                    page-break-before: always;
                    position: relative; top: 8em;
                }
            }`+ 
            `@page label{font-size: 9pt;}            
            `+
            `.card {border: white;}`+
            `.table{border: 3px solid;width: 100%;}`+
            `tr {border: 3px solid;}`+
            `th {border: 3px solid black;}`+
            `table.printer td:has(.middle-text){ vertical-align: middle !important;}`+
            `td {height: 2.5rem;border: 3px solid; width: 6.5rem;}`+
            `td.my-team {height: 2.5rem;border: 3px solid; width: 7rem !important;}`+
            `td.my-notes {height: 2.5rem;border: 3px solid; width: 17rem !important;}`+
            `.bg-spl-leave {background-color: #ffee07;}`+
            `.bg-a-l-leave {background-color: #007bff;}`+
            `.bg-med-dental-leave {background-color: #f8a88a;}`+
            `.bg-stiip-leave {background-color: #ff6439;}`+
            `.bg-cto-leave {background-color: #3ad15e;}`+
            `.bg-lwop-leave {background-color: #7a54c1b0;}`+
            `.bg-bereavement-leave {background-color: #5f6265;}`+
            `.bg-training-leave {background-color: #b8c6d4;}`+
            `.bg-overtime-leave {background-color: #b8c6d4;}`+
            `.bg-loaned {background-color: #b8c6d4;}` +
            `.bg-primary {background-color: #1b4f86;}`+
            `.dot {height: 20px; width: 20px; background-color: black;
                color: white;
                padding: 0.25rem 0.3rem 0.75rem 0.35rem;
                font-size: 0.75rem;
                border-radius: 50%;
                display: inline-block;
            }` +                        
            `
            body{				
                font-family: BCSans;
            }
            `+
        `</style>
        </head>
        <body>            
            <div class="container">
                    `+html+
        `</div></body></html>`	 
    // console.log(body)
    return body
})