using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace SimpleTranslation.Data {
    public class MainViewModel : BindableBase {

        /// <summary>
        /// search
        /// </summary>
        private string _searchWord = "";
        public String SearchWord {
            get { return this._searchWord; }
            set { this.SetProperty(ref this._searchWord, value);}

        }

        /// <summary>
        /// spread
        /// </summary>
        private string _result = "";
        public String Result {
            get { return this._result; }
            set {
                if (this.SetProperty(ref this._result, value)) {
                    this.SetProperty(nameof(CanUseSave));
                }
            }
        }

        /// <summary>
        /// command
        /// </summary>
        private SaveCommand _saveCommand;
        public SaveCommand Save {
            get {
                return _saveCommand ?? (_saveCommand = new SaveCommand(() => {
                    var api = new TranlationApi();
                    api.TranslationApiResopnse += (status, result) => {
                        this.SaveAction();
                    };
                }));
            }
        }

        /// <summary>
        /// command
        /// </summary>
        private CancelCommand _cancelCommand;
        public CancelCommand Cancel {
            get {
                return _cancelCommand ?? (_cancelCommand = new CancelCommand(() => {
                      this.CancelAction();
                }));
            }
        }

        /// <summary>
        /// Ok Button enabled
        /// </summary>
        public bool CanUseSave {
            get {
                return (0 < this._result.Length);
            }
        }

        public Action SaveAction { get; set; }
        public Action CancelAction { get; set; }
    }

    public class SaveCommand : ICommand {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public SaveCommand(Action action) {//コンストラクタでActionを登録
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


    public class CancelCommand : ICommand {
        //Command実行時に実行するアクション、引数を受け取りたい場合はこのActionをAction<object>などにする
        private Action _action;

        public CancelCommand(Action action) {//コンストラクタでActionを登録
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
