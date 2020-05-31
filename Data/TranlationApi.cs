using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleTranslation.Data {
    internal class TranlationApi {
        // https://www.kekyo.net/2016/12/06/6186

        #region QueryBuilder
        // build query parameter
        public class QueryBuilder {
            private StringBuilder _query = new StringBuilder();

            public void Append(string key, int value) {
                this.Append(key, "" + value);
            }

            public void Append(string key, string value) {
                if (0 < this._query.Length) {
                    this._query.Append("&");
                }
                this._query.Append($"{key}={HttpUtility.UrlEncode(value)}");
            }

            public override string ToString() {
                return "?" + this._query.ToString();
            }
        }
        #endregion

        #region Declaration
        private static class ApiMode {
            public static readonly int Search = 0;
            public static readonly int Save = 1;
            public static readonly int List = 2;
        }
        private int _apiMode;

        private static class QueryKey {
            public static readonly string Mode = "mode";
            public static readonly string SearchWord = "search_word";
            public static readonly string TranslatedText = "translated_text";
        }

        private static class JsonKey {
            public static readonly string Status = "status";
            public static readonly string TranslatedText = "translatedText";
        }

        private class JsonParser {
            private Dictionary<string, string> jsonData = new Dictionary<string, string>();

            private JsonParser() {
            }
            public string GetValue(string key) {
                return this.jsonData[key];
            }
            public Dictionary<string, string> GetList() {
                return this.jsonData;
            }
            public static JsonParser Create(string json) {
                var parser = new JsonParser();
                parser.Parse(json);
                return parser;
            }
            private void Parse(string json) {
                var tmp = json.Replace("{", "").Replace("}", "")
                    .Replace("[", "").Replace("]", "").Replace("\"", "");
                foreach (var pair in tmp.Split(',')) {
                    var p = pair.Split(':');
                    jsonData.Add(p[0].Trim(), p[1].Trim());
                }
            }
        }
        #endregion

        #region Public Property
        public bool IsBusy { private set; get; } = false;
        #endregion

        #region Event
        /// <summary>
        /// raised when translate api is success
        /// </summary>
        /// <param name="translatedText">translated text</param>
        public delegate void TranslateApiSuccessHandler(string translatedText);
        public event TranslateApiSuccessHandler TranslateApiSuccess;

        /// <summary>
        /// raised when save api is success
        /// </summary>
        public delegate void SaveApiSuccessHandler();
        public event SaveApiSuccessHandler SaveApiSuccess;

        /// <summary>
        /// raised when list api is success
        /// </summary>
        /// <param name="translatedText">translated text</param>
        public delegate void GetListApiSuccessHandler(Dictionary<string, string> list);
        public event GetListApiSuccessHandler GetListApiSuccess;

        /// <summary>
        /// raised when api is failure
        /// </summary>
        /// <param name="status">http status</param>
        public delegate void ApiFailureHandler(int status);
        public event ApiFailureHandler ApiFailure;

        /// <summary>
        /// 
        /// </summary>
        public Action ApiStart { set; get; }
        public Action ApiStop { set; get; }

        #endregion

        #region Public Method
        /// <summary>
        /// tranlate  word
        /// </summary>
        /// <param name="searchWord">word for translated</param>
        public void Translate(string searchWord) {
            this._apiMode = ApiMode.Search;
            var query = new QueryBuilder();
            query.Append(QueryKey.Mode, this._apiMode);
            query.Append(QueryKey.SearchWord, searchWord);
            _ = SendData(query);
        }

        /// <summary>
        /// save sarch word and translated text
        /// </summary>
        /// <param name="searchWord">word for translated</param>
        /// <param name="translationText">translated text</param>
        public void Save(string searchWord, string translationText) {
            this._apiMode = ApiMode.Search;
            var query = new QueryBuilder();
            query.Append(QueryKey.Mode, this._apiMode);
            query.Append(QueryKey.SearchWord, searchWord);
            query.Append(QueryKey.TranslatedText, translationText);
            _ = SendData(query);
        }

        /// <summary>
        /// get translated list
        /// </summary>
        public void List() {
            this._apiMode = ApiMode.List;
            var query = new QueryBuilder();
            query.Append(QueryKey.Mode, this._apiMode);
            _ = SendData(query);
        }
        #endregion

        #region Task
        /// <summary>
        /// api task
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<string> SendData(QueryBuilder query) {
            ApiStart?.Invoke();
            if (this.IsBusy) {
                ApiFailure?.Invoke(102);
                return "";
            }
            //            this.IsBusy = true;
            var url = AppRepository.GetInstance().TranslationApi + query.ToString();
            try {
                using (var client = new HttpClient()) {
                    var response = await client.GetAsync(url);
                    var body = await response.Content.ReadAsStringAsync(); // これが非同期だから、このメソッドを非同期にする意味ないかも。。。
                    var status = (int)response.StatusCode;
                    if (200 == status & !body.StartsWith("<!DOC")) {
                        var json = JsonParser.Create(body);
                        if (ApiMode.List == this._apiMode) {
                            GetListApiSuccess(json.GetList());
                        } else {
                            status = int.Parse(json.GetValue(JsonKey.Status));
                            if (200 == status) {
                                if (ApiMode.Search == this._apiMode) {
                                    TranslateApiSuccess?.Invoke(json.GetValue(JsonKey.TranslatedText));
                                } else {
                                    SaveApiSuccess?.Invoke();
                                }
                            } else {
                                ApiFailure?.Invoke(status);
                            }
                        }
                    } else {
                        ApiFailure?.Invoke(status);
                    }
                    this.IsBusy = false;
                    ApiStop();
                    return "";
                }
            } catch {
                ApiFailure?.Invoke(500);
                this.IsBusy = false;
                ApiStop();
                return "";
            }
        }
        #endregion
    }
}
