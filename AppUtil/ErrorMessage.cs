using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleTranslation.AppUtil {
    internal class ErrorMessage {

        #region Declaration
        public enum ErrMsgId {
            FailToLaunchSpread,
            FailToTranslate,
            FailToSave,
            APIIsNotSet,
        }
        private static Dictionary<ErrMsgId, string> _message = new Dictionary<ErrMsgId, string>() {
            { ErrMsgId.FailToLaunchSpread, "スプレッドシートの表示に失敗しました。"},
            { ErrMsgId.FailToTranslate,"翻訳に失敗しました。" },
            { ErrMsgId.FailToSave,"保存に失敗しました。" },
            { ErrMsgId.APIIsNotSet,"APIのURLが設定されていません。" },
        };
        #endregion

        #region Public Method
        /// <summary>
        /// show error message
        /// </summary>
        /// <param name="id"></param>
        public static void Show(ErrMsgId id) {
            MessageBox.Show(_message[id], "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        #endregion

    }
}
