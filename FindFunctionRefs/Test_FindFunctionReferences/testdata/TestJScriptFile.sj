function initializeSystem() {
  //readSetupIniFile();
  var arcPath = "F:\\";
  addVariable("ApplDataPath", "String", arcPath);


  var iniPath = "F:\\";
  addVariable("ApplIniFilePath", "String", iniPath);

  setProjectSuiteVariables();
  deleteSettingsProfiles();

  ChangeTrackingSimulatorComPort("COM3");
  StartTrackingSimulatorProcess();

  //copyIniFileToApplicationFolder();
}

 function   StartTrackingSimulatorProcess() {
  // do notching
}

function     ChangeTrackingSimulatorComPort() {
  // do notching
}
