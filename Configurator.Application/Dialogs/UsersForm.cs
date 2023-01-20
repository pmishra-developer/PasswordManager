using AutoMapper;
using Configurator.Repositories;
using Configurator.Repositories.Contracts;
using Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator.Application.Dialogs
{
    public partial class UsersForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly DataGridView _resultDataGridView;
        public List<UserViewModel> UserCollection { get; set; }

        public UsersForm(IServiceProvider serviceProvider, IMapper mapper, IUserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _userRepository = userRepository;

            InitializeComponent();


            _resultDataGridView = DataGridControl.Get();
            _resultDataGridView.Dock = DockStyle.Fill;
            _resultDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _resultDataGridView.DoubleClick += ResultDataGridView_DoubleClick;
            _resultDataGridView.DataBindingComplete += _resultDataGridView_DataBindingComplete;
            panelContainer.Controls.Add(_resultDataGridView);
        }

        private void _resultDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach(DataGridViewColumn col in _resultDataGridView.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            this._resultDataGridView.Columns["FirstName"].HeaderText = "First Name";
            this._resultDataGridView.Columns["LastName"].HeaderText = "Last Name";
            this._resultDataGridView.Columns["BusinessName"].HeaderText = "Business";
            this._resultDataGridView.Columns["EmailAddress"].HeaderText = "Email Address";
            this._resultDataGridView.Columns["ActiveSubscription"].HeaderText = "Active Subscription";
        }

        private void ResultDataGridView_DoubleClick(object sender, EventArgs e)
        {
            var selectedRow = _resultDataGridView.CurrentRow;
            var selectedUserId = Convert.ToInt32(selectedRow.Cells[0].Value);
            var userForm = _serviceProvider.GetRequiredService<UserForm>();
            userForm.UserId = selectedUserId;
            if (userForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var allUsers = _userRepository.GetAllAsync().Result;
            UserCollection = _mapper.Map<List<UserViewModel>>(allUsers);

            _resultDataGridView.DataSource = UserCollection.Select(x => new
            {
                x.Id, 
                x.FirstName,
                x.LastName, 
                x.BusinessName, 
                x.EmailAddress, 
                x.ActiveSubscription
            }).ToList();

            _resultDataGridView.StretchLastColumn();
        }

        private void UsersDlg_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewColumn cols in _resultDataGridView.Columns)
            {
                var a = cols.Width;
            }

            var userForm = _serviceProvider.GetRequiredService<UserForm>();
            if (userForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
