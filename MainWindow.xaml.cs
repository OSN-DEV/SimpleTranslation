﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
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

        #region Declaration
        private TranlationApi _api = new TranlationApi();
        #endregion

        #region Constructor
        public MainWindow() {
            InitializeComponent();

            this.Activated += (sender, e) => {
                this.cSearch.Focus();
            };

            this._api.TranslateApiSuccess += TranslationApiSuccess;
            this._api.SaveApiSuccess += SaveApiSuccess;
            this._api.GetListApiSuccess += GetListApiSuccess;
            this._api.ApiFailure += TranslationApiFailure;
            this._api.ApiStart += TranslationApiStart;
            this._api.ApiStop += TranslationApiStop;
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

            var model = new MainViewModel(this._api);
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
            if (this._api.IsBusy) {
                e.Handled = true;
                return;
            }
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
            if (this._api.IsBusy) {
                e.Handled = true;
                return;
            }
            var model = this.DataContext as MainViewModel;
            if (e.Key == Key.Enter ) {
                e.Handled = true;
                if (0 < model.SearchWord.Length) {
                    this._api.Translate(model.SearchWord);
                } else {
                    model.TranslatedText = "";
                }
            } else if (e.Key == Key.S && Util.IsModifierPressed(ModifierKeys.Control)) {
                this.cSave.Command.Execute(null);
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
            model.TranslatedText = "";
            base.SetWindowsState(false);
        }

        /// <summary>
        /// translation api success
        /// </summary>
        /// <param name="result"></param>
        private void TranslationApiSuccess(string translationText) {
            var model = this.DataContext as MainViewModel;
            model.TranslatedText = translationText;
        }

        /// <summary>
        /// save api success
        /// </summary>
        private void SaveApiSuccess() {

        }

        /// <summary>
        /// tranlation api failure
        /// </summary>
        /// <param name="status"></param>
        private void TranslationApiFailure(int status) {
            ErrorMessage.Show(ErrorMessage.ErrMsgId.FailToTranslate);
        }

        /// <summary>
        /// list api success
        /// </summary>
        /// <param name="list">list</param>
        private void GetListApiSuccess(Dictionary<string, string> list) {

        }

        private void TranslationApiStart() {
            this.cProceeding.Visibility = Visibility.Visible;
        }

        private void TranslationApiStop() {
            this.cProceeding.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
