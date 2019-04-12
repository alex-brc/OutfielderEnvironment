using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

public class ScriptCompiler
{
    private const string COMPILE_ERROR_FILE = @"compile_errors.txt";
    private const string SCRIPT_FILE = @"CustomBallScript.cs";

    public static bool CompileBallScript(ref CustomCompilerResults output)
    {
        try
        {
            string code = "";
            try
            {
                // Fill in the code
                code = ReadCode();
            }
            catch (Exception)
            {
                return false;
            }

            // Try to compile
            CSharpCompiler.CodeCompiler provider = new CSharpCompiler.CodeCompiler();
            CompilerParameters parameters = new CompilerParameters();

            // Assembly references
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                parameters.ReferencedAssemblies.Add(assembly.Location);
            }

            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            // Compile the source
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                // Compiled with errors or warnings, write to a file
                WriteErrors(results.Errors);
                return false;
            }

            Type customBallScript = results.CompiledAssembly.GetType("BallScript");

            if (customBallScript == null)
            {
                Log("Couldn't get class type. ");
                return false;
            }

            MethodInfo beforeCountdown = customBallScript.GetMethod("BeforeCountdown");
            MethodInfo beforeLaunch = customBallScript.GetMethod("BeforeLaunch");
            MethodInfo after = customBallScript.GetMethod("After");
            MethodInfo whileRunning = customBallScript.GetMethod("WhileRunning");

            if (beforeCountdown == null)
            {
                Log("BeforeCountdown() method not found.");
                return false;
            }
            if (beforeLaunch == null)
            {
                Log("BeforeLaunch() method not found.");
                return false;
            }
            if (after == null)
            {
                Log("After() method not found.");
                return false;
            }
            if (whileRunning == null)
            {
                Log("WhileRunning() method not found.");
                return false;
            }

            output = new CustomCompilerResults(customBallScript, beforeCountdown, beforeLaunch, after, whileRunning);

            return true;
        }
        catch (Exception e)
        {
            Log(e.ToString());
            return false;
        }
    }
    
    private static string ReadCode()
    {
        return File.ReadAllText(SCRIPT_FILE);
    }

    private static void WriteErrors(CompilerErrorCollection errors)
    {
        using (StreamWriter sr = new StreamWriter(COMPILE_ERROR_FILE))
            foreach (CompilerError err in errors) 
                sr.WriteLine("[T:" + DateTime.Now.ToShortTimeString() + "]" + err.ToString());
    }

    private static void Log(string log)
    {
        using (StreamWriter sr = new StreamWriter(COMPILE_ERROR_FILE))
            sr.WriteLine(log);
    }

    public class CustomCompilerResults
    {
        public Type type;
        public MethodInfo BeforeCountdown;
        public MethodInfo BeforeLaunch;
        public MethodInfo After;
        public MethodInfo WhileRunning;

        public CustomCompilerResults(Type type, MethodInfo BeforeCountdown, MethodInfo BeforeLaunch, MethodInfo After, MethodInfo WhileRunning)
        {
            this.type = type;
            this.BeforeCountdown = BeforeCountdown;
            this.BeforeLaunch = BeforeLaunch;
            this.After = After;
            this.WhileRunning = WhileRunning;
        }
    }
}
