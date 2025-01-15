using System.CommandLine;
//OPTIONS
var bundleOutput = new Option<FileInfo>("--output", "Path file");
var bundleLanguage = new Option<Languages>("--language", "language to Attach")
{
    IsRequired = true,
};
var bundleNote = new Option<bool>("--note", "list the source of the file");
var bundleSort = new Option<SortOptions>("--sort", "Sort by file name or type code");
var bundleRemoveEmptyLines = new Option<bool>("--RemoveEmptyLines", "Remove empty lines");
var bundleAuthor = new Option<string>("--author", "Include author's name at the top of the file");

var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(bundleOutput);
bundleCommand.AddOption(bundleLanguage);

bundleCommand.SetHandler((output, language, note, sort, RemoveEmptyLines, author) =>
{
    try
    {
        File.Create(output.FullName);
        Console.WriteLine("file was create");
    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine("ERROR: bundle files faild, path file invalid!");
    }
}, bundleOutput, bundleLanguage, bundleNote, bundleSort, bundleRemoveEmptyLines, bundleAuthor);

var rootCommand = new RootCommand("Root command for File bundle CLI");

rootCommand.AddCommand(bundleCommand);

rootCommand.InvokeAsync(args);
enum Languages { C_SHARP, JAVA, PYTHON, JAVASCRIPT, C_PLUS_PLUS, RUBY, PHP, SWIFT, GO, KOTLIN, R, TYPE_SCRIPT, PERL, SCALA, HASKELL }
enum SortOptions { CODE, NAME }
//create-rsp