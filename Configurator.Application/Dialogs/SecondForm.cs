using Configurator.Services.Contracts;
using Configurator.ViewModel;
using Microsoft.Extensions.Options;

namespace Configurator.Application.Dialogs
{
    public partial class SecondForm : Form
    {
        private readonly ISampleService _sampleService;
        private readonly AppSettings _settings;
        public int UserId { get; set; }
        private readonly IUserService _userService;

        public SecondForm(ISampleService sampleService, IUserService userService, IOptions<AppSettings> settings)
        {
            InitializeComponent();

            this._sampleService = sampleService;
            this._userService = userService;
            this._settings = settings.Value;
        }

        private void SecondForm_Load(object sender, EventArgs e)
        {
            if (UserId == 0)
            {
                textBoxUser.Text = string.Empty;
                return;
            }

            var user = _userService.GetUserAsync(UserId).GetAwaiter().GetResult();
            textBoxUser.Text = user.FirstName;
        }

        private bool ValidateControls()
        {
            var buttonToolTip = new ToolTip
            {
                ToolTipTitle = "Blank Entry",
                UseFading = true,
                UseAnimation = true,
                IsBalloon = true,
                ShowAlways = true,
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                ToolTipIcon = ToolTipIcon.Error,
            };

            if (!string.IsNullOrEmpty(textBoxUser.Text)) 
                return true;

            buttonToolTip.Show(string.Empty, textBoxUser, 1000);
            buttonToolTip.Show("User Name can not be blank.", textBoxUser);
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateControls())
            {
                btnOK.DialogResult = DialogResult.Cancel;
                return;
            }

            if (UserId == 0)
            {
                // New User
                UserViewModel userViewModel = new UserViewModel
                {
                    FirstName = textBoxUser.Text
                };

                _ = _userService.AddUserAsync(userViewModel);
            }
            else
            {
                // Update User
                var user = _userService.GetUserAsync(UserId).GetAwaiter().GetResult();
                user.FirstName = textBoxUser.Text;
                _ = _userService.UpdateUserAsync(user);
            }
        }
    }
}
