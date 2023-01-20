using AutoMapper;
using Configurator.Repositories;
using Configurator.Repositories.Contracts;
using Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator.Application.Dialogs
{
    public partial class SubscriptionsForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        public int UserID { get; set; }
        public string UserName { get; set; }
        private readonly DataGridView _resultDataGridView;
        public List<SubscriptionViewModel> SubscriptionCollection { get; set; }

        public SubscriptionsForm(IServiceProvider serviceProvider, IMapper mapper, ISubscriptionRepository subscriptionRepository, IUserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;

            InitializeComponent();

            _resultDataGridView = DataGridControl.Get();
            _resultDataGridView.Dock = DockStyle.Fill;
            _resultDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _resultDataGridView.DoubleClick += ResultDataGridView_DoubleClick;
            panelContainer.Controls.Add(_resultDataGridView);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var subscriptionForm = _serviceProvider.GetRequiredService<SubscriptionForm>();
          //  subscriptionForm.UserId = UserID;
            //subscriptionDlg.tempSubscriptionViewModels = SubscriptionCollection;
            var result = subscriptionForm.ShowDialog();
            if(result == DialogResult.OK)
            {
                SubscriptionCollection.Add(subscriptionForm.SubscriptionViewModel);
                _resultDataGridView.DataSource = SubscriptionCollection.Select(x => new { x.StartDate, x.ExpiryDate, x.iCodeSerialNumber, x.Active }).ToList();
                //LoadData();
            }
        }

        private void ResultDataGridView_DoubleClick(object sender, EventArgs e)
        {
            var selectedRow = _resultDataGridView.CurrentRow;
            var selectedSubscriptionId = Convert.ToInt32(selectedRow.Cells[0].Value);
            var subscriptionForm = _serviceProvider.GetRequiredService<SubscriptionForm>();
            subscriptionForm.SubscriptionId = selectedSubscriptionId;

            var subscriptionViewModel = new SubscriptionViewModel();
            subscriptionViewModel.StartDate = Convert.ToDateTime(selectedRow.Cells[1].Value);
            subscriptionViewModel.ExpiryDate = Convert.ToDateTime(selectedRow.Cells[2].Value);
            subscriptionViewModel.iCodeSerialNumber = selectedRow.Cells[3].Value.ToString();
            subscriptionForm.SubscriptionViewModel = subscriptionViewModel;

            if (subscriptionForm.ShowDialog() == DialogResult.OK)
            {
                LoadDataFromDatabase();
            }
        }

        private void SubscriptionsDlg_Load(object sender, EventArgs e)
        {
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            var allSubscriptions = _subscriptionRepository.GetWhereAsync(x=>x.UserId == UserID).Result.ToList();
            SubscriptionCollection =  _mapper.Map<List<SubscriptionViewModel>>(allSubscriptions);
            _resultDataGridView.DataSource = SubscriptionCollection.Select(x => new { x.Id, x.StartDate, x.ExpiryDate, x.iCodeSerialNumber, x.Active }).ToList();

            _resultDataGridView.StretchLastColumn();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var dlgResult = MessageBox.Show("Are you sure you want to delete the selected subscription", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(dlgResult == DialogResult.Yes)
            {
                var selectedRow = _resultDataGridView.CurrentRow;
                var selectedSubscriptionId = Convert.ToInt32(selectedRow.Cells[0].Value);
                var subscription = _subscriptionRepository.GetByIdAsync(selectedSubscriptionId).Result;
                _subscriptionRepository.RemoveAsync(subscription, subscription.Id);

                LoadDataFromDatabase();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
