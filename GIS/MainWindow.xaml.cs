﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
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
        List<String> listCode = new List<string>();

        private Color defaultLabelColor = new Color();
        public Color LabelColor
        {
            get
            {
                if (labelClrPicker.SelectedColor == null)
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
            //LoadMap();
        }

        private void LoadMap(string arcMapPath)
        {
            mapDoc.Open(arcMapPath);

            this.cbxActiveMap.Items.Clear();
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                this.cbxActiveMap.Items.Add(mapDoc.get_Map(i).Name);
            }
            this.cbxActiveMap.SelectedIndex = 0;
            m_toolbar.AddItem("esriControls.ControlsMapNavigationToolbar");
            //添加行政区划分
            this.districtList.Items.Clear();
            initData();

            // this.districtList.SelectedIndex = 0;

            m_map.Map = mapDoc.get_Map(0);
            m_toc.SetBuddyControl(m_map);
            m_toolbar.SetBuddyControl(m_map);
            m_toolbar.AddItem("esriControls.ControlsMapNavigationToolbar");
        }


        private void btnLabel_Click(object sender, RoutedEventArgs e)
        {
            ShowLabel();
        }

        private void initData()
        {
            this.districtList.Items.Add("杨浦区");
            listCode.Add("310110");
            this.districtList.Items.Add("嘉定区");
            listCode.Add("310114");
            this.districtList.Items.Add("宝山区");
            listCode.Add("310113");
            this.districtList.Items.Add("闸北区");
            listCode.Add("310108");
            this.districtList.Items.Add("虹口区");
            listCode.Add("310109");
            this.districtList.Items.Add("普陀区");
            listCode.Add("310107");
            this.districtList.Items.Add("青浦区");
            listCode.Add("310118");
            this.districtList.Items.Add("闵行区");
            listCode.Add("310112");
            this.districtList.Items.Add("黄浦区");
            listCode.Add("310101");
            this.districtList.Items.Add("长宁区");
            listCode.Add("310105");
            this.districtList.Items.Add("静安区");
            listCode.Add("310106");
            this.districtList.Items.Add("卢湾区");
            listCode.Add("310103");
            this.districtList.Items.Add("徐汇区");
            listCode.Add("310104");
            this.districtList.Items.Add("松江区");
            listCode.Add("310117");
            this.districtList.Items.Add("奉贤区");
            listCode.Add("310120");
            this.districtList.Items.Add("浦东新区");
            listCode.Add("310115");
            this.districtList.Items.Add("金山区");
            listCode.Add("310116");
            this.districtList.Items.Add("崇明县");
            listCode.Add("310230");
            this.districtList.Items.Add("--清除选择--");
            listCode.Add("310000");
        }

        private void ShowLabel()
        {
            if (labelOn)
            {
                if (!this.cbxActiveMap.HasItems || !this.cbxFeatureLayer.HasItems || !this.cbxField.HasItems)
                    return;
                if (annoPropCol != null)
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
            if (this.cbxActiveMap.SelectedIndex < 0)
                return;
            m_map.Map = mapDoc.get_Map(this.cbxActiveMap.SelectedIndex);
            m_toc.SetBuddyControl(m_map);
            m_toolbar.SetBuddyControl(m_map);
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
            if (this.cbxFeatureLayer.SelectedIndex < 0)
                return;
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
            if (this.cbxField.SelectedIndex < 0)
                return;
        }

        private FieldData getData(ILayer layer, IField field)
        {
            if (layer == null || field == null)
                return null;

            IFeatureLayer fLayer = layer as IFeatureLayer;
            IFeatureClass fClass = fLayer.FeatureClass;
            IFields fields = fClass.Fields;

            ITable table = fClass as ITable;
            ICursor cursor = table.Search(null, false);
            IRow row = cursor.NextRow();

            FieldData fData = new FieldData();
            fData.FieldName = field.Name;
            int fieldIndex = fields.FindField(field.Name);
            while (row != null)
            {
                object value = row.get_Value(fieldIndex);
                fData.Values.Add(value);
                row = cursor.NextRow();
            }
            return fData;
        }

        private FieldData getData(ILayer layer, string fieldName)
        {
            if (layer == null || fieldName == null)
                return null;

            IFeatureLayer fLayer = layer as IFeatureLayer;
            IFeatureClass fClass = fLayer.FeatureClass;
            IFields fields = fClass.Fields;

            ITable table = fClass as ITable;
            ICursor cursor = table.Search(null, false);
            IRow row = cursor.NextRow();

            FieldData fData = new FieldData();
            fData.FieldName = fieldName;
            int fieldIndex = fields.FindField(fieldName);
            while (row != null)
            {
                object value = row.get_Value(fieldIndex);
                fData.Values.Add(value);
                row = cursor.NextRow();
            }
            return fData;
        }

        private void labelClrPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void chkBoxLabel_Unchecked(object sender, RoutedEventArgs e)
        {
            labelOn = false;
            if (gLabelLayer != null)
            {
                gLabelLayer.DisplayAnnotation = false;
            }
            if (m_map.Map != null)
            {
                m_map.Refresh();
            }
        }

        private void chkBoxLabel_Checked(object sender, RoutedEventArgs e)
        {
            labelOn = true;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            CloseFile();
            OpenFile();
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".mxd";
            openFileDialog.Filter = "ArcMap Documents (*.mxd)|*.mxd";
            if (openFileDialog.ShowDialog() == true)
            {
                string arcMapPath = openFileDialog.FileName;
                LoadMap(arcMapPath);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseFile();
        }

        private void CloseFile()
        {
            cbxActiveMap.Items.Clear();
            cbxFeatureLayer.Items.Clear();
            cbxField.Items.Clear();
            districtList.Items.Clear();
            chkBoxLabel.IsChecked = false;
            labelClrPicker.SelectedColor = null;
            rbtnBarCht.IsChecked = false;
            rbtnPieCht.IsChecked = false;
            m_map.ClearLayers();
            m_toc.Update();
            m_toolbar.Update();
            m_map.Refresh();
        }

        private void districtSelect(object sender, SelectionChangedEventArgs e)
        {
            //Console.Out.WriteLine(sender.ToString());
            //Console.Out.WriteLine(this.districtList.Items.GetItemAt(this.districtList.SelectedIndex));
            if (this.districtList.SelectedIndex < 0)
                return;
            String districName = this.districtList.Items.GetItemAt(this.districtList.SelectedIndex).ToString();
            setDistrictColor(districName);
            Console.Out.WriteLine(listCode[this.districtList.SelectedIndex]);
        }

        private void setDistrictColor(string districtName)
        {
            if (m_map.Map.LayerCount < 5)
                return;
            IFeatureLayer pFeatureLayer = m_map.Map.get_Layer(4) as IFeatureLayer;
            if (pFeatureLayer == null)
                return;
            IQueryFilter pFilter;
            pFilter = new QueryFilterClass();
            pFilter.WhereClause = "Name = '" + districtName + "'";//"Name" = '嘉定区' OR "Name" = '杨浦区'
            IFeatureSelection pFeatureSelection;
            pFeatureSelection = pFeatureLayer as IFeatureSelection;
            pFeatureSelection.SelectFeatures(pFilter, esriSelectionResultEnum.esriSelectionResultNew, true);
            //将选择集添上颜色
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = 220;
            pColor.Green = 112;
            pColor.Blue = 60;
            ISelectionSet selectSet = pFeatureSelection.SelectionSet;

            // pFeatureSelection.SelectionColor = getRGB(220, 60, 60);//pColor
            //m_map.ClearLayers();
            m_map.Refresh();

            /* 删除闪烁效果*/
            ISelectionSet pFeatSet;
            pFeatSet = pFeatureSelection.SelectionSet;
            IFeatureCursor pFeatCursor;
            ICursor pCursor;
            pFeatSet.Search(pFilter, true, out pCursor);
            pFeatCursor = pCursor as IFeatureCursor;
            IFeature pFeat;
            pFeat = pFeatCursor.NextFeature();
            while (pFeat != null)
            {
                if (pFeat != null)
                {
                    Console.Out.WriteLine(pFeat.Fields);
                    ISimpleFillSymbol pFillsyl2;
                    pFillsyl2 = new SimpleFillSymbolClass();
                    pFillsyl2.Color = getRGB(220, 60, 60);
                    m_map.FlashShape(pFeat.Shape, 0, 0, pFillsyl2);
                }
                pFeat = pFeatCursor.NextFeature();
            }
            setPointColor(listCode[this.districtList.SelectedIndex].ToString());
            // */

        }


        private void setPointColor(String str)
        {
            IMap pMap = m_map.Map;
            IMap oriMap = m_map.Map;
            IFeatureLayer pFeatureLayer;
            pFeatureLayer = m_map.Map.get_Layer(0) as IFeatureLayer;
            IFeatureLayer pCurrentLayer = pFeatureLayer;

            IQueryFilter pQueryFilter;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "ADMINCODE = " + str;
            //查询
            IFeatureCursor pCursor;
            pCursor = pFeatureLayer.Search(pQueryFilter, true);


            IFeatureSelection pFeatureSelection;
            pFeatureSelection = pFeatureLayer as IFeatureSelection;
            pFeatureSelection.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, true);
            ISelectionSet selectSet = pFeatureSelection.SelectionSet;

            IFeature pFeat;
            pFeat = pCursor.NextFeature();
            while (pFeat != null)
            {
                pMap.SelectFeature(pCurrentLayer, pFeat);
                pFeat = pCursor.NextFeature();
            }


            // pFeatureLayer.Visible false;

            IActiveView pActiveView = pMap as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);

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

        private void CommandBindingOpenFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Click(sender, e);
        }

        private void CommandBindingCloseFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close_Click(sender, e);
        }

        private void CommandBindingExitProgramExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private IMap getCbxSelectedMap()
        {
            if (this.cbxActiveMap.SelectedIndex < 0)
                return null;
            return mapDoc.get_Map(this.cbxActiveMap.SelectedIndex);
        }

        private ILayer getCbxSelectedLayer()
        {
            if (this.cbxFeatureLayer.SelectedIndex < 0)
                return null;
            return this.m_map.Map.get_Layer(this.cbxFeatureLayer.SelectedIndex);
        }

        private IField getCbxSelectedField()
        {
            if (this.cbxField.SelectedIndex < 0)
                return null;
            string fieldName = cbxField.SelectedItem as string;
            IFeatureLayer fLayer = getCbxSelectedLayer() as FeatureLayer;
            IFields fields = fLayer.FeatureClass.Fields;
            return fields.get_Field(fields.FindField(fieldName));
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("Closing");
        }

        private List<KeyValuePair<string, long>> getDistrictPOI()
        {
            if (this.m_map.Map.LayerCount < 5)
                return new List<KeyValuePair<string, long>>();
            ILayer layer_POI = this.m_map.Map.get_Layer(0);
            ILayer layer_district = this.m_map.Map.get_Layer(4);
            FieldData POIAdmincode = getData(layer_POI, "ADMINCODE");
            FieldData districtName = getData(layer_district, "Name");
            FieldData districtCode = getData(layer_district, "Code");

            Dictionary<string, string> distriction = new Dictionary<string, string>();
            for (int i = 0; i < districtName.Count; i++)
            {
                string _name = districtName.Values[i].ToString();
                string _code = districtCode.Values[i].ToString();
                distriction.Add(_code, _name);
            }

            Dictionary<string, long> districtPOICount = new Dictionary<string, long>();
            foreach (object o in POIAdmincode.Values)
            {
                string _code = o.ToString();
                if (!districtPOICount.ContainsKey(_code))
                {
                    districtPOICount.Add(_code, 1);
                }
                else
                {
                    districtPOICount[_code]++;
                }
            }

            List<KeyValuePair<string, long>> list = new List<KeyValuePair<string, long>>();
            foreach (string _code in districtPOICount.Keys)
            {
                if (distriction.ContainsKey(_code))
                {
                    list.Add(new KeyValuePair<string, long>(distriction[_code], districtPOICount[_code]));
                }
            }
            return list;
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbtnBarCht.IsChecked == true)
                {
                    BarChartWindow bcw = new BarChartWindow();
                    bcw.InitializeChart(getDistrictPOI());
                    bcw.Show();
                }
                else if (rbtnPieCht.IsChecked == true)
                {
                    PieChartWindow pcw = new PieChartWindow();
                    pcw.InitializeChart(getDistrictPOI());
                    pcw.Show();
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }
    }
}
