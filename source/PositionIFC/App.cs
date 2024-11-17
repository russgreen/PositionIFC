using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using PositionIFC.Commands;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace PositionIFC;
public class App : IExternalApplication
{
    // get the absolute path of this assembly
    public static readonly string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

    // class instance
    public static App ThisApp;

    public static UIControlledApplication CachedUiCtrApp;
    public static UIApplication CachedUiApp;
    public static ControlledApplication CtrApp;
    public static Autodesk.Revit.DB.Document RevitDocument;

    public Result OnStartup(UIControlledApplication application)
    {
        ThisApp = this;
        CachedUiCtrApp = application;
        CtrApp = application.ControlledApplication;

        var panel = RibbonPanel(CachedUiCtrApp);

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    #region Ribbon Panel

    private RibbonPanel RibbonPanel(UIControlledApplication application)
    {

        RibbonPanel panel = CachedUiCtrApp.CreateRibbonPanel("PositionIFC_Panel");
        panel.Title = "PositionIFC";

        PushButton button = (PushButton)panel.AddItem(
            new PushButtonData(
                "CommandPositionIFC",
                "Position IFC",
                Assembly.GetExecutingAssembly().Location,
                $"{nameof(PositionIFC)}.{nameof(Commands)}.{nameof(CommandPositionIFC)}"));
        button.ToolTip = "Execute the PositionIFC command";
        button.LargeImage = PngImageSource("IFCLocation.Resources.PositionIFC_Button.png");

        return panel;
    }

    private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
    {
        var stream = GetType().Assembly.GetManifestResourceStream(embeddedPath);
        System.Windows.Media.ImageSource imageSource;
        try
        {
            imageSource = BitmapFrame.Create(stream);
        }
        catch (Exception ex)
        {
            imageSource = null;
        }

        return imageSource;
    }
    #endregion
}
