$(document).ready(function (){

         // Create and render the survey using jQuery
    $(document).ready(function () {
        var survey = new Survey.Model(loginJSON);
        

            (async () => {
                
                $.ajax({
                    url:      '/api/users',   // or whatever route you mapped the action to
                    type:     'GET',
                    contentType: 'application/json',
                    data:     {}
                })
                    .done(function (result) {
                        
                        alert(JSON.stringify(result.data))
                    })
                    .fail(function (xhr) {
                        console.error(xhr.responseText);
                        alert("get users failed");
                    });
            })();
        });
})