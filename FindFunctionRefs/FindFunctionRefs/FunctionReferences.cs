using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace FindFunctionRefs
{
  ///
  ///
  //HINT: make sure that the System.Management.Automation package is installed e.g. by using Nuget: Install-Package System.Management.Automation
  //HINT: if Microsoft SDK for powershell is is already on your disk it should be under C:\Program Files\Reference Assemblies\Microsoft\WindowsPowerShell
  //HINT: import module in powershell by Import-Module .\FindFunctionRefs.dll - but powershell holds a lock of that dll aftwards AND powershell taks too long to load
  //HINT: use an other posershell session by typing powershell and exiting this session by exit --> improve by typing powershell -noprofile to start faster
  ///
  ///
  /// add powershell as external tool: C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe to be started in debug
  ///
  ///
  [Cmdlet(VerbsCommon.Find, "FunctionReferences")]
  public class GetFunctionReferences : PSCmdlet
  {
    [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true)]
    public string LiteralFilePath { get; set; }

    /// <summary>
    /// property which returns the amount of functions found in the files
    /// </summary>
    public int GetNumOfFunctions
    {
      get
      {
        return this.listOfFunctions.Count;
      }
    }

    /// <summary>
    /// method overwritte from base case
    /// </summary>
    protected override void ProcessRecord()
    {
      foreach (string filePathName in this.filePaths)
      {
        this.ReadDataFromFile(filePathName);
      }

      foreach (string funct in this.listOfFunctions)
      {
        var count = this.listOfWords.Where(item => (item.IndexOf(funct) >= 0)).Count();
        WriteVerbose("The function with name: " + funct + " is counted: " + count.ToString() + " times.");
        if (count == 1)
        {
          WriteObject("The function with name: " + funct + " seems to be only declared but not used.");
        }
      }
    }

    /// <summary>
    /// method prepares the environment for parsing the data later on
    /// see: https://msdn.microsoft.com/en-us/library/bb882639.aspx
    ///
    /// </summary>
    protected override void BeginProcessing()
    {
      this.InitializeDataFields();
      this.SetFilePath();
    }

    /// <summary>
    /// see http://stackoverflow.com/questions/8505294/how-do-i-deal-with-paths-when-writing-a-powershell-cmdlet
    /// powershell path are different from win32 paths !
    /// powershell paths are: provider-qualified paths (e.g. FileSystem::c:\temp\foo.txt) and drive-qualified paths (e.g. temp:\foo.txt) which differ when
    /// a e.g. a PSDrive was created: new-psdrive temp filesystem c:\temp\
    /// </summary>
    protected void SetFilePath()
    {
      string path = this.LiteralFilePath;
      ProviderInfo provider;
      this.filePaths.AddRange(this.GetResolvedProviderPathFromPSPath(path, out provider));
    }

    /// <summary>
    /// parse the list with the data in order to extract the function name
    /// </summary>
    /// <param name="fileDataLines">list of all lines</param>
    private void ExtractFunctionNames(IEnumerable<string> fileDataLines)
    {
      System.Text.RegularExpressions.Regex searchTerm =
                new System.Text.RegularExpressions.Regex(@"^\s*function\s*(\w*)");
      var localListOfFunctions = from dataLine in fileDataLines
                                 let matches = searchTerm.Matches(dataLine)
                                 where matches.Count > 0
                                 select new
                                 {
                                   matchedFunctionName = matches[0].Groups[1].Value //just take the first one - and do not make use of the capturing group
                                 };

      foreach (var matchedLinesWithFunction in localListOfFunctions)
      {
        string localString = matchedLinesWithFunction.matchedFunctionName.ToString();

        this.listOfFunctions.Add(localString);
      }
    }

    /// <summary>
    /// parse the list with the data in order to collect all words
    /// </summary>
    /// <param name="fileDataLines">list of all files</param>
    private void ExtractWords(IEnumerable<string> fileDataLines)
    {
      foreach (var line in fileDataLines)
      {
        string[] separatedWords = line.Split(' ');
        foreach (string word in separatedWords)
        {
          this.listOfWords.Add(word);
        }
      }
    }

    /// <summary>
    /// Read data from each file
    /// </summary>
    public void ReadDataFromFile(string fileName)
    {
      if (!System.IO.File.Exists(fileName))
      {
        WriteWarning("Could not find a file with file name: " + fileName);
        return;
      }

      IEnumerable<string> fileDataLines = File.ReadAllLines(fileName );

      ExtractFunctionNames(fileDataLines);
      ExtractWords(fileDataLines);
    }

    /// <summary>
    /// initializes the data structure
    /// </summary>
    public void InitializeDataFields()
    {
      this.listOfFunctions = new List<string>();
      this.listOfWords = new List<string>();
      this.filePaths = new List<string>();
    }

    /// <summary>
    /// array to store all words
    /// </summary>
    private List<string> listOfWords;

    /// <summary>
    /// list to store all functions
    /// </summary>
    private List<string> listOfFunctions;

    /// <summary>
    /// list to store all functions
    /// </summary>
    private List<string> filePaths;
  }
}