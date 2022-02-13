using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicLocalizationSample
{
    public class MainWindowModel
    {
        public static event EventHandler LanguageChanged;

        protected virtual void OnLanguageChanged(EventArgs e)
        {
            LanguageChanged?.Invoke(this, e);
        }

        public ICommand EnglishLanguageCmd { get { return new RelayCommand(p => SetLanguageToEnglish()); } }

        public ICommand SpanishLanguageCmd { get { return new RelayCommand(p => SetLanguageToSpanish()); } }

        private void SetLanguageToEnglish()
        {
            AppSettings.AppLanguage = "English";
            OnLanguageChanged(EventArgs.Empty);
            UpdateLocalizedElements();
        }

        private void SetLanguageToSpanish()
        {
            AppSettings.AppLanguage = "Spanish";
            OnLanguageChanged(EventArgs.Empty);
            UpdateLocalizedElements();
        }

        private void UpdateLocalizedElements()
        {
            LocalizationProvider.UpdateAllObjects();
        }
    }
}
