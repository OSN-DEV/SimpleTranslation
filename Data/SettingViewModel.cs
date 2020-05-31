using System;
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
                    //this.SetProperty(nameof(OK));
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
                  //  this.SetProperty(nameof(OK));
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
       private Action _action;

        public OkCommand(Action action) {
            _action = action;
        }

        #region ICommandインターフェースの必須実装

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) {
            if (null != CanExecuteChanged) {
            }
            return _action != null;
        }

        public void Execute(object parameter) {
            _action?.Invoke();
        }

        #endregion
    }
}
