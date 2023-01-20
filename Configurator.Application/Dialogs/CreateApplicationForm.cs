using System.ComponentModel;
using Configurator.Repositories;
using Configurator.Repositories.Contracts;

namespace Configurator.Application.Dialogs
{
    public partial class CreateApplicationForm : Form
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IApplicationRepository _applicationRepository;

        const int MAX_APP_SIZE = 0x1DFF00;
        const int APPLICATION_START_ADDRESS = 0x08020000;
        const int RSA_SIGNATURE_ADDRESS = 0x01DFF00;
        const int MAX_SIGNATURE_SIZE = 256;

        const int BLOCK_SIZE = 512;

        public CreateApplicationForm(IDeviceRepository deviceRepository, IApplicationRepository applicationRepository)
        {
            _deviceRepository = deviceRepository;
            _applicationRepository = applicationRepository;
            InitializeComponent();

            createAppBackgroundWorker.DoWork += createAppBackgroundWorker_DoWork;
            createAppBackgroundWorker.ProgressChanged += createAppBackgroundWorker_ProgressChanged;
            createAppBackgroundWorker.WorkerReportsProgress = true;
            createAppBackgroundWorker.RunWorkerCompleted += CreateAppBackgroundWorker_RunWorkerCompleted;
        }

        private void CreateAppBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnClose.SetButtonEnable(true);
        }

        private void createAppBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.SetValue(e.ProgressPercentage);
        }

        private void createAppBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            progressBar1.ProgressBarVisible(true, true);
            btnClose.SetButtonEnable(false);
            btnStart.SetButtonEnable(false);

            //if (rangeGroupBox.Visible)
            //{
            //    var from = textBoxFrom.Text;
            //    var to = textBoxTo.Text;

            //    var applicationIds = _deviceRepository.GetAllDeviceIndexes(from, to);
            //    CreateApplication(applicationIds.From, applicationIds.To);
            //}
            //else
            //{
            //    CreateAllApplications();
            //}

            //progressBar1.ProgressBarVisible(false, true);
            //MessageBox.Show("Applications Creation Process Completed", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreateApplication_Load(object sender, EventArgs e)
        {
            try
            {
            //    // First Get all the devices.
            //    var devices = _deviceRepository.GetAllDeviceSerialNumbers();
            //    if (devices.Count != 0)
            //    {
            //        textBoxFrom.Text = devices.From;
            //        textBoxTo.Text = devices.To;
            //        comboBoxSelectOptions.SelectedIndex = 0;
            //        rangeGroupBox.Visible = false;
            //    }
            //    else
            //    {
            //        HelperFunctions.DisplayError("No Bootloader device found.");
            //        Close();
            //        return;
            //    }

            //    var connectionString = "DefaultEndpointsProtocol=https;AccountName=icodestorageaccount;AccountKey=X4RQDX2z3+bO0TkV43L0jgh0Zsik9/Gs+qBivyPQP24o8lNvF4dDYIRQVPo5lD1mKrEbPQ3YknaZ9JupkFKdgQ==;EndpointSuffix=core.windows.net";
            //    var shareName = "icodefunctionapp";
            //    var appFilePath = ApplicationSettings.GetSettingValue("ApplicationFilePath");
            //    if (!File.Exists(appFilePath))
            //    {
            //        HelperFunctions.DisplayError($"Application File not present at location {appFilePath}");
            //        Close();
            //        return;
            //    }

            //    // Upload the Appplication File on Azure. 
            //    var rawResponse = Upload(connectionString, shareName, appFilePath);
            //    if (rawResponse.Status != 201 && rawResponse.ReasonPhrase != "Created")
            //    {
            //        HelperFunctions.DisplayError($"Unable to upload file from location {appFilePath}");
            //        Close();
            //        return;
            //    }
            }
            catch (Exception ex)
            {
                HelperFunctions.DisplayError(ex.Message);                
                Close();
                return;
            }
        }

        private void comboBoxSelectOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedEntry = comboBoxSelectOptions.SelectedItem.ToString();
            rangeGroupBox.Visible = selectedEntry == "Range";
        }

        private void buttonCreateApp_Click(object sender, EventArgs e)
        {
            createAppBackgroundWorker.RunWorkerAsync();
        }

        private void CreateAllApplications()
        {
            //DeviceIndexRange range = _deviceRepository.GetAllDeviceIndexes();
            //CreateApplication(range.From, range.To);
        }

        public BaseResponse Upload(string connectionString, string shareName, string localFilePath)
        {
            //TODO
            //// Get a reference to a share and then create it
            //ShareClient shareClient = new ShareClient(connectionString, shareName);
            //shareClient.CreateIfNotExists();

            //// Get a reference to a directory and create it
            //string dirName = "iCodeFiles";
            //string fileName = "Application.bin";
            //ShareDirectoryClient shareClientDirectory = shareClient.GetDirectoryClient(dirName);
            //shareClientDirectory.CreateIfNotExists();

            //// We need to delete the file, before uploading it again
            //ShareFileClient shareFileClient = shareClientDirectory.GetFileClient(fileName);
            //shareFileClient.DeleteIfExists();
            
            //// Get a reference to a file and upload it
            //using (FileStream stream = File.OpenRead(localFilePath))
            //{
            //    shareFileClient.Create(stream.Length);
            //    var response = shareFileClient.UploadRange(new HttpRange(0, stream.Length), stream);
            //    return response.GetRawResponse();
                
            //}

            return BaseResponse.Successful();
        }

        private void CreateApplication(int deviceIdFrom, int deviceIdTo)
        {
            //TODO
            //var allDevices = _deviceRepository.GetAllDevices().ToList();
            //int allDevicesCount = allDevices.Count();
            //for (int i = 0; i < allDevicesCount; i++)
            //{
            //    var device = allDevices[i];
            //    try
            //    {
            //        if (Enumerable.Range(deviceIdFrom, deviceIdTo).Contains(device.Id))
            //        {
            //            int percentage = (i) * 100 / allDevicesCount;
            //            createAppBackgroundWorker.ReportProgress(percentage);

            //            var azureUuidStr = device.UUID;
            //            var baseURL = ApplicationSettings.GetSettingValue("BaseURL");
            //           // var completeUrl = $"{baseURL}/api/iCreateApp?azureUuid={azureUuidStr}";
            //            var completeUrl = $"{baseURL}/api/iCreateApp?code=8A0FgdkgnBY0M7a/xSZ/73W4B0t5Ya4AeckTFNHKtSBakdEhTE6v0w==&azureUuid={azureUuidStr}";

            //            progressLabel.WriteTextOnLabel($"Creating Application with UUID {device.SerialNumber}, Please Wait...");
            //            var webResponse = HelperFunctions.GetWebResponse(completeUrl);
            //            var iCodeResponse = JsonHelper.ToClass<CodeResponse>(webResponse);
            //            progressLabel.WriteTextOnLabel(string.Empty);

            //            if (iCodeResponse.Successful)
            //            {
            //                MessageBox.Show($"Application with Azure UUID {azureUuidStr} has been created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                LogWriter.WriteToLog($"Application Created for UUId: {azureUuidStr}.");
            //            }
            //            else
            //            {
            //                HelperFunctions.DisplayError($"Application with Azure UUID {azureUuidStr} could not be created. Exception : {iCodeResponse.ErrorMessage}");
            //                LogWriter.WriteToLog($"Application could not be created for UUId: {azureUuidStr}. Exception Message: {iCodeResponse.ErrorMessage}");
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        LogWriter.WriteToLog($"Exception {ex.Message} while creating application");
            //    }
            //}
        }
    }
}
