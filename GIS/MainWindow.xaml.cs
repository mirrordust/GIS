using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

using stdole;

namespace GIS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        AxMapControl m_map = new AxMapControl();
        AxTOCControl m_toc = new AxTOCControl();
        AxToolbarControl m_toolbar = new AxToolbarControl();
        IMapDocument mapDoc = new MapDocument();

        IFeatureLayer labelLayer;
        IGeoFeatureLayer gLabelLayer;
        IAnnotateLayerPropertiesCollection annoPropCol;
        bool labelOn = false;

        private Color defaultLabelColor = new Color();
        public Color LabelColor
        {
            get
            {
                if(labelClrPicker.SelectedColor == null)
                {
                    defaultLabelColor.R = 255;
                    defaultLabelColor.G = 0;
                    defaultLabelColor.B = 0;
                    return defaultLabelColor;
                }
                else
                {
                    return (Color)labelClrPicker.SelectedColor;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.mapControlHost.Child = m_map;
            this.tocHost.Child = m_toc;
            this.toolbarHost.Child = m_toolbar;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }

        private void LoadMap()
        {
            mapDoc.Open(Properties.Resources.ArcMapPath);

            this.cbxActiveMap.Items.Clear();
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                this.cbxActiveMap.Items.Add(mapDoc.get_Map(i).Name);
            }
            this.cbxActiveMap.SelectedIndex = 0;

            //添加行政区划分
            this.districtList.Items.Clear();
            this.districtList.Items.Add("杨浦区");
            this.districtList.Items.Add("嘉定区");
            this.districtList.Items.Add("宝山区");
            this.districtList.Items.Add("闸北区");
            this.districtList.Items.Add("虹口区");
            this.districtList.Items.Add("普陀区");
            this.districtList.Items.Add("青浦区");
            this.districtList.Items.Add("闵行区");
            this.districtList.Items.Add("黄浦区");
            this.districtList.Items.Add("长宁区");
            this.districtList.Items.Add("静安区");
            this.districtList.Items.Add("卢湾区");
            this.districtList.Items.Add("徐汇区");
            this.districtList.Items.Add("松江区");
            this.districtList.Items.Add("奉贤区");
            this.districtList.Items.Add("浦东新区");
            this.districtList.Items.Add("金山区");
            this.districtList.Items.Add("崇明县");
            this.districtList.SelectedIndex = 0;

            m_map.Map = mapDoc.get_Map(0);
            m_toc.SetBuddyControl(m_map);
            m_toolbar.SetBuddyControl(m_map);
            m_toolbar.AddItem("esriControls.ControlsMapNavigationToolbar");
        }


        private void btnLabel_Click(object sender, RoutedEventArgs e)
        {
            ShowLabel();
        }

        private void ShowLabel()
        {
            if (labelOn)
            {
                labelOn = false;
                gLabelLayer.DisplayAnnotation = false;
            }
            else
            {
                labelOn = true;

                if(annoPropCol != null)
                {
                    annoPropCol.Clear();
                }
                labelLayer = this.m_map.Map.get_Layer(this.cbxFeatureLayer.SelectedIndex) as IFeatureLayer;
                gLabelLayer = labelLayer as IGeoFeatureLayer;
                annoPropCol = gLabelLayer.AnnotationProperties;

                ITextSymbol pTextSyl = new TextSymbol();
                IFontDisp font = new StdFont() as IFontDisp;
                font.Size = 20;
                font.Italic = true;
                pTextSyl.Font = font;
                RgbColor color = new RgbColor();
                color.Red = LabelColor.R;
                color.Green = LabelColor.G;
                color.Blue = LabelColor.B;
                pTextSyl.Color = color;

                IBasicOverposterLayerProperties pBasic = new BasicOverposterLayerProperties();
                IPointPlacementPriorities pPlacementPoint = new PointPlacementPriorities();
                pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPoint;
                pBasic.PointPlacementPriorities = pPlacementPoint;
                pBasic.PointPlacementOnTop = false;
                pBasic.PointPlacementMethod = esriOverposterPointPlacementMethod.esriOnTopPoint;

                ILabelEngineLayerProperties labelField = new LabelEngineLayerProperties() as ILabelEngineLayerProperties;

                //IFeatureClass fClass = ((IFeatureLayer)labelLayer).FeatureClass;
                //IFields fields = fClass.Fields;
                //String fieldName = fields.get_Field(this.cbxField.SelectedIndex).Name;

                string fieldName = (string)cbxField.SelectedItem;
                //Console.WriteLine(fields.FindField(fieldName));
                labelField.Expression = "[" + fieldName + "]";
                //set postion
                labelField.BasicOverposterLayerProperties = pBasic;
                //set  symbol
                labelField.Symbol = pTextSyl;
                IAnnotateLayerProperties annoProp = labelField as IAnnotateLayerProperties;
                annoPropCol.Add(annoProp);
                gLabelLayer.DisplayAnnotation = true;
                m_map.Refresh();
            }
        }

        private void cbxActiveMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_map.Map = mapDoc.get_Map(this.cbxActiveMap.SelectedIndex);
            m_toc.SetBuddyControl(m_map);
            m_toolbar.SetBuddyControl(m_map);
            m_toolbar.AddItem("esriControls.ControlsMapNavigationToolbar");
            m_map.Refresh();

            LoadLayers();
        }

        private void LoadLayers()
        {
            this.cbxFeatureLayer.Items.Clear();

            UID uidLayer = new UIDClass();
            uidLayer.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";

            IEnumLayer allFLayers = m_map.Map.get_Layers(uidLayer, true);

            IFeatureLayer fLayer;
            fLayer = allFLayers.Next() as IFeatureLayer;
            while (fLayer != null)
            {
                this.cbxFeatureLayer.Items.Add(fLayer.Name);
                /*
                if ((fLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon))
                {
                    this.cbxFeatureLayer.Items.Add(fLayer.Name);
                }
                else if ((fLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline))
                {
                    this.cbxFeatureLayer.Items.Add(fLayer.Name);
                }
                else if ((fLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint))
                {
                    cbxFeatureLayer.Items.Add(fLayer.Name);
                }
                */
                fLayer = allFLayers.Next() as IFeatureLayer;
            }
            this.cbxFeatureLayer.SelectedIndex = 0;
        }

        private void cbxFeatureLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ILayer fLayer;
            fLayer = m_map.Map.get_Layer(this.cbxFeatureLayer.SelectedIndex);
            LoadFields(fLayer);
        }

        private void LoadFields(ILayer layer)
        {
            this.cbxField.Items.Clear();

            IFeatureClass fClass;
            IFeatureLayer fLayer = (IFeatureLayer)layer;
            fClass = fLayer.FeatureClass;
            IFields fields = fClass.Fields;
            IField fld;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                fld = fields.get_Field(i);
                if (Convert.ToInt32(fld.Type) <= 5)
                {
                    this.cbxField.Items.Add(fld.Name);
                }
            }
            cbxField.SelectedIndex = 0;
        }

        private void cbxField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void labelClrPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void districtSelect(object sender, SelectionChangedEventArgs e)
        {
            Console.Out.WriteLine(sender.ToString());
            Console.Out.WriteLine(this.districtList.Items.GetItemAt(this.districtList.SelectedIndex));
            String districName = this.districtList.Items.GetItemAt(this.districtList.SelectedIndex).ToString();
            setDistrictColor(districName);
        }


        private void setDistrictColor(string districtName)
        {
            IFeatureLayer pFeatureLayer = m_map.Map.get_Layer(4) as IFeatureLayer;
            IQueryFilter pFilter;
            pFilter = new QueryFilterClass();
            pFilter.WhereClause = "Name = '"+districtName+"'";
            IFeatureSelection pFeatureSelection;
            pFeatureSelection = pFeatureLayer as IFeatureSelection;
            pFeatureSelection.SelectFeatures(pFilter, esriSelectionResultEnum.esriSelectionResultNew, true);
            //将选择集添上颜色
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = 220;
            pColor.Green = 112;
            pColor.Blue = 60;
            pFeatureSelection.SelectionColor = pColor;
            //m_map.ClearLayers();
            m_map.Refresh();

            ISelectionSet pFeatSet;
            pFeatSet = pFeatureSelection.SelectionSet;
            IFeatureCursor pFeatCursor;
            ICursor pCursor;
            pFeatSet.Search(null, true, out pCursor);
            pFeatCursor = pCursor as IFeatureCursor;
            IFeature pFeat;
            pFeat = pFeatCursor.NextFeature();
            while (pFeat != null)
            {
                if (pFeat != null)
                {
                    ISimpleFillSymbol pFillsyl2;
                    pFillsyl2 = new SimpleFillSymbolClass();
                    pFillsyl2.Color = getRGB(220, 60, 60);
                    m_map.FlashShape(pFeat.Shape, 15, 20, pFillsyl2);
                }
                pFeat = pFeatCursor.NextFeature();
            }
        }

        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }

    }
}
