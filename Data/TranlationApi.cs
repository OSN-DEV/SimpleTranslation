using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTranslation.Data {
    internal class TranlationApi {
        // https://www.kekyo.net/2016/12/06/6186

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

        }

        /// <summary>
        /// save keyword
        /// </summary>
        /// <param name="word"></param>
        /// <param name="translationResult"></param>
        public void Save(string word, string translationResult) {

        }
        #endregion


    }
}
