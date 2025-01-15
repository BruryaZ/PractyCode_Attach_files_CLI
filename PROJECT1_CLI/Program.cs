using System.CommandLine;

//Validation function
static FileInfo OKPath()
{
    Console.Write("Enter Path file: ");
    string pathInput = OKString();

    FileInfo path = new FileInfo(pathInput);

    if (path.Exists)
    {
        Console.WriteLine($"File exists: {path.FullName}");
    }

    return path;
}
static string OKYesNo()
{
    string answer = Console.ReadLine();
    while (answer != "y" && answer != "n")
    {
        Console.WriteLine("WRONG ANSWER: press (y/n)");
        answer = Console.ReadLine();
    }
    return answer;
}
static string OKString()
{
    string path = Console.ReadLine();
    Console.Write("Enter Path file ");
    while (path == "")
    {
        Console.WriteLine("WRONG ANSWER: preee correct path");
        path = Console.ReadLine();
    }
    return path;
}
static SortOptions OKCodeName()
{
    string sortTmp = Console.ReadLine();
    while (sortTmp != "code" && sortTmp != "name")
    {
        Console.WriteLine("\"WRONG ANSWER: preee code or name");
        sortTmp = Console.ReadLine();
    }
    return sortTmp == "code" ? SortOptions.CODE : SortOptions.NAME;
}
static List<Languages> OKLanguage()
{
    string languagesInput = Console.ReadLine();
    List<Languages> selectedLanguages = new List<Languages>();
    List<string> validLanguages = new List<string>(Enum.GetNames(typeof(Languages)));

    foreach (var lang in languagesInput.Split(','))
    {
        string trimmedLang = lang.Trim().ToUpper();
        if (validLanguages.Contains(trimmedLang))
        {
            Enum.TryParse(trimmedLang, out Languages language);
            selectedLanguages.Add(language);
        }
        else
            Console.WriteLine($"Invalid language: {trimmedLang}");
    }
    return selectedLanguages;
}

//Attach to a single file
static void Attach(FileStream file, List<Languages> language, bool note, SortOptions sort, bool removeEmptyLines, string author)
{

}

#region bundle command
//OPTIONS
var bundleOutput = new Option<FileInfo>("--output", "Path file");
bundleOutput.AddAlias("-o");
var bundleLanguages = new Option<List<Languages>>("--language", "language to Attach") { IsRequired = true, };
bundleOutput.AddAlias("-l");
var bundleNote = new Option<bool>("--note", "list the source of the file");
bundleOutput.AddAlias("-n");
var bundleSort = new Option<SortOptions>("--sort", "Sort by file name or type code");
bundleOutput.AddAlias("-s");
var bundleRemoveEmptyLines = new Option<bool>("--removeEmptyLines", "Remove empty lines");
bundleOutput.AddAlias("-r");
var bundleAuthor = new Option<string>("--answer", "Include answer's name at the top of the file");
bundleOutput.AddAlias("-a");

var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(bundleOutput);
bundleCommand.AddOption(bundleLanguages);
bundleCommand.AddOption(bundleNote);
bundleCommand.AddOption(bundleRemoveEmptyLines);
bundleCommand.AddOption(bundleSort);
bundleCommand.AddOption(bundleAuthor);

//Action
bundleCommand.SetHandler((output, language, note, sort, removeEmptyLines, author) =>
{
    //Display the user choices
    Console.WriteLine($"Output Path: {output.FullName}");
    Console.WriteLine($"Selected Language: {language}");
    Console.WriteLine($"List Source: {note}");
    Console.WriteLine($"Sort Option: {sort}");
    Console.WriteLine($"Remove Empty Lines: {removeEmptyLines}");
    Console.WriteLine($"Author Name: {author}");
    try
    {
        FileStream file = File.Create(output.FullName);
        Console.WriteLine("file was create");
        Attach(file, language, note, sort, removeEmptyLines, author);
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

//Action
createRSP.SetHandler(() =>
{
    //Get options

    //bundleOutput
    Console.Write("Enter Path file ");
    FileInfo path = OKPath();
    fullCommand.Add("--output " + path);
    //bundleAuthor
    Console.Write("Include answer's name at the top of the file? (y/n) ");
    string author = OKYesNo(), authorName = "";
    if (author == "y")
    {
        Console.Write("Press answer name: ");
        authorName = OKString();
    }
    fullCommand.Add("--answer " + author);
    //bundleRemoveEmptyLines
    Console.Write("Remove empty lines? (y/n) ");
    string isRemoveTmp = OKYesNo();
    bool isRemove = isRemoveTmp == "y" ? true : false;
    fullCommand.Add("--removeEmptyLines " + isRemove);
    //bundleSort
    Console.Write("Sort by file name or type code? (code/name) ");
    SortOptions sort = OKCodeName();
    fullCommand.Add("--sort " + sort);
    //bundleLanguages
    Console.Write("Which languages to attach? (for all press all): [c_sharp, java, python, javascript, c_plus_plus, ruby," +
            " php, swift, go, kotlin, r, type_script, perl, scala, haskell] (Separate by commas): ");
    List<Languages> languages = OKLanguage();
    fullCommand.Add("--language" + languages);
    //bundleNote
    Console.Write("list the source of the file? (y/n) ");
    string isNoteTmp = OKYesNo();
    bool isNote = isNoteTmp == "y" ? true : false;
    fullCommand.Add("--note " + isNote);

    File.AppendAllLines(fullCommandFileName, fullCommand);
    Console.WriteLine($"Response file '{fullCommandFileName}' created with the following command:");
    Console.WriteLine(string.Join(" ", fullCommand));
});
#endregion

var rootCommand = new RootCommand("Root command for File bundle CLI");

//Add command
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRSP);

rootCommand.InvokeAsync(args);

//Models
enum Languages { C_SHARP, JAVA, PYTHON, JAVASCRIPT, C_PLUS_PLUS, RUBY, PHP, SWIFT, GO, KOTLIN, R, TYPE_SCRIPT, PERL, SCALA, HASKELL, ALL }
enum SortOptions { CODE, NAME }

//Attach the bundle file