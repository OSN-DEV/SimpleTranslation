using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleTranslation.Data {
    internal class TranlationApi {
        // https://www.kekyo.net/2016/12/06/6186

        #region QueryBuilder
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
        private static class QueryKey {
            public static readonly string Mode = "mode";
            public static readonly string Src = "src";
            public static readonly string Result = "result";
        }
        #endregion

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">HTTP Status Code</param>
        /// <param name="result">translation result</param>
        public delegate void ApiResultDelegate(int status, string result);
        public event ApiResultDelegate TranslationApiResopnse;
        #endregion

        #region Public Method
        /// <summary>
        /// tranlate 
        /// </summary>
        /// <param name="word"></param>
        public void Translate(string word) {
            var query = new QueryBuilder();
            query.Append(QueryKey.Mode, 0);
            query.Append(QueryKey.Src, word);
            _ = SendData(query);
        }

        /// <summary>
        /// save keyword
        /// </summary>
        /// <param name="word"></param>
        /// <param name="translationResult"></param>
        public void Save(string word, string translationResult) {
            var query = new QueryBuilder();
            query.Append(QueryKey.Mode, 0);
            query.Append(QueryKey.Src, word);
            query.Append(QueryKey.Result, translationResult);
            _ = SendData(query);
        }
        #endregion

        #region Private Method
        #endregion

        public async Task<string> SendData(QueryBuilder query) {
            //var parameters = new Dictionary<string, string>() {
            //    { "mode", "0" },
            //    { "src", "test" },
            //};
            //using (var client = new HttpClient()) {
            //    var response = await client.GetAsync(AppRepository.GetInstance().TranslationApi + $"?{ await new FormUrlEncodedContent(parameters).ReadAsStringAsync()} ");
            //    var body = await response.Content.ReadAsStringAsync();
            //    TranslationApiResopnse?.Invoke((int)response.StatusCode, body);
            //    return "";
            //}


            var url = AppRepository.GetInstance().TranslationApi + query.ToString();
//            url = "https://script.google.com/macros/s/AKfycbzZtvOvf14TaMdRIYzocRcf3mktzGgXvlFvyczo/exec?text=Hello&source=en&target=ja";
            using (var client = new HttpClient()) {
                var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync(); // これが非同期だから、このメソッドを非同期にする意味ないかも。。。
                TranslationApiResopnse?.Invoke((int)response.StatusCode, body);
                return "";
            }

        }

    }
}
