using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;

namespace PickTextValueTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<PickTextValueOption>(args).WithParsed(option =>
            {
                option.InputFilePath = Path.GetFullPath(option.InputFilePath);
                option.OutputFilePath = Path.GetFullPath(option.OutputFilePath);

                Console.WriteLine($"InputFile={option.InputFilePath}");
                Console.WriteLine($"InputRegex={option.InputRegex}");
                Console.WriteLine($"OutputFile={option.OutputFilePath}");
                Console.WriteLine($"ReplaceRegex={option.ReplaceRegex}");

                if (!File.Exists(option.InputFilePath))
                {
                    throw new ArgumentException($"InputFile={option.InputFilePath} not found");
                }

                if (!File.Exists(option.OutputFilePath))
                {
                    throw new ArgumentException($"OutputFile={option.OutputFilePath} not found");
                }

                var inputText = File.ReadAllText(option.InputFilePath);
                var outputText = File.ReadAllText(option.OutputFilePath);

                var match = Regex.Match(inputText, option.InputRegex);
                if (!match.Success)
                {
                    throw new ArgumentException($"Can not match regex={option.InputRegex} in InputFile={option.InputFilePath} \r\n The file content is\r\n {inputText}");
                }

                var inputValue = match.Groups[1].Value;
                Console.WriteLine($"The match input value is {inputValue}");

                var matchOutputText = Regex.Match(outputText, option.ReplaceRegex);

                if (!matchOutputText.Success)
                {
                    throw new ArgumentException($"Can not match regex={option.ReplaceRegex} in OutputFile={option.OutputFilePath} \r\n The file content is\r\n {outputText}");
                }

                var replaceText = matchOutputText.Groups[0].Value.Replace(matchOutputText.Groups[1].Value, inputValue);

                Console.WriteLine($"Replace {matchOutputText.Groups[0].Value} to {replaceText}");

                outputText = outputText.Replace(matchOutputText.Groups[0].Value, replaceText);

                File.WriteAllText(option.OutputFilePath, outputText);
            });
        }
    }

    class PickTextValueOption
    {
        [Option('f', "InputFile", Required = true)]
        public string InputFilePath { set; get; }

        [Option('r', "InputRegex", Required = true)]
        public string InputRegex { set; get; }

        [Option('o', "OutputFile", Required = true)]
        public string OutputFilePath { set; get; }

        [Option('s', "ReplaceRegex", Required = true)]
        public string ReplaceRegex { set; get; }
    }
}
