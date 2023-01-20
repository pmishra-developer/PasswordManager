using Configurator.Repositories.Contracts;
using Configurator.ViewModel;
using FTD2XX_NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using AutoMapper;
using Configurator.Database.Entities;

namespace Configurator.Application.Dialogs
{
    public partial class DeviceTestForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly IFunctionApp _functionApp;
        private readonly AppSettings _settings;
        private readonly IDeviceRepository _deviceRepository;

        private readonly ILookupRepository _lookupRepository;
        //DeviceViewModel testResults = new DeviceViewModel();


        // ***************************************************************************************************************************************************************************
        // ******************************************************************************** FT234X Comms *****************************************************************************
        // ***************************************************************************************************************************************************************************
        const int MAX_FT_DEVICES = 20;
        const bool ASSERT = true;
        const bool NEGATE = false;
        const byte FT230X_STM32_RESET = 0x02;
        const byte FT230X_STM32_BOOT0 = 0x08;
        const byte FT230X_STM32_USB_PWR = 0x04;

        FTDI ftdiDevice = new FTDI();
        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[MAX_FT_DEVICES];

        static int iCodeIdx;
        static UInt32 ftdiDeviceCount;

        static string iCodeSerialNumber = "";
        static byte[] iCodeUuid = new byte[12];

        public DeviceTestForm(IServiceProvider serviceProvider,
                              IMapper mapper,
                              IOptions<AppSettings> settings,
                              IFunctionApp functionApp,
                              IDeviceRepository deviceRepository, 
                              ILookupRepository lookupRepository)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _functionApp = functionApp;
            _settings = settings.Value;
            _deviceRepository = deviceRepository;
            _lookupRepository = lookupRepository;

            InitializeComponent();
            ft230xCloseDevice();

            loadBootloaderBackgroundWorker.DoWork += loadBootloaderBackgroundWorker_DoWork;
            loadBootloaderBackgroundWorker.ProgressChanged += bootloaderBackgroundWorker_ProgressChanged;
            loadBootloaderBackgroundWorker.WorkerReportsProgress = true;

            loadApplicationBackgroundWorker.DoWork += loadApplicationBackgroundWorker_DoWork;
            loadApplicationBackgroundWorker.ProgressChanged += ApplicationBackgroundWorker_ProgressChanged;
            loadApplicationBackgroundWorker.WorkerReportsProgress = true;
        }


        ushort crc16Update(byte[] data)
        {
            ushort tCrc, xorVal = 0xA001;
            ushort crc16 = 0xABCD; 

            for (int i=0; i<data.Length; i++)
            {
                crc16 ^= data[i];
                for (int j=0; j<8; j++)
                {
                    if ((crc16 & 0x0001) == 0x0001)
                        crc16 = (ushort)((crc16 >> 1) ^ 0xA001);
                    else
						crc16 = (ushort)(crc16 >> 1);
                }
            }
            return crc16;
        }
            
            /*
            crc16_update(self, data, crc16 = 0xABCD): ## CRC16 - Custom
		for x in data:
			crc16 ^= x;
			for i in range(0,8) :
					if (crc16 & 1):
							crc16 = (crc16 >> 1) ^ 0xA001;
					else:
							crc16 = (crc16 >> 1);
		return crc16;	
            */

        private void btniCodeOptionsOK_Click(object sender, EventArgs e)
        {
            //algChecks();
            //byte[] fobikSaltData = new byte[17] {0xA5, 0x00, 0x11, 0xF3, 0x04, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0xF0, 0x50 };
            //crc16Update(fobikSaltData);

            if (radioButtonBootloader.Checked)
            {
                loadBootloaderBackgroundWorker.RunWorkerAsync();
            }
            else if(radioButtonCreateApplication.Checked)
            {
                var createApplicationForm = _serviceProvider.GetRequiredService<CreateApplicationForm>();
                createApplicationForm.ShowDialog();
            }
            else
            {
                loadApplicationBackgroundWorker.RunWorkerAsync();
            }
        }

        private void ApplicationBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.SetValue(e.ProgressPercentage);
        }

        private void loadApplicationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] dataBlock = new byte[BLOCK_SIZE];

            var result = MessageBox.Show("Connect iCode to PC using USB cable", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No)
                return;

            bool success = initFt234x();

            var device = _deviceRepository.FirstOrDefaultAsync(x=>x.SerialNumber == iCodeSerialNumber).Result;
            if (device == null)
            {
                HelperFunctions.DisplayError($"No Device Found with Serial Number {iCodeSerialNumber}");
                return;
            }

            //byte[] azureUuid = StringToByteArray(device.UUID);
            
            if (success)
                success = ft230xSetCbusOutput(FT230X_STM32_BOOT0, NEGATE);      // make sure STM32 is not in ST bootloader mode
            
            if (success)
                success = setFt234xRts(true);    // Stay in bootloader, when resetStm32
            
            if (success)
                success = resetStm32();
            
            if (success)
                success = setUsbPwr(ASSERT);

            Thread.Sleep(1000);
            
            if (success)
                bootloaderOpenDevice();

            if (success)
            {
                success = bootloaderPing();
                if (success == false)
                    HelperFunctions.DisplayError("No Ping.");
            }
            /*
            if (success)
            {
                success = bootloaderReadSerial();
                if (success == false)
                    MessageBox.Show("Failed To Unlock", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            */
            if (success)
            {
                success = bootloaderUnlock();
                if (success == false)
                    HelperFunctions.DisplayError("Failed To Unlock.");
            }
            if (success)
            {
                success = bootloaderEraseApp();
                if (success == false)
                    HelperFunctions.DisplayError("Failed To Erase App.");
            }
            if (success)
            {
                int nrBlocks = (MAX_APP_SIZE + MAX_SIGNATURE_SIZE) / BLOCK_SIZE;
                
                byte[] encryptedApplication = default;

                var BaseUrl = _settings.BaseUrl;
                var completeUrl = $"{BaseUrl}/api/GetApplicationData?code=bLyp68n99feNpHnIIdsMxBiZIgeiHqSdOPeZQnYVtmla2aYxbYIhNw==&iCodeSerialNumber={iCodeSerialNumber}";
                encryptedApplication = HelperFunctions.StringToByteArray(HelperFunctions.GetWebResponse(completeUrl));
                if (encryptedApplication == null)
                {
                    HelperFunctions.DisplayError($"Application Not found for serial number {iCodeSerialNumber}.");
                    return;
                }

                /*

                try
                {
                    encryptedApplication = FunctionApp.GetApplicationContent(iCodeSerialNumber);
                    if (encryptedApplication == null)
                    {
                        HelperFunctions.DisplayError($"Application Not found for serial number {iCodeSerialNumber}.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Subscription"))
                    {
                        DialogResult dlgres = MessageBox.Show($"Subscription not found for serial number {iCodeSerialNumber}, Do you want to create one ?", "Missing Subscription", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dlgres == DialogResult.Yes)
                        {
                            SubscriptionDlg subscriptionDlg = new SubscriptionDlg();
                            subscriptionDlg.iCodeSerialNumber = iCodeSerialNumber;
                            subscriptionDlg.ShowDialog();
                        }
                    }
                    else
                    {
                        HelperFunctions.DisplayError(ex.Message);
                    }
                    return;
                }

                */

                int pgmBlockAddr;

                textBoxLoadStm32.TextBoxVisible(true);
                textBoxLoadStm32.SetTextBoxColor(Color.LightBlue);
                textBoxLoadStm32.WriteTextOnTextBox("Loading App......");

                //textBoxLoadStm32.Visible = true;
                //textBoxLoadStm32.BackColor = Color.LightBlue;
                //textBoxLoadStm32.Text = "Loading App......";
                //textBoxLoadStm32.ForeColor = Color.Black;
                //textBoxLoadStm32.Update();

                progressBar1.ProgressBarVisible(true, true);
                //progressBar1.Enabled = true;
                //progressBar1.Visible = true;
                progressBar1.SetMaximumValue(nrBlocks);
                //progressBar1.Maximum = nrBlocks;
                //progressBar1.Value = 0;
                loadApplicationBackgroundWorker.ReportProgress(0);
                int i = 0;
                for (int blockIdx = 0; blockIdx < nrBlocks; blockIdx++)
                {
                    pgmBlockAddr = blockIdx * BLOCK_SIZE;
                    for (int index = 0; index < BLOCK_SIZE; index++)
                        dataBlock[index] = encryptedApplication[pgmBlockAddr + index];
                    if (success)
                    {
                        if (WriteAppBlock(APPLICATION_START_ADDRESS + pgmBlockAddr, dataBlock) == false)
                        {
                            HelperFunctions.DisplayError("Failed To Write Block: " + pgmBlockAddr.ToString("X3"));
                            progressBar1.ProgressBarVisible(false, false);
                            textBoxLoadStm32.TextBoxVisible(false);
                            //progressBar1.Enabled = false;
                            //progressBar1.Visible = false;
                            //textBoxLoadStm32.Visible = false;
                            break;
                        }
                    }
                    loadApplicationBackgroundWorker.ReportProgress(i++);
                    //progressBar1.Value += 1;
                }

                if (success)
                {
                    if (bootloaderFinishAppWrite() == false)
                        HelperFunctions.DisplayError("Failed To Finish App Write");
                }

                progressBar1.ProgressBarVisible(false, false);
                textBoxLoadStm32.TextBoxVisible(false);
                //progressBar1.Enabled = false;
                //progressBar1.Visible = false;
                //textBoxLoadStm32.Visible = false;
            }
            if (success)
                success = setFt234xRts(false);    // exit bootloader on reset micro
            if (success)
                success = resetStm32();
            ft230xCloseDevice();

            if (success)
                MessageBox.Show("App Downloaded & running", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                HelperFunctions.DisplayError("Failed to Download App");
        }

        private void bootloaderBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.SetValue(e.ProgressPercentage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadBootloaderBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           byte[] unitRandomId = new byte[16];                             // 16 Bytes Unit Random Id                  (0x20-0x2F)
           byte[] unitRandomKey = new byte[16];
            var iCodeUuidString = "";
            // Running on the worker thread
           progressBar1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar1.Visible = false;
            });

            textEraseFlash.TextBoxVisible(false);
            textBoxLoadStm32.TextBoxVisible(false);
            textBoxSerialNumberRes.TextBoxVisible(false);
            textBoxUuidRes.TextBoxVisible(false);

            var result = MessageBox.Show("Connect iCode to PC using USB cable", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No)
                return;
            bool success = initFt234x();
            if (success)
                success = ft230xSetCbusOutput(FT230X_STM32_BOOT0, ASSERT);
            if (success)
                success = setFt234xRts(true);    // Stay in bootloader, when resetStm32. Although this doesn't matter here since STM32 bootloader is invoked
            if (success)
                success = resetStm32();
            if (success)
                success = setUsbPwr(ASSERT);
            if (success)
            {
                success = stm32OpenDevice();
                if (success)
                {
                    textBoxSerialNumberRes.WriteTextOnTextBox(iCodeSerialNumber);
                    textBoxSerialNumberRes.TextBoxVisible(true);
                }
            }
            if (success)
                success = stm32UnprotectRead();
            if (success)
                success = stm32UnprotectWrite();
            if (success)
                success = stm32GlobalErase();
/*  Not necessary ????????????
            if (success)
                success = stm32ReadID();    // ID stored in stm32Id 
*/
            if (success)
                success = stm32ReadUUID();  // UUID stored in stm32Uuid
            if (success)
                success = stm32LoadApp();
            if (success)
            {
                var status = uploadConfigBlock(stm32Uuid);
                success = status.status;
                unitRandomId = status.randomId;
                unitRandomKey = status.randomKey;
            }
            if (success)
                success = stm32Go(0x08000000);
            Thread.Sleep(1000);
            if (success)
                bootloaderOpenDevice();
            if (success)
            {
                success = bootloaderPing();
                if (success == false)
                {
                    //MessageBox.Show("No Ping", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogWriter.WriteToLog("No Ping");
                }
            }
            if (success)
            {
                //success = bootloaderReadSerial();       // Bootloader Get UUID
                if (success)
                {
                    for (int i = 0; i < 12; i++)
                        iCodeUuidString += (stm32Uuid[i].ToString("X2"));
                    iCodeUuidString.Trim();

                    textBoxUuidRes.WriteTextOnTextBox(iCodeUuidString + "89ABCDEF");
                    textBoxUuidRes.TextBoxVisible(true);
                }
            }
            if (success)
            {
                success = ft230xSetCbusOutput(FT230X_STM32_BOOT0, NEGATE);                
            }

            ft230xCloseDevice();
            //  Store iCodeSerialNumber in "iCode_Sensitive_Data" database 
            //  Store iCodeUuidString in "iCode_Sensitive_Data" database 
            //  Store iCodeTargetMarket in "iCode_Sensitive_Data" database 
            if (success)
            {
                
                string selectedTargetMarket = string.Empty;
                this.Invoke((MethodInvoker)delegate ()
                {
                    selectedTargetMarket = targetMarketCombo.SelectedItem.ToString();
                });

                if (selectedTargetMarket != string.Empty)
                {
                    var existingDevice = _deviceRepository.FirstOrDefaultAsync(x => x.SerialNumber == iCodeSerialNumber).Result;

                    int deviceId = 0;
                    if (existingDevice != null)
                    {
                        deviceId = existingDevice.Id;
                    }

                    var deviceViewModel = new DeviceViewModel(deviceId, iCodeSerialNumber, iCodeUuidString, selectedTargetMarket, unitRandomId, unitRandomKey);
                    var newDevice = _mapper.Map<Device>(deviceViewModel);
                    _deviceRepository.AddAsync(newDevice);

                    //TODO
                    //_deviceRepository .SaveDevice(deviceViewModel);
                }
                MessageBox.Show("iCode Bootloader Loaded successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Failed to Load iCode Bootloader", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void DeviceTestDlg_Load(object sender, EventArgs e)
        {
            if(IsAdministrator() && _settings.ConfiguratorMode)
            {
                buttonLookupData.Visible = true;
                btnUsers.Visible = true;
            }

            var targetMarket = _settings.TargetMarketOptions;
            targetMarketCombo.DataSource = targetMarket.Split(',').Select(x => x.Trim()).ToList();
        }

        private bool ftConnectDevice()
        {
            ftdiDeviceCount = 0;
            bool foundFtdiDevice;
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            foundFtdiDevice = false;
            do
            {
                ftStatus = ftdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
                if ((ftStatus == FTDI.FT_STATUS.FT_OK) && (ftdiDeviceCount != 0))
                    foundFtdiDevice = true;
                else
                {
                    HelperFunctions.DisplayError("Unable to detect FTDI devices");
                    return false;
                }
            }
            while (!foundFtdiDevice);
            return true;
        }



        private bool ftPopulateDeviceList()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            ftStatus = ftdiDevice.GetDeviceList(ftdiDeviceList);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to populate device list");
                return false;
            }
            return true;
        }

        private bool ftFindiCode()
        {
            var lc234xIdx = 0;
            bool lc234xFound = false;
            int i = 0;
            do
            {
                lc234xFound = (ftdiDeviceList[i].Description == "LC234X");
                if (lc234xFound)
                    lc234xIdx = i;
            }
            while ((lc234xFound == false) && (++i < ftdiDeviceCount));

            bool iCodefound = false;
            iCodeIdx = 0;
            do
            {
                iCodefound = (ftdiDeviceList[iCodeIdx].Description == "iCode");
                if (iCodefound)
                    return true;
            }
            while ((iCodefound == false) && (++iCodeIdx < ftdiDeviceCount));
            return false;
        }


        private bool ftPurgeDevice()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            ftStatus = ftdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_TX | FTDI.FT_PURGE.FT_PURGE_RX);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to Purge device");
                return false;
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ft230xOpenDevice()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            if (ftdiDevice.IsOpen == true)
                return true;

            ftStatus = ftdiDevice.OpenBySerialNumber(ftdiDeviceList[iCodeIdx].SerialNumber);
            bool success = (ftStatus == FTDI.FT_STATUS.FT_OK);
            if (success == true)
            {
                iCodeSerialNumber = ftdiDeviceList[iCodeIdx].SerialNumber;
            }
            else
            {
                HelperFunctions.DisplayError("Failed to open device by serial number (Error: " + ftStatus.ToString() + ")");
                ftdiDevice.Close();
            }
            Thread.Sleep(100);
            return success;
        }



        private bool ft230xCloseDevice()
        {
            if (ftdiDevice.IsOpen)
            {
                FTDI.FT_STATUS ftStatus = ftdiDevice.Close();
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    HelperFunctions.DisplayError("Failed to Close Device");
                    return false;
                }
            }
            return true;
        }



        FTDI.FT_XSERIES_EEPROM_STRUCTURE ftdiEEData = new FTDI.FT_XSERIES_EEPROM_STRUCTURE();

        private bool ft230xCBUSPinsDefaultState()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus = ftdiDevice.SetBitMode(0xE2, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are outputs. 0xn2 CBUS3(STM32 BOOT0)-Low,  CBUS1(STM32 reset)-High.  CBUS2,CBUS0 are don't care
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                HelperFunctions.DisplayError("Failed to SET both CBUS1 and CBUS3 pins");

            return true;
        }



        private bool ft230xSetCbusOutput(byte pinName, bool pinState)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte bitMode = 0;
            ftStatus = ftdiDevice.GetPinStates(ref bitMode);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to get FT230X Pins State");
                return false;
            }
            switch (pinName)
            {
                case FT230X_STM32_RESET:
                    if (pinState == ASSERT)
                    {
                        bitMode &= 0xFD;                                        // Assert Reset by pulling CBUS1 low
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to ASSERT STM32 Micro Reset");
                    }
                    if (pinState == NEGATE)
                    {
                        bitMode |= 0x02;                                        // Negate Reset by setting CBUS1 high
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to Negate STM32 Micro Reset");
                    }
                    break;
                case FT230X_STM32_BOOT0:
                    if (pinState == ASSERT)
                    {
                        bitMode |= 0x08;                                        // Assert BOOT0 by setting CBUS3 high
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to ASSERT STM32 BOOT0");
                    }
                    if (pinState == NEGATE)
                    {
                        bitMode &= 0xF7;                                        // Negate BOOT0 by pulling CBUS3 low
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to NEGATE STM32 BOOT0");
                    }
                    break;
                case FT230X_STM32_USB_PWR:
                    if (pinState == NEGATE)
                    {
                        bitMode |= 0x04;                                        // Assert USB_PWR by setting CBUS2 low
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to ASSERT STM32 USB_PWR");
                    }
                    if (pinState == ASSERT)
                    {
                        bitMode &= 0xFB;                                        // Negate USB_PWR by pulling CBUS2 High
                        ftStatus = ftdiDevice.SetBitMode(bitMode, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG); // 0xAn - CBUS3 & CBUS1 are used, 0xnA CBUS3 & CBUS1 are outputs and CBUS2,CBUS0 are don't care. 0x20 - mode for CBUS to be GPIO
                        if (ftStatus != FTDI.FT_STATUS.FT_OK)
                            HelperFunctions.DisplayError("Failed to NEGATE STM32 USB_PWR");
                    }
                    break;
                default:
                    HelperFunctions.DisplayError("Pin Name not recognised");
                    break;
            }
            return true;
        }


        private bool initFt234x()
        {
            var success = ftConnectDevice();
            if (success)
                success = ftPopulateDeviceList();
            if (success)
                success = ftFindiCode();
            if (success)
                success = ft230xOpenDevice();
            return success;
        }


        private bool resetStm32()
        {
            var success = ft230xSetCbusOutput(FT230X_STM32_RESET, ASSERT);
            if (success)
            {
                Thread.Sleep(100);                                          // apply Reset for 100mS
                success = ft230xSetCbusOutput(FT230X_STM32_RESET, NEGATE);
            }
            if (success)
                Thread.Sleep(100);                                          // wait a while to allow the STM32 to configure itself
            return success;
        }


        private bool setUsbPwr(bool useUsbPwrOpt)
        {
            var success = ft230xSetCbusOutput(FT230X_STM32_USB_PWR, useUsbPwrOpt);
            return success;
        }

        private bool setFt234xRts(bool enableRts)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus = ftdiDevice.SetRTS(enableRts);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to enable RTS");
                return false;
            }
            return true;
        }


        #region STM32

        // ***************************************************************************************************************************************************************************
        // ******************************************************************************** STM32 Comms ******************************************************************************
        // Uses FT234 Comms methods
        // ***************************************************************************************************************************************************************************

        const int STM32_BOOTLOADER_BAUDRATE = 115200;                   // max speed of stm32 bootloader
        const int STM32_START_BOOTLOADER_CMD = 0x7F;
        const int STM32_BOOTLOADER_GET_VER_CMD = 0x00;
        const int STM32_VER_LEN = 13;
        const int STM32_BOOTLOADER_GET_ID_CMD = 0x02;
        const int STM32_ID_LEN = 3;
        const int STM32_BOOTLOADER_READ_MEMORY_CMD = 0x11;
        const int STM32_BOOTLOADER_GO_CMD = 0x21;
        const int STM32_BOOTLOADER_WRITE_MEMORY_CMD = 0x31;
        const int STM32_BOOTLOADER_ERASE_MEMORY_CMD = 0x43;
        const int STM32_BOOTLOADER_EXTENDED_ERASE_MEMORY_CMD = 0x44;
        const int STM32_BOOTLOADER_WRITE_PROTECT_CMD = 0x63;
        const int STM32_BOOTLOADER_WRITE_UNPROTECT_CMD = 0x73;
        const int STM32_BOOTLOADER_READ_PROTECT_CMD = 0x82;
        const int STM32_BOOTLOADER_READ_UNPROTECT_CMD = 0x92;
        const int STM32_BOOTLOADER_ACK = 0x79;
        const int STM32_ACK_LEN = 1;
        const int STM32_BOOTLOADER_NACK = 0x1F;
        const int STM32_NACK_LEN = 1;

        const int STM32_WRITE_ARRAY_SIZE = 0x80;


        private bool stm32OpenDevice()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus = ftdiDevice.SetBaudRate(STM32_BOOTLOADER_BAUDRATE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set baudrate");
                return false;
            }
            ftStatus = ftdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_EVEN);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set data characteristics");
                return false;
            }
            ftStatus = ftdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set flow control");
                return false;
            }
            ftStatus = ftdiDevice.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set Read & Write Timeouts");
                return false;
            }
            return true;
        }





        private bool stm32RxBytesAvailable(int numBytes, int timeout)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            UInt32 bytesAvail = 0;
            int timeCnt = 0;
            do
            {
                ftStatus = ftdiDevice.GetRxBytesAvailable(ref bytesAvail);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    HelperFunctions.DisplayError("Failed to get number of bytes available");
                    return false;
                }
                Thread.Sleep(10);
                if (++timeCnt == timeout)
                {
                    HelperFunctions.DisplayError("Timed Out\nBytes Received: " + bytesAvail.ToString());
                    return false;
                }
            }
            while (bytesAvail < numBytes);
            return true;
        }



        private bool stm32SendBytes(int txbytesLen, byte[] txBytes)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte[] tempTxBuffer = new byte[2];
            uint bytesTxd = 0;
            for (int i = 0; i < txbytesLen; i++)
            {
                tempTxBuffer[0] = txBytes[i];
                ftStatus = ftdiDevice.Write(tempTxBuffer, 1, ref bytesTxd);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    HelperFunctions.DisplayError("Failed to Send Bootloader CMD(S)");
                    return false;
                }
            }
            return true;
        }


        private bool stm32CheckAck(int timeOut)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte[] stm32RxBuffer = new byte[10];
            UInt32 bytesRead = 0;

            if (stm32RxBytesAvailable(STM32_ACK_LEN, timeOut) == false)
                return false;

            ftStatus = ftdiDevice.Read(stm32RxBuffer, STM32_ACK_LEN, ref bytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to read ACK");
                return false;
            }
            switch (stm32RxBuffer[0])
            {
                case STM32_BOOTLOADER_ACK:
                    //stm32InfoListBox.Items.Add("ACK");
                    return true;
                case STM32_BOOTLOADER_NACK:
                    //stm32InfoListBox.Items.Add("NACK");
                    break;
                default:
                    //stm32InfoListBox.Items.Add("ACK Error: " + stm32RxBuffer[0].ToString("X"));
                    break;
            }
            //MessageBox.Show("ACK Error: " + stm32RxBuffer[0].ToString("X"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }



        private bool stm32InvokeBootloader()
        {
            ft230xSetCbusOutput(FT230X_STM32_BOOT0, ASSERT);    // hold BOOT0 pin high
            Thread.Sleep(250);
            ft230xSetCbusOutput(FT230X_STM32_RESET, ASSERT);
            Thread.Sleep(100);
            ft230xSetCbusOutput(FT230X_STM32_RESET, NEGATE);    // with BOOT0 high, pulse reset pin to enter STM32 bootloader
            Thread.Sleep(100);
            byte[] stm32TxBuffer = new byte[10];
            byte[] stm32RxBuffer = new byte[10];
            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_START_BOOTLOADER_CMD;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            return stm32CheckAck(100);
        }

        byte[] stm32Id = new byte[2];
        private bool stm32ReadID()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte[] stm32TxBuffer = new byte[10];
            byte[] stm32RxBuffer = new byte[10];
            UInt32 bytesRead = 0;

            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_GET_ID_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_GET_ID_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            ftStatus = ftdiDevice.Read(stm32RxBuffer, 1, ref bytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return false;
            if (stm32RxBuffer[0] != 1) // N = n-1
                return false;
            ftStatus = ftdiDevice.Read(stm32RxBuffer, 2, ref bytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return false;
            stm32Id[0] = stm32RxBuffer[0];
            stm32Id[1] = stm32RxBuffer[1];
            if (!stm32CheckAck(250))
                return false;
            return true;
        }



        byte[] stm32ReadFlashBuffer = new byte[256];
        private bool stm32ReadFlash(UInt32 readAddr, Byte readLen)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte[] stm32TxBuffer = new byte[10];
            byte[] stm32RxBuffer = new byte[256];
            UInt32 bytesRead = 0;

            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_READ_MEMORY_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_READ_MEMORY_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            txLen = 0;
            Byte checkSum = 0x00;
            stm32TxBuffer[txLen++] = (Byte)((readAddr >> 24) & 0xFF);
            stm32TxBuffer[txLen++] = (Byte)((readAddr >> 16) & 0xFF);
            stm32TxBuffer[txLen++] = (Byte)((readAddr >> 8) & 0xFF);
            stm32TxBuffer[txLen++] = (Byte)((readAddr) & 0xFF);
            for (int i=0; i<txLen; i++)
                checkSum ^= stm32TxBuffer[i];
            stm32TxBuffer[txLen++] = checkSum;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            txLen = 0;
            Byte sendLen = (Byte)(readLen - 1);
            stm32TxBuffer[txLen++] = sendLen;
            stm32TxBuffer[txLen++] = (Byte)(sendLen ^ 0xFF);
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            ftStatus = ftdiDevice.Read(stm32RxBuffer, readLen, ref bytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return false;
            for (int i=0; i< readLen; i++)
                stm32ReadFlashBuffer[i] = stm32RxBuffer[i];
            return true;
        }


        byte[] stm32Uuid = new byte[12];
        private bool stm32ReadUUID()
        {
            bool success = stm32ReadFlash(0x1FF1E800, 12);
            if (success)
            {
                for (int i = 0; i < 12; i++)
                    stm32Uuid[i] = stm32ReadFlashBuffer[i];
            }
            return success;
        }

        private bool stm32UnprotectRead()
        {
            byte[] stm32TxBuffer = new byte[10];

            if (!stm32InvokeBootloader())
                return false;
            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_READ_UNPROTECT_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_READ_UNPROTECT_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            if (!stm32CheckAck(250))
                return false;
            return true;
        }

        private bool stm32UnprotectWrite()
        {
            byte[] stm32TxBuffer = new byte[10];

            if (!stm32InvokeBootloader())
                return false;
            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_WRITE_UNPROTECT_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_WRITE_UNPROTECT_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            if (!stm32CheckAck(250))
                return false;
            return true;
        }


        private bool stm32GlobalErase()
        {
            if (!stm32InvokeBootloader())
                return false;
            byte[] stm32TxBuffer = new byte[10];
            byte[] stm32RxBuffer = new byte[10];
            textEraseFlash.TextBoxVisible(true);
            textEraseFlash.SetTextBoxColor(Color.Red);
            textEraseFlash.WriteTextOnTextBox("Erasing STM32 Flash....");
            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_EXTENDED_ERASE_MEMORY_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_EXTENDED_ERASE_MEMORY_CMD ^ 0xFF;
            var success = stm32SendBytes(txLen, stm32TxBuffer);
            if (success)
                success = stm32CheckAck(250);
            if (success)
            {
                txLen = 0;
                stm32TxBuffer[txLen++] = 0xFF;  // 0xFFFF is erase all pages
                stm32TxBuffer[txLen++] = 0xFF;
                stm32TxBuffer[txLen++] = 0x00;  // checksum 0xFF ^ 0xFF, previous 2 bytes
                success = stm32SendBytes(txLen, stm32TxBuffer);
            }
            if (success)
                success = stm32CheckAck(1000);
            textEraseFlash.TextBoxVisible(false);
            //textEraseFlash.Visible = false;
            return success;
        }

        private bool stm32WriteBytes(UInt32 writeAddr, byte numWriteBytes, byte[] writeBytesBuffer)
        {
            byte[] stm32TxBuffer = new byte[260];
            byte[] stm32RxBuffer = new byte[260];
            byte Nvalue = numWriteBytes;

            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_WRITE_MEMORY_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_WRITE_MEMORY_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;

            txLen = 0;
            stm32TxBuffer[txLen++] = (byte)((writeAddr >> 24) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)((writeAddr >> 16) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)((writeAddr >> 8) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)(writeAddr & 0xFF);
            byte checkSum = 0;
            for (int i = 0; i < 4; i++)
                checkSum ^= stm32TxBuffer[i];
            stm32TxBuffer[txLen++] = checkSum;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;

            Nvalue--;
            txLen = 0;
            checkSum = 0;
            stm32TxBuffer[txLen++] = Nvalue;
            checkSum ^= Nvalue;
            for (int i = 0; i < numWriteBytes; i++)
            {
                stm32TxBuffer[txLen++] = writeBytesBuffer[i];
                checkSum ^= writeBytesBuffer[i];
            }
            stm32TxBuffer[txLen++] = checkSum;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            return true;
        }

        private bool stm32Go(UInt32 goAddr)
        {
            byte[] stm32TxBuffer = new byte[10];

            int txLen = 0;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_GO_CMD;
            stm32TxBuffer[txLen++] = STM32_BOOTLOADER_GO_CMD ^ 0xFF;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            txLen = 0;
            stm32TxBuffer[txLen++] = (byte)((goAddr >> 24) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)((goAddr >> 16) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)((goAddr >> 8) & 0xFF);
            stm32TxBuffer[txLen++] = (byte)(goAddr & 0xFF);
            byte checkSum = 0;
            for (int i = 0; i < 4; i++)
                checkSum ^= stm32TxBuffer[i];
            stm32TxBuffer[txLen++] = checkSum;
            if (stm32SendBytes(txLen, stm32TxBuffer) == false)
                return false;
            if (!stm32CheckAck(250))
                return false;
            return true;
        }

        #endregion

        // **********************************************************************************************************************************************************************
        // **************************************************************************** Bootloader Comms ************************************************************************
        // Uses FT234 Comms methods
        // **********************************************************************************************************************************************************************
        const int DOWNLOAD_BAUDRATE = 921600;
        byte[] txBuffer = new byte[512];
        byte[] rxBuffer = new byte[512];
        int txLen;

        // Responses ####
        const byte CHAR_ACK = 0x06;
        const byte CHAR_NACK = 0x15;
        const byte CHAR_PENDING = 0x31;

        // Commands ####
        const byte CMD_APP_ERASE = 0x80;
        const byte CMD_WRITE_BLOCK = 0x81;
        const byte CMD_WRITE_ENC_BLOCK = 0xA1;
        const byte CMD_FINISH = 0x82;
        const byte CMD_PING = 0x83;
        const byte CMD_APP_START = 0x84;
        const byte CMD_APP_VALID = 0x85;

        const byte CMD_ERASE_CONFIG = 0x88;
        const byte CMD_WRITE_CONFIG = 0xAB;
        const byte CMD_FINISH_CONFIG = 0xBA;

        const byte CMD_MASS_ERASE = 0x89;
        const byte CMD_RESET = 0x8A;
        const byte CMD_READ_SERIAL = 0x8B;
        const byte CMD_SEED_REQUEST = 0x90;
        const byte CMD_SEED_RESPONSE = 0x91;
        const byte CMD_READ_RND = 0x9F;

        private bool bootloaderOpenDevice()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus = ftdiDevice.SetBaudRate(DOWNLOAD_BAUDRATE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set baudrate");
                return false;
            }
            ftStatus = ftdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set data characteristics");
                return false;
            }
            ftStatus = ftdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set flow control");
                return false;
            }
            ftStatus = ftdiDevice.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to set Read & Write Timeouts");
                return false;
            }
            return true;
        }

        private bool bootloaderTxMessage()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            uint bytesTxd = 0;

            ftStatus = ftdiDevice.Write(txBuffer, txLen, ref bytesTxd);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to transmit message");
                return false;
            }
            return true;
        }

        private bool bootloaderRxMessage(UInt32 NumBytesExpected, int timeOut)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            byte[] wrappedBuffer = new byte[250];
            byte[] bytesReadBuffer = new byte[250];
            UInt32 bytesAvail = 0;
            UInt32 bytesRead = 0;
            UInt32 timeCnt = 0;

            do
            {
                ftStatus = ftdiDevice.GetRxBytesAvailable(ref bytesAvail);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    HelperFunctions.DisplayError("Failed to get number of bytes available");
                    return false;
                }
                Thread.Sleep(1);
                if (++timeCnt == timeOut)
                    return false;
            }
            while (bytesAvail != NumBytesExpected);

            ftStatus = ftdiDevice.Read(bytesReadBuffer, bytesAvail, ref bytesRead);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                HelperFunctions.DisplayError("Failed to read bytes received");
                return false;
            }
            if (bytesAvail != bytesRead)
                return false;
            for (int i = 0; i < bytesRead; i++)
                rxBuffer[i] = bytesReadBuffer[i];
            return true;
        }



        private bool bootloaderTxRxMessage(UInt32 NumRxBytesExpected, int timeOut)
        {
            if (bootloaderTxMessage())
                if (bootloaderRxMessage(NumRxBytesExpected, timeOut))
                    return true;
            return false;
        }


        private bool bootloaderPing()
        {
            txLen = 0;
            txBuffer[txLen++] = CMD_PING;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_PING;
            if (bootloaderTxRxMessage(8, 100) == false)  // 8 bytes expected, timeout in 1000 mS
                return false;
            return true;
        }


        private bool bootloaderReadSerial()
        {
            txLen = 0;
            txBuffer[txLen++] = CMD_READ_SERIAL;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_READ_SERIAL;
            if (bootloaderTxRxMessage(13, 1000) == false)  // 13 bytes expected, timeout in 1000 mS
                return false;
            if (rxBuffer[12] != CHAR_ACK)
                return false;
            for (int i = 0; i < 12; i++)
                iCodeUuid[i] = rxBuffer[i];
            return true;
        }

        /*
        private bool bootloaderUnlock()
        {
            byte[] IV = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };                    // can be anything for AES ECB
            byte[] App_Key_Security = new byte[16] { 0xC1, 0x94, 0xFF, 0x31, 0x2F, 0xAB, 0xCC, 0x6D, 0x7E, 0x19, 0x94, 0x03, 0x8F, 0x58, 0xAF, 0x31 };
            byte[] seedData = new byte[16];
            byte[] bytesToEncrypt = new byte[16];

            txLen = 0;
            txBuffer[txLen++] = CMD_SEED_REQUEST;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_SEED_REQUEST;

            if (bootloaderTxRxMessage(17, 1000) == false)  // 8 bytes expected, timeout in 1000 mS
                return false;

            if (rxBuffer[16] != CHAR_ACK)
                return false;

            for (int i = 0; i < 16; i++)
                seedData[i] = rxBuffer[i];

            for (int i = 0; i < 12; i++)
                bytesToEncrypt[i] = iCodeUuid[i];

            for (int i = 12; i < 16; i++)
                bytesToEncrypt[i] = seedData[i];

            byte[] encryptedData = aesEncrypt(App_Key_Security, IV, AES_ECB, bytesToEncrypt);

            byte[] decryptSecurityKey = new byte[16];
            for (int i = 0; i < 16; i++)
                decryptSecurityKey[i] = encryptedData[i];
            byte[] decryptedData = aesDecrypt(decryptSecurityKey, IV, AES_ECB, seedData);

            for (int i = 0; i < 16; i++)
                decryptedData[i] ^= 0xFF;

            txLen = 0;
            txBuffer[txLen++] = CMD_SEED_RESPONSE;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_SEED_RESPONSE;
            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;

            txLen = 0;
            for (int i = 0; i < 16; i++)
                txBuffer[txLen++] = decryptedData[i];
            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;

            return true;
        }

*/
        const int AES_ECB = 0;
        const int AES_CBC = 1;

        public static byte[] aesEncrypt(byte[] securityKey, byte[] IV, int aesMode, byte[] bytesToEncrypt)
        {
            byte[] encrypted;
            MemoryStream mstream = new MemoryStream();
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.KeySize = 128;
            aesProvider.Padding = PaddingMode.None;
            switch (aesMode)
            {
                case AES_ECB:
                    aesProvider.Mode = CipherMode.ECB;
                    break;
                case AES_CBC:
                    aesProvider.Mode = CipherMode.CBC;
                    break;
            }

            ICryptoTransform encryptor = aesProvider.CreateEncryptor(securityKey, IV);
            CryptoStream cryptoStream = new CryptoStream(mstream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
            encrypted = mstream.ToArray();

            return encrypted;
        }

        public static byte[] aesDecrypt(byte[] securityKey, byte[] IV, int aesMode, byte[] bytesToDecrypt)
        {
            byte[] decrypted = new byte[16];
            using (MemoryStream mStream = new MemoryStream(bytesToDecrypt))
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 128;
                    aesProvider.Padding = PaddingMode.None;
                    switch (aesMode)
                    {
                        case AES_ECB:
                            aesProvider.Mode = CipherMode.ECB;
                            break;
                        case AES_CBC:
                            aesProvider.Mode = CipherMode.CBC;
                            break;
                    }
                    ICryptoTransform decryptor = aesProvider.CreateDecryptor(securityKey, IV);
                    using (CryptoStream cryptoStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(bytesToDecrypt, 0, bytesToDecrypt.Length);
                    }
                }
                decrypted = mStream.ToArray();
            }
            return decrypted;
        }


        private bool bootloaderEraseApp()
        {
            //textEraseFlash.Visible = true;
            //textEraseFlash.BackColor = Color.Red;
            //textEraseFlash.Text = "Erasing App Flash....";
            //textEraseFlash.ForeColor = Color.Black;
            //textEraseFlash.Update();

            textEraseFlash.TextBoxVisible(true);
            textEraseFlash.SetTextBoxColor(Color.Red);
            textEraseFlash.WriteTextOnTextBox("Erasing App Flash....");

            txLen = 0;
            txBuffer[txLen++] = CMD_APP_ERASE;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_APP_ERASE;
            if (bootloaderTxRxMessage(1, 30000) == false)  // 1 bytes expected, timeout in 30 Sec
            {
                textEraseFlash.TextBoxVisible(false);
                //textEraseFlash.Visible = false;
                return false;
            }
            textEraseFlash.TextBoxVisible(false);
            //textEraseFlash.Visible = false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;

            return true;
        }


        private bool bootloaderFinishAppWrite()
        {
            txLen = 0;
            txBuffer[txLen++] = CMD_FINISH;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_FINISH;
            if (bootloaderTxRxMessage(1, 5000) == false)  // 1 bytes expected, timeout in 30 Sec
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            return true;
        }



        private bool WriteAppBlock(int startBlockAddr, byte[] blockData)
        {
            byte checkSum = 0x00;
            txLen = 0;
            txBuffer[txLen++] = CMD_WRITE_ENC_BLOCK;
            txBuffer[txLen++] = (byte)((startBlockAddr >> 24) & 0xFF);
            txBuffer[txLen++] = (byte)((startBlockAddr >> 16) & 0xFF);
            txBuffer[txLen++] = (byte)((startBlockAddr >> 8) & 0xFF);
            txBuffer[txLen++] = (byte)(startBlockAddr & 0xFF);
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            for (int i = 0; i < 7; i++)
                checkSum ^= txBuffer[i];
            txBuffer[txLen++] = checkSum;

            for (int k = 0; k < txLen; k++)
                Console.Write(txBuffer[k].ToString());

            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1 s
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            txLen = 0;
            for (int i = 0; i < blockData.Length; i++)
                txBuffer[txLen++] = blockData[i];
            for (int k = 0; k < txLen; k++)
                Console.Write(txBuffer[k].ToString());
            if (bootloaderTxRxMessage(1, 5000) == false)  // 1 bytes expected, timeout in 5000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            return true;
        }


        private bool bootloaderEraseConfig()
        {
            txLen = 0;
            txBuffer[txLen++] = CMD_ERASE_CONFIG;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_ERASE_CONFIG;
            if (bootloaderTxRxMessage(1, 5000) == false)  // 1 bytes expected, timeout in 30 Sec
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            return true;
        }



        private bool bootloaderWriteConfigBlock(int configAddress, byte[] configData)
        {
            byte checkSum = 0x00;
            txLen = 0;
            txBuffer[txLen++] = CMD_WRITE_CONFIG;
            txBuffer[txLen++] = (byte)((configAddress >> 24) & 0xFF);
            txBuffer[txLen++] = (byte)((configAddress >> 16) & 0xFF);
            txBuffer[txLen++] = (byte)((configAddress >> 8) & 0xFF);
            txBuffer[txLen++] = (byte)(configAddress & 0xFF);
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            for (int i = 0; i < 7; i++)
                checkSum ^= txBuffer[i];
            txBuffer[txLen++] = checkSum;
            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1 s
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            txLen = 0;
            for (int i = 0; i < configData.Length; i++)
                txBuffer[txLen++] = configData[i];
            if (bootloaderTxRxMessage(1, 5000) == false)  // 1 bytes expected, timeout in 5000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            return true;
        }



        private bool bootloaderFinishConfigWrite()
        {
            txLen = 0;
            txBuffer[txLen++] = CMD_FINISH_CONFIG;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_FINISH_CONFIG;
            if (bootloaderTxRxMessage(1, 5000) == false)  // 1 bytes expected, timeout in 30 Sec
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            return true;
        }


        private byte[] getUcbOptions()  //To be defined
        {
            byte[] optionData = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            return optionData;
        }

        private (bool status, byte[] randomId, byte[] randomKey) uploadConfigBlock(byte[] microUuid)
        {
            Random rnd = new Random();
            byte[] ucbData = new byte[80];
            int ucbIdx = 0;

            byte[] unitRandomId = new byte[16];                             // 16 Bytes Unit Random Id                  (0x20-0x2F)
            byte[] unitRandomKey = new byte[16];                            // 16 Bytes Unit Random Key                 (0x30-0x3F)

            bool success = false;
            byte[] iCodeSN = Encoding.ASCII.GetBytes(iCodeSerialNumber);    // should be 6 bytes                       (0x00-0x05)
            for (int i = 0; i < 6; i++)
                ucbData[ucbIdx++] = iCodeSN[i];
            for (int i = 0; i < 10; i++)                                     // append 10 bytes of 00                     (0x06-0x0F)
                ucbData[ucbIdx++] = 0x00;

            byte[] optionData = getUcbOptions();                            // 16 Bytes indicating unit options         (0x10-0x1F)
            for (int i = 0; i < 16; i++)
                ucbData[ucbIdx++] = optionData[i];

            rnd.NextBytes(unitRandomId);
            for (int i = 0; i < 16; i++)
                ucbData[ucbIdx++] = unitRandomId[i];

            rnd.NextBytes(unitRandomKey);
            for (int i = 0; i < 16; i++)
                ucbData[ucbIdx++] = unitRandomKey[i];

            for (int i = 0; i < 12; i++)                        // 12 Bytes of UUID data                    (0x40-0x4C)
                ucbData[ucbIdx++] = microUuid[i];
            ucbData[ucbIdx++] = 0x89;
            ucbData[ucbIdx++] = 0xAB;
            ucbData[ucbIdx++] = 0xCD;
            ucbData[ucbIdx++] = 0xEF;
            success = stm32WriteBytes(0x0801FE00, 80, ucbData);
            return (success, unitRandomId, unitRandomKey);
        }





        // ***************************************************************************************************************************************************************************
        // **************************************************************************** Bootloader Loader *****************************************************************************
        // Uses FT234 Comms methods
        // Uses STM32 Comms methods
        // Gets Bootloader filename from a specified local location
        // ***************************************************************************************************************************************************************************


        byte[] Combine(byte[] a1, byte[] a2)
        {
            byte[] ret = new byte[a1.Length + a2.Length];
            Array.Copy(a1, 0, ret, 0, a1.Length);
            Array.Copy(a2, 0, ret, a1.Length, a2.Length);
            return ret;
        }


        IEnumerable<IEnumerable<byte>> Split(byte[] byteArray, int size)
        {
            for (var i = 0; i < (float)byteArray.Length / size; i++)
            {
                yield return byteArray.Skip(i * size).Take(size);
            }
        }


        List<byte[]> GetByteArrayFromFile(string filePath)
        {
            List<byte[]> listByteArrays = new List<byte[]>();
            if (File.Exists(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var totalFileSize = fileBytes.Length;
                var splittedFiles = Split(fileBytes, STM32_WRITE_ARRAY_SIZE).ToList();
                foreach (var splitFile in splittedFiles)
                {
                    listByteArrays.Add(splitFile.ToArray());
                }

                for (int i = 0; i < listByteArrays.Count; i++)
                {
                    var listBytes = listByteArrays[i];
                    var fileSize = listBytes.Length;
                    if (fileSize != STM32_WRITE_ARRAY_SIZE)
                    {
                        var bytesToFill = STM32_WRITE_ARRAY_SIZE - fileSize;

                        // This is our byte Array for remaining Bytes
                        List<byte> bytestoWrite = new List<byte>();
                        for (int b = 0; b < bytesToFill; b++)
                        {
                            bytestoWrite.Add(0xFF);
                        }

                        var newByteArray = Combine(listBytes, bytestoWrite.ToArray());

                        // Replace the last item
                        listByteArrays.RemoveAt(listByteArrays.Count - 1);
                        listByteArrays.Add(newByteArray);
                    }
                }
            }
            return listByteArrays;
        }

        private bool stm32LoadApp()
        {
            var osFilePath = _settings.BootloaderFilePath;
            if (File.Exists(osFilePath))
            {
                var success = true;
                var listByteArrays = GetByteArrayFromFile(osFilePath);
                //var success = stm32InvokeBootloader();
                if (success)
                {
                    Thread.Sleep(10);

                    textBoxLoadStm32.TextBoxVisible(true);
                    textBoxLoadStm32.WriteTextOnTextBox("Loading Bootloader...");
                    textEraseFlash.SetTextBoxColor(Color.LightBlue);

                    UInt32 memoryWriteAddr = 0x08000000;
                    progressBar1.ProgressBarVisible(true);
                    progressBar1.SetMaximumValue(listByteArrays.Count);
                    loadBootloaderBackgroundWorker.ReportProgress(0);
                    progressBar1.SetValue(0);
                    int i = 0;
                    foreach (var listBytes in listByteArrays)
                    {
                        var readArray = listBytes.ToArray();
                        success = stm32WriteBytes(memoryWriteAddr, STM32_WRITE_ARRAY_SIZE, readArray);
                        if (success == false)
                        {
                            HelperFunctions.DisplayError("Error in Downloading Bootloader");
                            progressBar1.ProgressBarVisible(false, false);
                            textBoxLoadStm32.TextBoxVisible(false);
                            return false;
                        }
                        memoryWriteAddr += STM32_WRITE_ARRAY_SIZE;
                        Thread.Sleep(1);
                        loadBootloaderBackgroundWorker.ReportProgress(i++);
                    }
                    progressBar1.ProgressBarVisible(false, false);
                    textBoxLoadStm32.TextBoxVisible(false);
                }
                else
                    HelperFunctions.DisplayError("Failed to invoke STM32 bootloader");

                return true;
            }

            return false;
        }

        /*
         * Write method to enter 'iCodeTargetMarket'
         */




        // Main method for loading Bootloader, getting the serial number , UUID, Target market
        // Azure Database (perhaps called "iCode_Sensitive_Data") required to store:-
        // 1. Serial NUmber - Read from FT234X USB IC (iCodeSerialNumber)
        // 2. UUID Number - Read from iCode via USB
        // 3. Target Market - this must be entered by the user
        private void buttonGo_Click(object sender, EventArgs e)
        {
            loadBootloaderBackgroundWorker.RunWorkerAsync();
        }


        //testResults.CurrentUserId = Utils.LoggedInUser.Id;
        //testResults.UserName = Utils.LoggedInUser.Name;
        //testResults.Successful = true;
        //_deviceRepository.SaveDevice(testResults);



        // ***************************************************************************************************************************************************************************
        // *************************************************************************** Encrypted App Creator *************************************************************************
        // Gets App filename (ApplicationFilePath) from a specified local location
        // Gets RSA key from file (PemFileName) from a specified local location
        // Signs App using SHA256 & RSA
        // Encrypts App using AES-128 CBC
        // 3 Options:- Create All, Create Connected, Create Range
        // ***************************************************************************************************************************************************************************
        const int MAX_APP_SIZE = 0x1DFF00;
        const int APPLICATION_START_ADDRESS = 0x08020000;
        const int RSA_SIGNATURE_ADDRESS = 0x01DFF00;
        const int MAX_SIGNATURE_SIZE = 256;

        const int BLOCK_SIZE = 512;

        static AsymmetricKeyParameter readPrivateKey(string privateKeyFileName)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(privateKeyFileName))
                keyPair = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            return keyPair.Private;
        }



        private byte[] signApplication()
        {
            byte[] appData = new byte[MAX_APP_SIZE];
            byte[] signData = new byte[MAX_SIGNATURE_SIZE];
            byte[] appSignData = new byte[MAX_APP_SIZE + MAX_SIGNATURE_SIZE];
            var applicationFilePath = _settings.ApplicationFilePath;
            if (File.Exists(applicationFilePath))
            {
                var appFileBytes = File.ReadAllBytes(applicationFilePath);
                var appSize = appFileBytes.Length;

                for (int i = 0; i < appSize; i++)
                    appData[i] = appFileBytes[i];
                for (int i = appSize; i < MAX_APP_SIZE; i++)
                    appData[i] = 0xFF;                      //pad remaining Flash memory with 0xFF
                SHA256 sha256Hash = SHA256.Create();
                byte[] hashData = sha256Hash.ComputeHash(appData);

                var signDataPtr = 0;
                signData[signDataPtr++] = 0x00;
                signData[signDataPtr++] = 0x01;
                for (int i = 0; i < 221; i++)
                    signData[signDataPtr++] = 0xFF;
                signData[signDataPtr++] = 0x00;
                for (int i = 0; i < hashData.Length; i++)
                    signData[signDataPtr++] = hashData[i];

                var pemFilePath = _settings.PemFileName;

                if (File.Exists(pemFilePath))
                {
                    AsymmetricKeyParameter privateKeyPair = readPrivateKey(pemFilePath);
                    IAsymmetricBlockCipher cipher = new RsaEngine();
                    RsaKeyParameters privateKey = (RsaKeyParameters)privateKeyPair;
                    cipher.Init(false, privateKeyPair);
                    byte[] pkcs = cipher.ProcessBlock(signData, 0, signData.Length);
                    return pkcs;
                }
            }

            return null;
        }



        private byte[] encryptAndSign(byte[] azureUuid)
        {
            byte[] app = new byte[MAX_APP_SIZE + MAX_SIGNATURE_SIZE];
            for (int i = 0; i < MAX_APP_SIZE; i++)
                app[i] = 0xFF;
            byte[] pkcs = new byte[MAX_SIGNATURE_SIZE];
            byte[] blockInit = azureUuid;

            pkcs = signApplication();
            var applicationFilePath = _settings.ApplicationFilePath;
            if (File.Exists(applicationFilePath))
            {
                var appFileBytes = File.ReadAllBytes(applicationFilePath);
                var appSize = appFileBytes.Length;
                for (int i = 0; i < appSize; i++)
                    app[i] = appFileBytes[i];
                for (int i = 0; i < MAX_SIGNATURE_SIZE; i++)
                    app[RSA_SIGNATURE_ADDRESS + i] = pkcs[i];

                byte[] App_Key_Comms = new byte[16] { 0xF3, 0x8B, 0x79, 0xA2, 0xCD, 0x05, 0x38, 0x44, 0x33, 0xBF, 0x64, 0x0D, 0xDD, 0x58, 0x31, 0x49 };
                byte[] App_IV = new byte[16] { 0x6A, 0x9F, 0xAA, 0x4E, 0x3D, 0x33, 0x9B, 0x24, 0x4A, 0x40, 0x71, 0x31, 0xF1, 0x33, 0x91, 0xAC };
                byte[] EncAesEcb = new byte[16];
                byte[] dataBlock = new byte[BLOCK_SIZE];

                AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
                EncAesEcb = aesEncrypt(App_Key_Comms, App_IV, AES_ECB, blockInit);
                MemoryStream mstream = new MemoryStream();
                ICryptoTransform encryptor = aesProvider.CreateEncryptor(EncAesEcb, App_IV);
                CryptoStream cryptoStream = new CryptoStream(mstream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(app, 0, (MAX_APP_SIZE + MAX_SIGNATURE_SIZE));
                byte[] encrypted = mstream.ToArray();
                return encrypted;
            }
            return null;
        }


        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }


        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("Cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            return arr;
        }



        public static string ByteArrayToString(byte[] seedData)
        {
            string hexString = "";
            string tempStr = BitConverter.ToString(seedData);
            for (int i = 0; i < tempStr.Length; i++)
                if (tempStr[i] != '-')
                    hexString += tempStr[i];
            return hexString;
        }


        private bool bootloaderUnlock()
        {
            byte[] seedData = new byte[16];
            txLen = 0;
            txBuffer[txLen++] = CMD_SEED_REQUEST;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_SEED_REQUEST;

            if (bootloaderTxRxMessage(17, 1000) == false)  // 8 bytes expected, timeout in 1000 mS
                return false;

            if (rxBuffer[16] != CHAR_ACK)
                return false;

            for (int i = 0; i < 16; i++)
                seedData[i] = rxBuffer[i];

            byte[] decryptedData = default;
            try
            {
                decryptedData = _functionApp.GetDecryptedData(iCodeSerialNumber, seedData);
            }
            catch(Exception ex)
            {
                HelperFunctions.DisplayError(ex.Message);
                return false;
            }

            txLen = 0;
            txBuffer[txLen++] = CMD_SEED_RESPONSE;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = 0x00;
            txBuffer[txLen++] = CMD_SEED_RESPONSE;
            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;
            txLen = 0;
            for (int i = 0; i < 16; i++)
                txBuffer[txLen++] = decryptedData[i];
            if (bootloaderTxRxMessage(1, 1000) == false)  // 1 bytes expected, timeout in 1000 mS
                return false;
            if (rxBuffer[0] != CHAR_ACK)
                return false;

            return true;
        }

        private void btnCreateApplication_Click(object sender, EventArgs e)
        {
            //CreateApplicationForm createApplication = new CreateApplicationForm();
            //createApplication.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            var usersForm = _serviceProvider.GetRequiredService<UsersForm>();
            usersForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lookupForm = _serviceProvider.GetRequiredService<LookupForm>();
            lookupForm.ShowDialog();
        }
    }
}




