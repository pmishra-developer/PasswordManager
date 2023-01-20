using Configurator.Services.Contracts;
using Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Configurator.Application.Dialogs
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserService _userService;
        private readonly AppSettings _settings;
        private readonly DataGridView _resultDataGridView;

        public MainForm(IServiceProvider serviceProvider, IUserService userService, IOptions<AppSettings> settings)
        {
            InitializeComponent();

            this._serviceProvider = serviceProvider;
            this._userService = userService;
            this._settings = settings.Value;

            _resultDataGridView = DataGridControl.Get();
            _resultDataGridView.Dock = DockStyle.Fill;
            _resultDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _resultDataGridView.DoubleClick += ResultDataGridView_DoubleClick;
            panelContainer.Controls.Add(_resultDataGridView);
        }

        private void ResultDataGridView_DoubleClick(object? sender, EventArgs e)
        {
            var userDialog = _serviceProvider.GetRequiredService<SecondForm>();
            userDialog.UserId = _resultDataGridView.GetSelectedRowId();
            if (userDialog.ShowDialog() == DialogResult.OK)
            {
                _ = RefreshData();
            }

            _resultDataGridView.ClearSelection();
        }

        async Task RefreshData()
        {
            _resultDataGridView.DataSource = null;
            var allUsers = await GetAllUsers();
            var usersList = allUsers.ToList();

            _resultDataGridView.DataSource = usersList;
            _resultDataGridView.StretchLastColumn();
        }

        private void OpenSecondFormButton_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<SecondForm>();
            if(form.ShowDialog(this) == DialogResult.OK)
            {
                _ = RefreshData();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _ = RefreshData();
        }

        async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            return await _userService.GetUsersAsync();
        }
    }
}
