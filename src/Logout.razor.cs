using MetaFrm.Auth;
using MetaFrm.Control;
using MetaFrm.Razor.ViewModels;

namespace MetaFrm.Razor
{
    /// <summary>
    /// Logout
    /// </summary>
    public partial class Logout
    {
        internal LogoutViewModel LogoutViewModel { get; set; } = Factory.CreateViewModel<LogoutViewModel>();

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            AuthenticationStateProvider authenticationStateProvider;

            if (firstRender)
            {
                try
                {
                    this.LogoutViewModel.IsBusy = true;

                    if (this.AuthenticationState != null)
                    {
                        var auth = this.AuthenticationState.Result;

                        if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
                        {
                            if (AuthStateProvider != null)
                            {
                                if (Session != null)
                                    await Session.ClearAsync();

                                Factory.ViewModelClear();

                                authenticationStateProvider = (AuthenticationStateProvider)AuthStateProvider;

                                await authenticationStateProvider.SetSessionTokenAsync("");
                                (AuthStateProvider as AuthenticationStateProvider)?.Notify();
                            }
                        }
                    }

                    this.Navigation?.NavigateTo("/", true);
                }
                finally
                {
                    this.LogoutViewModel.IsBusy = false;
                }
            }
        }
    }
}