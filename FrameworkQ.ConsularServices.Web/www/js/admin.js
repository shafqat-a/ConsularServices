

$(document).ready(function (){

    var pageAction = window.location.pathname.split("/").pop();
    
    var enums = {
        QueueStatus: {
            Empty : 0,
            Away : 1,
            Waiting: 2,
            InProgress: 3,
            Completed: 4
        }
    }

    var pages = {
        users :{
            url: '/users',
            type : 'list',
            actions : {
                init: {
                    url: '/api/users',  
                    type: 'GET',
                    success : function (result, action, subActionName) {
                        var columns = getColumnsFromObject(result[0]);
                        columns.byname['passwordHash'].hide = true;
                        jqCreateTable ("surveyContainer","user_table", result, columns, action.actions.item);        
                    },
                    fail: function (xhr) {
                        alert ("Failed to get users:", xhr.responseText);
                    },
                    custom: null

                },
                root : {
                }, 
                item: [{
                        label: "Edit",
                        callback: function(rowItem) {
                            console.log(rowItem.item.userId.toString());
                            postAction("/user" , {"user_id" : rowItem.item.userId.toString()});
                        }
                    }]
            }  
        },
        user: {
            url: '/user',
            type: "item",
            actions: {
                init: {
                    url: '/api/user',
                    type: 'GET',
                    params: ["user_id"],
                    success : function (result, action, subActionName) {
                        
                        var model =buildModelForType("user");
                        var survey = new Survey.Model(model);
                        survey.data = result;
                        $("#surveyContainer").Survey({model: survey});
                    },
                    fail: function (xhr) {
                        alert ("Failed to get user:", xhr.responseText);
                    },
                    custom: null
                },
                root : {
                },
                item: [{
                        label: "Save",
                        callback: function(rowItem) {
                            console.log("Saving user:", rowItem.item);
                            postAction("/api/user", rowItem.item);
                        }
                    }]
            }
        },
        stations: {
            url: '/stations',
            type: 'list',
            actions: {
                init: {
                    url: '/api/stations',
                    type: 'GET',
                    success : function (result, action, subActionName) {
                        var columns = getColumnsFromObject(result[0]);
                        jqCreateTable ("surveyContainer","station_table", result, columns, action.actions.item);        
                    },
                    fail: function (xhr) {
                        alert ("Failed to get stations:", xhr.responseText);
                    },
                    custom: null
                },
                root : {
                }, 
                item: [{
                        label: "Edit",
                        callback: function(rowItem) {
                            console.log(rowItem.item.stationId.toString());
                            postAction("/station" , {"station_id" : rowItem.item.stationId.toString()});
                        }
                    }]
            }  
        },
        services : buildListTemplate ("services"),
        service: buildItemTemplate ("service")
        
    };

   (async () => {
        console.log("Initializing...");
        await actionRunner(pages[pageAction], "init");
   }) ();

    
});



    
  
