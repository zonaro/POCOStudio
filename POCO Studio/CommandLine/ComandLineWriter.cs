using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Db;
using Db.DbObject;
using Db.Helpers;
using Db.POCOIterator;
using POCOGenerator.POCOWriter;

namespace POCOGenerator.CommandLine
{
    public class ComandLineWriter
    {
        private Options options;
        private Server Server;
        private string InitialCatalog;

        public ComandLineWriter(Options options)
        {
            this.options = options;
        }

        public CommandLineResult Export()
        {
            try
            {
                SetConnectionString(options.ConnectionString);
                DbHelper.BuildServerSchema(Server, InitialCatalog, null, null);
                List<IDbObjectTraverse> exportObjects = GetExportObjects();
                if (exportObjects.Count == 0)
                    return CommandLineResult.MissingIncludedObjects;
                CommandLineResult result = WritePocoToFiles(exportObjects);
                return result;
            }
            catch
            {
                return CommandLineResult.ExportError;
            }
        }

        private void SetConnectionString(string connectionString)
        {
            DbHelper.ConnectionString = connectionString;

            int index = connectionString.IndexOf("Data Source=");
            if (index != -1)
            {
                string server = connectionString.Substring(index + "Data Source=".Length);
                index = server.IndexOf(';');
                if (index != -1)
                    server = server.Substring(0, index);
                string instanceName = null;
                index = server.LastIndexOf("\\");
                if (index != -1)
                {
                    instanceName = server.Substring(index + 1);
                    server = server.Substring(0, index);
                }

                Server = new Server()
                {
                    ServerName = server,
                    InstanceName = instanceName
                };
            }

            index = connectionString.IndexOf("Initial Catalog=");
            if (index != -1)
            {
                string initialCatalog = connectionString.Substring(index + "Initial Catalog=".Length);
                index = initialCatalog.IndexOf(';');
                if (index != -1)
                    initialCatalog = initialCatalog.Substring(0, index);
                InitialCatalog = initialCatalog;
            }
        }

        private List<IDbObjectTraverse> GetExportObjects()
        {
            List<IDbObjectTraverse> exportObjects = new List<IDbObjectTraverse>();
            foreach (Database database in Server.Databases)
            {
                GetExportObjects(exportObjects, database.Tables.Cast<IDbObjectTraverse>(), options.IsIncludeAll, options.IsIncludeAllTables, options.IncludeTables, options.IsExcludeAllTables, options.ExcludeTables);
                GetExportObjects(exportObjects, database.Views.Cast<IDbObjectTraverse>(), options.IsIncludeAll, options.IsIncludeAllViews, options.IncludeViews, options.IsExcludeAllViews, options.ExcludeViews);
                GetExportObjects(exportObjects, database.Procedures.Cast<IDbObjectTraverse>(), options.IsIncludeAll, options.IsIncludeAllStoredProcedures, options.IncludeStoredProcedures, options.IsExcludeAllStoredProcedures, options.ExcludeStoredProcedures);
                GetExportObjects(exportObjects, database.Functions.Cast<IDbObjectTraverse>(), options.IsIncludeAll, options.IsIncludeAllFunctions, options.IncludeFunctions, options.IsExcludeAllFunctions, options.ExcludeFunctions);
                GetExportObjects(exportObjects, database.TVPs.Cast<IDbObjectTraverse>(), options.IsIncludeAll, options.IsIncludeAllTVPs, options.IncludeTVPs, options.IsExcludeAllTVPs, options.ExcludeTVPs);
            }
            return exportObjects;
        }

        private void GetExportObjects(List<IDbObjectTraverse> exportObjects, IEnumerable<IDbObjectTraverse> objects, bool isIncludeAll, bool isIncludeAllObjects, List<string> includeObjects, bool isExcludeAllObjects, List<string> excludeObjects)
        {
            if (objects != null && objects.Count() > 0)
            {
                if (isExcludeAllObjects == false)
                {
                    if (isIncludeAll || isIncludeAllObjects)
                    {
                        if (excludeObjects == null || excludeObjects.Count == 0)
                        {
                            exportObjects.AddRange(objects);
                        }
                        else
                        {
                            exportObjects.AddRange(
                                objects.Where(o => excludeObjects.Exists(eo => eo == o.ToString() || eo == o.Name) == false)
                            );
                        }
                    }
                    else if (includeObjects != null && includeObjects.Count > 0)
                    {
                        if (excludeObjects == null || excludeObjects.Count == 0)
                        {
                            exportObjects.AddRange(
                                objects.Where(o => includeObjects.Exists(io => io == o.ToString() || io == o.Name))
                            );
                        }
                        else
                        {
                            exportObjects.AddRange(
                                objects.Where(o => includeObjects.Exists(io => io == o.ToString() || io == o.Name) && excludeObjects.Exists(eo => eo == o.ToString() || eo == o.Name) == false)
                            );
                        }
                    }
                }
            }
        }

        private CommandLineResult WritePocoToFiles(List<IDbObjectTraverse> exportObjects)
        {
            string folder = options.Folder;
            bool isSingleFile = options.IsSingleFile;
            string fileName = options.FileName;

            if (isSingleFile && string.IsNullOrEmpty(fileName) == false)
            {
                StringBuilder sb = new StringBuilder();
                IterateDbObjects(exportObjects, sb);

                if (fileName.EndsWith(".cs") == false)
                    fileName += ".cs";
                foreach (char c in Path.GetInvalidFileNameChars())
                    fileName = fileName.Replace(c.ToString(), string.Empty);

                return WritePocoToFile(folder, fileName, sb.ToString(), true);
            }
            else
            {
                CommandLineResult exportResult = CommandLineResult.NoErrors;

                foreach (var dbObject in exportObjects)
                {
                    StringBuilder sb = new StringBuilder();
                    IterateDbObjects(dbObject, sb);

                    fileName = dbObject.ClassName + ".cs";
                    foreach (char c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c.ToString(), string.Empty);
                    CommandLineResult result = WritePocoToFile(folder, fileName, sb.ToString(), false);
                    if (result != CommandLineResult.NoErrors && exportResult == CommandLineResult.NoErrors)
                        exportResult = result;
                }

                return exportResult;
            }
        }

        private CommandLineResult WritePocoToFile(string folder, string fileName, string content, bool isAppend)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || fileName == ".cs")
                    return CommandLineResult.FileNameNotSet;

                string path = folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + fileName;
                if (isAppend)
                    File.AppendAllText(path, content);
                else
                    File.WriteAllText(path, content);

                return CommandLineResult.NoErrors;
            }
            catch
            {
                return CommandLineResult.FileWritingError;
            }
        }

        private void IterateDbObjects(IDbObjectTraverse dbObject, StringBuilder sb)
        {
            DbIterator iterator = GetPOCOIterator(new IDbObjectTraverse[] { dbObject }, sb);
            iterator.Iterate();
        }

        private void IterateDbObjects(IEnumerable<IDbObjectTraverse> dbObjects, StringBuilder sb)
        {
            DbIterator iterator = GetPOCOIterator(dbObjects, sb);
            iterator.Iterate();
        }

        private DbIterator GetPOCOIterator(IEnumerable<IDbObjectTraverse> dbObjects, StringBuilder sb)
        {
            IPOCOWriter writer = new StringBuilderWriterFactory(sb).CreatePOCOWriter();
            return Program.PropertyForm.Iterator;
          
        }
    }
}
