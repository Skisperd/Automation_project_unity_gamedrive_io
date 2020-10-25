GameDriver October 2020 - Release Notes
------------------------------

-New HierarchyPath (HPath) plugin for the Unity editor. Right-click any object in the Hierarchy view in Unity, and select "HierarchyPath" and either Relative or Absolute by Name, Tag or both. The output can be found in the Unity console, and can be copy/paste into your test for ease of use.
-Updated HPath usage. See "Working with HierarchyPath in GameDriver" for additional details.
-New SetObjectValue command.
-New contains(), first() and last() HPath methods.
-GetObjectPosition now returns all the data required to construct a Transform which contains Position and Rotation, or you can take the string result as a Vector3.
-Resolved issues with standalone builds not correctly copying GameDriver libraries to the right location.
-Resolved issues with Quit on macOS.
-Resolved inconsistencies in various API methods (WaitForObjectValue, CallMethod, Rotate)
-Updated communication components for added stability

Additional Notes:

-Additional logging enabled via GDIO\Editor\Log4net.config settings. Logging default is set to WARN to avoid performance impact.
-Test results have been moved to the <game_project>\GDIOResults\ folder. This can be queried via the read-only Api.ResultsPath object in your tests.
-Screenshots are saved to test project folder. Unity creates and stores a copy in the Assets directory of the Unity project. Be sure to clean these out prior to application release.

Known Issue:

-GameAttach not working consistently in standalone mode