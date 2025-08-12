$(document).ready(function (){
    
    

})

async function actionRunner (action, subActionName){     
    
    console.log("Running with action:", action); 
    var subAction = action.actions[subActionName];
    if (action.type === "list") {
        if (subAction.custom == null) {
            $.ajax({
                    url: subAction.url,
                    type: subAction.type,
                    contentType: 'application/json',
                })
                .done(function (result) {
                    return subAction.success(result, action, subActionName);
                })
                .fail(function (xhr) {
                    return subAction.fail(xhr, action, subActionName);
                });
        } else {
            return subAction.custom(action, subActionName);
        } 
    } else if  (action.type == "item") {
        alert ("item");
        if (subAction.custom == null) {
            var params = {};
            subAction.params.forEach(element => {
                params[element] = $("#" + element).val();
            });
            console.log("Item params:", params);
            var urlToCall = subAction.url;
            if (subAction.type === "GET") {
                urlToCall += "?" + buildQueryString(params);
            }
            $.ajax({
                    url: urlToCall,
                    type: subAction.type,
                    contentType: 'application/json',
                })
                .done(function (result) {
                    return subAction.success(result, action, subActionName);
                })
                .fail(function (xhr) {
                    return subAction.fail(xhr, action, subActionName);
                });
        } else {
            return subAction.custom(action, subActionName);
        } 
    }
};

function camelToProper(camelCaseStr) {
  if (!camelCaseStr) return "";

  return camelCaseStr
    // Insert space before capital letters
    .replace(/([a-z])([A-Z])/g, '$1 $2')
    // Capitalize each word
    .replace(/\b\w/g, char => char.toUpperCase());
}


function makeChoicesFromEnum(enumObject) {
    return Object.entries(enumObject).map(([key, value]) => ({
        value,
        text: camelToProper(key)
    }));
}


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
            headerRow.append($("<th></th>").text(camelToProper(column.name)));
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
            console.log (actions);
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

function postAction(actionUrl, data) {
  // Create the form element
  const form = document.createElement("form");
  form.method = "POST";
  form.action = actionUrl;

  // Add each key-value pair from the data object as a hidden input
  for (const [key, value] of Object.entries(data)) {
    const input = document.createElement("input");
    input.type = "hidden";
    input.name = key;
    input.value = value;
    form.appendChild(input);
  }

  // Append the form to the body and submit it
  document.body.appendChild(form);
  form.submit();
}

function buildQueryString(params) {
  const queryString = Object.entries(params)
    .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
    .join("&");
  return queryString;
}

function objectToKeyValueArray(obj) {
  return Object.entries(obj).map(([key, value]) => ({ key, value }));
}

function buildModelForType(objectType) {
    var obj = {
    }
    switch (objectType) {
        case "user":
            obj.title = "User";
            obj.elements = [
                { type: "text", name: "email", title: "Please type in your email", isRequired: true },
                { type: "text", name: "password", title: "Please enter your password", isRequired: true, inputType: "password" },
                { type: "text", name: "userId", visible: false },
                { type: "text", name: "passwordHash", visible: false },
            ]; 
            break;
        // Add 
        // more cases for other object types as needed
        case "station":
            obj.title = "Station";
            obj.elements = [
                { type: "text", name: "queueId", title: "Station ID", isRequired: true },
                { type: "text", name: "queueName", title: "Station Name", isRequired: true },
                { type: "text", name: "queueStatus", title: "Status", isRequired: true },
            ];
            break;
    }
    return obj
}

function camelCase(strdata) {
  return strdata.toLowerCase().replace(/_([a-z])/g, function (match, letter) {
    return letter.toUpperCase();
  });
}


function buildItemTemplate (actionName) {

    var templateString =  {
            url: '/$action',
            type: "item",
            actions: {
                init: {
                    url: '/api/$action',
                    type: 'GET',
                    params: ["$action_id"],
                    success : function (result, action, subActionName) {
                        
                        var model =buildModelForType("$action");
                        var survey = new Survey.Model(model);
                        survey.data = result;
                        $("#surveyContainer").Survey({model: survey});
                    },
                    fail: function (xhr) {
                        alert ("Failed to get $action:", xhr.responseText);
                    },
                    custom: null
                },
                root : {
                },
                item: [{
                        label: "Save",
                        callback: function(rowItem) {
                            console.log("Saving $action:", rowItem.item);
                            postAction("/api/$action", rowItem.item);
                        }
                    }]
            }
        };
    return buildTemplate( actionName, templateString);

}

function buildListTemplate (actionName) {
    
    var templateString =  {
        "$actions" : {
            url: '/$actions',
            type : 'list',
            actions : {
                init: {
                    url: '/api/$actions',  
                    type: 'GET',
                    success : function (result, action, subActionName) {
                        var columns = getColumnsFromObject(result[0]);
                        jqCreateTable ("surveyContainer","$action_table", result, columns, action.actions.item);        
                    },
                    fail: function (xhr) {
                        alert ("Failed to get $actions:", xhr.responseText);
                    },
                    custom: null

                },
                root : {
                }, 
                item: [{
                        label: "Edit",
                        callback: function(rowItem) {
                            postAction("/$action" , {"$action_id" : rowItem.item[camelCase("$action_id")].toString()});
                        }
                    }]
            }  
        }
    };

    return buildTemplate( actionName, templateString);
}

function buildTemplate (actionName, templateString) {

    // if actionName ends with "s" then remove the last character
    if (actionName.endsWith("s")) {
        actionName = actionName.slice(0, -1);
    }

    var actionTemplateString = stringifyWithFn(templateString);
    // replace actionTemplateString all $action with actionName 
    actionTemplateString = actionTemplateString.replace(/\$action/g, actionName);
    actionInstance = parseWithFn(actionTemplateString);

    console.log("Action instance for " + actionName + ":", actionInstance);
    return actionInstance[actionName + "s"];

}

function stringifyWithFn(obj) {
  return JSON.stringify(obj, function (key, value) {
    if (typeof value === "function") {
      return `__FUNC__${value.toString()}`; // Mark functions clearly
    }
    return value;
  });
}

function parseWithFn(jsonStr) {
  return JSON.parse(jsonStr, function (key, value) {
    if (typeof value === "string" && value.startsWith("__FUNC__")) {
      const funcStr = value.slice(8); // Remove the "__FUNC__" marker
      try {
        return new Function(`return (${funcStr})`)(); // Restore function
      } catch (e) {
        console.warn("Failed to restore function:", funcStr);
        return value;
      }
    }
    return value;
  });
}
