const sheet = SpreadsheetApp.getActiveSpreadsheet().getSheets()[0];
    
/*
 * request
 */
function doGet(e) {
  
  var param = e.parameter;
  var mode = param.mode; // 0: search, 1: save
  if (!mode) {
    mode = 0;
  }
  const src = param.src; // キー
  if (!src) {
    return sendResponse(400, 'src is not set.');
  }
  
  var savedRow = findRowNo(src);
  if (0 == savedRow) {
    savedRow = getNewRow();
  }
  
  // save value
  if (1 == mode) {
    if (!param.result) {
      return sendResponse(400, 'result is not set.');
    }
    sheet.getRange(savedRow, 2).setValue(param.result);
    return sendResponse(200,'');
  }
  
  // translate
  var translatedText = LanguageApp.translate(src, "en", "ja");
  if (!translatedText) {
    return sendResponse(204,'');
  }
  
  sheet.getRange(savedRow, 1).setValue(src);
  sheet.getRange(savedRow, 2).setValue(translatedText);

  return sendResponse(200, translatedText);
}

/*
 * create response body
 */
function sendResponse(status, text) {
  var body = {
    code: status,
    text: text
  };
  var response = ContentService.createTextOutput();
  response.setMimeType(ContentService.MimeType.JSON)
  response.setContent(JSON.stringify(body));
  return response;
}

/*
 * get matched row number
 * param val: search value.
 */
function findRowNo(val) {
  var data = sheet.getRange('A:B').getValues();
  for(var i=1; i<data.length; i++) {
    if (data[i][0] === val) {
      return i+1;
    }
  }
  return 0;
}

/*
 * get new row number
 */
function getNewRow() {
  return sheet.getLastRow() + 1;
}

function myDebug() {
  Logger.log(getNewRow());
}