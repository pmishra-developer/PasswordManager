using AutoMapper;
using Configurator.Database.Entities;
using Configurator.Repositories;
using Configurator.Repositories.Contracts;
using Configurator.ViewModel;

namespace Configurator.Application.Dialogs
{
    public partial class SubscriptionForm : Form
    {
        private readonly IMapper _mapper;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public int SubscriptionId { get; set; }
        

        public SubscriptionViewModel SubscriptionViewModel { get; set; }

        bool _editMode = false;
        bool _validationsPassed = false;
        int _deviceId { get; set; }
        int _userId { get; set; }

        public SubscriptionForm(IMapper mapper, IDeviceRepository deviceRepository, IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;

            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_editMode)
                {
                    DateTime now = DateTime.Now;
                    if (dateTimePickerTo.Value.Date < now.Date)
                    {
                        HelperFunctions.DisplayError("Expiry date should not be in past or today.");
                        return;
                    }

                    if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
                    {
                        HelperFunctions.DisplayError("Expiry date should be more than Start date");
                        return;
                    }

                    // Update the Subscription Date
                    DateTime newExpiryDate = dateTimePickerTo.Value;
                    var subscription = _subscriptionRepository.FirstOrDefaultAsync(x => x.Id == SubscriptionId).Result;
                    if (subscription != null)
                    {
                        subscription.ExpiryDate = newExpiryDate;
                        _subscriptionRepository.UpdateAsync(subscription);
                        //_subscriptionRepository.SaveChangesAsync();
                    }
                    
                    //TODO - Check this update
                    //_userRepository.UpdateSubscription(SubscriptionId, newExpiryDate);
                    _validationsPassed = true;
                }
                else
                {
                    var newSerialNumber = textBoxSerialNumber.Text;
                    var subscription = _subscriptionRepository.FirstOrDefaultAsync(x=> x.BootloaderDevice.SerialNumber == newSerialNumber).Result;
                    if (subscription != null)
                    {
                        HelperFunctions.DisplayError($"Subscription for this Serial number {newSerialNumber} already exists. Please select a unique iCode Serial Number");
                        return;
                    }

                    DateTime now = DateTime.Now;
                    if (dateTimePickerTo.Value.Date < now.Date)
                    {
                        HelperFunctions.DisplayError("Expiry date should not be in past or today.");
                        return;
                    }

                    if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
                    {
                        HelperFunctions.DisplayError("Expiry date should be more than Start date");
                        return;
                    }

                    _validationsPassed = true;

                    SubscriptionViewModel = new SubscriptionViewModel
                    {
                        Id = 0,
                        iCodeSerialNumber = newSerialNumber,
                        StartDate = dateTimePickerFrom.Value,
                        ExpiryDate = dateTimePickerTo.Value,
                        Active = now.InIsRange(dateTimePickerFrom.Value, dateTimePickerTo.Value),
                    };

                    var newSubscription = _mapper.Map<Subscription>(SubscriptionViewModel);
                    newSubscription.BootloaderDeviceId = _deviceId;
                    newSubscription.UserId = _userId;
                    _subscriptionRepository.AddAsync(newSubscription);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if(!_validationsPassed)
            {
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _validationsPassed = true;
        }

        private void SubscriptionDlg_Load(object sender, EventArgs e)
        {
            userNamePanel.Visible = false;
            textBoxSerialNumber.Enabled = false;
            dateTimePickerFrom.Enabled = false;
            dateTimePickerTo.Enabled = false;
            if (SubscriptionViewModel != null)
            {
                textBoxSerialNumber.Text = SubscriptionViewModel.iCodeSerialNumber;
                dateTimePickerFrom.Value = SubscriptionViewModel.StartDate;
                dateTimePickerTo.Value = SubscriptionViewModel.ExpiryDate;
                dateTimePickerTo.Enabled = true;
                _editMode = true;
                btnSave.Enabled = true;
                iCodeSerialNumberVerify.Enabled = false;
            }
            else
            {
                textBoxSerialNumber.Enabled = true;
                dateTimePickerFrom.Enabled = true;
                dateTimePickerTo.Enabled = true;
            }
        }

        private void userNameVerify_Click(object sender, EventArgs e)
        {
            var userName = textBoxUserName.Text;
            var user = _userRepository.FirstOrDefaultAsync(x => x.FirstName == userName).Result;
            if(user == null)
            {
                HelperFunctions.DisplayError($"User with First Name {userName} not found, Please check User's First Name");
            }
            else
            {
                _userId = user.Id;
                btnSave.Enabled = true;
            }
        }

        private void iCodeSerialNumberVerify_Click(object sender, EventArgs e)
        {
            var newiCodeSerialNumber = textBoxSerialNumber.Text;
            var device = _deviceRepository.FirstOrDefaultAsync(x => x.SerialNumber == newiCodeSerialNumber).Result;
            if (device == null)
            {
                HelperFunctions.DisplayError($"Boot Loader Device with Serial Number {newiCodeSerialNumber} could not be found, Please check iCode Serial Number is Valid");
            }
            else
            {
                _deviceId = device.Id;
                userNamePanel.Visible = true;
                //btnSave.Enabled = true;
            }
        }
    }
}
