using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;

namespace GIS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitArcGISLicence();
        }

        public void InitArcGISLicence()
        {
            RuntimeManager.Bind(ProductCode.EngineOrDesktop);

            AoInitialize aoi = new AoInitialize();
            esriLicenseProductCode productCode = esriLicenseProductCode.esriLicenseProductCodeAdvanced;
            if (aoi.IsProductCodeAvailable(productCode) == esriLicenseStatus.esriLicenseAvailable)
            {
                aoi.Initialize(productCode);
            }
        }
    }
}
