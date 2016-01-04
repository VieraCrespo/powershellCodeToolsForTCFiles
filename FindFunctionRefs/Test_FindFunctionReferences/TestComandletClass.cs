using NUnit.Core;
using NUnit.Framework;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using FindFunctionRefs;


namespace Test_FindFunctionReferences
{
  [TestFixture]
  public class TestComandletClass
  {
    [Test]
    public void TestMethod1()
    {
      var initialSessionState =
     InitialSessionState.CreateDefault();

      initialSessionState.Commands.Add(
          new SessionStateCmdletEntry("Find-FunctionReferences", typeof(GetFunctionReferences), null));

      using (var runspace = RunspaceFactory.CreateRunspace(initialSessionState))
      {
        runspace.Open();

        using (var powershell = PowerShell.Create())
        {
          powershell.Runspace = runspace;

          var funRefCommand = new Command("Find-FunctionReferences");
          funRefCommand.Parameters.Add("LiteralFilePath", ".\\testdata\\TestJScriptFile.sj");

          powershell.Commands.AddCommand(funRefCommand);

          var results = powershell.Invoke();

          Assert.AreEqual(1, results.Count);
          StringAssert.Contains("initializeSystem", results[0].ToString());
        }
      }
    }
  }
}