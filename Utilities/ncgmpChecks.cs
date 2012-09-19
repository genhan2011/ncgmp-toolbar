using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace ncgmpToolbar.Utilities
{
    public class ncgmpChecks
    {
        public static bool IsWorkspaceMinNCGMPCompliant(IWorkspace Workspace)
        {
            try
            {
                IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
                //Check that requisite datasets are present
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureDataset, commonFunctions.QualifyClassName(Workspace, "GeologicMap")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace,"MapUnitPolys")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace,"ContactsAndFaults")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace,"DescriptionOfMapUnits")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace,"DataSources")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace,"Glossary")) == false) { return false; }
                if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace,"DataSourcePolys")) == false) { return false; }

                //Check that requisite fields are present in each dataset
                ITable checkTable = commonFunctions.OpenTable(Workspace, "MapUnitPolys");
                if (checkTable.FindField("MapUnitPolys_ID") == -1) { return false; }
                if (checkTable.FindField("MapUnit") == -1) { return false; }
                if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
                if (checkTable.FindField("Label") == -1) { return false; }
                if (checkTable.FindField("Symbol") == -1) { return false; }
                if (checkTable.FindField("Notes") == -1) { return false; }
                if (checkTable.FindField("DataSourceID") == -1) { return false; }

                checkTable = commonFunctions.OpenTable(Workspace, "ContactsAndFaults");
                if (checkTable.FindField("ContactsAndFaults_ID") == -1) { return false; }
                if (checkTable.FindField("Type") == -1) { return false; }
                if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
                if (checkTable.FindField("ExistenceConfidence") == -1) { return false; }
                if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
                if (checkTable.FindField("IsConcealed") == -1) { return false; }
                if (checkTable.FindField("Symbol") == -1) { return false; }
                if (checkTable.FindField("Label") == -1) { return false; }
                if (checkTable.FindField("Notes") == -1) { return false; }
                if (checkTable.FindField("DataSourceID") == -1) { return false; }

                checkTable = commonFunctions.OpenTable(Workspace, "DescriptionOfMapUnits");
                if (checkTable.FindField("DescriptionOfMapUnits_ID") == -1) { return false; }
                if (checkTable.FindField("MapUnit") == -1) { return false; }
                if (checkTable.FindField("Label") == -1) { return false; }
                if (checkTable.FindField("Name") == -1) { return false; }
                if (checkTable.FindField("FullName") == -1) { return false; }
                if (checkTable.FindField("Age") == -1) { return false; }
                if (checkTable.FindField("Description") == -1) { return false; }
                if (checkTable.FindField("HierarchyKey") == -1) { return false; }
                if (checkTable.FindField("ParagraphStyle") == -1) { return false; }
                if (checkTable.FindField("AreaFillRGB") == -1) { return false; }
                if (checkTable.FindField("AreaFillPatternDescription") == -1) { return false; }
                if (checkTable.FindField("DescriptionSourceID") == -1) { return false; }
                if (checkTable.FindField("GeneralLithologyTerm") == -1){ return false; }
                if (checkTable.FindField("GeneralLithologyConfidence") == -1) { return false; }

                checkTable = commonFunctions.OpenTable(Workspace, "DataSources");
                if (checkTable.FindField("DataSources_ID") == -1) { return false; }
                if (checkTable.FindField("Source") == -1) { return false; }
                if (checkTable.FindField("Notes") == -1) { return false; }

                checkTable = commonFunctions.OpenTable(Workspace, "Glossary");
                if (checkTable.FindField("Glossary_ID") == -1) { return false; }
                if (checkTable.FindField("Term") == -1) { return false; }
                if (checkTable.FindField("Definition") == -1) { return false; }
                if (checkTable.FindField("DefinitionSourceID") == -1) { return false; }

                checkTable = commonFunctions.OpenTable(Workspace, "DataSourcePolys");
                if (checkTable.FindField("DataSourcePolys_ID") == -1) { return false; }
                if (checkTable.FindField("DataSourceID") == -1) { return false; }
                if (checkTable.FindField("Notes") == -1) { return false; }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Identify if the SysInfo table exists. If not, a new SysInfo table is inserted.
        /// </summary>
        /// <param name="Workspace">The opened workspace</param>
        /// <returns>The SysInfo table exists or not</returns>
        public static bool IsSysInfoPresent(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, "SysInfo") == false) 
            {
                createSysInfoTable(theWorkspace, "SysInfo");
            }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "SysInfo");
            if (checkTable.FindField("Sub") == -1) { return false; }
            if (checkTable.FindField("Pred") == -1) { return false; }
            if (checkTable.FindField("Obj") == -1) { return false; }

            return true;
        }      

        #region "Create SysInfo Table"

        ///<summary>Creates a table with some default fields.</summary>
        /// 
        ///<param name="workspace">An IWorkspace2 interface</param>
        ///<param name="tableName">A System.String of the table name in the workspace. Example: "owners"</param>
        ///<param name="fields">An IFields interface or Nothing</param>
        ///  
        ///<returns>An ITable interface or Nothing</returns>

        public static ITable createSysInfoTable(IWorkspace2 workspace, System.String tableName)
        {
            // create the behavior clasid for the featureclass
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();

            if (workspace == null) return null; // valid feature workspace not passed in as an argument to the method

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace; // Explicit Cast
            ITable table;

            uid.Value = "esriGeoDatabase.Object";

            IObjectClassDescription objectClassDescription = new ObjectClassDescriptionClass();

            // create the fields using the required fields method
            IFields fields = objectClassDescription.RequiredFields;
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields; // Explicit Cast

            // add field to field collection
            fieldsEdit.AddField(newField("Sub", "Subject"));
            fieldsEdit.AddField(newField("Pred", "Predicate"));
            fieldsEdit.AddField(newField("Obj", "Object"));

            fields = (IFields)fieldsEdit; // Explicit Cast


            // Use IFieldChecker to create a validated fields collection.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // create and return the table
            table = featureWorkspace.CreateTable(tableName, validatedFields, uid, null, "");

            return table;
        }

        private static IField newField(string name, string alias)
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;

            // setup field properties
            fieldEdit.Name_2 = name;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldEdit.AliasName_2 = alias;
            fieldEdit.Editable_2 = true;

            return field;
        }

        #endregion

        public static bool IsAzgsStationAddinPresent(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            //Check that requisite datasets are present
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureDataset, commonFunctions.QualifyClassName(Workspace, "StationData")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "StationPoints")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "SamplePoints")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "OrientationDataPoints")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "Notes")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "RelatedDocuments")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRelationshipClass, commonFunctions.QualifyClassName(Workspace, "StationDocumentLink")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRelationshipClass, commonFunctions.QualifyClassName(Workspace, "StationSampleLink")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRelationshipClass, commonFunctions.QualifyClassName(Workspace, "StationOrientationDataPointsLink")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRelationshipClass, commonFunctions.QualifyClassName(Workspace, "StationNotesLink")) == false) { return false; }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "StationPoints");
            if (checkTable.FindField("StationPoints_ID") == -1) { return false; }
            if (checkTable.FindField("FieldID") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("Latitude") == -1) { return false; }
            if (checkTable.FindField("Longitude") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "SamplePoints");
            if (checkTable.FindField("SamplePoints_ID") == -1) { return false; }
            if (checkTable.FindField("FieldID") == -1) { return false; }
            if (checkTable.FindField("StationID") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "OrientationDataPoints");
            if (checkTable.FindField("OrientationDataPoints_ID") == -1) { return false; }
            if (checkTable.FindField("StationID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("Azimuth") == -1) { return false; }
            if (checkTable.FindField("Inclination") == -1) { return false; }
            if (checkTable.FindField("OrientationConfidenceDegrees") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "Notes");
            if (checkTable.FindField("Notes_ID") == -1) { return false; }
            if (checkTable.FindField("OwnerID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "RelatedDocuments");
            if (checkTable.FindField("RelatedDocuments_ID") == -1) { return false; }
            if (checkTable.FindField("OwnerID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("DocumentPath") == -1) { return false; }
            if (checkTable.FindField("DocumentName") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            return true;
        }

        public static bool IsStandardLithAddinPresent(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "StandardLithology")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "ExtendedAttributes")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "GeologicEvents")) == false) { return false; }
            //if (theWorkspace.get_NameExists(esriDatasetType.esriDTRelationshipClass, "DmuStandardLithologyLink") == false) { return false; }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "StandardLithology");
            if (checkTable.FindField("StandardLithology_ID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("PartType") == -1) { return false; }
            if (checkTable.FindField("Lithology") == -1) { return false; }
            if (checkTable.FindField("ProportionTerm") == -1) { return false; }
            if (checkTable.FindField("ProportionValue") == -1) { return false; }
            if (checkTable.FindField("ScientificConfidence") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "ExtendedAttributes");
            if (checkTable.FindField("ExtendedAttributes_ID") == -1) { return false; }
            if (checkTable.FindField("OwnerTable") == -1) { return false; }
            if (checkTable.FindField("OwnerID") == -1) { return false; }
            if (checkTable.FindField("Property") == -1) { return false; }
            if (checkTable.FindField("PropertyValue") == -1) { return false; }
            if (checkTable.FindField("ValueLinkID") == -1) { return false; }
            if (checkTable.FindField("Qualifier") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "GeologicEvents");
            if (checkTable.FindField("GeologicEvents_ID") == -1) { return false; }
            if (checkTable.FindField("Event") == -1) { return false; }
            if (checkTable.FindField("AgeDisplay") == -1) { return false; }
            if (checkTable.FindField("AgeYoungerTerm") == -1) { return false; }
            if (checkTable.FindField("AgeOlderTerm") == -1) { return false; }
            if (checkTable.FindField("TimeScale") == -1) { return false; }
            if (checkTable.FindField("AgeYoungerValue") == -1) { return false; }
            if (checkTable.FindField("AgeOlderValue") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            return true;
        }

        public static bool AreRepresentationsUsed(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "r_OrientationDataPoints")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "r_ContactsAndFaults")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "r_OtherLines")) == false) { return false; }
            return true;
        }

        public static bool IsTopologyUsed(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTopology, commonFunctions.QualifyClassName(Workspace, "GeologicMapTopology")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "OtherLines")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "OverlayPolys")) == false) { return false; }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "OtherLines");
            if (checkTable.FindField("OtherLines_ID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("ExistenceConfidence") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "OverlayPolys");
            if (checkTable.FindField("OverlayPolys_ID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            return true;

        }

        #region "Identify if this is Ralph's geodatabase by Genhan"
        public static bool IsWorkspaceRalphNCGMPCompliant(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            //Check that requisite datasets are present
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureDataset, commonFunctions.QualifyClassName(Workspace, "GeologicMap")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "MapUnitPolys")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "ContactsAndFaults")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "DescriptionOfMapUnits")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "DataSources")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTTable, commonFunctions.QualifyClassName(Workspace, "Glossary")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "DataSourcePolys")) == false) { return false; }

            //Check that requisite fields are present in each dataset
            ITable checkTable = commonFunctions.OpenTable(Workspace, "MapUnitPolys");
            if (checkTable.FindField("MapUnitPolys_ID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "ContactsAndFaults");
            if (checkTable.FindField("ContactsAndFaults_ID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("ExistenceConfidence") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("IsConcealed") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "DescriptionOfMapUnits");
            if (checkTable.FindField("DescriptionOfMapUnits_ID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Name") == -1) { return false; }
            if (checkTable.FindField("FullName") == -1) { return false; }
            if (checkTable.FindField("Age") == -1) { return false; }
            if (checkTable.FindField("Description") == -1) { return false; }
            if (checkTable.FindField("HierarchyKey") == -1) { return false; }
            if (checkTable.FindField("ParagraphStyle") == -1) { return false; }
            if (checkTable.FindField("AreaFillRGB") == -1) { return false; }
            if (checkTable.FindField("AreaFillPatternDescription") == -1) { return false; }
            if (checkTable.FindField("DescriptionSourceID") == -1) { return false; }
            if (checkTable.FindField("GeneralLithology") == -1) { return false; }
            if (checkTable.FindField("GeneralLithologyConfidence") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "DataSources");
            if (checkTable.FindField("DataSources_ID") == -1) { return false; }
            if (checkTable.FindField("Source") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "Glossary");
            if (checkTable.FindField("Glossary_ID") == -1) { return false; }
            if (checkTable.FindField("Term") == -1) { return false; }
            if (checkTable.FindField("Definition") == -1) { return false; }
            if (checkTable.FindField("DefinitionSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "DataSourcePolys");
            if (checkTable.FindField("DataSourcePolys_ID") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }

            return true;
        }

        public static bool IsRalphStationAddinPresent(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            //Check that requisite datasets are present
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "Stations")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "GenericSamples")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "OrientationPoints")) == false) { return false; }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "Stations");
            if (checkTable.FindField("Stations_ID") == -1) { return false; }
            if (checkTable.FindField("FieldID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("LocationMethod") == -1) { return false; }
            if (checkTable.FindField("TimeDate") == -1) { return false; }
            if (checkTable.FindField("Observer") == -1) { return false; }
            if (checkTable.FindField("SignificantDimensionMeters") == -1) { return false; }
            if (checkTable.FindField("GPSCoordinates") == -1) { return false; }
            if (checkTable.FindField("PDOP") == -1) { return false; }
            if (checkTable.FindField("MapCoordinates") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "GenericSamples");
            if (checkTable.FindField("GenericSamples_ID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("StationID") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("FieldSampleID") == -1) { return false; }
            if (checkTable.FindField("AlternateSampleID") == -1) { return false; }
            if (checkTable.FindField("MaterialAnalyzed") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("LocationSourceID") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "OrientationPoints");
            if (checkTable.FindField("OrientationPoints_ID") == -1) { return false; }
            if (checkTable.FindField("StationID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("MapUnit") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("PlotAtScale") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("Azimuth") == -1) { return false; }
            if (checkTable.FindField("Inclination") == -1) { return false; }
            if (checkTable.FindField("OrientationConfidenceDegrees") == -1) { return false; }
            if (checkTable.FindField("LocationSourceID") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            return true;
        }

        public static bool IsRalphTopologyUsed(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "GeologicLines")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, commonFunctions.QualifyClassName(Workspace, "OtherPolys")) == false) { return false; }

            ITable checkTable = commonFunctions.OpenTable(Workspace, "GeologicLines");
            if (checkTable.FindField("GeologicLines_ID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("IsConcealed") == -1) { return false; }
            if (checkTable.FindField("LocationConfidenceMeters") == -1) { return false; }
            if (checkTable.FindField("ExistenceConfidence") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            checkTable = commonFunctions.OpenTable(Workspace, "OtherPolys");
            if (checkTable.FindField("OtherPolys_ID") == -1) { return false; }
            if (checkTable.FindField("Type") == -1) { return false; }
            if (checkTable.FindField("IdentityConfidence") == -1) { return false; }
            if (checkTable.FindField("Label") == -1) { return false; }
            if (checkTable.FindField("Symbol") == -1) { return false; }
            if (checkTable.FindField("Notes") == -1) { return false; }
            if (checkTable.FindField("DataSourceID") == -1) { return false; }

            return true;

        }

        public static bool AreRalphRepresentationsUsed(IWorkspace Workspace)
        {
            IWorkspace2 theWorkspace = (IWorkspace2)Workspace;
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "OrientationPoints_Rep")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "ContactsAndFaults_Rep")) == false) { return false; }
            if (theWorkspace.get_NameExists(esriDatasetType.esriDTRepresentationClass, commonFunctions.QualifyClassName(Workspace, "GeologicLines_Rep")) == false) { return false; }
            return true;
        }
        
        #endregion

    }
}
