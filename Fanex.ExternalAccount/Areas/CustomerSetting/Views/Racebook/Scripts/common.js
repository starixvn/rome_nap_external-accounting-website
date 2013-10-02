IBC = [];
IBC.addEvent = function(el, evname, func) {
    if (el.attachEvent) { // IE
        el.attachEvent("on" + evname, func);
    } else if (el.addEventListener) { // Gecko / W3C
        el.addEventListener(evname, func, true);
    } else { // Opera (or old browsers)
        el["on" + evname] = func;
    }
}

IBC.removeEvent = function(el, evname, func) {
    if (el.detachEvent) { // IE
        el.detachEvent("on" + evname, func);
    } else if (el.removeEventListener) { // Gecko / W3C
        el.removeEventListener(evname, func, true);
    } else { // Opera (or old browsers)
        el["on" + evname] = null;
    }
}

// JScript File
//Check if current page open as the top window(not in any frame) and redirect to the home page
function checkDirectView(){
	if(window==top){
		alert("You are not allowed to view this page directly!");
		top.location.href="index.html";
	}
}

function obj(id) {
    if (document.getElementById(id))
        return document.getElementById(id);
	else return null
}

// check or uncheck all checkbox
// @viet hoang
function checkAll(formName, id){
    try {                    
        var f =document.forms[formName];
        var isChecked =document.getElementById(id).checked;
        cnt = f.elements.length;
        for (var i=0; i <cnt; i++) 
        {
            var e =f.elements[i];
            if (typeof e !='undefined' && e.type =='checkbox') e.checked =isChecked;	  
        }
    }
    catch (ex) {
        alert(ex);
    }                
}

function createFrame(id){
	var fr = documnet.getElementById(id);
	if (!fr && document.createElement){		
	    try {
	        fr = document.createElement("iframe");
		    fr.setAttribute("id", id);
		    fr.setAttribute("name", id);
		    fr.setAttribute("frameborder", 0);
		    fr.setAttribute("border", 0);
		    fr.width = "0px";
		    fr.height = "0px";		
		    document.body.appendChild(fr);
		    if (document.frames) fr =document.frames[id];		    
	    }
	    catch (exception) {
	        var fHTML ='\<iframe id="' +id +'" style="';
	        fHTML +='boder: 0px;';
	        fHTML +='width: 0px;';
	        fHTML +='height: 0px;';
	        fHTML =='"><\/iframe>';
	        var _body =document.getElementsByTagName('body')[0];
	        if (_body) _body.innerHTML += fHTML;
	        fr =new Object();
	        fr.document =new Object();
	        fr.document.location = new Object();
            fr.document.location.iframe = document.getElementById(id);
            fr.document.location.replace = function(location) {
                this.iframe.src = location;
            }
	    }		
	}
	return fr;
}
function dataProcess(url, frame){
	var fr = createFrame(frame);
	if (fr){
		if (frame == "oddsinfo"){
		    var f = self.frames["oddsinfo"];
			f.finished = null;
		}		
		fr.src = url;		
	}
}

//format number
//@PhuongDN
//example
//var nr = 1345652345.67890;
//formatNumber(nr ,2,',','.','','','',' CR')-->1,345,652,345.68
function formatNumber(num,dec,thou,pnt,curr1,curr2,n1,n2) {
    var sign ='';
    if(num <0) sign='-';
    var x =Math.round(num * Math.pow(10,dec));
    if (x >=0) n1=n2='';
    var y =('' +Math.abs(x)).split('');
    var z =y.length -dec;
    y.splice(z, 0, pnt);
    while (z >3) {
        z -=3; 
        y.splice(z,0,thou);
    }
    var r = curr1 +n1 +y.join('') +n2 +curr2;
    if(num >-1 && num <1 && r.indexOf('.') ==0) r ="0" +r;
    return sign +r;
    
}
//format number to form: #,###,###
function formatNumber2(nr){
    return formatNumber(nr,0,',','','','','','');
}
//format number to form: #,###,###.00
function formatNumber32(nr){
    return formatNumber(nr,2,',','.','','','','');
}
//replace all occurrence of a sub string in a string
//@PhuongDN
//Find all occurrence of findStr in str and replace it with replacingStr
function replaceAll(str,findStr,replacingStr){         
    return str.replace(eval('/' +findStr +'/g'), replacingStr);
}

// check number
function isNumericChar(value){
     var check ="0123456789";
     if (check.indexOf(value) <0) return false;     
     return true;     
}

// check number
function checkOdds(value){                
     var check = "-.0123456789";
     if (check.indexOf(value) < 0) return false;     
     return true;
}
//Hanh add
function round(number,X)
{
    X = (!X ? 2 : X);
    return Math.round(number*Math.pow(10,X))/Math.pow(10,X);
}

function ReplaceAll(str,regexp,newstr)
{
	return str.replace(eval('/' +regexp +'/g'), newstr);
}
//trim function for string

String.prototype.ltrim = function() {return this.replace(/^\s+/,"");}
String.prototype.rtrim = function() {return this.replace(/\s+$/,"");}
//check empty input field by id
function isEmpty(id){
    try
    {   
        return obj(id).value.trim()=="";
    }
    catch(e)
    {
        alert('cannot found id: ' + id);
    }
}
//check string input field by id , return true or false
function isString(id)
{
    return isNaN(ReplaceAll(obj(id).value,',',''));
}
//check Numeric input field by id , return value
function isNumeric(id)
{
    return parseFloat(ReplaceAll(obj(id).value,',',''));
}
//check Numeric input field by id, can be null or empty
function isNumber(id){
    var result=false;
    try
    {
        if(obj(id)==null || obj(id).value.trim()==""){
            result=true;
        }else{
            result=!isNaN(obj(id).value);
        }
    }catch(error){
    }
}
//check Numeric input field by id
function isNumberNotNull(id){
    var result=false;
    try
    {
        if(!obj(id).value.trim()=="")
        {
            result=!isNaN(obj(id).value);
        }
    }catch(error){
    }
    return result;
}
//this function is used (in AddNewSuper.aspx & EditSuper.aspx pages) to:
//check keycode is inputed into textbox must be number
function CheckInputNumber(e)
{
    var code = "";
    if(e.keyCode) code=e.keyCode;
    else if(e.which) code=e.which;  				
    if ( (code < 48 || code > 57) && code != 8 && code != 13 && code!=0 && code!=46)
    {		
        return false;
    }
    return true;		
}
//Check for integer
function CheckIntNumber(e)
{
    if (!e) {
        e = window.event;
    }

    var code = (e.keyCode) ? e.keyCode : e.which;
    if (code == 9) {
        return code;
    }

    if ((code < 48 || code > 57) &&
        code != 8 &&
        code != 13 && 
        code !=0)
    {		
        return false;
    }

    return true;		
}
//Check for decimal number
function CheckDecNumber(e) {
   
    var code = "";
    if (e.keyCode) code = e.keyCode;
    else if (e.which) code = e.which;
    if (code == 9) {
        return code;
    }
    if ( (code < 48 || code > 57) && code != 8 && code != 13 && code!=0 && code!=46)
    {		
        return false;
    }
    return true;		
}

function CheckDecNumber_Ne(e){
    var code = "";
    if(e.keyCode) code=e.keyCode;
    else if (e.which) code = e.which;
    if (code == 9) {
        return code;
    }				
    if ( (code < 48 || code > 57) && code != 8 && code != 13 && code!=0 && code!=46 && code!=45)
    {		
        return false;
    }
    return true;		
}

//Check password with min length, having least 2 alpha characters and 2 numeric characters
function validPass(len,pass){
    
    if(pass==null || pass.length<len)
        return false;
    if(pass.match(/.*\d.*\d.*/)==null)
        return false;
    if(pass.match(/.*([a-z]|[A-Z]).*([a-z]|[A-Z]).*/)==null)
        return false;
    return true;
}
//Hanh add for change color of row
var flag=0;
var colorWhite="#ffffff";
var colorGrey="#e8e8e8";
var rowColor="#ffffff";
function ChangeColor()
{
    if(flag==0)
    {
        rowColor=colorWhite;
        flag=1;
    }
    else
    {
        rowColor=colorGrey;
        flag=0;
    }
}
//Check if a string is empty after remove space characrer at front and end
function isEmptyStr(str)
{
    var v;
    try{
        v = (str.trim()=="");
    }catch(error){
        v = false;
    }
    return v;
}

//Check if browser is IE
function isIE(){
    return (navigator.appName.indexOf("Internet Explorer")>-1)
}
//assign css class for clicked left menu item
//menu item must has id 'lm'+index
//len: number of menu item
function selectLeftMenu(itemIndex,len){
    for(var i=0;i<len;++i){
        if(i==itemIndex)
            obj("lm"+i).className="selected";
        else
            obj("lm"+i).className="";
    }
}
//Check if first value is null --> return replacing value; else return first value
function ifNull(val,rep){
    return (val==null)?rep:val;
}
//convert value(if null) to empty string
function null2Blank(val){
    return ifNull(val,"");
}
//Show sucessfull message
function showSuccMsg(id){
    obj(id).style.display='';
    setTimeout("hideSuccMsg('"+id+"');",3000);
}
//hide sucessful message
function hideSuccMsg(id){
    obj(id).style.display='none';
}
// to check Odds value must be number
function CheckOddsNumber(e)
{
    if (!e) e = window.event;
    var code = (e.keyCode) ? e.keyCode : e.which; 				
    if ( (code < 48 || code > 57) && code != 8 && code != 13 && code!=0 && code!=46 && code!=45)
    {		
        return false;
    }    
    return true;		
}

 function writeErrMsg(_errmsg, _spanId){
    var _err =document.getElementById(_spanId);
    if (typeof _err !='undefined') _err.innerHTML ="* " +_errmsg +"!";
}
// -----------------------------------------------------------------------------
// viet add 
//--- String properties------
String.prototype.trimLeft =function(){
  return this.replace(/^\s*/,"");
}

String.prototype.trimRight =function(){
  return this.replace(/\s*$/,"");
}

String.prototype.trim =function(){
  return this.trimRight().trimLeft();
}

String.prototype.isNullOrEmpty =function() {
   return (typeof this =='undefined' || this ==null || this.trim() =='');    
}

//12,234,345.023 || 12464524.34
String.prototype.isNumeric =function() {    
    var regs1 =new RegExp(/^-?\d+(\.\d+)?$/);
    //var regs2= new RegExp(/^-?\d{1,3}(\,\d{3})+(\.\d+)?\s*$/);    
    var regs2= new RegExp(/^-?\d+(\,\d+)+(\.\d+)?\s*$/);    
    return (regs1.test(this) || regs2.test(this));
}

String.prototype.isDateTime = function() { 
    return !(this.isNullOrEmpty() && isNaN(Date.parse(this)));
}

String.prototype.contains =function(item){
	return (this.indexOf(item) >=0);
}

String.prototype.startsWith =function(prefix){
  return (this.substr(0,prefix.length)==prefix);
}

String.prototype.endsWith =function(suffix){
  return (this.substr(this.length -suffix.length)==suffix);
}

String.prototype.compare =function(args, isIgnoreCase){		
	if(isIgnoreCase) {
		return ((new String(this.toLowerCase())==(new String(args)).toLowerCase()) ?0 :1);		
	}
	return ((this ==args) ?0 :1);
}

// --- Number property---
Number.prototype.toDecimals =function(n){
    n =(isNaN(n)) ?2 :n;
    var num =Math.pow(10,n);
    function pad(s) {
        s =s ||'.';
        return (s.length >n) ?s :pad(s +'0');
    }
    return (isNaN(this)) ?this :(new String(Math.round(this*num)/num)).replace(/(\.\d*)?$/,pad);
}


//--- Array properties ---
Array.prototype.clear =function() {
    var len =this.length;
    if (len >0) this.splice(0, len);
}

Array.prototype.isEmpty =function() {
    return (this.length ==0);
}

//Array.prototype.toString =function() {
    //return this.join(this ||'');    
//}

//-- StringBuilder ---
//StringBuilder =function() { 
//	var _parts =new Array();
//	this.append =function(text){
//		if(text) _parts.push(text);
//	}	
//	this.clear =function(){ 
//		return _parts.clear();
//	}		
//	this.toString =function(str){
//		return _parts.toString();
//	}
//}

//--------- Window Popup ------------

function openWindow(_url, _name, _width, _height, _top, _left) { 
    if (typeof _name =='undefined' || !_name) _name ='Accounting';
    if (typeof _width =='undefined' || !_width) _width ='450';
    if (typeof _height =='undefined' || !_height) _height ='280';
    var props ="toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=no,dependent=yes,titlebar=no,directories=no,width=" +_width +",height=" +_height;	
    if (typeof _top != 'undefined' && _top && typeof _left !='undefined' && _left) props +=",top=" +_top +",left=" +_left;             	        
    var newwindow =window.open(_url, _name, props);
    if (window.focus && typeof newwindow !='undefined' && newwindow !=null) newwindow.focus(); return false;
}

function PopUpWindow(url, width, height)
{
    var top=(screen.height-height)/2;
    var left=(screen.width-width)/2;
    var newwindow = window.open(url,"EditSuper","height="+ height +",width="+ width +",top="+ top +",left="+ left +",scrollbars=1");
    if(newwindow) newwindow.focus(); 
}

function exitWindow(hasChanged){    
    if (typeof hasChanged !='undefined' && hasChanged ==1) {
        this.location.reload();
    }
}
//---- return array of elements by ClassName ----
function getElementByClassName(classname) {
    return getElementsByClassName(classname, '', '')
}

function getElementByClassName(classname, node) {
    return getElementsByClassName(classname, node, '')
}

function getElementsByClassName(classname, node, tag){
    if (node.toString().isNullOrEmpty()) node =document.getElementsByTagName('body')[0];    
    if (tag.toString().isNullOrEmpty()) tag ='*';    
    var a =new Array();
    var regs =new RegExp('\\b' +classname +'\\b');
    var els =node.getElementsByTagName(tag);
    if (regs && els) {
		for(var i=0, j=els.length; i<j; i++) {
			if(regs.test(els[i].className)) a.push(els[i]);
		}
    }
    return a;
}
// -----------------------------------------------------------------------------

function check_UnameAndPwd(usertxt, pwdtxt, errmsg) {
    obj(errmsg).innerHTML = '';
    if (obj(usertxt).value == "") {
        obj(errmsg).innerHTML = "*Please enter username";
        obj(usertxt).focus();
        return false;
    } else if (obj(pwdtxt).value == "") {
        obj(errmsg).innerHTML = "*Please enter password";
        obj(pwdtxt).focus();
        return false;
    }

    if (obj('betfrom') && obj('betfrom').value == 'p') 
    {
        var str = obj('men').value + obj('mch').value;
        if (str.length > 0)
            return CheckValidPrivateMsg(errmsg);
        else
            return true;
    }
    else
        return CheckValidPrivateMsg(errmsg);
}

function CheckValidPrivateMsg(errmsg) { 
    if (obj('men') && obj('men').value == "") {
        obj(errmsg).innerHTML = "*You must enter English message";
        obj('men').focus();
        return false;
    }
    else if (obj('men') && obj('men').value.indexOf('@@') == -1) {
        obj(errmsg).innerHTML = "*You must include @@ in  English message for replacing ticket numbers";
        obj('men').focus();
        return false;
    }
    else if (obj('mch') && obj('mch').value == "") {
        obj(errmsg).innerHTML = "*You must enter Chinese message";
        obj('mch').focus();
        return false;
    }
    else if (obj('mch') && obj('mch').value.indexOf('@@') == -1) {
        obj(errmsg).innerHTML = "*You must include @@ in Chinese message for replacing ticket numbers";
        obj('mch').focus();
        return false;
    }
    else if (obj('mcs') && (obj('mcs').value != "") && (obj('mcs').value.indexOf('@@') == -1)) {
        obj(errmsg).innerHTML = "*You must include @@ in Simple Chinese message for replacing ticket numbers";
        obj('mcs').focus();
        return false;
    }
    else if (obj('mth') && (obj('mth').value != "") && (obj('mth').value.indexOf('@@') == -1)) {
        obj(errmsg).innerHTML = "*You must include @@ in Thai message for replacing ticket numbers";
        obj('mth').focus();
        return false;
    }
    else if (obj('mit') && (obj('mit').value != "") && (obj('mit').value.indexOf('@@') == -1)) {
        obj(errmsg).innerHTML = "*You must include @@ in Italian message for replacing ticket numbers";
        obj('mit').focus();
        return false;
    }
    else if (obj('mjp') && (obj('mjp').value != "") && (obj('mjp').value.indexOf('@@') == -1)) {
        obj(errmsg).innerHTML = "*You must include @@ in Japanese message for replacing ticket numbers";
        obj('mjp').focus();
        return false;
    }
    return true;
}

function alert_error(errmsg,errHolderId)
{    
   var intValue;
    try
    {
        errHolderId = (errHolderId) ? errHolderId:'errMsg';
        if(errmsg.length > 0)
        {
            if (document.getElementById('err'))
            {
                document.getElementById('err').innerHTML="";
            }
            if (document.getElementById('errMsg'))
            {
                document.getElementById('errMsg').innerHTML="";
            }
           
            document.getElementById(errHolderId).innerHTML = errmsg;
            
            if(errHolderId =="errfunc" && errmsg != '')
            {
              intValue = window.setTimeout("alert_error('','errfunc')",5000);
            }            
        }
        else 
        {
            document.getElementById(errHolderId).innerHTML = '';
        }
    }
    catch(e){alert('Cannot found id :' + errHolderId + '\nerrMsg: ' + errmsg);}
}
function open_report(link)
{
    var fdate=document.getElementById('fdate').value;
    var tdate=document.getElementById('tdate').value;
    var linkHref=link.href;
    if(linkHref.indexOf('?')>-1)
        top.centerFr.location.href=linkHref+"&fdate="+fdate+"&tdate="+tdate;
    else
        top.centerFr.location.href=linkHref+"?fdate="+fdate+"&tdate="+tdate;
}

function open_report2(link)
{
    var fdate=document.getElementById('fdate').value;
    var tdate=document.getElementById('tdate').value;
    var linkHref=link.href;
    if(linkHref.indexOf('?')>-1)
    {
        var url =linkHref+"&fdate="+fdate+"&tdate="+tdate;
        window.open(url, '_blank');
     }   
    else
    {
        var url =linkHref+"?fdate="+fdate+"&tdate="+tdate;
        window.open(url, '_blank');
    }
}

function open_dailyreport(link)
{
    if (parent.location.pathname.indexOf("Default.aspx") == -1 && parent.location.pathname.indexOf("index.aspx")) {
        if (parent.frames.length > 1) parent.window.location.href = link;
    }
    else {
        window.location.href = link;
    }
}

//this function is used to display span error for Adding, Editing of Customer
//when error of each Sporttype is shown, panel of each Sporttype is shown too
var arrPlaceHolder=["err","error1","error2","error3","error5","error8","error10","error11","error201","error99","error151", "error161", "error99_9"];
var arrPanel=["","panelSoccer","panelBasket","panelFootball","panelTennis","panelBaseball","panelGolf","panelMoto","panelFinancial","panelOtherSports","panelHorseRacing", "panelNumberGame", "panelMixedSports"];
function ShowError(errMsg,placeHolder)
{
	flag=false; 	
	window.scroll(0,0); 
	parent.scroll(0,0); 	
	var isEdit=(obj("hidEdit")!=null?true:false);
	
	for(var i=0;i<arrPlaceHolder.length;i++)
	{	    
		if(placeHolder==arrPlaceHolder[i])
		{	
			alert_error(errMsg,placeHolder);
			if (i==0) continue;
			obj(arrPanel[i]).style.display="";
		}
		else
		{	
		    //@phuong.nguyen
		    //object is null or not
		    if(null != obj(arrPlaceHolder[i]))
			    obj(arrPlaceHolder[i]).innerHTML="";
		}
	}
	if(obj("btnSubmit"))
	{
	    obj("btnSubmit").style.color="#ffffff";
		obj("btnSubmit").disabled=false;
	}
	else if(obj("btnAdd"))
	{
	    obj("btnAdd").style.color="#ffffff";
		obj("btnAdd").disabled=false;
	}
}

//Khoa add for when user click again on button Add or Submit, all old messages need to clear
function ClearAllMsg()
{
    for(var i=0;i<arrPlaceHolder.length;i++)
	{	
	    if(null != obj(arrPlaceHolder[i]))					
		    obj(arrPlaceHolder[i]).innerHTML="";
	}
}

// Hanh add for Increase or Reduce PositionTaking
//ddl: id of Positiontaking dropdown
//step: value of each step (1 or 10)
//max: maximum value that dropdown can increase to
function Increase(ddl,step,max)
{
    var objDDL=obj(ddl);
    var maxValue=Math.round(max*100);
    var currSelect=Math.round(parseFloat(objDDL.value)*100);
    var nextValue=currSelect+step;                        
    if(nextValue<=maxValue)
    {
        objDDL.value=nextValue/100.0;
    }
}

// Hanh add for Increase or Reduce PositionTaking
//ddl: id of Positiontaking dropdown
//step: value of each step (1 or 10)
//min: minimum value that dropdown can reduce to
function Reduce(ddl,step,min)
{
    var objDDL=obj(ddl);
    var minValue=Math.round(min*100);
    var currSelect=Math.round(parseFloat(objDDL.value)*100);
    var nextValue=currSelect-step;                          
    if(nextValue>=minValue)
    {
        objDDL.value=nextValue/100.0;
    }
}

//this function is used to disable all checkboxes of Weekly in Adding case
function selectTranferTime()
{		
    try {
	    var elements= obj("alldays").getElementsByTagName("input");						
	    for(var i=1; i<elements.length; i++)
	        elements[i].disabled=obj("rdDaily").checked;
	    if (obj("rdDaily").checked) {
	        var o=obj("errTransfer");
	        if (o) o.innerHTML = "";
	    }
	}
	catch (ex) {}
}	
//this function is used to check case editing transfer time
//When Weekly radio is selected and don't have any checkbox of days is checked
function CheckTime() {
    var strConfirm = true;
	if(typeof(lblConfirm)!='undefined')	
	{	    
		strConfirm=confirm(lblConfirm);
    }
	if(strConfirm)
	{
        var chkCount=0;
        var isSelect=false;
        if(obj("rdWeekly").checked)
        {
	        var elements= obj("alldays").getElementsByTagName("input");						
	        for(var i=1;i<elements.length;i++)
	        {
		        if(elements[i].checked)
		        {
			        isSelect=true;				
			        chkCount ++;				
		        }
	        }	
	        if (!isSelect)
	        {
	            showAlertMessage("err",lblChooseOneDay,1);
		        scroll(0,0);
		        return false;
	        }		
        }
        return true;	
    }
    return false;
}
//this function is used to check case editing transfer time
//When Weekly radio is selected and all checkbox of 7 days can not be checked
function ChooseDate()
{    
    var chkCount=0;
    if(obj("rdWeekly").checked)
    {
	    var elements= obj("alldays").getElementsByTagName("input");		
	    if(obj("err")!=null && obj("err").innerHTML!="")
	    {	
		    obj("err").innerHTML="";			
	    }
	    for(var i=1;i<elements.length;i++)
	    {
		    if(elements[i].checked)
		    {
			    isSelect=true;				
			    chkCount ++;				
			    if (chkCount==7)
			    {
			        showAlertMessage("err",lblChooseDaily,1);
				    return false;									
			    }
		    }
	    }				
    }
    return true;	
}

//check Numeric input field by id , return value
function isNumericSpan(id)
{
    return parseFloat(ReplaceAll(obj(id).innerHTML,',',''));
}

function OnSelectExRate(objDLL)
{
    var index=objDLL.selectedIndex;            
    var selVal= arrExRate[index];   
    obj("hidExRate").value=selVal;            
}

//this function to reload page when update position taking successful.
function doReload(msg) {
    setTimeout("reloadCallBack('" + msg + "');", 2000 );
}
function reloadCallBack(msg, ispopup) {
    PTUnBlockUI();
    var url = self.location.href.replace(/#/gi, "");
    if (url.indexOf("&msg") == -1) url = url + '&errorcode=0' + '&msg=' + msg;
    self.location.href = url;
    window.scroll(0, 0);
}

//this function to go to page in paging
function jumpto(url) {
 
    var page_menu = obj('page_menu');
    if(page_menu.style.display == 'none' && url !='') 
    {
        page_menu.style.left = String(xMousePos) + 'px';
        page_menu.style.position = 'absolute';
        page_menu.style.display = 'block';
    }   
    else
    {
        page_menu.style.display = 'none';
        }
}

//this function is used in paging
function gotopage(url)
{              
    var page=obj("txtpage").value;            
    if(page=="")
    {
        return false;      
    }
    else
    {                
        url += 'page=' + page;
        window.location.href = url;
    }
}
//this function is used when Enter key is pressed "Go to page" textbox
function KeyPress(e)
{
    if (!e) e = window.event;
    var code = (e.keyCode) ? e.keyCode : e.which; 	
    if(code == 13)
    {
        obj("pagenav_ibtn").click();                
    }
}   


//------------------------------------------
function DisableHR(idchkHR)
{
    if(obj(idchkHR).checked)
    {
        obj('hddisableHR').value=0;
    }
    else
    {
        obj('hddisableHR').value=1;
    }
}
function DisableHRByCurr(sel) {
    
    var CurrAllow="0";
    var currvalue="0";               
    if(sel.value != "0")
    {          
        CurrAllow=sel.value.split('_')[1];
        currvalue=sel.value.split('_')[0];       
    }
    obj("hdCurr").value=currvalue;
    obj("hdAllowCurr").value=CurrAllow;
}

//do not allow enter text on numberic field
function AllowNumber(e)
{
    var code = "";
    if(e.which)
        code=e.which;
    else if(e.keyCode)
        code=e.keyCode;            
   	if(code == 37 || code == 39)
   	    return true;
    if ( (code < 48 || code > 57) && code != 8 && code != 13 && code!=0 && code!=46 && code!=9)
    {		
        return false;
    }
    if(code ==13 )
    {
        if (typeof(UpdatePT) == "undefined")
        {
              BtnOnClick();
        }
        else
        {
            UpdatePT();
        }
    }
    return true;		
}

function getRealTop(el) {
    try {
        if (!$(el)) return 0;
        var yPos = document.getElementById(el).offsetTop;
        var tempEl = document.getElementById(el).offsetParent;
        while (tempEl != null) {
            yPos += tempEl.offsetTop;
            tempEl = tempEl.offsetParent;
        }
    }
    catch (e)
                { }

    return yPos;
}
function getRealLeft(el) {
    try {
        if (!$(el)) return 0;
        var xPos = document.getElementById(el).offsetLeft;
        var tempEl = document.getElementById(el).offsetParent;
        while (tempEl != null) {
            xPos += tempEl.offsetLeft;
            tempEl = tempEl.offsetParent;
        }
    }
    catch (e)
                { }

    return xPos;
}

function getPramJs(name) {
    var q = document.location.search;
    var i = q.indexOf(name + '=');

    if (i == -1) {
        return false;
    }

    var r = q.substr(i + name.length + 1, q.length - i - name.length - 1);

    i = r.indexOf('&');

    if (i != -1) {
        r = r.substr(0, i);
    }

    return r.replace(/\+/g, ' ');
}




function hideLanguage(obj) {
    if (String(obj.innerHTML).indexOf("-") != -1) {
        //current opened
        document.getElementById("otherLanguage_title").style.display = "none";
        document.getElementById("otherLanguage_content").style.display = "none";
        obj.innerHTML = "<span style=\"color: Black;\">Other Language</span> [+]";
    }
    else {
        //current closed
        document.getElementById("otherLanguage_title").style.display = "";
        document.getElementById("otherLanguage_content").style.display = "";
        obj.innerHTML = "<span style=\"color: Black;\">Other Language</span> [-]";
    }
}

function gup(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}

function DelayPage1( time, path, rootPath)
{
    overlay('disableControl');
    var delay = time?time:1500;
    setTimeout("window.divMaskLoading1.style.display = 'none'", delay);
}

function DelayReloadPage1(url, time, path, rootPath)
{
    
    var delay = time?time:1500;
    
    if(!url)
        setTimeout("location.reload(false)", delay);
    else
    {      
        if(!path)
        {
            setTimeout("location.href = '" + url + "'", delay);
        }
        else
        {
            setTimeout(path + "= '" + url + "'", delay);
        }
    }
    if (opener) {
        close();
    }
}

function GetWidthBrowser() {
    var x = 0;
    if (self.innerHeight) {
        x = self.innerWidth;
    }

    else if (document.documentElement && document.documentElement.clientHeight) {
        x = document.documentElement.clientWidth;
    }
    else if (document.body) {
        x = document.body.clientWidth;
    }
    return x;
}


function GetHeightBrowser() {
    var y = 0;
    if (self.innerHeight) {
        y = self.innerHeight;
    }
    else if (document.documentElement && document.documentElement.clientHeight) {
        y = document.documentElement.clientHeight;
    }
    else if (document.body) {
        y = document.body.clientHeight;
    }
    return y;
}

function ParseToBoolean(value) {
    if (value == true) {
        return true;
    }
    else if (value == false)
        return false;
    else if (isNaN(value)) {
    if (value.toLowerCase() == "true")
            return true;
        else
            return false;
    }
    else {
        var intValue = parseInt(value);
        if (intValue == 1)
            return true;
        else
            return false;
    }

}

function overlay(id) {
    el = document.getElementById(id);
    el.style.display = (el.style.display == "inline") ? "none" : "inline";
    //alert(el.style.display);
}

// Success=0, Error=1, Warning=2
function showAlertMessage(containerid, msg, type) {
   
    switch(type)
    {
        case 0:
            document.getElementById(containerid).innerHTML = "<div class='alertboxscc'><div>" + msg + "</div></div>";
          break;
        case 1:
            document.getElementById(containerid).innerHTML = "<div class='alertboxerr'><div>" + msg + "</div></div>";
          break;
        case 2:
            document.getElementById(containerid).innerHTML = "<div class='alertboxwrn'><div>" + msg + "</div></div>"; ;
          break;    
        default:
          break
    }
    window.scroll(0, 0);
    parent.scroll(0, 0); 	
}

function rowover(obj) {
        $(".tbllayout .overrow").removeClass("overrow");
        $(obj).addClass("overrow");
}
function rowout() {
    $(".tbllayout .overrow").removeClass("overrow");
}

function DelayReloadPage(url, time, path, rootPath) {
    var delay = time ? time : 1500;
    if (!url)
    {
        PTUnBlockUI();
        setTimeout("ReloadCallback()", delay);
    }
    else 
    {
      
        if (name=='fContent')
        {
            setTimeout("PTUnBlockUI();location.href = '" + url + "'", delay);
        }
        else
        {
            if (parent.name=="centerFr")
                {
                    setTimeout("PTUnBlockUI();parent.location.href = '" + url + "'", delay);
                }
                else
                {
                    setTimeout("PTUnBlockUI();location.href = '" + url + "'", delay);
                }
         }
      
    }
    if (typeof afterUpdate != 'undefined') afterUpdate(false, 2000);   
}


function ReloadCallback() {
    PTUnBlockUI();
    location.href=location.href;
}

function PTBlockUI() {

    if (parent.BlockUI) {
        try{parent.BlockUI();}catch(e){}
    }
    //Called inside a frame set (#centerFr)
    else {
        if (parent.parent.BlockUI) {
            parent.parent.BlockUI();
        }
    }
    //Called from popup
    if (opener) {
        if (opener.BlockUI) {
            opener.BlockUI();
        }
        opener.parent.BlockUI();
    }
}
function PTUnBlockUI() {
    if (parent.jQuery && parent.jQuery.unblockUI) {
        parent.jQuery.unblockUI();
    }
    //Called inside a frame set (#centerFr)
    if (parent.parent.jQuery && parent.parent.jQuery.unblockUI) {
            parent.parent.jQuery.unblockUI();
        }
        //Called from popup
        
    if (opener) {
        if (opener.parent.jQuery.unblockUI) {
            opener.parent.jQuery.unblockUI();
        }
        //parent.jQuery.unblockUI();
    }
}

function getElementsByClassName(classname, node) {
    if (!node) node = document.getElementsByTagName("body")[0];
    var a = [];
    var re = new RegExp('\\b' + classname + '\\b');
    var els = node.getElementsByTagName("*");
    for (var i = 0, j = els.length; i < j; i++)
        if (re.test(els[i].className)) a.push(els[i]);
    return a;
}

function MenuItemClick(divControl, divpanel) {
    var tdid = divControl.replace(/div/, "td");
    if ($("#" + divpanel).is(":hidden")) {
        $("#" + tdid).removeClass("md3");
        $("#" + tdid).addClass("md2");
        $("#" + divpanel).removeClass("hidden");
        $("#" + divpanel).addClass("show");
        
    } else {
        $("#" + tdid).removeClass("md2");
        $("#" + tdid).addClass("md3");
        $("#" + divpanel).removeClass("show");
        $("#" + divpanel).addClass("hidden");
    }
   
    //$("#" + divpanel).toggle();
}


function showEnrichMsg(msg) {
    $('#err').html(msg);
}


//regexp-validate-numbers
function uInteger(str) {
    /* Verify unsigned integer
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[0-9]+$/.test(str);
}//eof - uInteger

function uInteger2(str) {
    /* Verify unsigned integer
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^\d+$/.test(str);
}//eof uInteger2

function uInteger3to5(str) {
    /* Verify unsigned integer 3 to 5 digits
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^\d{3,5}$/.test(str);
}//eof - uInteger3to5

function sInteger(str) {
    /* Verify signed integer at least one digit
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?[0-9]+$/.test(str);
}//eof - sInteger

function sInteger2(str) {
    /* Verify signed integer at least one digit
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?\d+$/.test(str);
}//eof - sInteger2

function sInteger3to5(str) {
    /* Verify signed integer 3 to 5 digits
     *        ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?\d{3,5}$/.test(str);
}//eof - sInteger3to5

function isFloat(str) {
    /* Verify signed float decimal optional
     *    ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?[0-9]+(\.[0-9]+)?$/.test(str);
}//eof isFloat

function isFloat2(str) {
    /* Verify signed float decimal optional
     *    ignoring leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?\d+(\.\d+)?$/.test(str);
}//eof isFloat2

function isFloat3to5d0to3(str) {
    /* Verify float 3 to 5 digits, decimal max 3 digits
     *    if decimal point must have at least 1 decimal digit
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[-+]?\d{3,5}(\.\d{1,3})?$/.test(str);
}//eof - isFloat3to5d10to3

function isCurrency1(str) {
    /* Verify formatted currency optional $, no leading zero
     *    reguire comma separator, 2 digit decimal if any
     *    ignore leading and trailing spaces
     * Return boolean
     */
    str = str.replace(/^\s+|\s+$/g, '');
    return /^\$?[1-9][0-9]{0,2}(,[0-9]{3})*(\.[0-9]{2})?$/.test(str);
}//eof - isCurrency1

function isCurrency2(str) {
    /*  Verify formatted currency optional $, no leading zero
     *    reguire comma separator, 2 digit decimal if any
     *    fillin missing decimal
     *    ignore leading and trailing spaces
     * Return string
     */
    str = str.replace(/^\s+|\s+$/g, '');
    if (/^\$?[1-9][0-9]{0,2}(,[0-9]{3})*(\.[0-9]{0,2})?$/.test(str) ) {

        if (/\.[0-9]$/.test(str) ) {
            //needs trailing zero
            str += "0";
        }
        else if (/\.$/.test(str)) {
            //needs both trailing zeroes
            str += "00";
        }
        else if (!/\.[0-9]{2}$/.test(str) ) {
            //needs everything
            str += ".00";
        }
        return str;
    }
    else {
        return "Invalid";
    }
}//eof - isCurrency2

function isCurrency3(str) {
    /* Verify formatted currency optional $, no leading zero
     *    reguire comma separator, 2 digit decimal if any
     *    fillin missing decimal
     *    ignore leading and trailing spaces
     * Return string
     */
    str = str.replace(/^\s+|\s+$/g, '');

    if (/^[-+]?\$?[1-9]\d{0,2}(,\d{3})*(\.\d{0,2})?$/.test(str) ) {

        if (/\.\d$/.test(str)) {
            str += "0";
        }
        else if (/\.$/.test(str) ) {
            str += "00";
        }
        else if (!/\.\d{2}$/.test(str) ) {
            str += ".00";
        }
        return str;
    }
    else {
        return "Invalid";
    }
}//eof - isCurrency3


function trim(str, chars) {
	return ltrim(rtrim(str, chars), chars);
}
 
function ltrim(str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}
 
function rtrim(str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}

function convertUnitToLocalDatetime(objDatime) {
    var okDate = /Date\(([-+]?\d+[-+]?\d+)\)/.exec(objDatime);
    var date;
    if (okDate) {
        var when = new Date(eval(okDate[1]));
        date = when;
    } else {
        var currdate = new Date();
        date = currdate;
    }
    return date;
}

///// <reference path="stdlib.js" />
function formatDate(objDatime, strFormat) {
    objDatime.format(strFormat);
}

function convertUnitToLocalDatetime(objDatime, strFormat) {
    var okDate = /Date\(([-+]?\d+[-+]?\d+)\)/.exec(objDatime);
    var date;
    if (okDate) {
        var when = new Date(eval(okDate[1]));
        date = when;
    } else {
        var currdate = new Date();
        date = currdate;
    }
    return date.format(strFormat);
}

function SiteRendering(selectControlId, rootPath) {
    $.ajax({
        type: "POST",
        url: rootPath + "Services/WCF/HelperService.svc/GetSiteList",
        data: "",
        contentType: "application/json; charset=utf-8",
        success: function (objSite) {
            var objSelectControl = $("#" + selectControlId);
            objSelectControl.append("<option>Select</option>");
            var totalSiteDTO = objSite.length;
            for (var i = 0; i < totalSiteDTO; i++) {
                if (objSite[i].Name == "Licensee" && enableDeposit) continue;
                var siteDTO = objSite[i];
                if (siteDTO.Sites != null && siteDTO.Sites.length > 0) {
                    var totalSite = siteDTO.Sites.length;
                    if (totalSite == 1) {
                        objSelectControl.append("<option>" + siteDTO.Sites[0].Name + "</option>")
                    }
                    else {
                        var optgroup = "";
                        if (!enableDeposit) {
                            optgroup = "<optgroup label=" + siteDTO.Name + ">";
                        }
                        for (var j = 0; j < totalSite; j++) {
                            optgroup += "<option>" + siteDTO.Sites[j].Name + "</option>";
                        }
                        if (!enableDeposit) {
                            optgroup += "</optgroup>";
                        }
                        objSelectControl.append(optgroup);
                    }
                }

            }
        },
        error: Error
    });
}

function SiteRenderingCreditAndDeposit(selectControlId, rootPath) {
    $.ajax({
        type: "POST",
        url: rootPath + "Services/WCF/HelperService.svc/GetSiteList",
        data: "",
        contentType: "application/json; charset=utf-8",
        success: function (objSite) {
            var objSelectControl = $("#" + selectControlId);
            objSelectControl.append("<option>Select</option>");
            var totalSiteDTO = objSite.length;
            for (var i = 0; i < totalSiteDTO; i++) {
                var siteDTO = objSite[i];
                if (siteDTO.Sites != null && siteDTO.Sites.length > 0) {
                    var totalSite = siteDTO.Sites.length;
                    if (totalSite == 1) {
                        objSelectControl.append("<option>" + siteDTO.Sites[0].Name + "</option>")
                    }
                    else {
                        var optgroup = "<optgroup label=" + siteDTO.Name + ">";
                        for (var j = 0; j < totalSite; j++) {
                            optgroup += "<option>" + siteDTO.Sites[j].Name + "</option>";

                        }
                        optgroup += "</optgroup>";
                        objSelectControl.append(optgroup);
                    }
                }

            }
        },
        error: Error
    });
}

//Thai @20130528
function countChar(txtarea, l, charNum) {
    var len = $(txtarea).val().length;
    if (len > l) $(txtarea).val($(txtarea).val().slice(0, l));
    else $(charNum).text("Characters Left:" + (l - len));
}