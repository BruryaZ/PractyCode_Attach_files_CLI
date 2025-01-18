using System.CommandLine;

//Validation function
static FileInfo OKPath()
{
    string pathInput = OKString();

    FileInfo path = new FileInfo(pathInput);

    while (!IsPathValidAndCreateFile(path.ToString()))
    {
        Console.WriteLine($"File exists: {path.FullName} or wrong path");
        Console.Write("Enter Path for the single file: ");
        pathInput = OKString();
        path = new FileInfo(pathInput);
    }

    return path;
}
static bool IsPathValidAndCreateFile(string path)
{
    // בדיקה אם המחרוזת ריקה או null
    if (string.IsNullOrEmpty(path))
    {
        return false; // נתיב לא חוקי
    }

    // בדיקה אם הנתיב מכיל תווים לא חוקיים
    char[] invalidChars = Path.GetInvalidPathChars();
    foreach (char c in path)
    {
        if (Array.Exists(invalidChars, element => element == c))
        {
            return false; // נתיב לא חוקי
        }
    }

    // קבלת הנתיב לתיקייה
    string directoryPath = Path.GetDirectoryName(path);

    // בדיקה אם התיקייה קיימת, אם לא - יצירתה
    if (!Directory.Exists(directoryPath))
    {
        Directory.CreateDirectory(directoryPath);
    }

    // ניתן כאן ליצור את הקובץ
    File.Create(path).Dispose(); // יצירת הקובץ

    return true; // הנתיב תקין והקובץ נוצר
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
    string str = Console.ReadLine();
    while (str == "" || str.IndexOf(" ") != -1)
    {
        Console.WriteLine("WRONG ANSWER: preee correct string without spaces and signs");
        str = Console.ReadLine();
    }
    return str;
}
static SortOptions OKCodeName()
{
    string sortTmp = Console.ReadLine();
    while (sortTmp != "code" && sortTmp != "name")
    {
        Console.WriteLine("\"WRONG ANSWER: press code or name");
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
        case Languages.ALL:
            return "//";
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

//Attach the bundle to a single file
static void Attach(FileInfo path, FileStream sourceFile, List<Languages> languages, bool note, SortOptions sort, bool removeEmptyLines, string author)
{
    Console.WriteLine("--------Attaching-------------");
    string currentDirectory = Directory.GetCurrentDirectory();

    // Check if upFiles is empty
    string[] files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories);
    List<string> upFiles = new List<string>();

    // Languages
    Languages[] languagesLST = languages.ToArray();
    bool all = Array.Exists(languagesLST, l => l == Languages.ALL);
    foreach (var f in files)
    {
        string directoryName = Path.GetDirectoryName(f);
        if (directoryName != null && Enum.IsDefined(typeof(SystemDirectories), directoryName))
        { 

            continue; // דלג על קבצים מתקיות אלו
        }
        Languages languagesOfCurrentFile = GetProgrammingLanguage(f);
        if (Array.Exists(languagesLST, l => l == languagesOfCurrentFile))
            upFiles.Add(f);

        else if (all && GetProgrammingLanguage(f) != Languages.ALL)// if it is programming language and user choosen ALL
            upFiles.Add(f);
    }
    upFiles.Remove(sourceFile.Name);

    // Check if upFiles is empty
    if (upFiles.Count == 0)
    {
        Console.WriteLine("No files found for the selected languages.");
        return;
    }

    // Sort
    if (sort == SortOptions.CODE)
        upFiles.Sort((f1, f2) => GetProgrammingLanguage(f1).ToString().CompareTo(GetProgrammingLanguage(f2).ToString()));
    else
        upFiles.Sort();

    // Write author comment first if provided
    if (!string.IsNullOrEmpty(author))
    {
        using (StreamWriter writer = new StreamWriter(sourceFile))
        {
            writer.WriteLine("//" + author + Environment.NewLine);
            // Process files
            foreach (var f in upFiles)
            {
                // Note
                if (note)
                {
                    string sourceFileComments = GetLanguageComments(GetProgrammingLanguage(f));
                    sourceFileComments += $"Path: {f}{Environment.NewLine}";
                    writer.WriteLine(sourceFileComments);
                }

                using (StreamReader reader = new StreamReader(f))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Remove empty lines
                        if (removeEmptyLines && string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}

//OPTIONS
var bundleOutput = new Option<FileInfo>("--output", "Path destination File");
bundleOutput.AddAlias("-o");
var bundleLanguages = new Option<List<Languages>>("--language", "Languages to Attach") { IsRequired = true, };
bundleLanguages.AddAlias("-l");
var bundleNote = new Option<bool>("--note", "List the source of the File source by commants");
bundleNote.AddAlias("-n");
var bundleSort = new Option<SortOptions>("--sort", "Sort by name or type code");
bundleSort.AddAlias("-s");
var bundleRemoveEmptyLines = new Option<bool>("--remove-e-line", "Remove empty lines");
bundleRemoveEmptyLines.AddAlias("-r");
var bundleAuthor = new Option<string>("--author", "Include author's name at the top of the file");
bundleAuthor.AddAlias("-a");

#region bundle command

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
    Console.WriteLine($"Selected Languages: {string.Join(", ", language)}");
    Console.WriteLine($"List Source: {note}");
    Console.WriteLine($"Sort Option: {sort}");
    Console.WriteLine($"Remove Empty Lines: {removeEmptyLines}");
    Console.WriteLine($"Author Name: {author}");

    // Check if there are any files to attach
    string currentDirectory = Directory.GetCurrentDirectory();
    string[] files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories);
    List<string> upFiles = new List<string>();

    // Languages
    Languages[] languagesLST = language.ToArray();
    if (Array.Exists(languagesLST, l => l == Languages.ALL))
    {
        upFiles.AddRange(files);
    }

    else
    {

        foreach (var f in files)
        {
            Languages languages1 = GetProgrammingLanguage(f);
            if (Array.Exists(languagesLST, l => l == languages1))
                upFiles.Add(f);
        }
    }

    // Check if upFiles is empty
    if (upFiles.Count == 0)
    {
        Console.WriteLine("No files found for the selected languages. File will not be created.");
        return;
    }

    try
    {
        // Check if the output file already exists
        if (File.Exists(output.FullName))
        {
            Console.WriteLine($"ERROR: The file '{output.FullName}' already exists. Please choose a different output path.");
            return;
        }

        using (FileStream file = File.Create(output.FullName))
        {
            Console.WriteLine("--------File was created--------");
            Attach(output, file, language, note, sort, removeEmptyLines, author);
            Console.WriteLine("--------After attaching--------");
        }
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine($"ERROR: You do not have permission to create the file at '{output.FullName}'.");
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("ERROR: The directory for the output file does not exist.");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"ERROR: An I/O error occurred: {ex.Message}");
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
    fullCommand.Add("bundle");
    //Get options:

    //bundleOutput
    Console.Write("Enter Path file: ");
    FileInfo path = OKPath();
    fullCommand.Add("--output " + path);
    //bundleLanguages
    Console.Write("Which languages to attach? (for all press all): [c_sharp, java, python, javascript, c_plus_plus, ruby," +
            " php, swift, go, kotlin, r, type_script, perl, scala, haskell] (Separate by commas): ");
    List<Languages> languages = OKLanguage();
    fullCommand.Add("--language " + string.Join(",", languages.Select(l => l.ToString().ToLower())));
    //bundleNote
    Console.Write("list the source of the file? (y/n): ");
    string isNoteTmp = OKYesNo();
    bool isNote = isNoteTmp == "y" ? true : false;
    fullCommand.Add("--note " + isNote);
    //bundleSort
    Console.Write("Sort by file name or type code? (code/name): ");
    SortOptions sort = OKCodeName();
    fullCommand.Add("--sort " + sort);
    //bundleRemoveEmptyLines
    Console.Write("Remove empty lines? (y/n): ");
    string isRemoveTmp = OKYesNo();
    bool isRemove = isRemoveTmp == "y" ? true : false;
    fullCommand.Add("--remove-e-line " + isRemove);
    //bundleAuthor
    Console.Write("Include author's name at the top of the file? (y/n): ");
    string author = OKYesNo(), authorName = "empty";
    if (author == "y")
    {
        Console.Write("Press author name: ");
        authorName = OKString();
    }
    fullCommand.Add("--author " + authorName);

    File.WriteAllText(fullCommandFileName, string.Join("\n", fullCommand));
    Console.WriteLine($"Response file '{fullCommandFileName}' created with the following command:");
    Console.WriteLine(string.Join("\n", fullCommand));
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
enum SystemDirectories { BIN, OBJ, DEBUG, RELEASE, VS, GIT, NODE_MODULES, TEMP, LOGS, CACHE }
