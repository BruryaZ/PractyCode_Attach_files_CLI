using System.CommandLine;

#region bundle command
//OPTIONS
var bundleOutput = new Option<FileInfo>("--output", "Path file");
var bundleLanguages = new Option<Languages>("--language", "language to Attach") { IsRequired = true, };
var bundleNote = new Option<bool>("--note", "list the source of the file");
var bundleSort = new Option<SortOptions>("--sort", "Sort by file name or type code");
var bundleRemoveEmptyLines = new Option<bool>("--RemoveEmptyLines", "Remove empty lines");
var bundleAuthor = new Option<string>("--author", "Include author's name at the top of the file");

var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(bundleOutput);
bundleCommand.AddOption(bundleLanguages);
bundleCommand.AddOption(bundleNote);
bundleCommand.AddOption(bundleRemoveEmptyLines);
bundleCommand.AddOption(bundleSort);
bundleCommand.AddOption(bundleAuthor);

bundleCommand.SetHandler((output, language, note, sort, RemoveEmptyLines, author) =>
{
    try
    {
        FileStream file = File.Create(output.FullName);
        Console.WriteLine("file was create");
        file.Close();
    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine("ERROR: bundle files faild, path file invalid!");
    }
}, bundleOutput, bundleLanguages, bundleNote, bundleSort, bundleRemoveEmptyLines, bundleAuthor);
#endregion

#region create-rsp command
var createRSP = new Command("create-rsp", "Separate responses in stages");

string fullCommandFileName = "bundle.rsp";
var fullCommand = new List<string>();

createRSP.SetHandler(() =>
{
    //bundleOutput
    Console.Write("Enter Path file ");
    string path = Console.ReadLine();
    fullCommand.Add("--output " + path);
    //bundleAuthor
    Console.Write("Include author's name at the top of the file? (y/n) ");
    string author = Console.ReadLine(), authorName = "";
    if (author == "y")
    {
        Console.Write("Author name: ");
        authorName = Console.ReadLine();
    }
    fullCommand.Add("--author " + author);
    //bundleRemoveEmptyLines
    Console.Write("Remove empty lines? (y/n) ");
    string isRemoveTmp = Console.ReadLine();
    bool isRemove = isRemoveTmp == "y" ? true : false;
    fullCommand.Add("--RemoveEmptyLines " + isRemove);
    //bundleSort
    Console.Write("Sort by file name or type code? (code/name) ");
    string sortTmp = Console.ReadLine();
    SortOptions sort = sortTmp == "code" ? SortOptions.CODE : SortOptions.NAME;
    fullCommand.Add("--sort " + sort);
    //bundleLanguages
    Console.Write("Which languages to attach? [c_sharp, java, python, javascript, c_plus_plus, ruby," +
        " php, swift, go, kotlin, r, type_script, perl, scala, haskell] (Separate by commas): ");
    string languagesInput = Console.ReadLine();
    List<Languages> selectedLanguages = new List<Languages>();
    fullCommand.Add("--language ");
    string languageLST = "";
    foreach (var lang in languagesInput.Split(','))
    {
        if (Enum.TryParse(lang.Trim().ToUpper(), out Languages language))
        {
            selectedLanguages.Add(language);
            languageLST += language.ToString() + ", ";
        }
    }
    fullCommand.Add(languageLST);
    //bundleNote
    Console.Write("list the source of the file? (y/n) ");
    string isNoteTmp = Console.ReadLine();
    bool isNote = isNoteTmp == "y" ? true : false;
    fullCommand.Add("--note " + isNote);

    File.AppendAllLines(fullCommandFileName, fullCommand);
    Console.WriteLine($"Response file '{fullCommandFileName}' created with the following command:");
    Console.WriteLine(string.Join(" ", fullCommand));
});
#endregion

var rootCommand = new RootCommand("Root command for File bundle CLI");

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRSP);

rootCommand.InvokeAsync(args);
enum Languages { C_SHARP, JAVA, PYTHON, JAVASCRIPT, C_PLUS_PLUS, RUBY, PHP, SWIFT, GO, KOTLIN, R, TYPE_SCRIPT, PERL, SCALA, HASKELL }
enum SortOptions { CODE, NAME }