// Abfrage, ob man schon mal die Eignungsprüfung gemacht hat
// falls ja, soll der ganze Bereich "#jaSchonMalBeworben" eingeblendet werden
// ansonsten ist dieser unsichtbar

$(function(){
	$('#jaSchonMalBeworben').hide();

	$("input[name=Pruefung_schon_mal_gemacht]:radio").click(function() {
	    if($(this).attr("value")==="ja") {
	        //alert('schon mal geprüft!');
	        $('#jaSchonMalBeworben').slideDown('fast');
	        
	        // hat man auf "ja" geklickt,
	        // sollten die Input-Felder bei "#jaSchonMalBeworben" auch "required" werden!
	        $('#jaSchonMalBeworben input').attr('required', 'required');
	    }
	    if($(this).attr("value")==="nein") {
	        //alert('noch nicht geprüft!');
	        $('#jaSchonMalBeworben').slideUp('fast');
	        
	        // hat man auf "nein" geklickt
	        // sollten die Input-Felder bei "#jaSchonMalBeworben" nicht mehr "required" sein!
	        $('#jaSchonMalBeworben input').removeAttr('required');
	    }
	});
	
});



