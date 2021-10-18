using System;
using System.Windows.Forms;
using CommandLine;
using POCO_Studio;
using POCOGenerator.CommandLine;

namespace POCOGenerator
{
    public static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            //SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            CommandLineParsingResult<Options> parsingResult = CommandLineParser<Options>.Parse(args);
            CommandLineResult resultCode = CommandLineParser.ValidateOptions(parsingResult);

            if (resultCode > CommandLineResult.NoErrors)
                return (int)resultCode;

            if (resultCode == CommandLineResult.EmptyArgs)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(MainForm);
            }
            else if (resultCode == CommandLineResult.NoErrors)
            {
                ComandLineWriter writer = new ComandLineWriter(parsingResult.Options);
                CommandLineResult result = writer.Export();
                return (int)result;
            }

            return 0;
        }


        public static MainForm MainForm = new MainForm();

        public static POCOGeneratorForm PocoForm = new POCOGeneratorForm();

        public static PropertyForm PropertyForm = new PropertyForm();
    }
}
