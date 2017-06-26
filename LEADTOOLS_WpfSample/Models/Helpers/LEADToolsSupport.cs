using Leadtools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEADTOOLS_WpfSample.Models.Helpers
{
    internal sealed class LEADToolsSupport
    {
        private LEADToolsSupport()
        {
        }

        public static bool SetLicense()
        {
            try
            {
                // TODO: Change this to use your license file and developer key */
                string licenseFilePath = "C:\\LEADTOOLS 19\\Common\\License\\LEADTOOLS.LIC";
                string developerKey = "C:\\LEADTOOLS 19\\Common\\License\\LEADTOOLS.LIC.KEY";
                RasterSupport.SetLicense(licenseFilePath, developerKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }

            if (RasterSupport.KernelExpired)
            {
                //string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string dir = "C:\\LEADTOOLS 19\\Bin\\DotNet4\\x64";
                /* Try the common LIC directory */
                string licenseFileRelativePath = System.IO.Path.Combine(dir, "..\\..\\..\\Common\\License\\LEADTOOLS.LIC");
                string keyFileRelativePath = System.IO.Path.Combine(dir, "..\\..\\..\\Common\\License\\LEADTOOLS.LIC.key");

                if (System.IO.File.Exists(licenseFileRelativePath) && System.IO.File.Exists(keyFileRelativePath))
                {
                    string developerKey = System.IO.File.ReadAllText(keyFileRelativePath);
                    try
                    {
                        RasterSupport.SetLicense(licenseFileRelativePath, developerKey);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex.Message);
                    }
                }
            }

            if (RasterSupport.KernelExpired)
            {
                string msg = "Your license file is missing, invalid or expired. LEADTOOLS will not function. Please contact LEAD Sales for information on obtaining a valid license.";
                string logmsg = string.Format("*** NOTE: {0} ***{1}", msg, Environment.NewLine);
                System.Diagnostics.Debugger.Log(0, null, "*******************************************************************************" + Environment.NewLine);
                System.Diagnostics.Debugger.Log(0, null, logmsg);
                System.Diagnostics.Debugger.Log(0, null, "*******************************************************************************" + Environment.NewLine);

                System.Diagnostics.Process.Start("https://www.leadtools.com/downloads/evaluation-form.asp?evallicenseonly=true");

                return false;
            }

            return true;
        }
    }

}
