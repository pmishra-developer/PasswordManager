using AutoMapper;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;
using Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator.Application.Dialogs
{
    public partial class UserForm : Form
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;

        public int UserId { get; set; }
        public List<SubscriptionViewModel> SubscriptionCollection { get; set; }

        public UserForm(IMapper mapper, IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;

            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (UserId == 0)
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    FirstName = textBoxFirstName.Text,
                    LastName = textBoxLastName.Text,
                    EmailAddress = textBoxEmailAddress.Text,
                    BusinessName = textBoxBusinessName.Text,
                    Phone = textBoxPhoneNumber.Text
                };

                var newUser = _mapper.Map<User>(userViewModel);
                _ = _userRepository.AddAsync(newUser).Result;
            }
        }

        private void btnSubscription_Click(object sender, EventArgs e)
        {
            var subscriptionsForm = _serviceProvider.GetRequiredService<SubscriptionsForm>();
            subscriptionsForm.UserID = UserId;

            subscriptionsForm.UserName = textBoxFirstName.Text + textBoxLastName.Text;
            if (subscriptionsForm.ShowDialog() == DialogResult.OK)
            {
                SubscriptionCollection = subscriptionsForm.SubscriptionCollection;
            }
        }

        private void UserDlg_Load(object sender, EventArgs e)
        {
            if(UserId != 0)
            {
                var user = _userRepository.GetByIdAsync(UserId).Result;
                var userViewModel = _mapper.Map<UserViewModel>(user);

                if (userViewModel != null)
                {
                    textBoxFirstName.Text = userViewModel.FirstName;
                    textBoxLastName.Text = userViewModel.LastName;
                    textBoxEmailAddress.Text = userViewModel.EmailAddress;
                    textBoxBusinessName.Text = userViewModel.BusinessName;
                    textBoxPhoneNumber.Text = userViewModel.Phone;

                    btnSubscription.Visible = true;
                }
            }
        }
    }
}
