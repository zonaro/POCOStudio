using CommandLine;
using InnerLibs;
using System;
using System.IO;
using System.Linq;

namespace POCOGenerator.CommandLine
{
    public static class CommandLineParser
    {
        public static CommandLineResult ValidateOptions(CommandLineParsingResult<Options> parsingResult)
        {
            // empty args
            if (parsingResult.EmptyArgs)
                return CommandLineResult.EmptyArgs;

            // parsing error
            if (parsingResult.HasErrors)
                return CommandLineResult.CommandLineParsingError;

            // mutually exclusive set
            if (parsingResult.HasMutuallyExclusiveSetErrors)
            {
                if (parsingResult.InvalidMutuallyExclusiveSetsByDefaultValues != null && parsingResult.InvalidMutuallyExclusiveSetsByDefaultValues.Count() > 0)
                {
                    if (parsingResult.InvalidMutuallyExclusiveSetsByDefaultValues.Contains("Set1"))
                        parsingResult.Options.IsProperties = false;

                    if (parsingResult.InvalidMutuallyExclusiveSetsByDefaultValues.Contains("Set2"))
                        parsingResult.Options.IsNavigationPropertiesList = false;

                    if (parsingResult.InvalidMutuallyExclusiveSets.Except(parsingResult.InvalidMutuallyExclusiveSetsByDefaultValues).Count() > 0)
                        return CommandLineResult.CommandLineMutuallyExclusiveSetError;
                }
                else
                {
                    return CommandLineResult.CommandLineMutuallyExclusiveSetError;
                }
            }

            // missing connection string
            if (string.IsNullOrEmpty(parsingResult.Options.ConnectionString))
                return CommandLineResult.MissingConnectionString;

            // connection string validity
            try
            {
                new ConnectionStringParser(parsingResult.Options.ConnectionString);
            }
            catch
            {
                return CommandLineResult.MalformedConnectionString;
            }

            // missing export folder
            if (string.IsNullOrEmpty(parsingResult.Options.Folder))
                return CommandLineResult.MissingExportFolder;

            // export folder doesn't exist
            if (Directory.Exists(parsingResult.Options.Folder) == false)
                return CommandLineResult.ExportFolderNotExist;

            // no errors
            return CommandLineResult.NoErrors;
        }
    }
}