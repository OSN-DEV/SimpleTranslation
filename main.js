const sheet = SpreadsheetApp.getActiveSpreadsheet().getSheets()[0];
    
/*
 * request
 */
function doGet(e) {
  
  var body;
  var param = e.parameter;
  var mode = param.mode; // 0: search, 1: save, 2: list
  if (!mode) {
    return sendFailure(400, 'mode is not set.');
  }
  
  // get list
  if (2 == mode) {
    return sendListResponse();
  }
  
  const searchWord = param.search_word; // キー
  if (!searchWord) {
    return sendFailure(400, 'search word is not set.');
  }
  
  var savedRow = findRowNo(searchWord);
  
  // save translated text
  if (1 == mode) {
    if (!param.translated_text) {
      return sendFailure(400, 'translated text is not set.');
    }
    sheet.getRange(savedRow, 2).setValue(param.translated_text);
    return sendSuccess(200,'', '');
  }
  
   // translate
  if (savedRow == getNewRow()) {
    var translatedText = LanguageApp.translate(searchWord, "en", "ja");
    if (!translatedText) {
      return sendSuccess(204, '', '');
    }
    
    // save search word and translated text
    sheet.getRange(savedRow, 1).setValue(searchWord);
    sheet.getRange(savedRow, 2).setValue(translatedText);
  } else {
    translatedText = sheet.getRange(savedRow, 2).getValue();
  }
  
  return sendSuccess(200, searchWord, translatedText);
}

/*
 * create response body
 */
function sendSuccess(status, searchWord, translatedText) {
  var body = {
    status: status,
    searchWord: searchWord,
    translatedText: translatedText
  };
  var response = ContentService.createTextOutput();
  response.setMimeType(ContentService.MimeType.JSON)
  response.setContent(JSON.stringify(body));
  return response;
}

/*
 * create response body
 */
function sendFailure(status, error) {
  var body = {
    status: status,
    error: error
  };
  var response = ContentService.createTextOutput();
  response.setMimeType(ContentService.MimeType.JSON)
  response.setContent(JSON.stringify(body));
  return response;
}

/*
 * get list
 */
function sendListResponse() {
//  var xxx = sheet.getRange(1,1,sheet.getLastRow(), 2).getValues();
  var response = ContentService.createTextOutput();
  response.setMimeType(ContentService.MimeType.JSON)
  response.setContent(JSON.stringify(sheet.getRange(1,1,sheet.getLastRow(), 2).getValues()));
  return response;
}

/*
 * get matched row number
 * param val: search value.
 */
function findRowNo(val) {
  var lastRow = sheet.getLastRow();
  var data = sheet.getRange(1,1,sheet.getLastRow(), 2).getValues();
  for(var i=0; i<data.length; i++) {
    if (data[i][0] === val) {
      return i+1;
    }
  }
  return getNewRow();
}

/*
 * get new row number
 */
function getNewRow() {
  // 列を指定して行数を取得する場合はこちらを使用する
//  const sheet = SpreadsheetApp.getActiveSheet();
//  const columns = sheet.getRange('A:A').getValues();
//  return columns.filter(String).length;
  
  return sheet.getLastRow() + 1;
}

function myDebug() {
  Logger.log(getNewRow());
}