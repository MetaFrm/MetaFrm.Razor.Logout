using MetaFrm.Auth;
using MetaFrm.Database;
using MetaFrm.Maui.Devices;
using MetaFrm.Razor.ViewModels;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace MetaFrm.Razor
{
    /// <summary>
    /// Logout
    /// </summary>
    public partial class Logout
    {
        internal LogoutViewModel LogoutViewModel { get; set; } = Factory.CreateViewModel<LogoutViewModel>();


        [Inject]
        internal IDeviceToken? DeviceToken { get; set; }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            AuthenticationStateProvider authenticationStateProvider;

            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                try
                {
                    this.LogoutViewModel.IsBusy = true;

                    if (this.AuthState != null)
                    {
                        if (this.DeviceToken != null)
                        {
                            string? tmp = await this.DeviceToken.GetToken();

                            if (!tmp.IsNullOrEmpty()) 
                                this.DeleteToken(tmp);
                        }
                        var auth = this.AuthState.Result;

                        if (auth.User.Identity != null && auth.User.Identity.IsAuthenticated)
                        {
                            if (AuthStateProvider != null)
                            {
                                if (this.SessionStorage != null)
                                    await this.SessionStorage.ClearAsync();

                                Config.Client.Clear();

                                Factory.ViewModelClear();

                                authenticationStateProvider = (AuthenticationStateProvider)AuthStateProvider;

                                await authenticationStateProvider.SetSessionTokenAsync("");
                                (AuthStateProvider as AuthenticationStateProvider)?.Notify();
                            }
                        }
                    }

                    ValueTask? _ = this.LocalStorage?.RemoveItemAsync("Login.Password");

                    if (Factory.DeviceInfo != null && Factory.DeviceInfo.Platform == Maui.Devices.DevicePlatform.iOS)
                        this.Navigation?.NavigateTo("/", true);
                    else
                        this.Navigation?.Refresh();
                }
                finally
                {
                    this.LogoutViewModel.IsBusy = false;
                }
            }
        }

        private async void DeleteToken(string? TOKEN_STR)
        {
            Response? response;

            try
            {
                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("DeleteToken");
                serviceData["1"].AddParameter("TOKEN_TYPE", DbType.NVarChar, 50, "Firebase.FCM");
                serviceData["1"].AddParameter(nameof(TOKEN_STR), DbType.NVarChar, 200, TOKEN_STR);

                response = await serviceData.ServiceRequestAsync(serviceData);

                if (response.Status != Status.OK)
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
            }
        }
    }
}