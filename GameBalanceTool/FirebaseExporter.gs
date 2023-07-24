/**
* Copyright 2019 Google LLC.
* SPDX-License-Identifier: Apache-2.0
*/

function doGet(e)
{
    return downloadGameConfig();
}

function downloadGameConfig() {
    var sheetID = getEnvironment().spreadsheetID;
    var ss = SpreadsheetApp.openById(sheetID);
    SpreadsheetApp.setActiveSpreadsheet(ss);
    var sheets = ss.getSheets();
    var dataToDownload = {};
    for (var i = 0; i < sheets.length; ++i) {
        SpreadsheetApp.setActiveSheet(sheets[i]);
        downloadSheet(sheets[i], dataToDownload);
    }

    var fileContent = JSON.stringify(dataToDownload);

    var fileName = "GameBalanceConfig.json";

    var output = ContentService.createTextOutput(fileContent);

    output.setMimeType(ContentService.MimeType.JSON);

    return output.downloadAsFile(fileName);
}

function downloadSheet(sheet, dataToDownload)
{
    var name = sheet.getName();
    if (name.indexOf("Internal_") == 0) 
    {
        return;
    }

    var data = sheet.getDataRange().getValues();
    var dataToImport = {};

    if(data[0][0] == "Index")
    {
      dataToImport = [];
    }

    for (var i = 1; i < data.length; i++) 
    {
        if (data[i][0].toString().length == 0) 
        {
            continue;
        }
        dataToImport[data[i][0]] = {};
        for (var j = 0; j < data[0].length; j++) 
        {
            var cellValue = data[i][j];
            var jsonCellValue = getArray(cellValue);
            assign(dataToImport[data[i][0]], data[0][j].split("__"), jsonCellValue);
        }
    }

    dataToDownload[name] = dataToImport;
}

function getEnvironment() 
{
    return environment;
}

// Creates a Google Sheets on change trigger for the specific sheet
function createSpreadsheetEditTrigger(sheetID) 
{
    var triggers = ScriptApp.getProjectTriggers();
    var triggerExists = false;
    for (var i = 0; i < triggers.length; i++) 
    {
        if (triggers[i].getTriggerSourceId() == sheetID) 
        {
            triggerExists = true;
            break;
        }
    }

    if (!triggerExists) 
    {
        var spreadsheet = SpreadsheetApp.openById(sheetID);
        ScriptApp.newTrigger("importSheet")
            .forSpreadsheet(spreadsheet)
            .onChange()
            .create();
    }
}

// Delete all the existing triggers for the project
function deleteTriggers() 
{
    var triggers = ScriptApp.getProjectTriggers();
    for (var i = 0; i < triggers.length; i++) 
    {
        ScriptApp.deleteTrigger(triggers[i]);
    }
}

// Initialize
function initialize(e) 
{
    writeDataToFirebase();
}

// Write the data to the Firebase URL
function writeDataToFirebase() 
{
    //importTimeStamp();

    sheetID = getEnvironment().spreadsheetID
    var ss = SpreadsheetApp.openById(sheetID);
    SpreadsheetApp.setActiveSpreadsheet(ss);
    var sheets = ss.getSheets();
    for (var i = 0; i < sheets.length; i++) 
    {
        var sheetName = sheets[i].getName();
        importSheet(sheets[i]);
        SpreadsheetApp.setActiveSheet(sheets[i]);
    }
}

// Import each sheet when there is a change
function importSheet(sheet) 
{
    var name = sheet.getName();
    if (name.indexOf("Internal_") == 0) 
    {
        return;
    }

    var data = sheet.getDataRange().getValues();
    var dataToImport = {};

    for (var i = 1; i < data.length; i++) 
    {
        if (data[i][0].toString().length == 0) 
        {
            continue;
        }
        dataToImport[data[i][0]] = {};
        for (var j = 0; j < data[0].length; j++) 
        {
            var cellValue = data[i][j];
            var jsonCellValue = getArray(cellValue);
            assign(dataToImport[data[i][0]], data[0][j].split("__"), jsonCellValue);
        }
    }

    console.log(dataToImport);

    var token = ScriptApp.getOAuthToken();
    var firebaseUrl = getEnvironment().firebaseUrl;
    var remoteConfigUrl = firebaseUrl + "projects/" + getEnvironment().projectId + "/remoteConfig";
    var headers = {
        Authorization: "Bearer " + token,
        "Content-Type": "application/json",
    };
    var payload = JSON.stringify({
        conditions: [],
        parameters: dataToImport,
    });

    var options = {
        method: "put",
        headers: headers,
        payload: payload,
        muteHttpExceptions: true,
    };

    var response = UrlFetchApp.fetch(remoteConfigUrl, options);
    Logger.log(response.getContentText());
}

function importTimeStamp() 
{
    var dataToImport = {};
    var currentDate = new Date();
    // Get the current timestamp in milliseconds
    var timestamp = currentDate.getTime();

    dataToImport["TimeStamp"] = timestamp;

    var token = ScriptApp.getOAuthToken();
    var firebaseUrl =
        getEnvironment().firebaseUrl + "MetaData";
    var base = FirebaseApp.getDatabaseByUrl(firebaseUrl, token);
    base.setData("", dataToImport);
}

// Utilities //

// A utility function to parse nested objects or arrays
// Other primitive data types remain unchanged
function parseCustom(value) 
{
    var parsedData;

    try 
    {
        parsedData = JSON.parse(value);
    } catch (error) 
    {
        // If parsing fails, treat it as primitive type
        parsedData = value;
    }

    if (Array.isArray(parsedData) || typeof parsedData == 'object' || isPrimitive(parsedData)) 
    {
        return parsedData;
    }
    else 
    {
        throw new Error('Invalid JSON data. Expected an array, object or primitive type.');
    }
}

function getArray(value) 
{
    var result;
    if(value == "")
    {
      return value;
    }
    
    if (typeof value != 'string') 
    {
        return value;
    }

    if (value.split(",").length == 0) 
    {
        new Error("Invalid input. Expected list with length > 0 and delimiter ','");
    }

    if (value.split(",").length == 1) 
    {
        result = Number(value);
        if (isNaN(result)) 
        {
            return value;
        }
    }

    result = value.split(",").map(Number);
    if (result.some((element) => isNaN(element))) 
    {
        return value.split(",");
    }

    return result;
}

// A utility function to check if type of value is primitive
function isPrimitive(value) 
{
    var type = typeof value;
    return (
        type === 'boolean' ||
        type === 'number' ||
        type === 'string' ||
        type === 'symbol'
    );
}

// A utility function to generate nested object when
// given a keys in array format
function assign(obj, keyPath, value) 
{
    lastKeyIndex = keyPath.length - 1;
    for (var i = 0; i < lastKeyIndex; ++i) 
    {
        key = keyPath[i];
        if (!(key in obj)) obj[key] = {};
        obj = obj[key];
    }
    obj[keyPath[lastKeyIndex]] = value;
}