

$(document).ready(function (){

    var pageAction = window.location.pathname.split("/").pop();

    var pages = {
        users : buildListTemplate("users"),
        user: buildItemTemplate("user"),
        stations: buildListTemplate("stations"),
        station: buildItemTemplate("station"),
        services : buildListTemplate ("services"),
        service: buildItemTemplate ("service")
        
        
    };

   (async () => {
        console.log("Initializing...");
        await actionRunner(pages[pageAction], "init");
   }) ();

    
});



    
  
