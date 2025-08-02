$(document).ready(function (){


    var actions = [
        {
            label: "Edit",
            callback: function(rowItem) {
                console.log(["Edit action triggered for item:", rowItem]);
            }
        }
    ];

    (async () => {
        
        $.ajax({
            url:      '/api/users',   // or whatever route you mapped the action to
            type:     'GET',
            contentType: 'application/json',
        })
            .done(function (result) {
                
               var columns = getColumnsFromObject(result[0]);
               columns.byname['passwordHash'].hide = true;
               jqCreateTable ("surveyContainer","user_table", result, columns, actions);
            })
            .fail(function (xhr) {
                console.error(xhr.responseText);
                alert("get users failed");
            });
    })();
  
})