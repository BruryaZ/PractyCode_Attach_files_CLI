using System.CommandLine;

//Validation function
static FileInfo OKPath()
{
    Console.Write("Enter Path sourceFileCommants: ");
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
    Console.Write("Enter Path sourceFileCommants ");
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
    Console.WriteLine("Enter languages (comma-separated):");
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
        {
            Console.WriteLine($"Invalid language: {trimmedLang}");
        }
    }
    return selectedLanguages;
}

//Get language commants
static string GetLanguageComments(Languages language)
{
    switch (language)
    {
        case Languages.C_SHARP:
            return "//";
        case Languages.JAVA:
            return "//";
        case Languages.PYTHON:
            return "#";
        case Languages.JAVASCRIPT:
            return "//";
        case Languages.C_PLUS_PLUS:
            return "//";
        case Languages.RUBY:
            return "#";
        case Languages.PHP:
            return "//";
        case Languages.SWIFT:
            return "//";
        case Languages.GO:
            return "//";
        case Languages.KOTLIN:
            return "//";
        case Languages.R:
            return "#";
        case Languages.TYPE_SCRIPT:
            return "//";
        case Languages.PERL:
            return "#";
        case Languages.SCALA:
            return "//";
        case Languages.HASKELL:
            return "--";
        case Languages.ALL://?
            return "Comments vary by language.";
        default:
            return "//";
    }
}

//Returns programming languages
static Languages GetProgrammingLanguage(string filePath)
{
    FileInfo fileInfo = new FileInfo(filePath);

    if (!fileInfo.Exists)
    {
        Console.WriteLine("File does not exist.");
        return Languages.ALL;
    }

    string extension = fileInfo.Extension.ToLower();

    switch (extension)
    {
        case ".cs":
            return Languages.C_SHARP;
        case ".java":
            return Languages.JAVA;
        case ".py":
            return Languages.PYTHON;
        case ".js":
            return Languages.JAVASCRIPT;
        case ".cpp":
            return Languages.C_PLUS_PLUS;
        case ".rb":
            return Languages.RUBY;
        case ".php":
            return Languages.PHP;
        case ".swift":
            return Languages.SWIFT;
        case ".go":
            return Languages.GO;
        case ".kt":
            return Languages.KOTLIN;
        case ".r":
            return Languages.R;
        case ".ts":
            return Languages.TYPE_SCRIPT;
        case ".pl":
            return Languages.PERL;
        case ".scala":
            return Languages.SCALA;
        case ".hs":
            return Languages.HASKELL;
        default:
            return Languages.ALL;
    }
}

//Attach to a single sourceFileCommants
static void Attach(FileInfo path, FileStream sourceFile, List<Languages> languages, bool note,
    SortOptions sort, bool removeEmptyLines, string author)
{
    string currentDirectory = Directory.GetCurrentDirectory();

    //author
    if (author != null)
        File.WriteAllText(path.FullName, "//" + author);

    string[] files = Directory.GetFiles(currentDirectory);
    List<string> upFiles = new List<string>();

    //languages
    Languages[] languagesLST = languages.ToArray();
    if (!Array.Exists(languagesLST, l => l == Languages.ALL))
    {
        foreach (var f in files)
        {
            Languages languages1 = GetProgrammingLanguage(f);
            if (Array.Exists(languagesLST, l => l == languages1))
                upFiles.Add(f);
        }
    }

    //sort
    if (sort != null && sort == SortOptions.CODE)
        upFiles.Sort((f1, f2) => GetProgrammingLanguage(f1).ToString().CompareTo(GetProgrammingLanguage(f2).ToString()));
    else
        upFiles.Sort((f1, f2) => f1.CompareTo(f2));

    //read and file to a signal file
    foreach (var f in upFiles)
    {
        //note
        if (note == true)
        {
            string sourceFileCommants = GetLanguageComments(GetProgrammingLanguage(f));
            sourceFileCommants += $"Path: {f}";
            File.WriteAllText(path.FullName, sourceFileCommants);
        }

        using (StreamReader reader = new StreamReader(f))
        using (StreamWriter writer = new StreamWriter(sourceFile))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //remove-empty-lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                writer.WriteLine(line);
            }
        }
    }
}

#region bundle command
//OPTIONS
var bundleOutput = new Option<FileInfo>("--output", "Path sourceFileCommants");
bundleOutput.AddAlias("-o");
var bundleLanguages = new Option<List<Languages>>("--languages", "languages to Attach") { IsRequired = true, };
bundleLanguages.AddAlias("-l");
var bundleNote = new Option<bool>("--note", "list the source of the File source by commants");
bundleNote.AddAlias("-n");
var bundleSort = new Option<SortOptions>("--sort", "Sort by sourceFileCommants name or type code");
bundleSort.AddAlias("-s");
var bundleRemoveEmptyLines = new Option<bool>("--remove-e-line", "Remove empty lines");
bundleRemoveEmptyLines.AddAlias("-r");
var bundleAuthor = new Option<string>("--answer", "Include answer's name at the top of the sourceFileCommants");
bundleAuthor.AddAlias("-a");

var bundleCommand = new Command("bundle", "Bundle code files to a single sourceFileCommants");
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
        Console.WriteLine("sourceFileCommants was create");
        Attach(output, file, language, note, sort, removeEmptyLines, author);
        file.Close();
    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine("ERROR: bundle files faild, path sourceFileCommants invalid!");
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
    Console.Write("Enter Path sourceFileCommants ");
    FileInfo path = OKPath();
    fullCommand.Add("--output " + path);
    //bundleAuthor
    Console.Write("Include answer's name at the top of the sourceFileCommants? (y/n) ");
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
    fullCommand.Add("--remove-e-line " + isRemove);
    //bundleSort
    Console.Write("Sort by sourceFileCommants name or type code? (code/name) ");
    SortOptions sort = OKCodeName();
    fullCommand.Add("--sort " + sort);
    //bundleLanguages
    Console.Write("Which languages to attach? (for all press all): [c_sharp, java, python, javascript, c_plus_plus, ruby," +
            " php, swift, go, kotlin, r, type_script, perl, scala, haskell] (Separate by commas): ");
    List<Languages> languages = OKLanguage();
    fullCommand.Add("--languages" + languages);
    //bundleNote
    Console.Write("list the source of the sourceFileCommants? (y/n) ");
    string isNoteTmp = OKYesNo();
    bool isNote = isNoteTmp == "y" ? true : false;
    fullCommand.Add("--note " + isNote);

    File.AppendAllLines(fullCommandFileName, fullCommand);
    Console.WriteLine($"Response sourceFileCommants '{fullCommandFileName}' created with the following command:");
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

//Attach the bundle sourceFileCommants