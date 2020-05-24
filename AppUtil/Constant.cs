using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsnCsLib.Common;
namespace SimpleTranslation.AppUtil {
    internal class Constant {
        /// <summary>
        /// アプリの設定関連情報
        /// </summary>
        public static readonly string SettingFile = OsnCsLib.Common.Util.GetAppPath() + @"app.settings";
    }
}
