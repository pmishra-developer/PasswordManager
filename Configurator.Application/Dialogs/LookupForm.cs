using Configurator.Repositories;
using Configurator.Repositories.Contracts;
using Configurator.ViewModel;

namespace Configurator.Application.Dialogs
{
    public partial class LookupForm : Form
    {
        private readonly ILookupRepository _lookupRepository;

        public LookupForm(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;

            InitializeComponent();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pathLabel.Text = openFileDialog1.FileName;
            }
        }

        private void uploadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxClearExisitingData.Checked)
                {
                    //TODO
                    //_lookupRepository.ClearData();
                    //_lookupRepository.ResetIdentity();
                }

                var lookUpSampleFilePath = pathLabel.Text;
                if (string.IsNullOrEmpty(lookUpSampleFilePath))
                {
                    HelperFunctions.DisplayError("Please Select a file before uploading");
                    return;
                }

                if (!File.Exists(lookUpSampleFilePath))
                {
                    HelperFunctions.DisplayError($"File does not exists at the Path {lookUpSampleFilePath}.");
                    return;
                }

                // Load up our Look up Table
                List<LookupViewModel> lookupViewModels = new List<LookupViewModel>();
                int lookUpType = 1;
                int offset = 1;
                using (var file = File.OpenText(lookUpSampleFilePath))
                {
                    while (file.ReadLine()?.Trim() is { } line)
                    {
                        // skip empty lines
                        if (line == string.Empty)
                        {
                            lookUpType++;
                            offset = 1;
                            continue;
                        }

                        lookupViewModels.Add(new LookupViewModel(lookUpType.ToString(), offset++, line));
                    }
                }

                //TODO
                //_lookupRepository.SaveLookups(lookupViewModels);
                MessageBox.Show($"Completed uploading {lookupViewModels.Count} Lookup Data.");
            }
            catch (FormatException ex)
            {
                HelperFunctions.DisplayError(ex.Message);
            }
        }
    }
}
