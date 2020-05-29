using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OsnCsLib.WPFComponent;
using OsnCsLib.Common;
using SimpleTranslation.Data;
using SimpleTranslation.AppUtil;
using SimpleTranslation.UI;

namespace SimpleTranslation {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : ResidentWindow {

        #region Constructor
        public MainWindow() {
            InitializeComponent();

            this.Activated += (sender, e) => {
                this.cSearch.Focus();
            };
        }
        #endregion

        #region Protected Method
        /// <summary>
        /// setup
        /// </summary>
        protected override void SetUp() {
            var setting = AppRepository.Init(Constant.SettingFile);

            base.SetUpHotKey(ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, Key.L);
            base.SetupNofityIcon("SimpleTranslation", new System.Drawing.Icon("app.ico"));

            base.AddContextMenu("Show", (sender,e)=> { ShowScreen(); });
            base.AddContextMenu("Setting", Setting_Click);
            base.AddContextMenu("Database", Database_Click);
            base.AddContextMenuSeparator();
            base.AddContextMenu("Exit", (sender, e) => { this.Close(); });
            this.SetContextMenuEnabled();

            Util.SetWindowXPosition(this, setting.Pos.X);
            Util.SetWindowYPosition(this, setting.Pos.Y);

            base.Minimized += MainWindow_Minimized;
            base.Normalized += () => { this.ShowScreen(); };

            var model = new MainViewModel();
            model.SaveAction = new Action(this.SaveAction);
            model.CancelAction = new Action(this.CancelAction);
            this.DataContext = model;
        }
        #endregion

        #region User Event
        /// <summary>
        /// Window Minimized
        /// </summary>
        private void MainWindow_Minimized() {
            var setting = AppRepository.GetInstance();
            setting.Pos.X = this.Left;
            setting.Pos.Y = this.Top;
            setting.Save();
        }

        private void SaveAction() {

        }

        /// <summary>
        /// cancel
        /// </summary>
        private void CancelAction() {
            base.SetWindowsState(true);
        }
        #endregion

        #region Event
        /// <summary>
        /// context menu setting click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_Click(object sender, EventArgs e) {
            var dialog = new Setting();
            if (true == dialog.ShowDialog()) {
                this.SetContextMenuEnabled();
            }
        }

        /// <summary>
        /// context menu setting click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Database_Click(object sender, EventArgs e) {
            var setting = AppRepository.GetInstance();
            if (!Util.RunApp(setting.SpreadSheet)) {
                ErrorMessage.Show(ErrorMessage.ErrMsgId.FailToLaunchSpread);
            }
        }

        /// <summary>
        /// key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                e.Handled = true;
                base.SetWindowsState(true);
            }
        }

        /// <summary>
        /// key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchWord_PreviewKeyDown(object sender, KeyEventArgs e) {
            var model = this.DataContext as MainViewModel;
            if (e.Key == Key.Enter) {
                e.Handled = true;
                var api = new TranlationApi();
                api.TranslationApiResopnse += (status, result) => {
                    switch(status) {
                        case 200:
                            model.Result = result;
                            break;
                        default:
                            ErrorMessage.Show(ErrorMessage.ErrMsgId.FailToLaunchSpread);
                            break;
                    }
                };
//                var model = this.DataContext as MainViewModel;
                if (0 < model.SearchWord.Length) {
                    api.Translate(model.SearchWord);
                } else {
                    model.Result = "";
                }
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// set context menu enabled
        /// </summary>
        private void SetContextMenuEnabled() {
            var setting = AppRepository.GetInstance();
            bool enabled = (0 < setting.TranslationApi?.Length);
            base.SetContextMenuEnabled(0, enabled);
            base.SetContextMenuEnabled(2, enabled);
        }

        /// <summary>
        /// show 
        /// </summary>
        private void ShowScreen() {
            var model = this.DataContext as MainViewModel;
            model.SearchWord = "";
            model.Result = "";
            base.SetWindowsState(false);
        }
        #endregion
    }
}
