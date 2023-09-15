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

        Auth.AuthenticationStateProvider AuthenticationState;

        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.AuthenticationState ??= (this.AuthStateProvider as Auth.AuthenticationStateProvider) ?? (Auth.AuthenticationStateProvider)Factory.CreateInstance(typeof(Auth.AuthenticationStateProvider));
        }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:올바르게 ValueTasks 사용", Justification = "<보류 중>")]
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                try
                {
                    this.LogoutViewModel.IsBusy = true;

                    if (this.AuthenticationState != null)
                    {
                        if (this.DeviceToken != null)
                        {
                            string? tmp = await this.DeviceToken.GetToken();

                            if (!tmp.IsNullOrEmpty()) 
                                this.DeleteToken(tmp);
                        }

                        if (this.AuthenticationState.IsLogin())
                        {
                            if (Session != null)
                                await Session.ClearAsync();

                            Config.Client.Clear();

                            Factory.ViewModelClear();

                            await this.AuthenticationState.SetSessionTokenAsync("");
                            this.AuthenticationState.Notify();
                        }
                    }

                    this.LocalStorage?.RemoveItemAsync("Login.Password");
                    this.Navigation?.NavigateTo("/", true);
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
                    Token = this.AuthenticationState.UserClaim("Token")
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