$(document).ready(function (){
    
    

})

function jqCreateTable ( parent_div_id,  table_id, data, columns){
    var table = $("<table></table>").attr("id", table_id);
    var thead = $("<thead></thead>");
    var tbody = $("<tbody></tbody>");

    // Create table header
    var headerRow = $("<tr></tr>");
    columns.forEach(function(column) {
        headerRow.append($("<th></th>").text(column));
    });
    thead.append(headerRow);

    // Create table body
    data.forEach(function(item) {
        var row = $("<tr></tr>");
        columns.forEach(function(column) {
            row.append($("<td></td>").text(item[column]));
        });
        tbody.append(row);
    });

    table.append(thead).append(tbody);
    $("#" + parent_div_id).append(table);
}