using MetaFrm.MVVM;
using Microsoft.Extensions.Localization;

namespace MetaFrm.Razor.ViewModels
{
    /// <summary>
    /// LogoutViewModel
    /// </summary>
    public partial class LogoutViewModel : BaseViewModel
    {
        /// <summary>
        /// LogoutViewModel
        /// </summary>
        public LogoutViewModel() { }

        /// <summary>
        /// LogoutViewModel
        /// </summary>
        /// <param name="localization"></param>
        public LogoutViewModel(IStringLocalizer? localization) : base(localization) { }
    }
}