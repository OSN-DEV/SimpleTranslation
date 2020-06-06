using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace SimpleTranslation.Data {
    public class MainViewModel : BindableBase {

        #region Declaration
        private readonly TranlationApi _api = null;
        public Action SaveAction { get; set; }
        public Action SaveFailedAction { get; set; }
        public Action CancelAction { get; set; }
        public Action ShowDataAction { get; set; }
        #endregion

        #region Constructor
        internal MainViewModel(TranlationApi api) {
            this._api = api;
        }
        #endregion

        #region Public Property
        /// <summary>
        /// search keyword
        /// </summary>
        private string _searchWord = "";
        public String SearchWord {
            get { return this._searchWord; }
            set { this.SetProperty(ref this._searchWord, value); }

        }

        /// <summary>
        /// spread
        /// </summary>
        private string _translatedText = "";
        public String TranslatedText {
            get { return this._translatedText; }
            set {
                if (this.SetProperty(ref this._translatedText, value)) {
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
                    this._api.Save(this.SearchWord, this.TranslatedText);
                }));
            }
        }

        /// <summary>
        /// command
        /// </summary>
        private GeneralCommand _cancelCommand;
        public GeneralCommand Cancel {
            get {
                return _cancelCommand ?? (_cancelCommand = new GeneralCommand(() => {
                    this.CancelAction();
                }));
            }
        }

        /// <summary>
        /// command
        /// </summary>
        private GeneralCommand _showDataCommand;
        public GeneralCommand ShowData {
            get {
                return _showDataCommand ?? (_showDataCommand = new GeneralCommand(() => {
                    this.ShowDataAction();
                }));
            }
        }

        /// <summary>
        /// Ok Button enabled
        /// </summary>
        public bool CanUseSave {
            get {
                return (0 < this._translatedText.Length);
            }
        }
        #endregion
    }

    public class SaveCommand : ICommand {
        private Action _action;

        public SaveCommand(Action action) {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) {
            if (null != CanExecuteChanged) {
            }
            return _action != null;
        }

        public void Execute(object parameter) {
            _action?.Invoke();
        }
    }


    public class GeneralCommand : ICommand {
        private Action _action;

        public GeneralCommand(Action action) {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) {
            if (null != CanExecuteChanged) {
            }
            return _action != null;
        }

        public void Execute(object parameter) {//今回は引数を使わずActionを実行
            _action?.Invoke();
        }
    }
}
