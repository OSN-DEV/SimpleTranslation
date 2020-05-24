using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace SimpleTranslation.Data {
    public class SettingViewModel : BindableBase {

        /// <summary>
        /// api
        /// </summary>
        private string _translationApi = "";
        public String TranslationApi {
            get { return this._translationApi; }
            set {
                if (this.SetProperty(ref this._translationApi, value)) {
                    this.SetProperty(nameof(CanUseOk));
                    this.SetProperty(nameof(OK));
                }
            }
        }

        /// <summary>
        /// spread
        /// </summary>
        private string _spreadSheet = "";
        public String SpreadSheet {
            get { return this._spreadSheet; }
            set {
                if (this.SetProperty(ref this._spreadSheet, value)) {
                    this.SetProperty(nameof(CanUseOk));
                    this.SetProperty(nameof(OK));
                }
            }
        }

        /// <summary>
        /// command
        /// </summary>
        private OkCommand _okCommand;
        public OkCommand OK {
            get {
                return _okCommand ?? (_okCommand = new OkCommand(() => {
                    var setting = AppRepository.GetInstance();
                    setting.TranslationApi = this._translationApi;
                    setting.SpreadSheet = this._spreadSheet;
                    setting.Save();
                    this.OkAction();
                }));
            }
        }

        /// <summary>
        /// Ok Button enabled
        /// </summary>
        public bool CanUseOk {
            get {
                return (0 < this._translationApi.Length && 0 < this._spreadSheet.Length);
            }
        }

        public Action OkAction { get; set; }

    }

    public class OkCommand : ICommand {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public OkCommand(Action action) {//コンストラクタでActionを登録
            _action = action;
        }

        #region ICommandインターフェースの必須実装

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {//とりあえずActionがあれば実行可能
            return _action != null;
        }

        public void Execute(object parameter) {//今回は引数を使わずActionを実行
            _action?.Invoke();
        }

        #endregion
    }
}
