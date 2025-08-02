$(document).ready(function (){
    
    

})

function jqCreateTable ( parent_div_id,  table_id, data, columns, actions){
    console.log(columns);

    var table = $("<table></table>").attr("id", table_id);
    var thead = $("<thead></thead>");
    var tbody = $("<tbody></tbody>");

    // Create table header
    var headerRow = $("<tr></tr>");
    columns.columns.forEach(function(column) {
         if (column['hide']!=null && column['hide'] == true) {} 
         else {
            headerRow.append($("<th></th>").text(column.name));
         }
    });
    
    if (actions != null) {
        var actionHeader = $("<th></th>").text("Actions");
        headerRow.append(actionHeader);
    }


    thead.append(headerRow);

    // Create table body
    data.forEach(function(item) {
        var row = $("<tr></tr>");
        columns.columns.forEach(function(column) {
            if (column['hide']!=null && column['hide'] == true) {}
            else {
               row.append($("<td></td>").text(item[column.name]));
               tbody.append(row);
            };
        });
        if (actions != null) {
            var actionCell = $("<td></td>");
            actions.forEach(function(action) {
                var actionButton = $("<button></button>")
                    .text(action.label)
                    .addClass("btn")
                    .on("click", function() {
                        var rowItem = { "item": item  };
                        action.callback(rowItem);
                    });
                actionCell.append(actionButton);
            });
            row.append(actionCell);
        }
        tbody.append(row);
    });
    table.append(thead).append(tbody);
    $("#" + parent_div_id).append(table);
}

function getColumnsFromObject(objectRow) {
    

    const retobject = {
        columns: [],
        byname: {}
    };

    var columns = Object.keys(objectRow);
   
    columns.forEach ( function(col)  {
        const colx = { name: col , hide: false };
        retobject.columns.push(colx);
        retobject.byname[col] = colx;
        
    });
    
    return retobject;
}