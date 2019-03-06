using System;
using System.IO;
//Namespaces pertaining to RSMF validation
using Relativity.RSMFU.Validator;
using Relativity.RSMFU.Validator.Interfaces;

// ----------------------------------------------------------------------------
// <copyright file="SampleProgram.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace RSMFValidationSampleCode
{
    class SampleProgram
    {
        /// <summary>
        /// The main routine.
        /// </summary>
        /// <param name="args">The command line argument(s)</param>
        /// <remarks>Uses the Relativity RSMF Validation library to validate a directory of RSMF files (.rsmf, .zip and .json)</remarks>
        /// <returns>0 if successful</returns>
        public static int Main(string[] args)
        {

            // Test if input RSMF directory was supplied
            if (args.Length == 0)
            {
                System.Console.WriteLine("Usage: RSMFValidationSampleCode.exe <directory>");
                return 1;
            }

            string rsmfDirectory = args[0];

            // Test if directory exists and compile a list of RSMF files for validation
            if (Directory.Exists(rsmfDirectory))
            {
                string[] fileList = Directory.GetFiles(rsmfDirectory);

                if (fileList.Length > 0)
                {
                    foreach (string file in fileList)
                    {
                        ValidateMyFile(file);
                    }
                }
                else
                {
                    Console.Error.WriteLine("Directory is empty!");
                    return 1;
                }
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", rsmfDirectory);
            }

            return 0;
        }

        private static void ValidateMyFile(string filepath)
        {
            //RSMF Validation library validates 3 types of files: .rsmf, rsmf_manifest.json and rsmf.zip against the RSMF specification
            //Select the validation method based on the file type
            string fileExt = Path.GetExtension(filepath);

            try
            {

                IValidator myValidator; //An IValidator object is used to validate an RSMF component. IValidators are created by the RSMFValidatorFactory
                //ResultTypes enumerate the possible values for the Result property
                //ResultTypes are Passed, Failed or PassedWithWarnings
                RSMFValidatorResult result; //The overall results of running a validation
                
                //Use the PerformTests method (Stream or String) to check if the file passes validation or create a log if it passed with warnings or failed
                //PerformTests returns an RSMFValidatorResult object
                //Use the Dispose method release unmanaged resources used by your application
                switch (fileExt)
                {
                    case ".rsmf":
                        myValidator = RSMFValidatorFactory.GetRSMFValidator(); //Create a validator that validates an RSMF file
                        result = myValidator.PerformTests(filepath);
                        Console.WriteLine($"Result for {filepath}: {result.Result}");
                        if (result.Result == ResultTypes.Failed || result.Result == ResultTypes.PassedWithWarnings)
                            CreateLog(filepath, result);
                        myValidator.Dispose();
                        break;

                    case ".json":
                        myValidator = RSMFValidatorFactory.GetRSMFJsonValidator(); //Create a validator that validates the contents of an rsmf_manifest.json file
                        result = myValidator.PerformTests(filepath);
                        Console.WriteLine($"Result for {filepath}: {result.Result}");

                        if (result.Result == ResultTypes.Failed || result.Result == ResultTypes.PassedWithWarnings)
                            CreateLog(filepath, result);
                        myValidator.Dispose();
                        break;

                    case ".zip":
                        myValidator = RSMFValidatorFactory.GetRSMFZipValidator(); //Create a validator that validates an RSMF ZIP file
                        result = myValidator.PerformTests(filepath);
                        Console.WriteLine($"Result for {filepath}: {result.Result}");
                        if (result.Result == ResultTypes.Failed || result.Result == ResultTypes.PassedWithWarnings)
                            CreateLog(filepath, result);
                        myValidator.Dispose();
                        break;

                    default:
                        Console.WriteLine($"Not a valid extension for RSMF Validation: {filepath}");
                        break;

                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unable to complete validation test:{Environment.NewLine}{ex.Message}");
            }

        }

        private static void CreateLog(string file, RSMFValidatorResult result)
        {


            //Create a log to capture the warnings and errors 
            //Errors and warnings should be addressed before attempting to ingest an RSMF file into Relativity
            try
            {
                using (StreamWriter w = File.AppendText("rsmf.validation.log.txt"))
                {
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine(file);

                    //Use the IValidatorError object to check the ErrorMessage property for specific information about the error
                    //Use this information to correct the problem in the RSMF file and repeat the verification process
                    //Capture the errors
                    foreach (IValidatorError err in result.Errors)
                    {
                        w.WriteLine($"*{err.Error.ToString()}*");
                        if (err.LineNumber != null)
                        {
                            w.WriteLine($"Line number: {err.LineNumber}");
                            w.WriteLine($"Line position: {err.LinePosition}");
                        }
                        w.WriteLine($"*{err.ErrorMessage}*");
                    }
                   
                    //Capture the warnings
                    foreach (IValidatorError war in result.Warnings)
                    {
                        w.WriteLine($"*{war.Error.ToString()}*");
                        if (war.LineNumber != null)
                        {
                            w.WriteLine($"Line number: {war.LineNumber}");
                            w.WriteLine($"Line position: {war.LinePosition}");
                        }
                        w.WriteLine($"*{war.ErrorMessage}*");
                    }

                    w.WriteLine($"--------{Environment.NewLine}");

                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unable to create log:{Environment.NewLine}{ex.Message}");
            }
        }
    }
}
