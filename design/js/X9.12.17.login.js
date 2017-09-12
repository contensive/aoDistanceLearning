$( document ).ready(function() {

  // ************************
	$('#loginBtn').click(function(e){
	e.preventDefault();
	e.stopPropagation();
	un = $('#username').val();
	pw = $('#password').val();
	jsonDataObject = {};
	jsonDataObject.username = un;
	jsonDataObject.password = pw;
	console.log('processLogin');
	console.log(JSON.stringify(jsonDataObject));
  // $.post('/processLogin', { jsonData : JSON.stringify(jsonDataObject) })
	$.get('processLogin.html', { jsonData : JSON.stringify(jsonDataObject) })
	  .then(function(result){
  		console.log('post result');
  		console.log(result);
  		var json = $.parseJSON(result);
  		console.log(json.data);
  		console.log(json);
  		console.log(json.errors);
  		if(json.errors.length > 0) {
  		  console.log('an error occured');
  		  console.log(json.errors[0].userMsg);
  		  $( ".fieldError" ).html(json.errors[0].userMsg);
  		}
  		else {
  		  console.log('No Errors!');
        window.location.reload();
        // window.location.href = '/Online-Quiz';
  		  // window.location.href = 'Online-Quiz.html';
  		}
	  });
	});

	// ************************

	$('#forgotPasswordLink').click(function(e){
		e.preventDefault();
		e.stopPropagation();
		$('#loginBlock').css("display", "none");
		$('#requestPassBlock').css("display", "block");
	});

	// ************************

	$('#sendEmail').click(function(e){
		e.preventDefault();
		e.stopPropagation();
		var forgotusername = jQuery("#forgotusername").val();
		if (forgotusername){
			$.get('/?remotemethodaddon=processForgotPassword&email='+forgotusername).then(function(result){
				var jsonResult = $.parseJSON(result);
				console.log('jsonResult');
				console.log(jsonResult);
				jQuery("#emailSendMessage").show();
			});
		}
	});

	// ************************

	$('#returntoLoginLink').click(function(e){
	 e.preventDefault();
		e.stopPropagation();
		$('#loginBlock').css("display", "block");
		$('#requestPassBlock').css("display", "none");
	});

	$('#returntoLoginFromRegister').click(function(e){
	 e.preventDefault();
		e.stopPropagation();
		$('#loginBlock').css("display", "block");
		$('#registerBlock').css("display", "none");
	});

	// ************************

  $('#registerLink').click(function(e){
    e.preventDefault();
    e.stopPropagation();
    $('#loginBlock').css("display", "none");
    $('#registerBlock').css("display", "block");
  });

  // ************************



  // ************************ REGISTRATION ***********************
  $('#registerBtn').click(function(e){
    e.preventDefault();
    e.stopPropagation();
    un = $('#username').val();
    pw = $('#password').val();
    fn = $('#firstname').val();
    ln = $('#lastname').val();
    jsonDataObject = {};
    jsonDataObject.username = un;
    jsonDataObject.password = pw;
    jsonDataObject.firstname = fn;
    jsonDataObject.lastname = ln;
    console.log('processLogin');
    console.log(JSON.stringify(jsonDataObject));
    // $.post('/processRegistration', { jsonData : JSON.stringify(jsonDataObject) })
    $.get('processRegistration.html', { jsonData : JSON.stringify(jsonDataObject) })
      .then(function(result) {
        console.log('post result');
        console.log(result);
        var json = $.parseJSON(result);
        console.log(json.data);
        console.log(json);
        console.log(json.errors);
        if(json.errors.length > 0) {
          console.log('an error occured');
          console.log(json.errors[0].userMsg);
          $( ".fieldError" ).html(json.errors[0].userMsg);
        }
        else {
          console.log('No Errors!');
          // window.location.reload();
          // window.location.href = '/Online-Quiz';
          window.location.href = 'ThanksForRegistering.html';
        }
      });
    });

});
