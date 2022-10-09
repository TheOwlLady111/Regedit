using System.Text;
using Microsoft.Win32;

namespace Regedit
{
    //General info
    // Top-level key(I am not sure in this naming, in russian it is ключ верхнего уровня)
    //4 or 5 main keys(folders). They contains others subkey and parametres
    //Subkey. It is like folder, where we can find another subkeys and(or) parameter
    //Parameter. Object with structure name - type - value
    //Class Registry - for working with top-level keys
    // Class RegistryKey - for working with subkeys

    public partial class Form1 : Form
    {
        //For working inside of one of top-level keys
        private RegistryKey _rKey1;
        // For choosing top level key
        private List<string> _regs = new List<string>()
        {
            "HKEY_CLASSES_ROOT",
            "HKEY_CURRENT_USER",
            "HKEY_LOCAL_MACHINE",
            "HKEY_PERFORMANCE_DATA"
        };

        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = _regs;
        }

        /// <summary>
        /// Here we choose top-level key(ключ верхнего уровня)
        /// It is five main folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="comboBox1">Choosing key</param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                {
                    _rKey1 = Registry.ClassesRoot;
                    break;
                }
                case 1:
                {
                    _rKey1 = Registry.CurrentUser;
                    break;
                }
                case 2:
                {
                    _rKey1 = Registry.LocalMachine;
                    break;
                }
                case 3:
                {
                    _rKey1 = Registry.PerformanceData;
                    break;
                }
            }
        }

        /// <summary>
        /// Show subkeys and(or) parameters of entered subkey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="textBox1">Input for path(exp: AppEvents\EventLabels\.Default)</param>
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            RegistryKey rKey2 = _rKey1.OpenSubKey(textBox1.Text);

            if (rKey2 != null)
            {
                string[] subNames = rKey2.GetSubKeyNames();
                string[] vNames = rKey2.GetValueNames();

                //Adding rows with subkeys
                for (int i = 0; i < subNames.Length; i++)
                {
                    dataGridView1.Rows.Add(subNames[i], "subkey", "-");
                }

                //Adding rows with parameters
                for (int i = 0; i < vNames.Length; i++)
                {
                    var value = rKey2.GetValue(vNames[i]);

                    RegistryValueKind kind = rKey2.GetValueKind(vNames[i]);

                    //For kind it is something like mapping it from enum
                    dataGridView1.Rows.Add(vNames[i], (RegistryValueKindNative)kind, value.ToString());
                }
            }
            else
            {
                MessageBox.Show("Incorrect path!");
            }
        }

        /// <summary>
        /// Create subkey method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="textBox2">Input for path(exp: AppEvents\EventLabels\.Default)</param>
        /// <param name="textBox3">Input for subkey's name</param>
        private void button4_Click(object sender, EventArgs e)
        {
            RegistryKey rKey2 = _rKey1.OpenSubKey(textBox2.Text, true);

            if (rKey2 != null)
            {
                if (textBox3.Text != String.Empty)
                {
                    RegistryKey newKey = rKey2.CreateSubKey(textBox3.Text);
                    newKey.Close();
                }
                else
                {
                    MessageBox.Show("Enter subkey name!");
                }
            }
            else
            {
                MessageBox.Show("Incorrect path!");
            }
        }

        /// <summary>
        /// Create parameter method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="textBox4">Input for path(exp: AppEvents\EventLabels\.Default)</param>
        /// <param name="textBox5">Input for parameter name</param>
        /// <param name="textBox6">Input for parameter value</param>
        private void button5_Click(object sender, EventArgs e)
        {
            RegistryKey rKey2 = _rKey1.OpenSubKey(textBox4.Text, true);

            if (rKey2 != null)
            {
                if (textBox5.Text != null && textBox6.Text != null)
                {
                    rKey2.SetValue(textBox5.Text, textBox6.Text);
                    rKey2.Close();
                }
                else
                {
                    MessageBox.Show("Empty value for name and(or) value!");
                }
            }
            else
            {
                MessageBox.Show("Incorrect path!");
            }
        }

        /// <summary>
        /// Method for deleting subkeys and parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="textBox7">Input for path(exp: AppEvents\EventLabels\.Default)</param>
        /// <param name="textBox8">Input for parameter name</param>>
        private void button6_Click(object sender, EventArgs e)
        {
            RegistryKey rKey2 = _rKey1.OpenSubKey(textBox7.Text, true);

            if (rKey2 != null)
            {
                if (textBox8.Text != String.Empty)
                {
                    //Delete parameter
                    var value = _rKey1.GetValue(textBox8.Text);

                    if (value != null)
                    {
                        rKey2.DeleteValue(textBox8.Text);
                        rKey2.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect parameter name!");
                    }
                    
                }
                else
                {
                    //Delete subkey
                    _rKey1.DeleteSubKey(textBox7.Text);
                    _rKey1.Close();
                }
            }
            else
            {
                MessageBox.Show("Incorrect subkey path!");
            }
           
        }

        /// <summary>
        /// This method is for updating subkey
        /// For update subkey we need to create other one with new name
        /// than we copy data from subkey with old name to new one
        /// and after that we delete old subkey 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="textBox9">Input for path(exp: AppEvents\EventLabels\.Default)</param>
        /// <param name="textBox10">Input for new name(we will change last value in the path)</param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox9.Text != String.Empty && textBox10.Text != String.Empty)
            {
                RegistryKey key = _rKey1.OpenSubKey(textBox9.Text);

                if (key != null)
                {
                    //We need this part of code because
                    //in path we have full old path
                    //but in name we have to input only new name for update
                    //This part creates full path for new name
                    string path = CreateFullPath(textBox9.Text);

                    CopyKey(_rKey1, textBox9.Text, path);
                    _rKey1.DeleteSubKeyTree(textBox9.Text);
                }
                else
                {
                    MessageBox.Show("Incorrect path!");
                }
            }
        }

        public bool CopyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            //Create new key
            RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName, true);

            //Open the sourceKey we are copying from
            RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy, true);

            RecurseCopyKey(sourceKey, destinationKey);

            return true;
        }

        private void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call itself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName, true);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
                RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }

        /// <summary>
        /// This method is for updating parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox9.Text != String.Empty && textBox11.Text != String.Empty)
            {
                var key = _rKey1.OpenSubKey(textBox9.Text, true);
                if (key != null)
                {
                    var parameter = key.GetValue(textBox11.Text);
                    if (parameter != null)
                    {
                        // If we want to change both(name and value)
                        if (textBox13.Text != String.Empty && textBox12.Text != String.Empty)
                        {
                            key.DeleteValue(textBox11.Text);
                            key.SetValue(textBox12.Text, textBox13.Text);
                            key.Close();
                        }
                        // If we want to change only value
                        else if (textBox13.Text != String.Empty && textBox12.Text == String.Empty)
                        {
                            key.DeleteValue(textBox11.Text);
                            key.SetValue(textBox11.Text, textBox13.Text);
                            key.Close();
                        }
                        // If we want to change only name
                        else if (textBox12.Text != String.Empty && textBox13.Text == String.Empty)
                        {
                            key.DeleteValue(textBox11.Text);
                            key.SetValue(textBox12.Text, parameter);
                            key.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect path to parameter!");
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect path to subkey!");
                }
            }
            else
            {
                MessageBox.Show("Incorrect input!");
            }
        }

        private string CreateFullPath(string text)
        {
            string[] words = text.Split('\\');
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var str in words)
            {
                if (i == words.Length)
                {
                    sb.Append(textBox10.Text);
                }
                else
                {
                    sb.Append($"{str}\\");
                }

                i++;
            }

            return sb.ToString();
        }
    }
}