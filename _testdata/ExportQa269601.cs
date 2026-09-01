using System;
using System.IO;
using System.Reflection;

internal static class ExportQa269601
{
    [STAThread]
    private static int Main(string[] args)
    {
        string root = args.Length > 0 ? Path.GetFullPath(args[0]) : Environment.CurrentDirectory;
        string bin = Path.Combine(root, "bin", "Debug");
        string output = Path.Combine(root, "_testdata", "QA26-9601_FM-QA-B08-F.xlsx");

        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(root, "App.config"));
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs eventArgs)
        {
            string dependency = new AssemblyName(eventArgs.Name).Name + ".dll";
            string dependencyPath = Path.Combine(bin, dependency);
            return File.Exists(dependencyPath) ? Assembly.LoadFrom(dependencyPath) : null;
        };

        Assembly app = Assembly.LoadFrom(Path.Combine(bin, "Material_Receiving_System_ver1.0.0.exe"));
        Type propertyType = app.GetType("RawMat.Property.QAdataProperty", true);
        Type controllerType = app.GetType("RawMat.Controllers.QAdataControllers", true);
        Type builderType = app.GetType("RawMat.ViewsMaterial.ReceiveWH.B08ContentBuilder", true);
        Type exporterType = app.GetType("RawMat.ViewsMaterial.ReceiveWH.ExportExcellB08", true);

        object dataItem = Activator.CreateInstance(propertyType);
        propertyType.GetProperty("Report_No").SetValue(dataItem, "QA26-9601", null);
        object controller = Activator.CreateInstance(controllerType);

        object content = builderType.GetMethod("Build", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new[] { controller, dataItem });

        object result = exporterType.GetMethod("CreateCheckSheet", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new[] { dataItem, content, output });

        Console.WriteLine(Convert.ToString(result));
        return File.Exists(output) ? 0 : 1;
    }
}
