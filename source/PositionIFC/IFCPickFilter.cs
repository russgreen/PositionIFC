using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace IFCLocation;
internal class IFCPickFilter : ISelectionFilter
{
    public bool AllowElement(Element elem)
    {
        var linkInstance = elem as RevitLinkInstance;

        if (linkInstance == null) return false;

        if (linkInstance.GetLinkDocument().PathName.EndsWith(".ifc.RVT")) return true;

        return false;
    }

    public bool AllowReference(Reference reference, XYZ position)
    {
        return false;
    }
}
