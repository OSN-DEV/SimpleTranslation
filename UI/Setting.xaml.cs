using SimpleTranslation.Data;
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
using System.Windows.Shapes;

namespace SimpleTranslation.UI {
    /// <summary>
    /// Setting.xaml の相互作用ロジック
    /// </summary>
    public partial class Setting : Window {

        #region Constructor
        public Setting() {
            InitializeComponent();

            var model = new SettingViewModel();
            var setting = AppRepository.GetInstance();
            model.TranslationApi = setting.TranslationApi;
            model.SpreadSheet = setting.SpreadSheet;
            model.OkAction = new Action(this.OkClick);
            this.DataContext = model;
        }
        #endregion

        #region Private Method
        public void OkClick() {
            this.DialogResult = true;
        }
        #endregion
    }
}
