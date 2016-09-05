using System;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.DesktopEdition;


namespace  FeatureIdsToExclude
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-96.8122, 37.3795, -96.3571, 37.1264);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay worldMapKitDesktopOverlay = new ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            //InMemoryFeatureLayer for the dams.
            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(new GeoImage(@"..\..\Data\dam_L_32x32.png"));
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.462, 37.185, "Dam 21"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.563, 37.188, "Dam 22"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.570, 37.208, "Dam 23"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.558, 37.217, "Dam 24"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.475, 37.245, "Dam 26"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.445, 37.247, "Dam 27"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.573, 37.248, "Dam 31"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.453, 37.275, "Dam 28"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.78333, 37.28167, "Elk City Lake"));
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(-96.493, 37.307, "Dam 32"));

            //InMemoryFeatureLayer for the searching point.
            InMemoryFeatureLayer inMemoryFeatureLayer2 = new InMemoryFeatureLayer();
            inMemoryFeatureLayer2.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColor.SimpleColors.Red, 12);
            inMemoryFeatureLayer2.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            inMemoryFeatureLayer2.InternalFeatures.Add(new Feature(-96.5566, 37.2717));

            //InMemoryFeatureLayer for the selected dams.
            InMemoryFeatureLayer highlightInMemoryFeatureLayer = new InMemoryFeatureLayer();
            highlightInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColor.FromArgb(100, GeoColor.SimpleColors.Red), 14);
            highlightInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(inMemoryFeatureLayer);
            layerOverlay.Layers.Add(inMemoryFeatureLayer2);
            layerOverlay.Layers.Add(highlightInMemoryFeatureLayer);

            winformsMap1.Overlays.Add(layerOverlay);

            winformsMap1.Refresh();
        }

        //Finds the five nearest dams using FeatureIdsToExclude.
        private void radioButtonWith_CheckedChanged(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)winformsMap1.Overlays[1];
            InMemoryFeatureLayer inMemoryFeatureLayer = (InMemoryFeatureLayer)layerOverlay.Layers[0];

            //Adds the features to exlude using the feature id.
            //Will exclude the dam 24 and dam 31 from the search.
            inMemoryFeatureLayer.FeatureIdsToExclude.Add("Dam 24");
            inMemoryFeatureLayer.FeatureIdsToExclude.Add("Dam 31");

            InMemoryFeatureLayer inMemoryFeatureLayer2 = (InMemoryFeatureLayer)layerOverlay.Layers[1];
            PointShape pointShape = (PointShape)inMemoryFeatureLayer2.InternalFeatures[0].GetShape();

            inMemoryFeatureLayer.Open();
            int numberToFind = 5;
            Collection<Feature> nearestFeatures = inMemoryFeatureLayer.QueryTools.GetFeaturesNearestTo(pointShape, GeographyUnit.DecimalDegree, numberToFind, ReturningColumnsType.NoColumns);

            //We may have to adjust numberOfItemsToFind parameter to find the nearest 5 features.
            if (nearestFeatures.Count < numberToFind)
            {
                numberToFind = numberToFind + (numberToFind - nearestFeatures.Count);
                nearestFeatures = inMemoryFeatureLayer.QueryTools.GetFeaturesNearestTo(pointShape, GeographyUnit.DecimalDegree, numberToFind, ReturningColumnsType.NoColumns);
            }

            //Before refreshing the map, we clear the FeatureIdsToExclude so that all the features of the InMemoryFeatureLayer will be drawn.
            inMemoryFeatureLayer.FeatureIdsToExclude.Clear();
            inMemoryFeatureLayer.Close();

            InMemoryFeatureLayer highllightInMemoryFeatureLayer = (InMemoryFeatureLayer)layerOverlay.Layers[2];
            highllightInMemoryFeatureLayer.InternalFeatures.Clear();

            //Adds the result features to the InMemoryFeatureLayer for displaying.
            foreach (Feature feature in nearestFeatures)
            {
                highllightInMemoryFeatureLayer.InternalFeatures.Add(feature);
            }

            winformsMap1.Refresh(layerOverlay);

        }

        private void radioButtonWithout_CheckedChanged(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)winformsMap1.Overlays[1];
            InMemoryFeatureLayer inMemoryFeatureLayer = (InMemoryFeatureLayer)layerOverlay.Layers[0];
            inMemoryFeatureLayer.FeatureIdsToExclude.Clear();

            InMemoryFeatureLayer inMemoryFeatureLayer2 = (InMemoryFeatureLayer)layerOverlay.Layers[1];
            PointShape pointShape = (PointShape)inMemoryFeatureLayer2.InternalFeatures[0].GetShape();

            inMemoryFeatureLayer.Open();
            int numberToFind = 5;
            Collection<Feature> nearestFeatures = inMemoryFeatureLayer.QueryTools.GetFeaturesNearestTo(pointShape, GeographyUnit.DecimalDegree, numberToFind, ReturningColumnsType.NoColumns);
            inMemoryFeatureLayer.Close();

            InMemoryFeatureLayer highllightFeatureLayer = (InMemoryFeatureLayer)layerOverlay.Layers[2];
            highllightFeatureLayer.InternalFeatures.Clear();

            foreach (Feature feature in nearestFeatures)
            {
                highllightFeatureLayer.InternalFeatures.Add(feature);
            }

            winformsMap1.Refresh(layerOverlay);
        }

      
        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

    }
}
