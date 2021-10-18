using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Db.DbObject;
using Db.Helpers;
using POCOGenerator.CommandLine;

namespace Db.POCOIterator
{
    public partial class DbIterator
    {


        public Options POCOConfiguration { get; set; } = new Options();


        #region Constructor

        protected IEnumerable<IDbObjectTraverse> dbObjects;
        protected IPOCOWriter pocoWriter;

        public const string TAB = "    ";
        public virtual string Tab { get; set; }

        public DbIterator(IEnumerable<IDbObjectTraverse> dbObjects, IPOCOWriter pocoWriter)
        {
            this.dbObjects = dbObjects;
            this.pocoWriter = pocoWriter;
            this.Tab = TAB;
        }

        #endregion

        #region Iterate

        public void Iterate()
        {
            Clear();

            if (dbObjects == null || dbObjects.Count() == 0)
                return;

            bool isExistDbObject = (dbObjects.Any(o => o.Error == null));

            string namespaceOffset = string.Empty;
            if (isExistDbObject)
            {
                // Using
                WriteUsing();

                // Namespace Start
                namespaceOffset = WriteNamespaceStart();
            }

            /*IEnumerable<Table> tables = null;
            if (IsNavigationProperties)
            {
                tables = dbObjects.Where(t => t.DbType == DbType.Table).Cast<Table>();
            }*/

            IDbObjectTraverse lastDbObject = dbObjects.Last();
            foreach (IDbObjectTraverse dbObject in dbObjects)
            {
                // Class Name
                string className = GetClassName(dbObject.Database.ToString(), dbObject.Schema, dbObject.Name, dbObject.DbType);
                dbObject.ClassName = className;

                if (dbObject.Error != null)
                {
                    // Error
                    WriteError(dbObject, namespaceOffset);
                }
                else
                {
                    // Navigation Properties
                    List<NavigationProperty> navigationProperties = GetNavigationProperties(dbObject/*, tables*/);

                    if (IsWriteObject(navigationProperties, dbObject))
                    {
                        // Class Attributes
                        WriteClassAttributes(dbObject, namespaceOffset);

                        // Class Start
                        WriteClassStart(className, dbObject, namespaceOffset);

                        // Constructor
                        WriteConstructor(className, navigationProperties, dbObject, namespaceOffset);

                        // Columns
                        if (dbObject.Columns != null && dbObject.Columns.Any())
                        {
                            var columns = dbObject.Columns.OrderBy<IColumn, int>(c => c.ColumnOrdinal ?? 0);
                            var lastColumn = columns.Last();
                            foreach (IColumn column in columns)
                                WriteColumn(column, column == lastColumn, dbObject, namespaceOffset);
                        }

                        // Navigation Properties
                        WriteNavigationProperties(navigationProperties, dbObject, namespaceOffset);

                        // Class End
                        WriteClassEnd(dbObject, namespaceOffset);
                    }
                }

                if (dbObject != lastDbObject)
                    pocoWriter.WriteLine();
            }

            if (isExistDbObject)
            {
                // Namespace End
                WriteNamespaceEnd();
            }
        }

        #endregion

        #region Clear

        public void Clear()
        {
            pocoWriter.Clear();
        }

        #endregion

        #region Using

        public virtual bool IsUsing { get; set; }

        protected virtual void WriteUsing()
        {
            if (IsUsing)
            {
                WriteUsingClause();
                pocoWriter.WriteLine();
            }
        }

        protected virtual void WriteUsingClause()
        {
            pocoWriter.WriteKeyword("using");
            pocoWriter.WriteLine(" System;");


            if (this.POCOConfiguration.IsEF)
            {
                if (dbObjects != null && dbObjects.Count() > 0)
                {
                    if (dbObjects.Any(o => o.DbType == POCODbType.Table))
                    {
                        if (this.POCOConfiguration.IsEFDescription)
                        {
                            pocoWriter.WriteKeyword("using");
                            pocoWriter.WriteLine(" System.ComponentModel;");
                        }

                        pocoWriter.WriteKeyword("using");
                        pocoWriter.WriteLine(" System.ComponentModel.DataAnnotations;");

                        pocoWriter.WriteKeyword("using");
                        pocoWriter.WriteLine(" System.ComponentModel.DataAnnotations.Schema;");
                    }
                }
            }


            if (this.POCOConfiguration.IsNavigationProperties)
            {
                pocoWriter.WriteKeyword("using");
                pocoWriter.WriteLine(" System.Collections.Generic;");
            }

            if (this.POCOConfiguration.IsEFCore)
            {
                pocoWriter.WriteKeyword("using");
                pocoWriter.WriteLine(" Microsoft.EntityFrameworkCore;");
            }

            if (IsSpecialSQLTypes())
            {
                pocoWriter.WriteKeyword("using");
                pocoWriter.WriteLine(" Microsoft.SqlServer.Types;");
            }
        }

        protected virtual bool IsSpecialSQLTypes()
        {
            if (dbObjects == null || dbObjects.Count() == 0)
                return false;

            foreach (var dbObject in dbObjects)
            {
                if (dbObject.Columns != null && dbObject.Columns.Any())
                {
                    foreach (IColumn column in dbObject.Columns)
                    {
                        string data_type = (column.DataTypeName ?? string.Empty).ToLower();
                        if (data_type.Contains("geography") || data_type.Contains("geometry") || data_type.Contains("hierarchyid"))
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion


        protected virtual bool IsCompositePrimaryKey(IDbObjectTraverse dbObject)
        {
            if (dbObject.Columns != null && dbObject.Columns.Count() > 0)
            {
                var primaryKeys = dbObject.Columns.Where(c => c.IsPrimaryKey);
                return (primaryKeys.Count() > 1);
            }
            return false;
        }

        protected virtual void WriteEFPrimaryKey(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Key");
            pocoWriter.WriteLine("]");
        }



        protected virtual void WriteEFCompositePrimaryKey(string columnName, string dataTypeName, byte ordinal, string namespaceOffset)
        {
            WriteEFPrimaryKey(namespaceOffset);

            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Column");
            pocoWriter.Write("(");

            if (this.POCOConfiguration.IsEFColumn)
            {
                pocoWriter.WriteString("\"");
                pocoWriter.WriteString(columnName);
                pocoWriter.WriteString("\"");
                pocoWriter.Write(", TypeName = ");
                pocoWriter.WriteString("\"");
                pocoWriter.WriteString(dataTypeName);
                pocoWriter.WriteString("\"");
                pocoWriter.Write(", ");
            }

            pocoWriter.Write("Order = ");
            pocoWriter.Write(ordinal.ToString());
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteEFIndex(string indexName, bool? isUnique, bool? isClustered, bool? isDescending, string namespaceOffset)
        {
            WriteEFIndexSortOrderError(indexName, isDescending, namespaceOffset);
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Index");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(indexName);
            pocoWriter.WriteString("\"");
            if (isUnique == true)
            {
                pocoWriter.Write(", IsUnique = ");
                pocoWriter.WriteKeyword("true");
            }
            if (isClustered == true)
            {
                pocoWriter.Write(", IsClustered = ");
                pocoWriter.WriteKeyword("true");
            }
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteEFCompositeIndex(string indexName, bool? isUnique, bool? isClustered, bool? isDescending, byte ordinal, string namespaceOffset)
        {
            WriteEFIndexSortOrderError(indexName, isDescending, namespaceOffset);
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Index");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(indexName);
            pocoWriter.WriteString("\"");
            pocoWriter.Write(", ");
            pocoWriter.Write(ordinal.ToString());
            if (isUnique == true)
            {
                pocoWriter.Write(", IsUnique = ");
                pocoWriter.WriteKeyword("true");
            }
            if (isClustered == true)
            {
                pocoWriter.Write(", IsClustered = ");
                pocoWriter.WriteKeyword("true");
            }
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteEFIndexSortOrderError(string indexName, bool? isDescending, string namespaceOffset)
        {
            if (isDescending == true)
            {
                pocoWriter.Write(namespaceOffset);
                pocoWriter.Write(Tab);
                pocoWriter.WriteError("/* ");
                pocoWriter.WriteError(indexName);
                pocoWriter.WriteLineError(". Sort order is Descending. Index doesn't support sort order. */");
            }
        }

        protected virtual void WriteEFColumn(string columnName, string dataTypeName, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Column");
            //pocoWriter.Write("(Name = ");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(columnName);
            pocoWriter.WriteString("\"");
            pocoWriter.Write(", TypeName = ");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(dataTypeName);
            pocoWriter.WriteString("\"");
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteEFMaxLength(int? stringPrecision, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("MaxLength");
            if (stringPrecision > 0)
            {
                pocoWriter.Write("(");
                pocoWriter.Write(stringPrecision.ToString());
                pocoWriter.Write(")");
            }
            pocoWriter.WriteLine("]");
        }

        protected virtual void WriteEFStringLength(int stringPrecision, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("StringLength");
            pocoWriter.Write("(");
            pocoWriter.Write(stringPrecision.ToString());
            pocoWriter.Write(")");
            pocoWriter.WriteLine("]");
        }

        protected virtual void WriteEFTimestamp(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Timestamp");
            pocoWriter.WriteLine("]");
        }

        protected virtual void WriteEFConcurrencyCheck(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("ConcurrencyCheck");
            pocoWriter.WriteLine("]");
        }

        protected virtual void WriteEFDatabaseGeneratedIdentity(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("DatabaseGenerated");
            pocoWriter.Write("(");
            pocoWriter.WriteUserType("DatabaseGeneratedOption");
            pocoWriter.WriteLine(".Identity)]");
        }

        protected virtual void WriteEFDatabaseGeneratedComputed(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("DatabaseGenerated");
            pocoWriter.Write("(");
            pocoWriter.WriteUserType("DatabaseGeneratedOption");
            pocoWriter.WriteLine(".Computed)]");
        }

        protected static readonly Regex regexDisplay1 = new Regex("[^0-9a-zA-Z]", RegexOptions.Compiled);
        protected static readonly Regex regexDisplay2 = new Regex("([^A-Z]|^)(([A-Z\\s]*)($|[A-Z]))", RegexOptions.Compiled);
        protected static readonly Regex regexDisplay3 = new Regex("\\s{2,}", RegexOptions.Compiled);

        protected virtual string GetEFDisplay(string columnName)
        {
            string display = columnName;
            display = regexDisplay1.Replace(display, " ");
            display = regexDisplay2.Replace(display, "$1 $3 $4");
            display = display.Trim();
            display = regexDisplay3.Replace(display, " ");
            return display;
        }

        protected virtual void WriteEFRequired(string display, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Required");
            if (this.POCOConfiguration.IsEFRequiredWithErrorMessage)
                WriteEFRequiredErrorMessage(display);
            pocoWriter.WriteLine("]");
        }

        protected virtual void WriteEFRequiredErrorMessage(string display)
        {
            pocoWriter.Write("(ErrorMessage = ");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(display);
            pocoWriter.WriteString(" is required");
            pocoWriter.WriteString("\"");
            pocoWriter.Write(")");
        }

        protected virtual void WriteEFDisplay(string display, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Display");
            pocoWriter.Write("(Name = ");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(display);
            pocoWriter.WriteString("\"");
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteEFDescription(string description, bool writeTab, string namespaceOffset)
        {
            if (string.IsNullOrEmpty(description) == false)
            {
                pocoWriter.Write(namespaceOffset);
                if (writeTab)
                    pocoWriter.Write(Tab);
                pocoWriter.Write("[");
                pocoWriter.WriteUserType("Description");
                pocoWriter.Write("(");
                pocoWriter.WriteString("\"");
                pocoWriter.WriteString(description);
                pocoWriter.WriteString("\"");
                pocoWriter.WriteLine(")]");
            }
        }

        protected void WriteColumnDataType(IColumn column)
        {

            if (this.POCOConfiguration.IsEF && this.POCOConfiguration.IsEFComplexType && column is IComplexType)
                pocoWriter.WriteUserType(column.DataTypeDisplay);
            else
                switch ((column.DataTypeDisplay ?? string.Empty).ToLower())
                {
                    case "bigint": WriteColumnBigInt(column.IsNullable); break;
                    case "binary": WriteColumnBinary(); break;
                    case "bit": WriteColumnBit(column.IsNullable); break;
                    case "char": WriteColumnChar(); break;
                    case "date": WriteColumnDate(column.IsNullable); break;
                    case "datetime": WriteColumnDateTime(column.IsNullable); break;
                    case "datetime2": WriteColumnDateTime2(column.IsNullable); break;
                    case "datetimeoffset": WriteColumnDateTimeOffset(column.IsNullable); break;
                    case "decimal": WriteColumnDecimal(column.IsNullable); break;
                    case "filestream": WriteColumnFileStream(); break;
                    case "float": WriteColumnFloat(column.IsNullable); break;
                    case "geography": WriteColumnGeography(); break;
                    case "geometry": WriteColumnGeometry(); break;
                    case "hierarchyid": WriteColumnHierarchyId(); break;
                    case "image": WriteColumnImage(); break;
                    case "int": WriteColumnInt(column.IsNullable); break;
                    case "money": WriteColumnMoney(column.IsNullable); break;
                    case "nchar": WriteColumnNChar(); break;
                    case "ntext": WriteColumnNText(); break;
                    case "numeric": WriteColumnNumeric(column.IsNullable); break;
                    case "nvarchar": WriteColumnNVarChar(); break;
                    case "real": WriteColumnReal(column.IsNullable); break;
                    case "rowversion": WriteColumnRowVersion(); break;
                    case "smalldatetime": WriteColumnSmallDateTime(column.IsNullable); break;
                    case "smallint": WriteColumnSmallInt(column.IsNullable); break;
                    case "smallmoney": WriteColumnSmallMoney(column.IsNullable); break;
                    case "sql_variant": WriteColumnSqlVariant(); break;
                    case "text": WriteColumnText(); break;
                    case "time": WriteColumnTime(column.IsNullable); break;
                    case "timestamp": WriteColumnTimeStamp(); break;
                    case "tinyint": WriteColumnTinyInt(column.IsNullable); break;
                    case "uniqueidentifier": WriteColumnUniqueIdentifier(column.IsNullable); break;
                    case "varbinary": WriteColumnVarBinary(); break;
                    case "varchar": WriteColumnVarChar(); break;
                    case "xml": WriteColumnXml(); break;
                    default: WriteColumnObject(); break;
                }
        }

        #region Namespace Start

        public virtual string Namespace { get; set; }

        protected virtual string WriteNamespaceStart()
        {
            string namespaceOffset = string.Empty;

            if (string.IsNullOrEmpty(Namespace) == false)
            {
                WriteNamespaceStartClause();
                namespaceOffset = Tab;
            }

            return namespaceOffset;
        }

        protected virtual void WriteNamespaceStartClause()
        {
            pocoWriter.WriteKeyword("namespace");
            pocoWriter.Write(" ");
            pocoWriter.WriteLine(Namespace);
            pocoWriter.WriteLine("{");
        }

        #endregion

        #region Error

        protected virtual void WriteError(IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteLineError("/*");

            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteLineError(string.Format("{0}.{1}", dbObject.Database.ToString(), dbObject.ToString()));

            Exception currentError = dbObject.Error;
            while (currentError != null)
            {
                pocoWriter.Write(namespaceOffset);
                pocoWriter.WriteLineError(currentError.Message);
                currentError = currentError.InnerException;
            }

            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteLineError("*/");
        }

        #endregion

        #region Is Write Object

        protected virtual bool IsWriteObject(List<NavigationProperty> navigationProperties, IDbObjectTraverse dbObject)
        {
            if (dbObject.DbType == POCODbType.Table)
            {
                if (this.POCOConfiguration.IsNavigationPropertiesShowJoinTable == false)
                {
                    if (navigationProperties != null && navigationProperties.Count > 0)
                    {
                        // hide many-to-many join table.
                        // join table is complete. all the columns are part of the pk. there are no columns other than the pk.
                        return navigationProperties.All(p => p.IsRefFrom && p.ForeignKey.Is_Many_To_Many && p.ForeignKey.Is_Many_To_Many_Complete) == false;
                    }
                }
            }

            return true;
        }

        #endregion



        #region Class Name

        public virtual string Prefix { get; set; }
        public virtual string FixedClassName { get; set; }
        public virtual bool IsIncludeDB { get; set; }
        public virtual bool IsCamelCase { get; set; }
        public virtual string WordsSeparator { get; set; }
        public virtual bool IsUpperCase { get; set; }
        public virtual bool IsLowerCase { get; set; }
        public virtual string DBSeparator { get; set; }
        public virtual bool IsIncludeSchema { get; set; }
        public virtual bool IsIgnoreDboSchema { get; set; }
        public virtual string SchemaSeparator { get; set; }
        public virtual bool IsSingular { get; set; }
        public virtual string Search { get; set; }
        public virtual string Replace { get; set; }
        public virtual bool IsSearchIgnoreCase { get; set; }
        public virtual string Suffix { get; set; }

        protected virtual string GetClassName(string database, string schema, string name, POCODbType dbType)
        {
            string className = null;

            // prefix
            if (string.IsNullOrEmpty(Prefix) == false)
                className += Prefix;

            if (string.IsNullOrEmpty(FixedClassName))
            {
                if (IsIncludeDB)
                {
                    // database
                    if (IsCamelCase || string.IsNullOrEmpty(WordsSeparator) == false)
                        className += NameHelper.TransformName(database, WordsSeparator, IsCamelCase, IsUpperCase, IsLowerCase);
                    else if (IsUpperCase)
                        className += database.ToUpper();
                    else if (IsLowerCase)
                        className += database.ToLower();
                    else
                        className += database;

                    // db separator
                    if (string.IsNullOrEmpty(DBSeparator) == false)
                        className += DBSeparator;
                }

                if (IsIncludeSchema)
                {
                    if (IsIgnoreDboSchema == false || schema != "dbo")
                    {
                        // schema
                        if (IsCamelCase || string.IsNullOrEmpty(WordsSeparator) == false)
                            className += NameHelper.TransformName(schema, WordsSeparator, IsCamelCase, IsUpperCase, IsLowerCase);
                        else if (IsUpperCase)
                            className += schema.ToUpper();
                        else if (IsLowerCase)
                            className += schema.ToLower();
                        else
                            className += schema;

                        // schema separator
                        if (string.IsNullOrEmpty(SchemaSeparator) == false)
                            className += SchemaSeparator;
                    }
                }

                // name
                if (IsSingular)
                {
                    if (dbType == POCODbType.Table || dbType == POCODbType.View || dbType == POCODbType.TVP)
                        name = NameHelper.GetSingularName(name);
                }

                if (IsCamelCase || string.IsNullOrEmpty(WordsSeparator) == false)
                    className += NameHelper.TransformName(name, WordsSeparator, IsCamelCase, IsUpperCase, IsLowerCase);
                else if (IsUpperCase)
                    className += name.ToUpper();
                else if (IsLowerCase)
                    className += name.ToLower();
                else
                    className += name;

                if (string.IsNullOrEmpty(Search) == false)
                {
                    if (IsSearchIgnoreCase)
                        className = Regex.Replace(className, Search, Replace ?? string.Empty, RegexOptions.IgnoreCase);
                    else
                        className = className.Replace(Search, Replace ?? string.Empty);
                }
            }
            else
            {
                // fixed name
                className += FixedClassName;
            }

            // postfix
            if (string.IsNullOrEmpty(Suffix) == false)
                className += Suffix;

            return NameHelper.CleanName(className);
        }

        #endregion

        #region Class Start

        public virtual bool IsPartialClass { get; set; }
        public virtual string Inherit { get; set; }

        protected virtual void WriteClassStart(string className, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteKeyword("public");
            pocoWriter.Write(" ");
            if (IsPartialClass)
            {
                pocoWriter.WriteKeyword("partial");
                pocoWriter.Write(" ");
            }
            pocoWriter.WriteKeyword("class");
            pocoWriter.Write(" ");
            pocoWriter.WriteUserType(className);
            if (string.IsNullOrEmpty(Inherit) == false)
            {
                pocoWriter.Write(" : ");
                string[] inherit = Inherit.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                pocoWriter.WriteUserType(inherit[0]);
                for (int i = 1; i < inherit.Length; i++)
                {
                    pocoWriter.Write(", ");
                    pocoWriter.WriteUserType(inherit[i]);
                }
            }
            pocoWriter.WriteLine();

            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteLine("{");
        }

        #endregion

        #region Class Constructor

        protected virtual void WriteConstructor(string className, List<NavigationProperty> navigationProperties, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            bool hasColumnDefaults = HasColumnDefaults(dbObject);
            bool hasNavigationProperties = HasNavigationProperties(dbObject, navigationProperties);

            if (hasColumnDefaults || hasNavigationProperties)
            {
                WriteConstructorStart(className, dbObject, namespaceOffset);

                if (hasColumnDefaults)
                {
                    Table table = (Table)dbObject;
                    foreach (var column in table.TableColumns.Where(c => c.column_default != null).OrderBy(c => c.ordinal_position ?? 0))
                        WriteColumnDefaultConstructorInitialization(column, namespaceOffset);
                }

                if (hasColumnDefaults && hasNavigationProperties)
                    pocoWriter.WriteLine();

                if (hasNavigationProperties)
                {
                    foreach (var np in navigationProperties.Where(p => p.IsSingle == false))
                        WriteNavigationPropertyConstructorInitialization(np, namespaceOffset);
                }

                WriteConstructorEnd(dbObject, namespaceOffset);
                pocoWriter.WriteLine();
            }
        }

        protected virtual void WriteConstructorStart(string className, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.WriteKeyword("public");
            pocoWriter.Write(" ");
            pocoWriter.Write(className);
            pocoWriter.WriteLine("()");

            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.WriteLine("{");
        }

        protected virtual void WriteConstructorEnd(IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.WriteLine("}");
        }

        #region Class Constructor - Navigation Properties

        protected virtual bool HasNavigationProperties(IDbObjectTraverse dbObject, List<NavigationProperty> navigationProperties)
        {
            return
                IsNavigableObject(dbObject) &&
                navigationProperties != null &&
                navigationProperties.Count > 0 &&
                navigationProperties.Any(p => p.IsSingle == false);
        }

        protected virtual void WriteNavigationPropertyConstructorInitialization(NavigationProperty navigationProperty, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write(Tab);
            pocoWriter.WriteKeyword("this");
            pocoWriter.Write(".");
            pocoWriter.Write(navigationProperty.ToString());
            pocoWriter.Write(" = ");
            pocoWriter.WriteKeyword("new");
            pocoWriter.Write(" ");
            pocoWriter.WriteUserType(this.POCOConfiguration.IsNavigationPropertiesICollection ? "HashSet" : "List");
            pocoWriter.Write("<");
            pocoWriter.WriteUserType(navigationProperty.ClassName);
            pocoWriter.WriteLine(">();");
        }

        #endregion

        #region Class Constructor - Column Defaults

        public virtual bool IsColumnDefaults { get; set; }

        protected virtual bool HasColumnDefaults(IDbObjectTraverse dbObject)
        {
            if (IsColumnDefaults && dbObject.DbType == POCODbType.Table)
            {
                Table table = (Table)dbObject;
                if (table.TableColumns != null && table.TableColumns.Count > 0)
                    return table.TableColumns.Any(c => c.column_default != null);
            }

            return false;
        }

        protected virtual void WriteColumnDefaultConstructorInitialization(TableColumn column, string namespaceOffset)
        {
            string columnDefault = column.column_default;
            columnDefault = CleanColumnDefault(columnDefault);

            if (column.DataTypeName == "uniqueidentifier")
            {
                if (columnDefault.IndexOf("newid", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    WriteColumnDefaultConstructorInitializationGuid(column, namespaceOffset);
                    return;
                }
            }
            else if (column.DataTypeName == "int" ||
                column.DataTypeName == "smallint" ||
                column.DataTypeName == "bigint" ||
                column.DataTypeName == "tinyint" ||
                column.DataTypeName == "float" ||
                column.DataTypeName == "decimal" ||
                column.DataTypeName == "numeric" ||
                column.DataTypeName == "money" ||
                column.DataTypeName == "smallmoney" ||
                column.DataTypeName == "real")
            {
                WriteColumnDefaultConstructorInitializationNumber(columnDefault, column, namespaceOffset);
                return;
            }
            else if (column.DataTypeName == "bit")
            {
                if (columnDefault.IndexOf("1", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    WriteColumnDefaultConstructorInitializationBool(true, column, namespaceOffset);
                    return;
                }
                else if (columnDefault.IndexOf("0", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    WriteColumnDefaultConstructorInitializationBool(false, column, namespaceOffset);
                    return;
                }
            }
            else if (column.DataTypeName == "date" ||
                column.DataTypeName == "datetime" ||
                column.DataTypeName == "datetime2" ||
                column.DataTypeName == "smalldatetime" ||
                column.DataTypeName == "time" ||
                column.DataTypeName == "datetimeoffset")
            {
                if (columnDefault.IndexOf("getutcdate", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                    columnDefault.IndexOf("sysutcdatetime", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    WriteColumnDefaultConstructorInitializationDateTimeUtcNow(column, namespaceOffset);
                    return;
                }
                else if (columnDefault.IndexOf("getdate", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                    columnDefault.IndexOf("sysdatetime", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                    columnDefault.IndexOf("sysdatetimeoffset", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                    columnDefault.IndexOf("current_timestamp", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    WriteColumnDefaultConstructorInitializationDateTimeNow(column, namespaceOffset);
                    return;
                }
                else
                {
                    DateTime date = DateTime.MinValue;
                    if (DateTime.TryParse(columnDefault, out date))
                    {
                        WriteColumnDefaultConstructorInitializationDateTime(date, column, namespaceOffset);
                        return;
                    }
                }
            }
            else if (column.DataTypeName == "char" ||
                column.DataTypeName == "nchar" ||
                column.DataTypeName == "ntext" ||
                column.DataTypeName == "nvarchar" ||
                column.DataTypeName == "text" ||
                column.DataTypeName == "varchar" ||
                column.DataTypeName == "xml")
            {
                WriteColumnDefaultConstructorInitializationString(columnDefault, column, namespaceOffset);
                return;
            }
            else if (column.DataTypeName == "binary" ||
                column.DataTypeName == "varbinary" ||
                column.DataTypeName == "filestream" ||
                column.DataTypeName == "image")
            {
                WriteColumnDefaultConstructorInitializationBinary(columnDefault, column, namespaceOffset);
                return;
            }

            WriteColumnDefaultConstructorInitializationDefault(column.column_default, column, namespaceOffset);
        }

        protected virtual string CleanColumnDefault(string columnDefault)
        {
            if (columnDefault.StartsWith("('") && columnDefault.EndsWith("')"))
                columnDefault = columnDefault.Substring(2, columnDefault.Length - 4);
            else if (columnDefault.StartsWith("(N'") && columnDefault.EndsWith("')"))
                columnDefault = columnDefault.Substring(3, columnDefault.Length - 5);
            else if (columnDefault.StartsWith("((") && columnDefault.EndsWith("))"))
                columnDefault = columnDefault.Substring(2, columnDefault.Length - 4);
            return columnDefault;
        }

        protected virtual void WriteColumnDefaultConstructorInitializationGuid(TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteUserType("Guid");
            pocoWriter.Write(".NewGuid()");
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationNumber(string columnDefault, TableColumn column, string namespaceOffset)
        {
            columnDefault = columnDefault.Replace("(", string.Empty).Replace(")", string.Empty);
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.Write(columnDefault);
            if (column.DataTypeName == "decimal" || column.DataTypeName == "numeric" || column.DataTypeName == "money" || column.DataTypeName == "smallmoney")
                pocoWriter.Write("M");
            else if (column.DataTypeName == "real")
                pocoWriter.Write("F");
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationBool(bool value, TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteKeyword(value.ToString().ToLower());
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationDateTimeUtcNow(TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteUserType(column.DataTypeName == "datetimeoffset" ? "DateTimeOffset" : "DateTime");
            pocoWriter.Write(".UtcNow");
            if (column.DataTypeName == "time")
                pocoWriter.Write(".TimeOfDay");
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationDateTimeNow(TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteUserType(column.DataTypeName == "datetimeoffset" ? "DateTimeOffset" : "DateTime");
            pocoWriter.Write(".Now");
            if (column.DataTypeName == "time")
                pocoWriter.Write(".TimeOfDay");
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationDateTime(DateTime date, TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);

            if (column.DataTypeName == "datetimeoffset")
            {
                pocoWriter.WriteKeyword("new ");
                pocoWriter.WriteUserType("DateTimeOffset");
                pocoWriter.Write("(");
            }

            pocoWriter.WriteKeyword("new ");
            pocoWriter.WriteUserType("DateTime");
            pocoWriter.Write("(");
            pocoWriter.Write(date.Year.ToString());
            pocoWriter.Write(", ");
            pocoWriter.Write(date.Month.ToString());
            pocoWriter.Write(", ");
            pocoWriter.Write(date.Day.ToString());
            if (date.Hour != 0 || date.Minute != 0 || date.Second != 0 || date.Millisecond != 0)
            {
                pocoWriter.Write(", ");
                pocoWriter.Write(date.Hour.ToString());
                pocoWriter.Write(", ");
                pocoWriter.Write(date.Minute.ToString());
                pocoWriter.Write(", ");
                pocoWriter.Write(date.Second.ToString());
                if (date.Millisecond != 0)
                {
                    pocoWriter.Write(", ");
                    pocoWriter.Write(date.Millisecond.ToString());
                }
            }
            pocoWriter.Write(")");

            if (column.DataTypeName == "time")
                pocoWriter.Write(".TimeOfDay");

            if (column.DataTypeName == "datetimeoffset")
                pocoWriter.Write(")");

            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationString(string columnDefault, TableColumn column, string namespaceOffset)
        {
            columnDefault = columnDefault.Replace("\\", "\\\\").Replace("\"", "\\\"");
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(columnDefault);
            pocoWriter.WriteString("\"");
            WriteColumnDefaultConstructorInitializationEnd();
        }

        protected virtual void WriteColumnDefaultConstructorInitializationBinary(string columnDefault, TableColumn column, string namespaceOffset)
        {
            columnDefault = columnDefault.Replace("(", string.Empty).Replace(")", string.Empty);

            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset);
            pocoWriter.WriteUserType("BitConverter");
            pocoWriter.Write(".GetBytes(");
            if (columnDefault.StartsWith("0x"))
            {
                pocoWriter.WriteUserType("Convert");
                pocoWriter.Write(".ToInt32(");
                pocoWriter.WriteString("\"");
                pocoWriter.WriteString(columnDefault);
                pocoWriter.WriteString("\"");
                pocoWriter.Write(", 16)");
            }
            else
            {
                pocoWriter.Write(columnDefault);
            }
            pocoWriter.Write(")");
            WriteColumnDefaultConstructorInitializationEnd();

            string cleanColumnName = NameHelper.CleanName(column.ColumnName);
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write(Tab);
            pocoWriter.WriteKeyword("if");
            pocoWriter.Write(" (");
            pocoWriter.WriteUserType("BitConverter");
            pocoWriter.WriteLine(".IsLittleEndian)");
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write(Tab);
            pocoWriter.Write(Tab);
            pocoWriter.WriteUserType("Array");
            pocoWriter.Write(".Reverse(");
            pocoWriter.WriteKeyword("this");
            pocoWriter.Write(".");
            pocoWriter.Write(cleanColumnName);
            pocoWriter.WriteLine(");");
        }

        protected virtual void WriteColumnDefaultConstructorInitializationDefault(string columnDefault, TableColumn column, string namespaceOffset)
        {
            WriteColumnDefaultConstructorInitializationStart(column, namespaceOffset, true);
            pocoWriter.WriteComment(columnDefault);
            WriteColumnDefaultConstructorInitializationEnd(true);
        }

        protected virtual void WriteColumnDefaultConstructorInitializationStart(TableColumn column, string namespaceOffset, bool isComment = false)
        {
            string cleanColumnName = NameHelper.CleanName(column.ColumnName);
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write(Tab);
            if (isComment)
            {
                pocoWriter.WriteComment("/* this.");
                pocoWriter.WriteComment(cleanColumnName);
                pocoWriter.WriteComment(" = ");
            }
            else
            {
                pocoWriter.WriteKeyword("this");
                pocoWriter.Write(".");
                pocoWriter.Write(cleanColumnName);
                pocoWriter.Write(" = ");
            }
        }

        protected virtual void WriteColumnDefaultConstructorInitializationEnd(bool isComment = false)
        {
            if (isComment)
                pocoWriter.WriteLineComment("; */");
            else
                pocoWriter.WriteLine(";");
        }

        #endregion

        #endregion

        #region Column Attributes


        protected void WriteColumnDocumentation(IColumn column, string cleanColumnName, IDbObjectTraverse dbObject, string namespaceOffset)
        {

            if (this.POCOConfiguration.IsComments)
            {
                pocoWriter.WriteLine(""); pocoWriter.Write(namespaceOffset); pocoWriter.Write(Tab);
                pocoWriter.WriteComment($"//// <summary>");

                pocoWriter.WriteLine(""); pocoWriter.Write(namespaceOffset); pocoWriter.Write(Tab);
                pocoWriter.WriteComment($"//// {dbObject.DbType}: {dbObject.Name} | {column.ColumnName} {column.DataTypeName}{column.Precision} {(!this.POCOConfiguration.IsCommentsWithoutNull ? (column.IsNullable ? "NULL" : "NOT NULL") : "")}");

                pocoWriter.WriteLine(""); pocoWriter.Write(namespaceOffset); pocoWriter.Write(Tab);
                pocoWriter.WriteComment($"//// </summary>");
                pocoWriter.WriteLine("");
            }


        }


        #endregion



      


        protected virtual void WriteColumnBase(IColumn column, bool isLastColumn, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            string cleanColumnName = NameHelper.CleanName(column.ColumnName);


            WriteColumnDocumentation(column, cleanColumnName, dbObject, namespaceOffset);


            WriteColumnAttributes(column, cleanColumnName, dbObject, namespaceOffset);

            WriteColumnStart(namespaceOffset);



            WriteColumnDataType(column);

            WriteColumnName(cleanColumnName);

            WriteColumnEnd();



            pocoWriter.WriteLine();

            if (this.POCOConfiguration.IsNewLineBetweenMembers && isLastColumn == false)
                pocoWriter.WriteLine();
        }

        protected virtual void WriteColumnStart(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.WriteKeyword("public");
            pocoWriter.Write(" ");

            if (this.POCOConfiguration.IsProperties && this.POCOConfiguration.IsVirtualProperties)
            {
                pocoWriter.WriteKeyword("virtual");
                pocoWriter.Write(" ");
            }
            else if (this.POCOConfiguration.IsProperties && this.POCOConfiguration.IsOverrideProperties)
            {
                pocoWriter.WriteKeyword("override");
                pocoWriter.Write(" ");
            }
        }



        protected virtual void WriteColumnName(string columnName)
        {
            pocoWriter.Write(" ");
            pocoWriter.Write(columnName);
        }

        protected virtual void WriteColumnEnd()
        {
            if (this.POCOConfiguration.IsProperties)
            {
                pocoWriter.Write(" { ");
                pocoWriter.WriteKeyword("get");
                pocoWriter.Write("; ");
                pocoWriter.WriteKeyword("set");
                pocoWriter.Write("; }");
            }
            else
            {
                pocoWriter.Write(";");
            }
        }



        #region Column Data Types

        protected virtual void WriteColumnBigInt(bool isNullable)
        {
            pocoWriter.WriteKeyword("long");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnBinary()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnBit(bool isNullable)
        {
            pocoWriter.WriteKeyword("bool");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnChar()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnDate(bool isNullable)
        {
            pocoWriter.WriteUserType("DateTime");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnDateTime(bool isNullable)
        {
            pocoWriter.WriteUserType("DateTime");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnDateTime2(bool isNullable)
        {
            pocoWriter.WriteUserType("DateTime");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnDateTimeOffset(bool isNullable)
        {
            pocoWriter.WriteUserType("DateTimeOffset");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnDecimal(bool isNullable)
        {
            pocoWriter.WriteKeyword("decimal");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnFileStream()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnFloat(bool isNullable)
        {
            pocoWriter.WriteKeyword("double");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnGeography()
        {
            if (IsUsing == false)
                pocoWriter.Write("Microsoft.SqlServer.Types.");
            pocoWriter.WriteUserType("SqlGeography");
        }

        protected virtual void WriteColumnGeometry()
        {
            if (IsUsing == false)
                pocoWriter.Write("Microsoft.SqlServer.Types.");
            pocoWriter.WriteUserType("SqlGeometry");
        }

        protected virtual void WriteColumnHierarchyId()
        {
            if (IsUsing == false)
                pocoWriter.Write("Microsoft.SqlServer.Types.");
            pocoWriter.WriteUserType("SqlHierarchyId");
        }

        protected virtual void WriteColumnImage()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnInt(bool isNullable)
        {
            pocoWriter.WriteKeyword("int");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnMoney(bool isNullable)
        {
            pocoWriter.WriteKeyword("decimal");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnNChar()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnNText()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnNumeric(bool isNullable)
        {
            pocoWriter.WriteKeyword("decimal");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnNVarChar()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnReal(bool isNullable)
        {
            pocoWriter.WriteUserType("Single");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnRowVersion()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnSmallDateTime(bool isNullable)
        {
            pocoWriter.WriteUserType("DateTime");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnSmallInt(bool isNullable)
        {
            pocoWriter.WriteKeyword("short");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnSmallMoney(bool isNullable)
        {
            pocoWriter.WriteKeyword("decimal");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnSqlVariant()
        {
            pocoWriter.WriteKeyword("object");
        }

        protected virtual void WriteColumnText()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnTime(bool isNullable)
        {
            pocoWriter.WriteUserType("TimeSpan");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnTimeStamp()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnTinyInt(bool isNullable)
        {
            pocoWriter.WriteKeyword("byte");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnUniqueIdentifier(bool isNullable)
        {
            pocoWriter.WriteUserType("Guid");
            if (isNullable || this.POCOConfiguration.IsAllStructNullable)
                pocoWriter.Write("?");
        }

        protected virtual void WriteColumnVarBinary()
        {
            pocoWriter.WriteKeyword("byte");
            pocoWriter.Write("[]");
        }

        protected virtual void WriteColumnVarChar()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnXml()
        {
            pocoWriter.WriteKeyword("string");
        }

        protected virtual void WriteColumnObject()
        {
            pocoWriter.WriteKeyword("object");
        }

        #endregion

 

        #region Navigation Properties

  

        protected virtual bool IsNavigableObject(IDbObjectTraverse dbObject)
        {
            return (this.POCOConfiguration.IsNavigationProperties && dbObject.DbType == POCODbType.Table);
        }

        #region Get Navigation Properties

        protected virtual List<NavigationProperty> GetNavigationProperties(IDbObjectTraverse dbObject/*, IEnumerable<Table> tables*/)
        {
            List<NavigationProperty> navigationProperties = null;

            if (IsNavigableObject(dbObject))
            {
                if (dbObject.Columns != null && dbObject.Columns.Any())
                {
                    // columns are referencing (IsForeignKey)
                    var columnsFrom = dbObject.Columns.Where(c => c.HasForeignKeys).OrderBy<IColumn, int>(c => c.ColumnOrdinal ?? 0);
                    if (columnsFrom.Any())
                    {
                        if (navigationProperties == null)
                            navigationProperties = new List<NavigationProperty>();

                        foreach (var column in columnsFrom.Cast<TableColumn>())
                        {
                            foreach (var fk in column.ForeignKeys)
                            {
                                string className = GetClassName(dbObject.Database.ToString(), fk.Primary_Schema, fk.Primary_Table, dbObject.DbType);
                                fk.NavigationPropertyRefFrom.ClassName = className;
                                navigationProperties.Add(fk.NavigationPropertyRefFrom);
                            }
                        }
                    }

                    // columns are referenced (IsPrimaryForeignKey)
                    var columnsTo = dbObject.Columns.Where(c => c.HasPrimaryForeignKeys).OrderBy<IColumn, int>(c => c.ColumnOrdinal ?? 0);
                    if (columnsTo.Any())
                    {
                        if (navigationProperties == null)
                            navigationProperties = new List<NavigationProperty>();

                        foreach (var column in columnsTo.Cast<TableColumn>())
                        {
                            foreach (var fk in column.PrimaryForeignKeys)
                            {
                                string className = GetClassName(dbObject.Database.ToString(), fk.Foreign_Schema, fk.Foreign_Table, dbObject.DbType);

                                if (this.POCOConfiguration.IsNavigationPropertiesShowJoinTable == false && fk.NavigationPropertiesRefToManyToMany != null)
                                {
                                    foreach (var np in fk.NavigationPropertiesRefToManyToMany)
                                    {
                                        np.ClassName = className;
                                        navigationProperties.Add(np);
                                    }
                                }
                                else
                                {
                                    fk.NavigationPropertyRefTo.ClassName = className;
                                    navigationProperties.Add(fk.NavigationPropertyRefTo);
                                }
                            }
                        }
                    }

                    // remove tables that don't participate
                    /*if (navigationProperties != null && navigationProperties.Count > 0)
                    {
                        if (tables == null || tables.Count() == 0)
                        {
                            navigationProperties.Clear();
                        }
                        else
                        {
                            navigationProperties.RemoveAll(np => (
                                (np.IsRefFrom && tables.Contains(np.ForeignKey.ToTable)) ||
                                (np.IsRefFrom == false && tables.Contains(np.ForeignKey.FromTable))
                            ) == false);
                        }
                    }*/

                    // rename duplicates
                    RenameDuplicateNavigationProperties(navigationProperties, dbObject);
                }
            }

            GetNavigationPropertiesMultipleRelationships(navigationProperties);
            return navigationProperties;

        }

        protected static readonly Regex regexEndNumber = new Regex("(\\d+)$", RegexOptions.Compiled);

        protected virtual void RenameDuplicateNavigationProperties(List<NavigationProperty> navigationProperties, IDbObjectTraverse dbObject)
        {
            if (navigationProperties != null && navigationProperties.Count > 0)
            {
                // groups of navigation properties with the same name
                var groups1 = navigationProperties.GroupBy(p => p.ToString()).Where(g => g.Count() > 1);

                // if the original column name ended with a number, then assign that number to the property name
                foreach (var group in groups1)
                {
                    foreach (var np in group)
                    {
                        string columnName = (np.IsRefFrom ? np.ForeignKey.Primary_Column : np.ForeignKey.Foreign_Column);
                        var match = regexEndNumber.Match(columnName);
                        if (match.Success)
                            np.RenamedPropertyName = np.ToString() + match.Value;
                    }
                }

                // if there are still duplicate property names, then rename them with a running number suffix
                var groups2 = navigationProperties.GroupBy(p => p.ToString()).Where(g => g.Count() > 1);
                foreach (var group in groups2)
                {
                    int suffix = 1;
                    foreach (var np in group.Skip(1))
                        np.RenamedPropertyName = np.ToString() + (suffix++);
                }
            }
        }

        #endregion

        #region Write Navigation Properties

        protected virtual void WriteNavigationProperties(List<NavigationProperty> navigationProperties, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (IsNavigableObject(dbObject))
            {
                if (navigationProperties != null && navigationProperties.Count > 0)
                {
                    if (this.POCOConfiguration.IsNewLineBetweenMembers == false)
                        pocoWriter.WriteLine();

                    foreach (var np in navigationProperties)
                        WriteNavigationProperty(np, dbObject, namespaceOffset);
                }
            }
        }

        protected virtual void WriteNavigationProperty(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsNewLineBetweenMembers)
                pocoWriter.WriteLine();

            WriteNavigationPropertyComments(navigationProperty, dbObject, namespaceOffset);

            WriteNavigationPropertyAttributes(navigationProperty, dbObject, namespaceOffset);

            if (navigationProperty.IsSingle)
                WriteNavigationPropertySingle(navigationProperty, dbObject, namespaceOffset);
            else
                WriteNavigationPropertyMultiple(navigationProperty, dbObject, namespaceOffset);
        }

        protected virtual void WriteNavigationPropertyComments(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsNavigationPropertiesComments)
            {
                pocoWriter.Write(namespaceOffset);
                pocoWriter.Write(Tab);
                pocoWriter.WriteComment("// ");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Foreign_Schema);
                pocoWriter.WriteComment(".");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Foreign_Table);
                pocoWriter.WriteComment(".");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Foreign_Column);
                pocoWriter.WriteComment(" -> ");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Primary_Schema);
                pocoWriter.WriteComment(".");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Primary_Table);
                pocoWriter.WriteComment(".");
                pocoWriter.WriteComment(navigationProperty.ForeignKey.Primary_Column);
                if (string.IsNullOrEmpty(navigationProperty.ForeignKey.Name) == false)
                {
                    pocoWriter.WriteComment(" (");
                    pocoWriter.WriteComment(navigationProperty.ForeignKey.Name);
                    pocoWriter.WriteComment(")");
                }
                pocoWriter.WriteLine();
            }
        }



        protected virtual void WriteNavigationPropertySingle(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            WriteNavigationPropertyStart(namespaceOffset);
            pocoWriter.WriteUserType(navigationProperty.ClassName);
            pocoWriter.Write(" ");
            pocoWriter.Write(navigationProperty.ToString());
            WriteNavigationPropertyEnd();
            pocoWriter.WriteLine();
        }

        protected virtual void WriteNavigationPropertyMultiple(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            WriteNavigationPropertyStart(namespaceOffset);
            if (this.POCOConfiguration.IsNavigationPropertiesList)
                pocoWriter.WriteUserType("List");
            else if (this.POCOConfiguration.IsNavigationPropertiesICollection)
                pocoWriter.WriteUserType("ICollection");
            else if (this.POCOConfiguration.IsNavigationPropertiesIEnumerable)
                pocoWriter.WriteUserType("IEnumerable");
            pocoWriter.Write("<");
            pocoWriter.WriteUserType(navigationProperty.ClassName);
            pocoWriter.Write("> ");
            pocoWriter.Write(navigationProperty.ToString());
            WriteNavigationPropertyEnd();
            pocoWriter.WriteLine();
        }

        protected virtual void WriteNavigationPropertyStart(string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.WriteKeyword("public");
            pocoWriter.Write(" ");

            if (this.POCOConfiguration.IsProperties && this.POCOConfiguration.IsNavigationPropertiesVirtual)
            {
                pocoWriter.WriteKeyword("virtual");
                pocoWriter.Write(" ");
            }
            else if (this.POCOConfiguration.IsProperties && this.POCOConfiguration.IsNavigationPropertiesOverride)
            {
                pocoWriter.WriteKeyword("override");
                pocoWriter.Write(" ");
            }
        }

        protected virtual void WriteNavigationPropertyEnd()
        {
            if (this.POCOConfiguration.IsProperties)
            {
                pocoWriter.Write(" { ");
                pocoWriter.WriteKeyword("get");
                pocoWriter.Write("; ");
                pocoWriter.WriteKeyword("set");
                pocoWriter.Write("; }");
            }
            else
            {
                pocoWriter.Write(";");
            }
        }

        #endregion

        #endregion

        #region Class End

        protected virtual void WriteClassEnd(IDbObjectTraverse dbObject, string namespaceOffset)
        {

            if (this.POCOConfiguration.IsEF && dbObject.DbType == POCODbType.Table && this.POCOConfiguration.IsEFComplexType)
            {
                if (complexTypeColumns != null && complexTypeColumns.Count > 0)
                    WriteComplexTypes(dbObject, namespaceOffset);
            }

            pocoWriter.Write(namespaceOffset);
            pocoWriter.WriteLine("}");
        }

        #endregion

        #region Namespace End

        protected virtual void WriteNamespaceEnd()
        {
            if (string.IsNullOrEmpty(Namespace) == false)
                pocoWriter.WriteLine("}");
        }

        #endregion






        #region Class Attributes

        protected void WriteClassAttributes(IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsEF && dbObject.DbType == POCODbType.Table)
            {
                WriteEFTable(dbObject, namespaceOffset);

                if (this.POCOConfiguration.IsEFDescription)
                {
                    Table table = (Table)dbObject;
                    if (table.HasExtendedProperties)
                    {
                        foreach (ExtendedProperty extendedProperty in table.ExtendedProperties)
                            WriteEFDescription(extendedProperty.Description, false, namespaceOffset);
                    }
                }
            }
        }

        protected virtual void WriteEFTable(IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("Table");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            if (dbObject.Schema != "dbo")
            {
                pocoWriter.WriteString(dbObject.Schema);
                pocoWriter.WriteString(".");
            }
            pocoWriter.WriteString(dbObject.Name);
            pocoWriter.WriteString("\"");
            pocoWriter.WriteLine(")]");
        }

        #endregion

        #region Column Attributes

        protected void WriteColumnAttributes(IColumn column, string cleanColumnName, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsEF && dbObject.DbType == POCODbType.Table)
            {
                // Primary Key
                bool isCompositePrimaryKey = IsCompositePrimaryKey(dbObject);
                if (column.IsPrimaryKey)
                {
                    if (!isCompositePrimaryKey)
                        WriteEFCompositePrimaryKey(column.ColumnName, column.DataTypeName, ((TableColumn)column).PrimaryKey.Ordinal, namespaceOffset);
                    else
                        WriteEFPrimaryKey(namespaceOffset);
                }
                else
                {
                    // Column
                    //if ((IsEFColumn && (column.IsPrimaryKey == false || isCompositePrimaryKey == false)) || (column.ColumnName != cleanColumnName))
                    if (column is IComplexType == false)
                        WriteEFColumn(column.ColumnName, column.DataTypeName, namespaceOffset);
                }

                // Index
                if (this.POCOConfiguration.IsEFIndex && column.HasIndexColumns)
                {
                    TableColumn tableColumn = (TableColumn)column;
                    foreach (IndexColumn indexColumn in tableColumn.IndexColumns.OrderBy(ic => ic.Name))
                    {
                        bool isCompositeIndex = tableColumn.Table.TableColumns.Exists(tc => tc != tableColumn && tc.HasIndexColumns && tc.IndexColumns.Exists(ic => ic.Name == indexColumn.Name));
                        if (isCompositeIndex)
                            WriteEFCompositeIndex(indexColumn.Name, indexColumn.Is_Unique, indexColumn.Is_Clustered, indexColumn.Is_Descending, indexColumn.Ordinal, namespaceOffset);
                        else
                            WriteEFIndex(indexColumn.Name, indexColumn.Is_Unique, indexColumn.Is_Clustered, indexColumn.Is_Descending, namespaceOffset);
                    }
                }


                // MaxLength
                if (column.DataTypeName == "binary" || column.DataTypeName == "char" || column.DataTypeName == "nchar" || column.DataTypeName == "nvarchar" || column.DataTypeName == "varbinary" || column.DataTypeName == "varchar")
                    WriteEFMaxLength(column.StringPrecision, namespaceOffset);

                // StringLength
                if (this.POCOConfiguration.IsEFStringLength)
                {
                    if (column.DataTypeName == "binary" || column.DataTypeName == "char" || column.DataTypeName == "nchar" || column.DataTypeName == "nvarchar" || column.DataTypeName == "varbinary" || column.DataTypeName == "varchar")
                    {
                        if (column.StringPrecision > 0)
                            WriteEFStringLength(column.StringPrecision.Value, namespaceOffset);
                    }
                }

                // Timestamp
                if (column.DataTypeName == "timestamp")
                    WriteEFTimestamp(namespaceOffset);

                // ConcurrencyCheck
                if (this.POCOConfiguration.IsEFConcurrencyCheck)
                {
                    if (column.DataTypeName == "timestamp" || column.DataTypeName == "rowversion")
                        WriteEFConcurrencyCheck(namespaceOffset);
                }

                // DatabaseGenerated Identity
                if (column.IsIdentity == true)
                    WriteEFDatabaseGeneratedIdentity(namespaceOffset);

                // DatabaseGenerated Computed
                if (column.IsComputed)
                    WriteEFDatabaseGeneratedComputed(namespaceOffset);

                string display = null;
                if (this.POCOConfiguration.IsEFRequiredWithErrorMessage || this.POCOConfiguration.IsEFDisplay)
                    display = GetEFDisplay(column.ColumnName);

                // Required
                if (this.POCOConfiguration.IsEFRequired || this.POCOConfiguration.IsEFRequiredWithErrorMessage)
                {
                    if (column.IsNullable == false)
                        WriteEFRequired(display, namespaceOffset);
                }

                // Display
                if (this.POCOConfiguration.IsEFDisplay)
                    WriteEFDisplay(display, namespaceOffset);

                // Description
                if (this.POCOConfiguration.IsEFDescription)
                {
                    TableColumn tableColumn = (TableColumn)column;

                    if (tableColumn.HasExtendedProperties)
                    {
                        foreach (ExtendedProperty extendedProperty in tableColumn.ExtendedProperties)
                            WriteEFDescription(extendedProperty.Description, true, namespaceOffset);
                    }

                    /*if (IsEFIndex && tableColumn.HasIndexColumns)
                    {
                        foreach (IndexColumn indexColumn in tableColumn.IndexColumns.OrderBy(ic => ic.Name))
                        {
                            if (indexColumn.HasExtendedProperties)
                            {
                                foreach (ExtendedProperty extendedProperty in indexColumn.ExtendedProperties)
                                    WriteEFDescription(extendedProperty.Description, true, namespaceOffset);
                            }
                        }
                    }*/
                }
            }
        }

        #endregion

        #region Column

        private List<string> complexTypeNames;
        private List<ComplexTypeColumn> complexTypeColumns;

        protected void WriteColumn(IColumn column, bool isLastColumn, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsEF && dbObject.DbType == POCODbType.Table && this.POCOConfiguration.IsEFComplexType)
            {
                string columnName = column.ColumnName.Trim();
                int index = columnName.IndexOf('_');
                if (index != -1 && index != 0 && index != columnName.Length - 1)
                {
                    string complexTypeName = NameHelper.CleanName(columnName.Substring(0, index));
                    string complexTypeColumnName = columnName.Substring(index + 1);

                    if (complexTypeNames == null)
                        complexTypeNames = new List<string>();
                    if (complexTypeNames.Contains(complexTypeName) == false)
                    {
                        ComplexType complexType = new ComplexType(complexTypeName, (TableColumn)column);
                        WriteColumnBase(complexType, isLastColumn, dbObject, namespaceOffset);
                        complexTypeNames.Add(complexTypeName);
                    }

                    ComplexTypeColumn complexTypeColumn = new ComplexTypeColumn(complexTypeName, complexTypeColumnName, (TableColumn)column);
                    if (complexTypeColumns == null)
                        complexTypeColumns = new List<ComplexTypeColumn>();
                    complexTypeColumns.Add(complexTypeColumn);
                }
                else
                {
                    WriteColumnBase(column, isLastColumn, dbObject, namespaceOffset);
                }
            }
            else
            {
                WriteColumnBase(column, isLastColumn, dbObject, namespaceOffset);
            }
        }



        #endregion

        #region Navigation Properties



        protected virtual void GetNavigationPropertiesMultipleRelationships(List<NavigationProperty> navigationProperties)
        {
            if (navigationProperties != null && navigationProperties.Count > 0)
            {
                var multipleRels = navigationProperties
                    .GroupBy(np => new
                    {
                        np.ForeignKey.Foreign_Schema_Id,
                        np.ForeignKey.Foreign_Table_Id,
                        np.ForeignKey.Primary_Schema_Id,
                        np.ForeignKey.Primary_Table_Id
                    })
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g);

                foreach (var np in multipleRels)
                    np.HasMultipleRelationships = true;
            }
        }

        protected void WriteNavigationPropertyAttributes(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            if (this.POCOConfiguration.IsEF && this.POCOConfiguration.IsEFForeignKey)
            {
                if (IsNavigableObject(dbObject))
                {
                    if (navigationProperty.IsRefFrom)
                        WriteNavigationPropertyForeignKeyAttribute(navigationProperty, dbObject, namespaceOffset);

                    if (navigationProperty.IsRefFrom == false && navigationProperty.HasMultipleRelationships)
                        WriteNavigationPropertyInversePropertyAttribute(navigationProperty, dbObject, namespaceOffset);
                }
            }
        }

        protected virtual void WriteNavigationPropertyForeignKeyAttribute(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("ForeignKey");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            if (navigationProperty.HasMultipleRelationships)
                pocoWriter.WriteString(navigationProperty.ForeignKey.Foreign_Column);
            else
                pocoWriter.WriteString(navigationProperty.ForeignKey.Primary_Column);
            pocoWriter.WriteString("\"");
            pocoWriter.WriteLine(")]");
        }

        protected virtual void WriteNavigationPropertyInversePropertyAttribute(NavigationProperty navigationProperty, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("InverseProperty");
            pocoWriter.Write("(");
            pocoWriter.WriteString("\"");
            pocoWriter.WriteString(navigationProperty.InverseProperty.ToString());
            pocoWriter.WriteString("\"");
            pocoWriter.WriteLine(")]");
        }

        #endregion

        #region Class End



        protected virtual void WriteComplexTypes(IDbObjectTraverse dbObject, string namespaceOffset)
        {
            var complexTypes = complexTypeColumns.GroupBy(x => x.ComplexTypeName);
            foreach (var complexType in complexTypes)
                WriteComplexType(complexType.Key, complexType, dbObject, namespaceOffset);
        }

        protected virtual void WriteComplexType(string complexTypeName, IEnumerable<ComplexTypeColumn> complexTypeColumns, IDbObjectTraverse dbObject, string namespaceOffset)
        {
            pocoWriter.WriteLine();

            // Class Attribute
            pocoWriter.Write(namespaceOffset);
            pocoWriter.Write(Tab);
            pocoWriter.Write("[");
            pocoWriter.WriteUserType("ComplexType");
            pocoWriter.WriteLine("]");

            namespaceOffset += Tab;

            // Class Start
            WriteClassStart(complexTypeName, dbObject, namespaceOffset);

            // Columns
            var columns = complexTypeColumns.OrderBy<IColumn, int>(c => c.ColumnOrdinal ?? 0);
            var lastColumn = columns.Last();
            foreach (IColumn column in columns)
                WriteColumn(column, column == lastColumn, dbObject, namespaceOffset);

            // Class End
            WriteClassEnd(dbObject, namespaceOffset);
        }

        #endregion
    }

}
